namespace Snobol4.Common;

/// <summary>
/// Cloning strategy for array variables
/// Creates a deep copy of the array structure
/// </summary>
public class ArrayCloningStrategy : ICloningStrategy
{
    public Var Clone(Var self)
    {
        var arraySelf = (ArrayVar)self;

        var clonedArray = new ArrayVar
        {
            Symbol = arraySelf.Symbol,
            Prototype = arraySelf.Prototype,
            Dimensions = arraySelf.Dimensions,
            TotalSize = arraySelf.TotalSize,
            InputChannel = arraySelf.InputChannel,
            OutputChannel = arraySelf.OutputChannel
        };

        // Deep copy lists
        clonedArray.Sizes = new List<long>(arraySelf.Sizes);
        clonedArray.LowerBounds = new List<long>(arraySelf.LowerBounds);
        clonedArray.UpperBounds = new List<long>(arraySelf.UpperBounds);
        clonedArray.Multipliers = new List<long>(arraySelf.Multipliers);

        // Deep copy data elements
        clonedArray.Data = new List<Var>(arraySelf.Data.Count);
        foreach (var item in arraySelf.Data)
        {
            clonedArray.Data.Add(item.Clone());
        }

        // Clone fill value
        clonedArray.Fill = arraySelf.Fill.Clone();

        return clonedArray;
    }
}