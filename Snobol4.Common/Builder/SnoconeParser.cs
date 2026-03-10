namespace Snobol4.Common;

/// <summary>
/// Snocone expression parser — Step 2.
///
/// Implements the shunting-yard algorithm from snocone.sc (binop/endexp/begexp).
/// Takes a flat List of ScToken from SnoconeLexer and returns postfix (RPN).
///
/// Reduce condition (from binop() in snocone.sc):
///   while existing_op.lp >= incoming_op.rp  → reduce
/// </summary>
public static class SnoconeParser
{
    // -------------------------------------------------------------------------
    // Precedence table — (lp, rp) from bconv in snocone.sc
    // -------------------------------------------------------------------------
    private static readonly Dictionary<SnoconeLexer.ScKind, (int lp, int rp)> _prec = new()
    {
        [SnoconeLexer.ScKind.OpAssign]    = (1,  2),
        [SnoconeLexer.ScKind.OpQuestion]  = (2,  2),
        [SnoconeLexer.ScKind.OpPipe]      = (3,  3),
        [SnoconeLexer.ScKind.OpOr]        = (4,  4),
        [SnoconeLexer.ScKind.OpConcat]    = (5,  5),
        [SnoconeLexer.ScKind.OpEq]        = (6,  6),
        [SnoconeLexer.ScKind.OpNe]        = (6,  6),
        [SnoconeLexer.ScKind.OpLt]        = (6,  6),
        [SnoconeLexer.ScKind.OpGt]        = (6,  6),
        [SnoconeLexer.ScKind.OpLe]        = (6,  6),
        [SnoconeLexer.ScKind.OpGe]        = (6,  6),
        [SnoconeLexer.ScKind.OpStrIdent]  = (6,  6),
        [SnoconeLexer.ScKind.OpStrDiffer] = (6,  6),
        [SnoconeLexer.ScKind.OpStrLt]     = (6,  6),
        [SnoconeLexer.ScKind.OpStrGt]     = (6,  6),
        [SnoconeLexer.ScKind.OpStrLe]     = (6,  6),
        [SnoconeLexer.ScKind.OpStrGe]     = (6,  6),
        [SnoconeLexer.ScKind.OpStrEq]     = (6,  6),
        [SnoconeLexer.ScKind.OpStrNe]     = (6,  6),
        [SnoconeLexer.ScKind.OpPlus]      = (7,  7),
        [SnoconeLexer.ScKind.OpMinus]     = (7,  7),
        [SnoconeLexer.ScKind.OpSlash]     = (8,  8),
        [SnoconeLexer.ScKind.OpStar]      = (8,  8),
        [SnoconeLexer.ScKind.OpPercent]   = (8,  8),
        [SnoconeLexer.ScKind.OpCaret]     = (9,  10),
        [SnoconeLexer.ScKind.OpPeriod]    = (10, 10),
        [SnoconeLexer.ScKind.OpDollar]    = (10, 10),
    };

    // Unary operator set — ANY("+-*&@~?.$") from snocone.sc
    private static readonly HashSet<SnoconeLexer.ScKind> _unaryOps = new()
    {
        SnoconeLexer.ScKind.OpPlus,   SnoconeLexer.ScKind.OpMinus,
        SnoconeLexer.ScKind.OpStar,   SnoconeLexer.ScKind.OpAmpersand,
        SnoconeLexer.ScKind.OpAt,     SnoconeLexer.ScKind.OpTilde,
        SnoconeLexer.ScKind.OpQuestion, SnoconeLexer.ScKind.OpPeriod,
        SnoconeLexer.ScKind.OpDollar,
    };

    private static bool IsOperand(SnoconeLexer.ScKind k) => k switch
    {
        SnoconeLexer.ScKind.Identifier or
        SnoconeLexer.ScKind.Integer    or
        SnoconeLexer.ScKind.Real       or
        SnoconeLexer.ScKind.String     => true,
        _ => false
    };

