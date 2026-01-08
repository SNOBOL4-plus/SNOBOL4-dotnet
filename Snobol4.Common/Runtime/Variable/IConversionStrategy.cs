namespace Snobol4.Common;

/// <summary>
/// Strategy interface for type conversion operations on variables
/// </summary>
public interface IConversionStrategy
{
    /// <summary>
    /// Convert this variable to the specified type
    /// </summary>
    bool TryConvert(Var self, Executive.VarType targetType, out Var varOut, out object valueOut, Executive executive);

    /// <summary>
    /// Get the data type name of this variable
    /// </summary>
    string GetDataType(Var self);

    /// <summary>
    /// Get the key to use for table indexing
    /// </summary>
    object GetTableKey(Var self);
}