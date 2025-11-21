namespace ShadowObservableConfig.Attributes;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
public sealed class ObservableConfigPropertyChangedForAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableConfigPropertyChangedForAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">The name of the property to also notify when the annotated property changes.</param>
    public ObservableConfigPropertyChangedForAttribute(string propertyName)
    {
        PropertyNames = [propertyName];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableConfigPropertyChangedForAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">The name of the property to also notify when the annotated property changes.</param>
    /// <param name="otherPropertyNames">
    /// The other property names to also notify when the annotated property changes. This parameter can optionally
    /// be used to indicate a series of dependent properties from the same attribute, to keep the code more compact.
    /// </param>
    public ObservableConfigPropertyChangedForAttribute(string propertyName, params string[] otherPropertyNames)
    {
        PropertyNames = new[] { propertyName }.Concat(otherPropertyNames).ToArray();
    }

    /// <summary>
    /// Gets the property names to also notify when the annotated property changes.
    /// </summary>
    public string[] PropertyNames { get; }
}
