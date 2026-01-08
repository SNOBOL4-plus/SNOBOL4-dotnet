namespace Snobol4.Common;

/// <summary>
/// Cloning strategy for table variables
/// Creates a deep copy of the table
/// </summary>
public class TableCloningStrategy : ICloningStrategy
{
    public Var Clone(Var self)
    {
        var tableSelf = (TableVar)self;

        // Clone the fill value
        var clonedFill = tableSelf.Fill.Clone();
        var clonedTable = new TableVar(clonedFill)
        {
            Symbol = tableSelf.Symbol,
            InputChannel = tableSelf.InputChannel,
            OutputChannel = tableSelf.OutputChannel
        };

        // Deep copy all key-value pairs
        foreach (var kvp in tableSelf.Data)
        {
            // Clone the value (keys are immutable primitives)
            clonedTable.Data[kvp.Key] = kvp.Value.Clone();
        }

        return clonedTable;
    }
}