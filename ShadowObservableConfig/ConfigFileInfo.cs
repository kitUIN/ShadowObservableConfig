namespace ShadowObservableConfig;

public class ConfigFileInfo
{
    public ConfigFileInfo(string fileName, string dirFolder)
    {
        DirFolder = dirFolder;
        FileName = fileName + ShadowConfigGlobalSetting.ConfigSetting.ConfigLoader.Ext;
        ConfigFilePath = Path.Combine(
            ShadowConfigGlobalSetting.ConfigSetting.RootFolder, DirFolder, FileName);
    }

    public string ConfigFilePath { get; }
    public string DirFolder { get; }
    public string FileName { get; }
}