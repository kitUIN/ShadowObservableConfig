namespace ShadowObservableConfig.Attributes;

/// <summary>
/// Used to identify this property as an Observable Configuration Property
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ObservableConfigPropertyAttribute : Attribute
{
    /// <summary>
    /// Property name
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Property description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Property alias for serialization
    /// </summary>
    public string? Alias { get; init; }

    /// <summary>
    /// Property ignore
    /// </summary>
    public bool Ignore { get; init; }

    /// <summary>
    /// If true, the configuration will be automatically saved when a property value changes
    /// </summary>
    public bool AutoSave { get; init; } = true;
}