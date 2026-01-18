namespace Snobol4.Common;

public partial class Executive
{
    #region  Binary Comparison Helper Function

    private void BinaryComparison(List<Var> arguments, IntegerCompare integerCompare, RealCompare realCompare, int errorLeft, int errorRight)
    {
        if (!Var.ToNumeric(arguments[0], out var isIntegerLeft, out var lLeft, out var dLeft, this))
        {
            LogRuntimeException(errorLeft);
            return;
        }

        if (!Var.ToNumeric(arguments[1], out var isIntegerRight, out var lRight, out var dRight, this))
        {
            LogRuntimeException(errorRight);
            return;
        }

        switch (isIntegerLeft)
        {
            case true when isIntegerRight:
            {
                if (integerCompare(lLeft, lRight))
                {
                    PredicateSuccess();
                    return;
                }

                NonExceptionFailure();
                return;
            }
            case true:
                dLeft = System.Convert.ToDouble(lRight);
                break;
        }

        if (isIntegerRight)
            dRight = System.Convert.ToDouble(lRight);

        if (realCompare(dLeft, dRight))
        {
            PredicateSuccess();
            return;
        }

        NonExceptionFailure();
    }

    #endregion
}