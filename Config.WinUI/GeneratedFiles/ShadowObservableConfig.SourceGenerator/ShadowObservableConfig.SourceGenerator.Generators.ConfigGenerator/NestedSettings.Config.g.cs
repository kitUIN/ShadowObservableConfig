// Automatic Generate From ShadowObservableConfig.SourceGenerator
using global::System.ComponentModel;
using global::System.Runtime.CompilerServices;

namespace Config.WinUI;

/// <summary>
/// 嵌套配置
/// Version: 1.0.0
/// </summary>
public partial class NestedSettings : global::ShadowObservableConfig.BaseConfig
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public NestedSettings()
    {
        Initialized = true;
        AfterConfigInit();
    }

    /// <summary>
    /// 嵌套值
    /// </summary>
    [global::YamlDotNet.Serialization.YamlMember(Description = "嵌套值", Alias = "NestedValue")]
    public string NestedValue
    {
        get => _nestedValue;
        set
        {
            if (!global::System.Collections.Generic.EqualityComparer<string>.Default.Equals(_nestedValue, value))
            {
                var oldValue = _nestedValue;
                _nestedValue = value;
                OnPropertyChanged(nameof(NestedValue));
                OnConfigChanged(nameof(NestedValue), nameof(NestedValue), oldValue, value, typeof(string));
            }
        }
    }
    /// <summary>
    /// 嵌套数字
    /// </summary>
    [global::YamlDotNet.Serialization.YamlMember(Description = "嵌套数字", Alias = "NestedNumber")]
    public int NestedNumber
    {
        get => _nestedNumber;
        set
        {
            if (!global::System.Collections.Generic.EqualityComparer<int>.Default.Equals(_nestedNumber, value))
            {
                var oldValue = _nestedNumber;
                _nestedNumber = value;
                OnPropertyChanged(nameof(NestedNumber));
                OnConfigChanged(nameof(NestedNumber), nameof(NestedNumber), oldValue, value, typeof(int));
            }
        }
    }
    /// <summary>
    /// 嵌套布尔值
    /// </summary>
    [global::YamlDotNet.Serialization.YamlMember(Description = "嵌套布尔值", Alias = "NestedBoolean")]
    public bool NestedBoolean
    {
        get => _nestedBoolean;
        set
        {
            if (!global::System.Collections.Generic.EqualityComparer<bool>.Default.Equals(_nestedBoolean, value))
            {
                var oldValue = _nestedBoolean;
                _nestedBoolean = value;
                OnPropertyChanged(nameof(NestedBoolean));
                OnConfigChanged(nameof(NestedBoolean), nameof(NestedBoolean), oldValue, value, typeof(bool));
            }
        }
    }


partial void AfterConfigInit();
}