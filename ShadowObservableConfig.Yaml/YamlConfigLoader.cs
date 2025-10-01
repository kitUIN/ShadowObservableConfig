namespace ShadowObservableConfig.Yaml;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using ShadowObservableConfig;

/// <summary>
/// 提供YAML格式的配置文件加载和保存功能
/// </summary>
public class YamlConfigLoader: IConfigLoader
{
    /// <summary>
    /// 获取YAML文件扩展名
    /// </summary>
    public string Ext => ".yaml";

    private readonly ISerializer _serializer = new SerializerBuilder()
        .Build();

    private readonly IDeserializer _deserializer = new DeserializerBuilder()
        .IgnoreUnmatchedProperties()
        .Build();

    /// <summary>
    /// 从指定的YAML配置文件路径加载配置对象
    /// </summary>
    /// <typeparam name="T">配置类型，必须继承自BaseConfig</typeparam>
    /// <param name="configPath">YAML配置文件路径</param>
    /// <returns>加载的配置对象，如果文件不存在或加载失败则返回null</returns>
    public T? Load<T>(string configPath) where T : BaseConfig
    {
        if (!File.Exists(configPath)) return null;
        var yaml = File.ReadAllText(configPath);
        return _deserializer.Deserialize<T>(yaml);
    }

    /// <summary>
    /// 将对象保存到指定的YAML配置文件路径
    /// </summary>
    /// <param name="configPath">YAML配置文件路径</param>
    /// <param name="obj">要保存的对象</param>
    public void Save(string configPath, object obj)
    {
        var yaml = _serializer.Serialize(obj);
        File.WriteAllText(configPath, yaml);
    }
}