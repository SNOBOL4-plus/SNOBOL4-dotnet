using System.Diagnostics;

namespace Snobol4.Common;

[DebuggerDisplay("{DebugString()}")]
public class TableVar : Var
{
    #region Data

    internal Dictionary<object, Var> Data;
    internal readonly Var Fill;

    #endregion

    #region Strategy Instances (Lazy-loaded singletons for performance)

    private static readonly TableArithmeticStrategy _arithmeticStrategy = new();
    private static readonly TableComparisonStrategy _comparisonStrategy = new();
    private static readonly TableConversionStrategy _conversionStrategy = new();
    private static readonly TableCloningStrategy _cloningStrategy = new();
    private static readonly TableFormattingStrategy _formattingStrategy = new();

    protected override IArithmeticStrategy ArithmeticStrategy => _arithmeticStrategy;
    protected override IComparisonStrategy ComparisonStrategy => _comparisonStrategy;
    protected override IConversionStrategy ConversionStrategy => _conversionStrategy;
    protected override ICloningStrategy CloningStrategy => _cloningStrategy;
    protected override IFormattingStrategy FormattingStrategy => _formattingStrategy;

    #endregion

    #region Constructors

    internal TableVar(Var fill)
    {
        Data = [];
        Fill = fill;
    }

    #endregion

    #region Table-Specific Methods

    /// <summary>
    /// Get value by key, returning fill value if key doesn't exist
    /// </summary>
    internal Var GetOrDefault(object key)
    {
        return Data.TryGetValue(key, out var value) ? value : Fill.Clone();
    }

    /// <summary>
    /// Set value by key
    /// </summary>
    internal void Set(object key, Var value)
    {
        value.Key = key;
        value.Collection = this;
        Data[key] = value;
    }

    /// <summary>
    /// Check if table contains a key
    /// </summary>
    internal bool ContainsKey(object key)
    {
        return Data.ContainsKey(key);
    }

    /// <summary>
    /// Remove a key-value pair from the table
    /// </summary>
    internal bool Remove(object key)
    {
        return Data.Remove(key);
    }

    /// <summary>
    /// Clear all entries from the table
    /// </summary>
    internal void Clear()
    {
        Data.Clear();
    }

    #endregion

    #region Double Dispatch Methods

    // Tables don't support arithmetic operations with other types

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