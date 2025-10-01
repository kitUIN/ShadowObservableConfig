namespace ShadowObservableConfig;

/// <summary>
/// 表示配置文件的信息，包括文件路径、目录和文件名
/// </summary>
public class ConfigFileInfo
{
    /// <summary>
    /// 初始化ConfigFileInfo的新实例
    /// </summary>
    /// <param name="fileName">配置文件名（不包含扩展名）</param>
    /// <param name="dirFolder">配置文件所在的目录文件夹</param>
    public ConfigFileInfo(string fileName, string dirFolder)
    {
        DirFolder = dirFolder;
        FileName = fileName + ShadowConfigGlobalSetting.ConfigSetting.ConfigLoader.Ext;
        ConfigFilePath = Path.Combine(
            ShadowConfigGlobalSetting.ConfigSetting.RootFolder, DirFolder, FileName);
    }

    /// <summary>
    /// 获取配置文件的完整路径
    /// </summary>
    public string ConfigFilePath { get; }
    
    /// <summary>
    /// 获取配置文件所在的目录文件夹
    /// </summary>
    public string DirFolder { get; }
    
    /// <summary>
    /// 获取配置文件名（包含扩展名）
    /// </summary>
    public string FileName { get; }
}