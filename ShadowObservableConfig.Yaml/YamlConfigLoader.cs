namespace ShadowObservableConfig.Yaml;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
public class YamlConfigLoader: IConfigLoader
{
    public string Ext => ".yaml";

    private readonly ISerializer _serializer = new SerializerBuilder()
        .Build();

    private readonly IDeserializer _deserializer = new DeserializerBuilder()
        .IgnoreUnmatchedProperties()
        .Build();

    public T? Load<T>(string configPath) where T : BaseConfig
    {
        if (!File.Exists(configPath)) return null;
        var yaml = File.ReadAllText(configPath);
        return _deserializer.Deserialize<T>(yaml);
    }

    public void Save(string configPath, object obj)
    {
        var yaml = _serializer.Serialize(obj);
        File.WriteAllText(configPath, yaml);
    }
}