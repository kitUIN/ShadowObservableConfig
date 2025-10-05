namespace ShadowObservableConfig.Attributes;

/// <summary>
/// Used to identify this class as an Observable Configuration Class
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ObservableConfigAttribute: Attribute
{
    /// <summary>
    /// Configuration file name
    /// </summary>
    public string? FileName { get; init; }

    /// <summary>
    /// Configuration file extension (with dot)
    /// </summary>
    public string? Ext { get; init; } = ".yaml";

    /// <summary>
    /// Configuration file directory
    /// </summary>
    public string? DirPath { get; init; } = "config";

    /// <summary>
    /// Configuration description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Configuration version
    /// </summary>
    public string? Version { get; init; }
}