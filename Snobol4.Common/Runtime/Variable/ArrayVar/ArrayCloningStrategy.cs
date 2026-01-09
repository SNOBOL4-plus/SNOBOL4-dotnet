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
        var dimensions = (int)arraySelf.Dimensions;
        var totalSize = (int)arraySelf.TotalSize;

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

        // Deep copy dimension metadata with exact capacity pre-allocation
        CloneDimensionMetadata(arraySelf, clonedArray, dimensions);

        // Deep copy data elements with exact capacity pre-allocation
        CloneDataElements(arraySelf, clonedArray, totalSize);

        // Clone fill value
        clonedArray.Fill = arraySelf.Fill.Clone();

        return clonedArray;
    }

    /// <summary>
    /// Clone dimension metadata lists
    /// </summary>
    private static void CloneDimensionMetadata(ArrayVar source, ArrayVar target, int dimensions)
    {
        // Pre-allocate with exact capacity to avoid resizing
        target.Sizes.Capacity = dimensions;
        target.LowerBounds.Capacity = dimensions;
        target.UpperBounds.Capacity = dimensions;
        target.Multipliers.Capacity = dimensions;
        
        target.Sizes.AddRange(source.Sizes);
        target.LowerBounds.AddRange(source.LowerBounds);
        target.UpperBounds.AddRange(source.UpperBounds);
        target.Multipliers.AddRange(source.Multipliers);
    }

    /// <summary>
    /// Clone data elements
    /// </summary>
    private static void CloneDataElements(ArrayVar source, ArrayVar target, int totalSize)
    {
        target.Data.Capacity = totalSize;
        
        // Use for loop instead of foreach for better performance
        var sourceData = source.Data;
        for (var i = 0; i < totalSize; i++)
        {
            target.Data.Add(sourceData[i].Clone());
        }
    }
}