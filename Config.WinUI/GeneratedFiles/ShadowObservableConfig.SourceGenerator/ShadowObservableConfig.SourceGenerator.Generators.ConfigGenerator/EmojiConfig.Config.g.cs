// Automatic Generate From ShadowObservableConfig.SourceGenerator
using global::System.ComponentModel;
using global::System.Runtime.CompilerServices;

namespace Config.WinUI;

/// <summary>
/// Emoji插件配置
/// Version: 1.0.0
/// </summary>
public partial class EmojiConfig : global::ShadowObservableConfig.BaseConfig
{ 
    [global::YamlDotNet.Serialization.YamlIgnore] 
    protected static global::ShadowObservableConfig.ConfigFileInfo Info => new global::ShadowObservableConfig.ConfigFileInfo("emoji_config", "config");
    
    /// <summary>
    /// Constructor
    /// </summary>
    public EmojiConfig()
    {
    }
    public void Init()
    {
        Initialized = true;
        ConfigChanged += InvokeSaveFileOnChange;
        AfterConfigInit();
    }
    public static EmojiConfig Load()
    {
        var configDir = global::System.IO.Path.GetDirectoryName(Info.ConfigFilePath);
        if (configDir == null) throw new global::System.ArgumentNullException($"{nameof(Info.ConfigFilePath)} not found");
        if (!global::System.IO.Directory.Exists(configDir))
        {
            global::System.IO.Directory.CreateDirectory(configDir);
        }
        var obj = global::ShadowObservableConfig.ShadowConfigGlobalSetting.ConfigSetting.ConfigLoader.Load<EmojiConfig>(Info.ConfigFilePath);
        if (obj is null)
        {
            obj = new EmojiConfig();
            obj.Save();
        }
        obj.Init();
        return obj;
    }
    
    protected void InvokeSaveFileOnChange(object sender, global::ShadowObservableConfig.Args.ConfigChangedEventArgs e)
    {
        Save();
    }
    
    public void Save()
    {
        global::ShadowObservableConfig.ShadowConfigGlobalSetting.ConfigSetting.ConfigLoader.Save(Info.ConfigFilePath, this);
    }
    
    partial void AfterConfigInit();
    
    /// <summary>
    /// 默认表情大小
    /// </summary>
    [global::YamlDotNet.Serialization.YamlMember(Description = "默认表情大小", Alias = "DefaultEmojiSize")]
    public int DefaultEmojiSize
    {
        get => _defaultEmojiSize;
        set
        {
            if (!global::System.Collections.Generic.EqualityComparer<int>.Default.Equals(_defaultEmojiSize, value))
            {
                var oldValue = _defaultEmojiSize;
                _defaultEmojiSize = value;
                OnPropertyChanged(nameof(DefaultEmojiSize));
                if (!Initialized) return;
                OnConfigChanged(nameof(DefaultEmojiSize), nameof(DefaultEmojiSize), oldValue, value, typeof(int));
            }
        }
    }
    /// <summary>
    /// 启用自动完成
    /// </summary>
    [global::YamlDotNet.Serialization.YamlMember(Description = "启用自动完成", Alias = "EnableAutoComplete")]
    public bool EnableAutoComplete
    {
        get => _enableAutoComplete;
        set
        {
            if (!global::System.Collections.Generic.EqualityComparer<bool>.Default.Equals(_enableAutoComplete, value))
            {
                var oldValue = _enableAutoComplete;
                _enableAutoComplete = value;
                OnPropertyChanged(nameof(EnableAutoComplete));
                if (!Initialized) return;
                OnConfigChanged(nameof(EnableAutoComplete), nameof(EnableAutoComplete), oldValue, value, typeof(bool));
            }
        }
    }
    /// <summary>
    /// 最大表情历史记录数
    /// </summary>
    [global::YamlDotNet.Serialization.YamlMember(Description = "最大表情历史记录数", Alias = "MaxEmojiHistory")]
    public int MaxEmojiHistory
    {
        get => _maxEmojiHistory;
        set
        {
            if (!global::System.Collections.Generic.EqualityComparer<int>.Default.Equals(_maxEmojiHistory, value))
            {
                var oldValue = _maxEmojiHistory;
                _maxEmojiHistory = value;
                OnPropertyChanged(nameof(MaxEmojiHistory));
                if (!Initialized) return;
                OnConfigChanged(nameof(MaxEmojiHistory), nameof(MaxEmojiHistory), oldValue, value, typeof(int));
            }
        }
    }
    /// <summary>
    /// 默认肤色
    /// </summary>
    [global::YamlDotNet.Serialization.YamlMember(Description = "默认肤色", Alias = "DefaultSkinTone")]
    public string DefaultSkinTone
    {
        get => _defaultSkinTone;
        set
        {
            if (!global::System.Collections.Generic.EqualityComparer<string>.Default.Equals(_defaultSkinTone, value))
            {
                var oldValue = _defaultSkinTone;
                _defaultSkinTone = value;
                OnPropertyChanged(nameof(DefaultSkinTone));
                if (!Initialized) return;
                OnConfigChanged(nameof(DefaultSkinTone), nameof(DefaultSkinTone), oldValue, value, typeof(string));
            }
        }
    }
    /// <summary>
    /// 动画速度
    /// </summary>
    [global::YamlDotNet.Serialization.YamlMember(Description = "动画速度", Alias = "AnimationSpeed")]
    public double AnimationSpeed
    {
        get => _animationSpeed;
        set
        {
            if (!global::System.Collections.Generic.EqualityComparer<double>.Default.Equals(_animationSpeed, value))
            {
                var oldValue = _animationSpeed;
                _animationSpeed = value;
                OnPropertyChanged(nameof(AnimationSpeed));
                if (!Initialized) return;
                OnConfigChanged(nameof(AnimationSpeed), nameof(AnimationSpeed), oldValue, value, typeof(double));
            }
        }
    }
    /// <summary>
    /// 表情设置
    /// </summary>
    [global::YamlDotNet.Serialization.YamlMember(Description = "表情设置", Alias = "Settings")]
    public global::Config.WinUI.NestedSettings Settings
    {
        get => _settings;
        set
        {
            if (!global::System.Collections.Generic.EqualityComparer<global::Config.WinUI.NestedSettings>.Default.Equals(_settings, value))
            {

                _settings.ConfigChanged -= OnSettingsConfigChanged;
                var oldValue = _settings;
                _settings = value;
                _settings.ConfigChanged += OnSettingsConfigChanged;
                if (!Initialized) return;
                OnPropertyChanged(nameof(Settings));
                OnConfigChanged(nameof(Settings), nameof(Settings), oldValue, value, typeof(global::Config.WinUI.NestedSettings));
            }
        }
    }
    
    /// <summary>
    /// Settings实体变更事件处理
    /// </summary>
    private void OnSettingsConfigChanged(object sender, global::ShadowObservableConfig.Args.ConfigChangedEventArgs e)
    {
        // 使用实体传递的完整路径，如果没有则构建：外部属性名.内部属性名
        var fullPropertyPath = string.IsNullOrEmpty(e.FullPropertyPath) 
            ? $"Settings.{e.PropertyName}" 
            : $"Settings.{e.FullPropertyPath}";
        OnConfigChanged(nameof(Settings), fullPropertyPath, e.OldValue, e.NewValue, e.PropertyType);
    }





}