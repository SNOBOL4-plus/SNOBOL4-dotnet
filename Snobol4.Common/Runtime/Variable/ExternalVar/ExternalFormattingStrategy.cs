namespace Snobol4.Common;

public class ExternalFormattingStrategy : IFormattingStrategy
{
    public string ToString(Var self)   => $"<EXTERNAL 0x{((ExternalVar)self).Pointer:X}>";
    public string DumpString(Var self) => $"EXTERNAL(0x{((ExternalVar)self).Pointer:X})";
    public string DebugVar(Var self)   => $"EXTERNAL Pointer=0x{((ExternalVar)self).Pointer:X}";
}
