namespace ShadowObservableConfig;

/// <summary>
/// 定义配置加载器的接口，用于保存和加载配置文件
/// </summary>
public interface IConfigLoader
{
    /// <summary>
    /// 获取配置文件扩展名
    /// </summary>
    string Ext { get; }
    
    /// <summary>
    /// 将对象保存到指定的配置文件路径
    /// </summary>
    /// <param name="configPath">配置文件路径</param>
    /// <param name="obj">要保存的对象</param>
    void Save(string configPath, object obj);
    
    /// <summary>
    /// 从指定的配置文件路径加载配置对象
    /// </summary>
    /// <typeparam name="T">配置类型，必须继承自BaseConfig</typeparam>
    /// <param name="configPath">配置文件路径</param>
    /// <returns>加载的配置对象，如果加载失败则返回null</returns>
    T? Load<T>(string configPath) where T : BaseConfig;
}