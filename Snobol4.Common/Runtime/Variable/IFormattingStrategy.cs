namespace Snobol4.Common;

/// <summary>
/// Strategy interface for formatting/display operations on variables
/// </summary>
public interface IFormattingStrategy
{
    /// <summary>
    /// Convert variable to string representation
    /// </summary>
    string ToString(Var self);

    /// <summary>
    /// Get dump string for debugging/diagnostics
    /// </summary>
    string DumpString(Var self);

    /// <summary>
    /// Get debug string with detailed information
    /// </summary>
    string DebugString(Var self);
}