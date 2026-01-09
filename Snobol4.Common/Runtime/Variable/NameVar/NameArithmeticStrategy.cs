using System.Runtime.CompilerServices;

namespace Snobol4.Common;

/// <summary>
/// Arithmetic strategy for name variables
/// Names support arithmetic by dereferencing to the target variable
/// </summary>
public class NameArithmeticStrategy : IArithmeticStrategy
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Var Add(Var self, Var other, Executive executive)
    {
        var nameSelf = (NameVar)self;

        // Dereference the name to get the actual variable
        var target = GetDereferencedValue(nameSelf, executive);

        // Perform operation on the dereferenced value
        return target.Add(other, executive);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Var Subtract(Var self, Var other, Executive executive)
    {
        var nameSelf = (NameVar)self;
        var target = GetDereferencedValue(nameSelf, executive);
        return target.Subtract(other, executive);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Var Multiply(Var self, Var other, Executive executive)
    {
        var nameSelf = (NameVar)self;
        var target = GetDereferencedValue(nameSelf, executive);
        return target.Multiply(other, executive);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Var Divide(Var self, Var other, Executive executive)
    {
        var nameSelf = (NameVar)self;
        var target = GetDereferencedValue(nameSelf, executive);
        return target.Divide(other, executive);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Var Power(Var self, Var other, Executive executive)
    {
        var nameSelf = (NameVar)self;
        var target = GetDereferencedValue(nameSelf, executive);
        return target.Power(other, executive);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Var Negate(Var self, Executive executive)
    {
        var nameSelf = (NameVar)self;
        var target = GetDereferencedValue(nameSelf, executive);
        return target.Negate(executive);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Var GetDereferencedValue(NameVar nameVar, Executive executive)
    {
        // Fast path: pointer dereference (most common case)
        if (nameVar.Collection is null)
        {
            return executive.IdentifierTable[nameVar.Pointer];
        }

        // Slower path: collection element access
        return nameVar.Collection switch
        {
            ArrayVar arrayVar => arrayVar.Data[(int)(long)nameVar.Key!],
            TableVar tableVar => tableVar.Data.TryGetValue(nameVar.Key!, out var value)
                ? value
                : tableVar.Fill.Clone(),
            _ => StringVar.Null()
        };
    }
}