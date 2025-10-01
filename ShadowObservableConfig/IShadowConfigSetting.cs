namespace ShadowObservableConfig;

public interface IShadowConfigSetting
{
    public string RootFolder { get; }
    public IConfigLoader ConfigLoader { get; }
}