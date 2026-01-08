using System.Diagnostics;

namespace Snobol4.Common;

[DebuggerDisplay("{DebugString()}")]
public class ArrayVar : Var
{
    #region Data

    internal List<long> Sizes = [];
    internal List<Var> Data = [];
    internal long Dimensions;
    internal long TotalSize = 1;
    internal string Prototype = "";
    internal Var Fill = StringVar.Null();
    internal List<long> LowerBounds = [];
    internal List<long> Multipliers = [];
    internal List<long> UpperBounds = [];

    #endregion

    #region Strategy Instances (Lazy-loaded singletons for performance)

    private static readonly ArrayArithmeticStrategy _arithmeticStrategy = new();
    private static readonly ArrayComparisonStrategy _comparisonStrategy = new();
    private static readonly ArrayConversionStrategy _conversionStrategy = new();
    private static readonly ArrayCloningStrategy _cloningStrategy = new();
    private static readonly ArrayFormattingStrategy _formattingStrategy = new();

    protected override IArithmeticStrategy ArithmeticStrategy => _arithmeticStrategy;
    protected override IComparisonStrategy ComparisonStrategy => _comparisonStrategy;
    protected override IConversionStrategy ConversionStrategy => _conversionStrategy;
    protected override ICloningStrategy CloningStrategy => _cloningStrategy;
    protected override IFormattingStrategy FormattingStrategy => _formattingStrategy;

    #endregion

    #region Array-Specific Methods

    /// <summary>
    /// Configure array dimensions and bounds from prototype string
    /// </summary>
    /// <param name="prototype">Prototype string (e.g., "1:10,1:20")</param>
    /// <param name="fill">Fill value for array elements</param>
    /// <returns>0 on success, error code otherwise</returns>
    internal int ConfigurePrototype(string prototype, Var fill)
    {
        Fill = fill;

        while (prototype.Length > 0)
        {
            var match = CompiledRegex.ArrayPrototypePattern().Match(prototype);
            prototype = prototype[match.Length..];
            if (!match.Success)
                return 65;

            long lower = 1;
            long upper;
            if (match.Groups[3].Success)
            {
                if (!ToInteger(match.Groups[1].Value, out lower))
                    return 65;

                if (!ToInteger(match.Groups[3].Value, out upper))
                    return 66;
            }
            else
            {
                if (!ToInteger(match.Groups[1].Value, out upper))
                    return 67;
            }

            if (lower > upper)
                return 67;

            LowerBounds.Insert(0, lower);
            UpperBounds.Insert(0, upper);
            Dimensions++;
            Sizes.Insert(0, upper - lower + 1);
        }

        Multipliers.Add(1);
        for (var j = 0; j < Dimensions - 1; ++j)
            Multipliers.Add(Multipliers[^1] * Sizes[j]);

        TotalSize = Multipliers[^1] * Sizes[^1];

        for (var i = 0; i < TotalSize; i++)
            Data.Add(Fill);

        Prototype = "";
        for (var d = Dimensions - 1; d >= 0; --d)
        {
            Prototype += $"{LowerBounds[(int)d]}:{UpperBounds[(int)d]}";
            if (d > 0)
                Prototype += ",";
        }

        return 0;
    }

    /// <summary>
    /// Convert multi-dimensional indices to linear index
    /// </summary>
    /// <param name="indices">List of dimension indices</param>
    /// <returns>Linear index into Data array</returns>
    internal long Index(List<long> indices)
    {
        long key = 0;

        for (var i = 0; i < Dimensions; ++i)
            key += (indices[i] - LowerBounds[i]) * Multipliers[i];

        return key;
    }

    #endregion

    #region Double Dispatch Methods

    // Arrays don't support arithmetic operations with other types

    protected internal override Var AddInteger(IntegerVar left, Executive executive)
    {
        executive.LogRuntimeException(2); // Right operand of + is not numeric
        return StringVar.Null();
    }

    protected internal override Var AddReal(RealVar left, Executive executive)
    {
        executive.LogRuntimeException(2); // Right operand of + is not numeric
        return StringVar.Null();
    }

    protected internal override Var SubtractInteger(IntegerVar left, Executive executive)
    {
        executive.LogRuntimeException(33); // Right operand of - is not numeric
        return StringVar.Null();
    }

    protected internal override Var SubtractReal(RealVar left, Executive executive)
    {
        executive.LogRuntimeException(33); // Right operand of - is not numeric
        return StringVar.Null();
    }

    protected internal override Var MultiplyInteger(IntegerVar left, Executive executive)
    {
        executive.LogRuntimeException(27); // Right operand of * is not numeric
        return StringVar.Null();
    }

    protected internal override Var MultiplyReal(RealVar left, Executive executive)
    {
        executive.LogRuntimeException(27); // Right operand of * is not numeric
        return StringVar.Null();
    }

    protected internal override Var DivideInteger(IntegerVar left, Executive executive)
    {
        executive.LogRuntimeException(13); // Right operand of / is not numeric
        return StringVar.Null();
    }

    protected internal override Var DivideReal(RealVar left, Executive executive)
    {
        executive.LogRuntimeException(13); // Right operand of / is not numeric
        return StringVar.Null();
    }

    #endregion
}