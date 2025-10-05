namespace ShadowObservableConfig.Yaml;

using YamlDotNet.Serialization;
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

    /// <inheritdoc />
    public object? Load(string configPath, Type type)
    {
        if (!File.Exists(configPath)) return null;
        var yaml = File.ReadAllText(configPath);
        return _deserializer.Deserialize(yaml, type);
    }
    /// <inheritdoc />
    public T? Load<T>(string configPath) where T : BaseConfig
    {
        if (!File.Exists(configPath)) return null;
        var yaml = File.ReadAllText(configPath);
        return _deserializer.Deserialize<T>(yaml);
    }

    /// <inheritdoc />
    public void Save(string configPath, object obj)
    {
        var yaml = _serializer.Serialize(obj);
        File.WriteAllText(configPath, yaml);
    }
}