    // Stack frame kind — distinguishes grouping parens from call parens
    private enum FrameKind { Group, Call, ArrayCall }
    private record struct Frame(FrameKind Kind, int ArgCount, string Name, int Line, int OutputStart);

    /// <summary>
    /// Parse a flat infix token list into postfix (RPN).
    /// </summary>
    public static List<SnoconeLexer.ScToken> ParseExpression(
        List<SnoconeLexer.ScToken> tokens)
    {
        var output   = new List<SnoconeLexer.ScToken>();
        var opStack  = new Stack<SnoconeLexer.ScToken>();
        var callStack = new Stack<Frame>(); // tracks call/group frames

        int i = 0;
        while (i < tokens.Count)
        {
            var tok = tokens[i];

            // dotck: leading-dot float → prepend "0."
            if (tok.Kind == SnoconeLexer.ScKind.Real && tok.Text.StartsWith('.'))
                tok = tok with { Text = "0" + tok.Text };

            // ---- Operand ----
            if (IsOperand(tok.Kind))
            {
                // Check if immediately followed by '(' or '[' → function/array call
                var next = i + 1 < tokens.Count ? tokens[i + 1].Kind
                                                 : SnoconeLexer.ScKind.Eof;
                if (tok.Kind == SnoconeLexer.ScKind.Identifier &&
                    next == SnoconeLexer.ScKind.LParen)
                {
                    output.Add(tok);
                    callStack.Push(new Frame(FrameKind.Call, 0, tok.Text, tok.Line, output.Count));
                    opStack.Push(tokens[i + 1]);
                    i += 2;
                    continue;
                }
                if (tok.Kind == SnoconeLexer.ScKind.Identifier &&
                    next == SnoconeLexer.ScKind.LBracket)
                {
                    output.Add(tok);
                    callStack.Push(new Frame(FrameKind.ArrayCall, 0, tok.Text, tok.Line, output.Count));
                    opStack.Push(tokens[i + 1]);
                    i += 2;
                    continue;
                }
                output.Add(tok);
                i++;
                continue;
            }

            // ---- Left paren (grouping) ----
            if (tok.Kind == SnoconeLexer.ScKind.LParen)
            {
                callStack.Push(new Frame(FrameKind.Group, 0, "", tok.Line, output.Count));
                opStack.Push(tok);
                i++;
                continue;
            }

            // ---- Right paren ----
            if (tok.Kind == SnoconeLexer.ScKind.RParen)
            {
                // drain ops to matching '('
                while (opStack.Count > 0 &&
                       opStack.Peek().Kind != SnoconeLexer.ScKind.LParen)
                    output.Add(opStack.Pop());
                if (opStack.Count > 0) opStack.Pop(); // discard '('

                if (callStack.Count > 0)
                {
                    var frame = callStack.Pop();
                    if (frame.Kind == FrameKind.Call)
                    {
                        // ArgCount = commas seen; if output grew, there's argCount+1 args
                        var hasArgs = output.Count > frame.OutputStart;
                        var argCount = hasArgs ? frame.ArgCount + 1 : 0;
                        output.Add(new SnoconeLexer.ScToken(
                            SnoconeLexer.ScKind.ScCall, "()", tok.Line)
                            { ArgCount = argCount });
                    }
                }
                i++;
                continue;
            }

            // ---- Left bracket ----
            if (tok.Kind == SnoconeLexer.ScKind.LBracket)
            {
                callStack.Push(new Frame(FrameKind.Group, 0, "", tok.Line, output.Count));
                opStack.Push(tok);
                i++;
                continue;
            }

            // ---- Right bracket ----
            if (tok.Kind == SnoconeLexer.ScKind.RBracket)
            {
                while (opStack.Count > 0 &&
                       opStack.Peek().Kind != SnoconeLexer.ScKind.LBracket)
                    output.Add(opStack.Pop());
                if (opStack.Count > 0) opStack.Pop();

                if (callStack.Count > 0)
                {
                    var frame = callStack.Pop();
                    if (frame.Kind == FrameKind.ArrayCall)
                    {
                        var hasArgs = output.Count > frame.OutputStart;
                        var argCount = hasArgs ? frame.ArgCount + 1 : 0;
                        output.Add(new SnoconeLexer.ScToken(
                            SnoconeLexer.ScKind.ScArrayRef, "[]", tok.Line)
                            { ArgCount = argCount });
                    }
                }
                i++;
                continue;
            }

            // ---- Comma ----
            if (tok.Kind == SnoconeLexer.ScKind.Comma)
            {
                // drain ops to nearest open delimiter
                while (opStack.Count > 0 &&
                       opStack.Peek().Kind != SnoconeLexer.ScKind.LParen &&
                       opStack.Peek().Kind != SnoconeLexer.ScKind.LBracket)
                    output.Add(opStack.Pop());

                // increment arg count in the top call frame
                if (callStack.Count > 0)
                {
                    var frame = callStack.Pop();
                    callStack.Push(frame with { ArgCount = frame.ArgCount + 1 });
                }
                i++;
                continue;
            }

            // ---- Unary operator ----
            if (_unaryOps.Contains(tok.Kind) && IsUnaryPosition(tokens, i))
            {
                i++;
                i = ParseOperandInto(tokens, i, output, opStack, callStack);
                output.Add(tok with { IsUnary = true });
                continue;
            }

            // ---- Binary operator ----
            if (_prec.TryGetValue(tok.Kind, out var incoming))
            {
                while (opStack.Count > 0 &&
                       _prec.TryGetValue(opStack.Peek().Kind, out var top) &&
                       top.lp >= incoming.rp)
                    output.Add(opStack.Pop());
                opStack.Push(tok);
                i++;
                continue;
            }

            i++;
        }

        // endexp: drain remaining ops
        while (opStack.Count > 0 &&
               opStack.Peek().Kind != SnoconeLexer.ScKind.LParen)
            output.Add(opStack.Pop());

        return output;
    }

