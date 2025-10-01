using ShadowObservableConfig.Args;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShadowObservableConfig;

/// <summary>
/// 提供配置类的基础功能，包括属性变更通知和配置变更事件
/// </summary>
public abstract class BaseConfig : INotifyPropertyChanged
{
    /// <summary>
    /// 指示配置是否已初始化
    /// </summary>
    protected bool Initialized = false;

    /// <summary>
    /// 指示此配置是否为根配置
    /// </summary>
    public virtual bool IsRootConfig => false;

    /// <summary>
    /// Property changed event that is triggered when any property value changes
    /// Used to support WPF/WinUI data binding mechanism
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Config changed event that is triggered when config properties change
    /// Used for nested config change notifications
    /// </summary>
    public event EventHandler<ConfigChangedEventArgs>? ConfigChanged;

    /// <summary>
    /// Triggers the property changed event
    /// Call this method when property values change to notify UI updates
    /// </summary>
    /// <param name="propertyName">The name of the changed property, usually provided automatically by the compiler</param>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Triggers the config changed event with full property path
    /// Call this method when config properties change for nested config notifications with full path
    /// </summary>
    /// <param name="propertyName">The name of the changed property</param>
    /// <param name="fullPropertyPath">The full property path from root</param>
    /// <param name="oldValue">The old value of the property</param>
    /// <param name="newValue">The new value of the property</param>
    /// <param name="type">The type of the property</param>
    /// <param name="autoSave">Whether to automatically save the configuration file when this change occurs</param>
    protected virtual void OnConfigChanged(string propertyName, string fullPropertyPath, object oldValue, object newValue, Type type, bool autoSave = true)
    {
        ConfigChanged?.Invoke(this, new ConfigChangedEventArgs(propertyName, fullPropertyPath, oldValue, newValue, type, autoSave));
    }

}