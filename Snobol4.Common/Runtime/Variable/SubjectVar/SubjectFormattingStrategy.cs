namespace Snobol4.Common;

/// <summary>
/// Formatting strategy for subject variables
/// </summary>
public class SubjectFormattingStrategy : IFormattingStrategy
{
    public string ToString(Var self)
    {
        var subjectSelf = (SubjectVar)self;
        return subjectSelf.Subject;
    }

    public string DumpString(Var self)
    {
        var subjectSelf = (SubjectVar)self;
        var matchedPortion = subjectSelf.Subject.Substring(
            subjectSelf.MatchResult.PreCursor,
            subjectSelf.MatchResult.PostCursor - subjectSelf.MatchResult.PreCursor);
        return $"'{subjectSelf.Subject}' [matched: '{matchedPortion}']";
    }

    public string DebugString(Var self)
    {
        var subjectSelf = (SubjectVar)self;
        var symbol = subjectSelf.Symbol == "" ? "<no name>" : subjectSelf.Symbol;
        return $"SUBJECT Symbol: {symbol}  Subject: '{subjectSelf.Subject}'  MatchStart: {subjectSelf.MatchResult.PreCursor}  MatchEnd: {subjectSelf.MatchResult.PostCursor}  Succeeded: {subjectSelf.Succeeded}";
    }
}