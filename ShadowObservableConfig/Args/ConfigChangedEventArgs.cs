namespace ShadowObservableConfig.Args;
/// <summary>
/// Configuration change event arguments containing property change information and file information
/// </summary>
/// <param name="PropertyName">The name of the changed property</param>
/// <param name="FullPropertyPath">The full name of the changed property</param>
/// <param name="OldValue">The old value of the property</param>
/// <param name="NewValue">The new value of the property</param>
/// <param name="PropertyType">The type of the property</param>
/// <param name="AutoSave">Whether to automatically save the configuration file when this change occurs</param>
public record ConfigChangedEventArgs(
    string PropertyName,
    string FullPropertyPath,
    object OldValue,
    object NewValue,
    Type PropertyType,
    bool AutoSave = true
);