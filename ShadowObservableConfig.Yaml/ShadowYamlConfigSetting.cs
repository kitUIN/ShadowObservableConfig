using Windows.Storage;

namespace ShadowObservableConfig.Yaml;
public class ShadowYamlConfigSetting : IShadowConfigSetting
{
    public string RootFolder { get; } = ApplicationData.Current.LocalFolder.Path;
    public IConfigLoader ConfigLoader { get; } = new YamlConfigLoader();
}