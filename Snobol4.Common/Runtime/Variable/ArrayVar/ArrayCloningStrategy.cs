namespace Snobol4.Common;

/// <summary>
/// Cloning strategy for array variables.
/// Creates a deep copy of the array including all elements and metadata.
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
            OutputChannel = arraySelf.OutputChannel,
            Succeeded = arraySelf.Succeeded
        };

        // Deep copy dimension metadata with capacity pre-allocation
        CloneDimensionMetadata(arraySelf, clonedArray);

        // Deep copy data elements with capacity pre-allocation
        CloneDataElements(arraySelf, clonedArray);

        // Clone fill value
        clonedArray.Fill = arraySelf.Fill.Clone();

        return clonedArray;
    }

    /// <summary>
    /// Clone dimension metadata lists
    /// </summary>
    private static void CloneDimensionMetadata(ArrayVar source, ArrayVar target)
    {
        target.Sizes.AddRange(source.Sizes);
        target.LowerBounds.AddRange(source.LowerBounds);
        target.UpperBounds.AddRange(source.UpperBounds);
        target.Multipliers.AddRange(source.Multipliers);
    }

    /// <summary>
    /// Clone data elements
    /// </summary>
    private static void CloneDataElements(ArrayVar source, ArrayVar target)
    {
        target.Data.Capacity = source.Data.Count;
        foreach (var item in source.Data)
        {
            target.Data.Add(item.Clone());
        }
    }
}