using ShadowObservableConfig.Args;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShadowObservableConfig;

public abstract class BaseConfig : INotifyPropertyChanged
{
    protected bool Initialized = false;

    protected bool IsRootConfig = false;

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
    protected virtual void OnConfigChanged(string propertyName, string fullPropertyPath, object oldValue, object newValue, Type type)
    {
        ConfigChanged?.Invoke(this, new ConfigChangedEventArgs(propertyName, fullPropertyPath, oldValue, newValue, type));
    }

}