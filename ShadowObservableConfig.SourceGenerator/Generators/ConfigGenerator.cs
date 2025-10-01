using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowObservableConfig.SourceGenerator.Generators;

[Generator]
public class ConfigGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 创建语法提供者，查找带有ObservableConfig属性的类
        var configClassesProvider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsConfigClass(node),
                transform: static (ctx, _) => GetConfigClassInfo(ctx))
            .Where(static info => info != null);

        // 创建生成器
        context.RegisterSourceOutput(configClassesProvider, Execute);
    }

    private static bool IsConfigClass(SyntaxNode node)
    {
        if (node is not ClassDeclarationSyntax classDeclaration) return false;
        
        // 检查是否是分部类
        if (!classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))) return false;
        
        // 检查是否有ConfigAttribute
        return HasConfigAttribute(classDeclaration);
    }

    private static bool HasConfigAttribute(ClassDeclarationSyntax classDeclaration)
    {
        return classDeclaration.AttributeLists
            .SelectMany(al => al.Attributes)
            .Any(attr => attr.Name.ToString() == "ObservableConfig");
    }

    private static ConfigClassInfo? GetConfigClassInfo(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclaration) return null;
        
        var semanticModel = context.SemanticModel;
        var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;
        
        if (classSymbol == null) return null;

        // 获取ConfigAttribute信息
        var configAttribute = classSymbol.GetAttribute("ShadowObservableConfig.Attributes.ObservableConfigAttribute", semanticModel.Compilation);
        if (configAttribute == null) return null;

        return new ConfigClassInfo
        {
            ClassDeclaration = classDeclaration,
            ClassSymbol = classSymbol,
            ConfigAttribute = configAttribute,
            SemanticModel = semanticModel
        };
    }

    private static void Execute(SourceProductionContext context, ConfigClassInfo? configClassInfo)
    {
        if (configClassInfo == null) return;

        var logger = new Logger("ConfigGenerator", context);
        
        try
        {
            var classSymbol = configClassInfo.ClassSymbol;
            var configAttribute = configClassInfo.ConfigAttribute;

            // 提取ConfigAttribute参数
            var fileName = GetAttributeValue(configAttribute, "FileName", "");
            var dirPath = GetAttributeValue(configAttribute, "DirPath", "config");
            var description = GetAttributeValue(configAttribute, "Description", "");
            var version = GetAttributeValue(configAttribute, "Version", "1.0.0");
            
            // 如果FileName为空且是内部类，使用外部类的文件名
            if (string.IsNullOrEmpty(fileName) && classSymbol.ContainingType != null)
            {
                // 内部类不生成单独文件，跳过
                return;
            }

            // 查找带有ConfigFieldAttribute的字段（包括内部类）
            var configFields = GetConfigFields(classSymbol, configClassInfo.SemanticModel.Compilation);

            if (configFields.Count == 0) return;

            // 生成Config类代码
            var generatedCode = GenerateConfigClass(classSymbol, configFields, fileName, dirPath, description, version);
            
            var fileNameSafe = classSymbol.Name.Replace("<", "").Replace(">", "");
            context.AddSource($"{fileNameSafe}.Config.g.cs", generatedCode);
        }
        catch (Exception ex)
        {
            logger.Error("SPLW006", $"ConfigGenerator error: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private static List<ConfigFieldInfo> GetConfigFields(INamedTypeSymbol classSymbol, Compilation compilation)
    {
        var configFields = new List<ConfigFieldInfo>();
        const string configFieldAttributeName = "ShadowObservableConfig.Attributes.ObservableConfigPropertyAttribute";

        // 获取当前类的配置字段
        foreach (var member in classSymbol.GetMembers().OfType<IFieldSymbol>())
        {
            if (!member.HasAttribute(configFieldAttributeName, compilation)) continue;

            var fieldAttribute = member.GetAttribute(configFieldAttributeName, compilation);
            if (fieldAttribute == null) continue;

            var fieldType = member.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var isEntityClass = IsEntityClass(member.Type, compilation);
            var isCollectionOfEntities = IsCollectionOfEntities(member.Type, compilation);
            var isObservableCollection = isCollectionOfEntities ? isCollectionOfEntities : IsObservableCollection(member.Type, compilation);
            var fieldInfo = new ConfigFieldInfo
            {
                FieldName = member.Name,
                FieldType = fieldType,
                Name = GetAttributeValue(fieldAttribute, "Name", member.Name),
                Description = GetAttributeValue(fieldAttribute, "Description", ""),
                Alias = GetAttributeValue(fieldAttribute, "Alias", ""),
                IsEntityClass = isEntityClass,
                IsObservableCollection = isObservableCollection,
                IsCollectionOfEntities = isCollectionOfEntities
            };

            configFields.Add(fieldInfo);
        }

        // 递归查找内部类中的配置字段
        foreach (var innerType in classSymbol.GetTypeMembers().OfType<INamedTypeSymbol>())
        {
            var innerConfigFields = GetConfigFields(innerType, compilation);
            configFields.AddRange(innerConfigFields);
        }

        return configFields;
    }

    private static string GenerateConfigClass(INamedTypeSymbol classSymbol, List<ConfigFieldInfo> configFields, 
        string fileName, string dirPath, string description, string version)
    {
        var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
        var className = classSymbol.Name;
        var privateFieldName = ToLowerFirstChar(className);
        
        // 如果是内部类，需要包含外部类名
        var fullClassName = className;
        if (classSymbol.ContainingType != null)
        {
            fullClassName = $"{classSymbol.ContainingType.Name}.{className}";
        }
        
        // 检查是否为内部类（FileName为空）
        var isInnerClass = string.IsNullOrEmpty(fileName);

        var properties = new StringBuilder();
        var initialization = new StringBuilder("\n");
        var saveMethod = new StringBuilder();
        var loadMethod = new StringBuilder();

        // 生成属性
        foreach (var field in configFields)
        {
            var privateField = $"{ToLowerFirstChar(field.FieldName)}";
            var propertyName = field.Name;
            var fieldType = field.FieldType;
            
            // 生成YamlMember属性
            var yamlMemberAttributes = new List<string>();
            if (!string.IsNullOrEmpty(field.Description))
            {
                yamlMemberAttributes.Add($"Description = \"{field.Description}\"");
            }

            if (string.IsNullOrEmpty(field.Alias))
            {
                field.Alias = propertyName;
            }
            yamlMemberAttributes.Add($"Alias = \"{field.Alias}\"");
            var yamlMemberAttribute = $"[global::YamlDotNet.Serialization.YamlMember({string.Join(", ", yamlMemberAttributes)})]";

            // 根据字段类型生成不同的属性实现
            if (field.IsEntityClass)
            {
                properties.AppendLine(GenerateEntityProperty(field, privateField, propertyName, fieldType, yamlMemberAttribute));
                initialization.AppendLine($"        if ({propertyName} == null) {propertyName} = new();");
            }
            else if (field.IsObservableCollection)
            {
                properties.AppendLine(GenerateEntityCollectionProperty(field, privateField, propertyName, fieldType, yamlMemberAttribute));
                initialization.AppendLine($"        if ({propertyName} == null) {propertyName} = new();");
            }
            else
            {
                properties.AppendLine(GenerateSimpleProperty(field, privateField, propertyName, fieldType, yamlMemberAttribute));
            }
        }


        if (isInnerClass)
        {
            // 内部类也继承BaseConfig，但不生成单独文件
            return $$"""
                     // Automatic Generate From ShadowObservableConfig.SourceGenerator
                     using global::System.ComponentModel;
                     using global::System.Runtime.CompilerServices;
                     
                     namespace {{namespaceName}};

                     /// <summary>
                     /// {{description}}
                     /// Version: {{version}}
                     /// </summary>
                     public partial class {{fullClassName}} : global::ShadowObservableConfig.BaseConfig
                     {
                         /// <summary>
                         /// 构造函数
                         /// </summary>
                         public {{fullClassName}}()
                         {{{initialization}}
                             Initialized = true;
                             AfterConfigInit();
                         }

                     {{properties}}

                     partial void AfterConfigInit();
                     }
                     """;
        }
        else
        {
            // 外部类生成完整的配置类
            return $$"""
                     // Automatic Generate From ShadowObservableConfig.SourceGenerator
                     using global::System.ComponentModel;
                     using global::System.Runtime.CompilerServices;
                     
                     namespace {{namespaceName}};

                     /// <summary>
                     /// {{description}}
                     /// Version: {{version}}
                     /// </summary>
                     public partial class {{fullClassName}} : global::ShadowObservableConfig.BaseConfig
                     { 
                         [global::YamlDotNet.Serialization.YamlIgnore] 
                         protected static global::ShadowObservableConfig.ConfigFileInfo Info => new global::ShadowObservableConfig.ConfigFileInfo("{{fileName}}", "{{dirPath}}");
                         
                         /// <summary>
                         /// Constructor
                         /// </summary>
                         public {{fullClassName}}()
                         {
                         {{initialization}}
                         }
                         public void Init()
                         {
                             Initialized = true;
                             ConfigChanged += InvokeSaveFileOnChange;
                             AfterConfigInit();
                         }
                         public static {{fullClassName}} Load()
                         {
                             var configDir = global::System.IO.Path.GetDirectoryName(Info.ConfigFilePath);
                             if (configDir == null) throw new global::System.ArgumentNullException($"{nameof(Info.ConfigFilePath)} not found");
                             if (!global::System.IO.Directory.Exists(configDir))
                             {
                                 global::System.IO.Directory.CreateDirectory(configDir);
                             }
                             var obj = global::ShadowObservableConfig.ShadowConfigGlobalSetting.ConfigSetting.ConfigLoader.Load<{{fullClassName}}>(Info.ConfigFilePath);
                             if (obj is null)
                             {
                                 obj = new {{fullClassName}}();
                                 obj.Save();
                             }
                             obj.Init();
                             return obj;
                         }
                         
                         protected void InvokeSaveFileOnChange(object? sender, global::ShadowObservableConfig.Args.ConfigChangedEventArgs e)
                         {
                             Save();
                         }
                         
                         public void Save()
                         {
                             global::ShadowObservableConfig.ShadowConfigGlobalSetting.ConfigSetting.ConfigLoader.Save(Info.ConfigFilePath, this);
                         }
                         
                         partial void AfterConfigInit();
                         
                     {{properties}}

                     {{saveMethod}}

                     {{loadMethod}}
                     }
                     """;
        }
    }

    private static string GetAttributeValue(AttributeData attribute, string propertyName, object defaultValue)
    {
        foreach (var namedArgument in attribute.NamedArguments)
        {
            if (namedArgument.Key == propertyName)
            {
                return namedArgument.Value.Value?.ToString() ?? defaultValue?.ToString() ?? "";
            }
        }
        return defaultValue?.ToString() ?? "";
    }
 
    private static string ToLowerFirstChar(string input)
    {
        if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
            return input;

        return char.ToLower(input[0]) + input.Substring(1);
    }

    /// <summary>
    /// 检测类型是否为实体类（ObservableConfig的FileName为空）
    /// </summary>
    private static bool IsEntityClass(ITypeSymbol? typeSymbol, Compilation compilation)
    {
        // 检查ObservableConfig属性中的FileName是否为空
        var configAttribute = typeSymbol?.GetAttribute("ShadowObservableConfig.Attributes.ObservableConfigAttribute", compilation);
        if (configAttribute == null) return false;
        
        var fileName = GetAttributeValue(configAttribute, "FileName", "");
        return string.IsNullOrEmpty(fileName);
    }

    /// <summary>
    /// 检测类型是否为实体类集合
    /// </summary>
    private static bool IsCollectionOfEntities(ITypeSymbol? typeSymbol, Compilation compilation)
    {
        if (typeSymbol == null) return false;
        
        // 检查是否实现了INotifyCollectionChanged接口
        if (!IsObservableCollection(typeSymbol, compilation)) return false;
        
        // 获取集合元素的类型
        var elementType = GetCollectionElementType(typeSymbol);
        // 检查元素类型是否为实体类
        return elementType != null &&
               IsEntityClass(elementType, compilation);
    }

    /// <summary>
    /// 检测类型是否实现了INotifyCollectionChanged接口
    /// </summary>
    private static bool IsObservableCollection(ITypeSymbol typeSymbol, Compilation compilation)
    {
        if (typeSymbol == null) return false;
        
        // 获取INotifyCollectionChanged接口
        var notifyCollectionChangedInterface = compilation.GetTypeByMetadataName("System.Collections.Specialized.INotifyCollectionChanged");
        if (notifyCollectionChangedInterface == null) return false;
        
        // 检查类型是否实现了INotifyCollectionChanged接口
        return typeSymbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, notifyCollectionChangedInterface));
    }

    /// <summary>
    /// 检测类型是否为集合类型（ObservableCollection）
    /// </summary>
    private static bool IsCollectionType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol == null) return false;
        
        // 检查是否为ObservableCollection<T>类型
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol.Name == "ObservableCollection" && 
                   namedTypeSymbol.ContainingNamespace.ToDisplayString() == "System.Collections.ObjectModel" &&
                   namedTypeSymbol.TypeArguments.Length == 1;
        }
        
        return false;
    }

    /// <summary>
    /// 获取集合的元素类型
    /// </summary>
    private static ITypeSymbol? GetCollectionElementType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol == null) return null;
        
        // 对于泛型集合类型，直接获取第一个泛型参数
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeArguments.Length > 0)
        {
            return namedTypeSymbol.TypeArguments[0];
        }
        
        // 尝试从IEnumerable<T>接口中获取元素类型
        var enumerableInterface = typeSymbol.AllInterfaces
            .FirstOrDefault(i => i.Name == "IEnumerable" && 
                                i.ContainingNamespace.ToDisplayString() == "System.Collections.Generic" &&
                                i.TypeArguments.Length == 1);
        
        return enumerableInterface?.TypeArguments[0];
    }

    /// <summary>
    /// 生成内部类属性（继承BaseConfig的属性）
    /// </summary>
    private static string GenerateInnerClassProperty(ConfigFieldInfo field, string privateField, string propertyName, string fieldType, string yamlMemberAttribute)
    {
        return $$"""
                                    /// <summary>
                                    /// {{field.Description}}
                                    /// </summary>
                                    {{yamlMemberAttribute}}
                                    public {{fieldType}} {{propertyName}}
                                    {
                                        get => {{privateField}};
                                        set
                                        {
                                            // Auto Generate from GenerateInnerClassProperty
                                            if (!global::System.Collections.Generic.EqualityComparer<{{fieldType}}>.Default.Equals({{privateField}}, value))
                                            {
                                                var oldValue = {{privateField}};
                                                {{privateField}} = value;
                                                OnPropertyChanged(nameof({{propertyName}}));
                                                OnConfigChanged(nameof({{propertyName}}), nameof({{propertyName}}), oldValue, value, typeof({{fieldType}}));
                                            }
                                        }
                                    }
                                """;
    }

    /// <summary>
    /// 生成简单属性（非实体类）
    /// </summary>
    private static string GenerateSimpleProperty(ConfigFieldInfo field, string privateField, string propertyName, string fieldType, string yamlMemberAttribute)
    {
        return $$"""
                                    /// <summary>
                                    /// {{field.Description}}
                                    /// </summary>
                                    {{yamlMemberAttribute}}
                                    public {{fieldType}} {{propertyName}}
                                    {
                                        get => {{privateField}};
                                        set
                                        {
                                            if (!global::System.Collections.Generic.EqualityComparer<{{fieldType}}>.Default.Equals({{privateField}}, value))
                                            {
                                                var oldValue = {{privateField}};
                                                {{privateField}} = value;
                                                OnPropertyChanged(nameof({{propertyName}}));
                                                if (!Initialized) return;
                                                OnConfigChanged(nameof({{propertyName}}), nameof({{propertyName}}), oldValue, value, typeof({{fieldType}}));
                                            }
                                        }
                                    }
                                """;
    }

    /// <summary>
    /// 生成实体类属性（支持递归变更通知）
    /// </summary>
    private static string GenerateEntityProperty(ConfigFieldInfo field, string privateField, string propertyName, string fieldType, string yamlMemberAttribute)
    {
        return $$"""
                                    /// <summary>
                                    /// {{field.Description}}
                                    /// </summary>
                                    {{yamlMemberAttribute}}
                                    public {{fieldType}} {{propertyName}}
                                    {
                                        get => {{privateField}};
                                        set
                                        {
                                            if (!global::System.Collections.Generic.EqualityComparer<{{fieldType}}>.Default.Equals({{privateField}}, value))
                                            {

                                                if ({{privateField}} != null) {{privateField}}.ConfigChanged -= On{{propertyName}}ConfigChanged;
                                                var oldValue = {{privateField}};
                                                {{privateField}} = value;
                                                {{privateField}}.ConfigChanged += On{{propertyName}}ConfigChanged;
                                                if (!Initialized) return;
                                                OnPropertyChanged(nameof({{propertyName}}));
                                                OnConfigChanged(nameof({{propertyName}}), nameof({{propertyName}}), oldValue, value, typeof({{fieldType}}));
                                            }
                                        }
                                    }
                                    
                                    /// <summary>
                                    /// {{propertyName}}实体变更事件处理
                                    /// </summary>
                                    private void On{{propertyName}}ConfigChanged(object? sender, global::ShadowObservableConfig.Args.ConfigChangedEventArgs e)
                                    {
                                        // 使用实体传递的完整路径，如果没有则构建：外部属性名.内部属性名
                                        var fullPropertyPath = string.IsNullOrEmpty(e.FullPropertyPath) 
                                            ? $"{{propertyName}}.{e.PropertyName}" 
                                            : $"{{propertyName}}.{e.FullPropertyPath}";
                                        OnConfigChanged(nameof({{propertyName}}), fullPropertyPath, e.OldValue, e.NewValue, e.PropertyType);
                                    }
                                """;
    }

    /// <summary>
    /// 生成实体类集合属性（支持集合中实体的递归变更通知）
    /// </summary>
    private static string GenerateEntityCollectionProperty(ConfigFieldInfo field, string privateField, string propertyName, string fieldType, string yamlMemberAttribute)
    {
        return $$"""
                                    /// <summary>
                                    /// {{field.Description}}
                                    /// </summary>
                                    {{yamlMemberAttribute}}
                                    public {{fieldType}} {{propertyName}}
                                    {
                                        get => {{privateField}};
                                        set
                                        {
                                            if (!global::System.Collections.Generic.EqualityComparer<{{fieldType}}>.Default.Equals({{privateField}}, value))
                                            {
                                                if ({{privateField}} != null) {{privateField}}.CollectionChanged -= On{{propertyName}}CollectionChanged;
                                                var oldValue = {{privateField}};
                                                {{privateField}} = value;
                                                {{privateField}}.CollectionChanged += On{{propertyName}}CollectionChanged;
                                                if (!Initialized) return;
                                                OnPropertyChanged(nameof({{propertyName}}));
                                                OnConfigChanged(nameof({{propertyName}}), nameof({{propertyName}}), oldValue, value, typeof({{fieldType}}));
                                            }
                                        }
                                    }
                                    
                                    protected void On{{propertyName}}CollectionChanged(object? sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
                                    {
                                        OnConfigChanged(nameof({{propertyName}}), nameof({{propertyName}}), e.OldItems, e.NewItems, typeof({{fieldType}}));
                                    }
                                    
                                    /// <summary>
                                    /// {{propertyName}}集合中实体变更事件处理
                                    /// </summary>
                                    private void On{{propertyName}}ItemConfigChanged(object? sender, global::ShadowObservableConfig.Args.ConfigChangedEventArgs e)
                                    {
                                        // 使用实体传递的完整路径，如果没有则构建：集合属性名[Item].内部属性名
                                        var fullPropertyPath = string.IsNullOrEmpty(e.FullPropertyPath) 
                                            ? $"{{propertyName}}[Item].{e.PropertyName}" 
                                            : $"{{propertyName}}[Item].{e.FullPropertyPath}";
                                        OnConfigChanged(nameof({{propertyName}}), fullPropertyPath, e.OldValue, e.NewValue, e.PropertyType);
                                    }
                                """;
    }


    private static INamedTypeSymbol? FindConfigClassInSameNamespace(INamedTypeSymbol mainPluginSymbol, 
        List<ClassDeclarationSyntax> configClasses, Compilation compilation)
    {
        var mainPluginNamespace = mainPluginSymbol.ContainingNamespace.ToDisplayString();
        
        foreach (var configDeclaration in configClasses)
        {
            var semanticModel = compilation.GetSemanticModel(configDeclaration.SyntaxTree);
            var configSymbol = semanticModel.GetDeclaredSymbol(configDeclaration) as INamedTypeSymbol;
            
            if (configSymbol == null) continue;
            
            // 检查是否在同一命名空间
            if (configSymbol.ContainingNamespace.ToDisplayString() == mainPluginNamespace)
            {
                return configSymbol;
            }
        }
        
        return null;
    }

    private class ConfigClassInfo
    {
        public ClassDeclarationSyntax ClassDeclaration { get; set; } = null!;
        public INamedTypeSymbol ClassSymbol { get; set; } = null!;
        public AttributeData ConfigAttribute { get; set; } = null!;
        public SemanticModel SemanticModel { get; set; } = null!;
    }

    private class ConfigFieldInfo
    {
        public string FieldName { get; set; } = "";
        public string FieldType { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Alias { get; set; } = "";
        public bool IsEntityClass { get; set; } = false;
        public bool IsObservableCollection { get; set; } = false;
        public bool IsCollectionOfEntities { get; set; } = false;
    }
}
