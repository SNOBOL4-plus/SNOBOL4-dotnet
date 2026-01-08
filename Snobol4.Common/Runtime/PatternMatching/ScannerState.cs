namespace Snobol4.Common;

internal class ScannerState
{
    public int PreviousCursorPosition { get; set; }
    public int CursorPosition { get; set; }
    public string Subject { get; }

    private readonly Stack<int> _alternatePatternStack = [];
    private readonly Stack<int> _alternateCursorStack = [];

    public ScannerState(string subject, int startPosition)
    {
        Subject = subject;
        PreviousCursorPosition = CursorPosition = startPosition;
        
        // Initialize with sentinel values
        _alternatePatternStack.Push(-1);
        _alternateCursorStack.Push(-1);
    }

    public void SaveAlternate(int nodeIndex)
    {
        _alternatePatternStack.Push(nodeIndex);
        _alternateCursorStack.Push(CursorPosition);
    }

    public (int nodeIndex, int cursorPosition) RestoreAlternate()
    {
        var cursor = _alternateCursorStack.Pop();
        var node = _alternatePatternStack.Pop();
        CursorPosition = cursor;
        return (node, cursor);
    }

    public bool HasAlternates() => _alternatePatternStack.Peek() != -1;

    public void ClearAlternates()
    {
        _alternatePatternStack.Clear();
        _alternateCursorStack.Clear();
        _alternatePatternStack.Push(-1);
        _alternateCursorStack.Push(-1);
    }
}