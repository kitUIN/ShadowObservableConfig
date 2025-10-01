namespace ShadowObservableConfig;

/// <summary>
/// 定义影子配置设置的接口，包含根文件夹和配置加载器
/// </summary>
public interface IShadowConfigSetting
{
    /// <summary>
    /// 获取配置文件的根文件夹路径
    /// </summary>
    public string RootFolder { get; }
    
    /// <summary>
    /// 获取配置加载器实例
    /// </summary>
    public IConfigLoader ConfigLoader { get; }
}