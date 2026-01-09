using System.Runtime.CompilerServices;

namespace Snobol4.Common;

/// <summary>
/// Arithmetic strategy for statement separator
/// Statement separators don't support any arithmetic operations
/// </summary>
public sealed class StatementSeparatorArithmeticStrategy : IArithmeticStrategy
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Var Add(Var self, Var other, Executive executive)
    {
        throw new InvalidOperationException("Statement separators cannot participate in arithmetic operations");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Var Subtract(Var self, Var other, Executive executive)
    {
        throw new InvalidOperationException("Statement separators cannot participate in arithmetic operations");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Var Multiply(Var self, Var other, Executive executive)
    {
        throw new InvalidOperationException("Statement separators cannot participate in arithmetic operations");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Var Divide(Var self, Var other, Executive executive)
    {
        throw new InvalidOperationException("Statement separators cannot participate in arithmetic operations");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Var Power(Var self, Var other, Executive executive)
    {
        throw new InvalidOperationException("Statement separators cannot participate in arithmetic operations");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Var Negate(Var self, Executive executive)
    {
        throw new InvalidOperationException("Statement separators cannot participate in arithmetic operations");
    }
}