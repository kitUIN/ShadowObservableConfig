using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowObservableConfig.SourceGenerator;

internal static class GeneratorExtension
{

    public static bool HasAttribute(this ISymbol symbol, string attributeName, Compilation compilation)
    {
        var serializableSymbol =
            compilation.GetTypeByMetadataName(attributeName);
        return symbol.GetAttributes()
            .Any(a => a.AttributeClass!.Equals(serializableSymbol, SymbolEqualityComparer.Default));
    }
    public static AttributeData? GetAttribute(this ISymbol symbol, string attributeName, Compilation compilation)
    {
        var serializableSymbol =
            compilation.GetTypeByMetadataName(attributeName);
        return symbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass!.Equals(serializableSymbol, SymbolEqualityComparer.Default));
    }

    // 在文件顶部添加扩展方法定义（可放在文件末尾或合适位置）
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