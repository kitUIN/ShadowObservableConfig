using Windows.Storage;
using ShadowObservableConfig;

namespace ShadowObservableConfig.Yaml;

/// <summary>
/// 提供YAML格式的配置设置实现
/// </summary>
public class ShadowYamlConfigSetting : IShadowConfigSetting
{
    /// <summary>
    /// 获取配置文件的根文件夹路径，默认为应用程序本地文件夹
    /// </summary>
    public string RootFolder { get; } = ApplicationData.Current.LocalFolder.Path;
    
    /// <summary>
    /// 获取YAML配置加载器实例
    /// </summary>
    public IConfigLoader ConfigLoader { get; } = new YamlConfigLoader();
}