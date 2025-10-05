using System.Data;

namespace ShadowObservableConfig;

/// <summary>
/// 提供全局配置设置的静态访问点
/// </summary>
public static class GlobalSetting
{
    /// <summary>
    /// 获取配置加载器实例
    /// </summary>
    public static List<IConfigLoader> ConfigLoaders { get; } = [];

    /// <summary>
    /// 获取配置文件的根文件夹路径
    /// </summary>
    public static string RootFolder => _rootFolder ?? throw new NoNullAllowedException("RootFolder Can't Be Null");

    private static string? _rootFolder;

    /// <summary>
    /// 初始化全局配置设置
    /// </summary>
    public static void Init(string rootFolder, IEnumerable<IConfigLoader> loaders)
    {
        foreach (var loader in loaders)
        {
            ConfigLoaders.Add(loader);
        }

        _rootFolder = rootFolder;
    }
    /// <summary>
    /// GetConfigLoader
    /// </summary>
    /// <param name="ext"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static IConfigLoader GetConfigLoader(string ext)
    {
        var loader = ConfigLoaders.FirstOrDefault(l => l.Ext.Equals(ext, StringComparison.OrdinalIgnoreCase));
        return loader ?? throw new NotSupportedException($"No ConfigLoader Support Ext: {ext}");
    }

}