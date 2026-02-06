namespace Snobol4.Common;

public partial class Executive
{
    /// <summary>
    /// Implements the TERMINAL variable input. Similar to INPUT but always
    /// reads from the keyboard
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    internal Var TerminalIn(Var? v, string _)
    {
        var s = Console.ReadLine();

        if (string.IsNullOrEmpty(s)) 
            return StringVar.Null();
        
        if (AmpTrim != 0)
            s = s.TrimEnd();

        return new StringVar(s, "terminal", true);

    }

    /// <summary>
    /// Implementation of TERMINAL variable for output. Similar to OUTPUT but always
    /// write to the display console.
    ///</summary>
    // ReSharper disable once UnusedMember.Global
    internal Var TerminalOut(Var v, string symbol)
    {
        var s = v.ToString() ?? "";
        Console.Error.WriteLine(s);
        return new StringVar(s);
    }
}