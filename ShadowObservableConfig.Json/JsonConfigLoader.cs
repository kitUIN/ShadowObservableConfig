namespace ShadowObservableConfig.Json;

using Newtonsoft.Json;
using ShadowObservableConfig;

/// <summary>
/// 提供Json格式的配置文件加载和保存功能
/// </summary>
public class JsonConfigLoader : IConfigLoader
{
    /// <summary>
    /// 获取Json文件扩展名
    /// </summary>
    public string Ext => ".json";

    /// <summary>
    /// 
    /// </summary>
    public JsonSerializerSettings SerializerSetting { get; set; } = new();

    /// <inheritdoc />
    public object? Load(string configPath, Type type)
    {
        if (!File.Exists(configPath)) return null;
        var json = File.ReadAllText(configPath);
        return JsonConvert.DeserializeObject(json, type, SerializerSetting);
    }

    /// <inheritdoc />
    public T? Load<T>(string configPath) where T : BaseConfig
    {
        return Load(configPath, typeof(T)) as T;
    }

    /// <inheritdoc />
    public void Save(string configPath, object obj)
    {
        var json = JsonConvert.SerializeObject(obj, SerializerSetting);
        File.WriteAllText(configPath, json);
    }
}