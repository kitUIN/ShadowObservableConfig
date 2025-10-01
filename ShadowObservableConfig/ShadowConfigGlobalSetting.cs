namespace ShadowObservableConfig;

/// <summary>
/// 提供全局配置设置的静态访问点
/// </summary>
public static class ShadowConfigGlobalSetting
{
    /// <summary>
    /// 获取或设置全局配置设置实例
    /// </summary>
    public static IShadowConfigSetting ConfigSetting { get; private set; } = null!;

    /// <summary>
    /// 初始化全局配置设置
    /// </summary>
    /// <param name="configSetting">要设置的配置设置实例</param>
    public static void Init(IShadowConfigSetting configSetting)
    {
        ConfigSetting = configSetting;
    }
}