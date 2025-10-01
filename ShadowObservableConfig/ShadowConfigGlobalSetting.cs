namespace ShadowObservableConfig;

public static class ShadowConfigGlobalSetting
{
    public static IShadowConfigSetting ConfigSetting { get; private set; } = null!;

    public static void Init(IShadowConfigSetting configSetting)
    {
        ConfigSetting = configSetting;
    }
}