    // -------------------------------------------------------------------------
    // Is operator at position i in unary position?
    // -------------------------------------------------------------------------
    private static bool IsUnaryPosition(List<SnoconeLexer.ScToken> tokens, int i)
    {
        if (i == 0) return true;
        var prev = tokens[i - 1].Kind;
        return prev == SnoconeLexer.ScKind.LParen    ||
               prev == SnoconeLexer.ScKind.LBracket  ||
               prev == SnoconeLexer.ScKind.Comma     ||
               _prec.ContainsKey(prev)               ||
               _unaryOps.Contains(prev);
    }

    // -------------------------------------------------------------------------
    // Parse one complete operand (possibly prefixed by unary ops) into output.
    // Returns the index after the consumed tokens.
    // -------------------------------------------------------------------------
    private static int ParseOperandInto(
        List<SnoconeLexer.ScToken> tokens, int i,
        List<SnoconeLexer.ScToken> output,
        Stack<SnoconeLexer.ScToken> opStack,
        Stack<Frame> callStack)
    {
        if (i >= tokens.Count) return i;
        var tok = tokens[i];

        if (tok.Kind == SnoconeLexer.ScKind.Real && tok.Text.StartsWith('.'))
            tok = tok with { Text = "0" + tok.Text };

        if (IsOperand(tok.Kind))
        {
            output.Add(tok);
            return i + 1;
        }
        if (_unaryOps.Contains(tok.Kind))
        {
            int next = ParseOperandInto(tokens, i + 1, output, opStack, callStack);
            output.Add(tok with { IsUnary = true });
            return next;
        }
        if (tok.Kind == SnoconeLexer.ScKind.LParen)
        {
            callStack.Push(new Frame(FrameKind.Group, 0, "", tok.Line, output.Count));
            opStack.Push(tok);
            return i + 1;
        }
        return i + 1;
    }
}
