namespace ShadowObservableConfig;

public interface IConfigLoader
{
    string Ext { get; }
    void Save(string configPath, object obj);
    T? Load<T>(string configPath) where T : BaseConfig;
}