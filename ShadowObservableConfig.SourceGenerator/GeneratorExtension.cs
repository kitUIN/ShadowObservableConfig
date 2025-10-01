using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowObservableConfig.SourceGenerator;

/// <summary>
/// 提供源代码生成器的扩展方法
/// </summary>
internal static class GeneratorExtension
{

    /// <summary>
    /// 检查符号是否具有指定的特性
    /// </summary>
    /// <param name="symbol">要检查的符号</param>
    /// <param name="attributeName">特性名称</param>
    /// <param name="compilation">编译上下文</param>
    /// <returns>如果符号具有指定特性则返回true，否则返回false</returns>
    public static bool HasAttribute(this ISymbol symbol, string attributeName, Compilation compilation)
    {
        var serializableSymbol =
            compilation.GetTypeByMetadataName(attributeName);
        return symbol.GetAttributes()
            .Any(a => a.AttributeClass!.Equals(serializableSymbol, SymbolEqualityComparer.Default));
    }
    
    /// <summary>
    /// 获取符号的指定特性
    /// </summary>
    /// <param name="symbol">要检查的符号</param>
    /// <param name="attributeName">特性名称</param>
    /// <param name="compilation">编译上下文</param>
    /// <returns>如果找到指定特性则返回特性数据，否则返回null</returns>
    public static AttributeData? GetAttribute(this ISymbol symbol, string attributeName, Compilation compilation)
    {
        var serializableSymbol =
            compilation.GetTypeByMetadataName(attributeName);
        return symbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass!.Equals(serializableSymbol, SymbolEqualityComparer.Default));
    }

    /// <summary>
    /// 检查类型是否继承自指定的基类型
    /// </summary>
    /// <param name="type">要检查的类型</param>
    /// <param name="baseType">基类型</param>
    /// <returns>如果类型继承自基类型则返回true，否则返回false</returns>
    public static bool InheritsFrom(this ITypeSymbol? type, ITypeSymbol? baseType)
    {
        if (type == null || baseType == null) return false;
        var current = type.BaseType;
        while (current != null)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
                return true;
            current = current.BaseType;
        }
        return false;
    }
}