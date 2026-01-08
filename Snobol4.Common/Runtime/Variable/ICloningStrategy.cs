namespace Snobol4.Common;

/// <summary>
/// Strategy interface for cloning operations on variables
/// </summary>
public interface ICloningStrategy
{
    /// <summary>
    /// Create a deep copy of this variable
    /// </summary>
    Var Clone(Var self);
}