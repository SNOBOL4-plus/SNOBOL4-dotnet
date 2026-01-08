using System.Diagnostics;

namespace Snobol4.Common;

[DebuggerDisplay("{DebugString()}")]
internal class AbstractSyntaxTreeNode
{
    internal Pattern Self { get; }
    internal int SelfIndex { get; set; }
    internal NodeType ChildType { get; }
    internal int ParentIndex { get; }
    internal int LeftChild { get; set; } = -1;
    internal int RightChild { get; set; } = -1;
    internal int Subsequent { get; set; } = -1;
    internal int Alternate { get; set; } = -1;

    private readonly List<AbstractSyntaxTreeNode> _tree;

    internal enum NodeType
    {
        NONE,
        LEFT,
        RIGHT
    }

    internal AbstractSyntaxTreeNode(Pattern self, int selfIndex, NodeType childType, int parentIndex, List<AbstractSyntaxTreeNode> tree)
    {
        Self = self;
        SelfIndex = selfIndex;
        ChildType = childType;
        ParentIndex = parentIndex;
        _tree = tree;
    }

    internal bool IsTerminal() => Self.IsTerminal();

    internal bool HasSubsequent() => Subsequent != -1;
    
    internal bool HasAlternate() => Alternate != -1;

    internal AbstractSyntaxTreeNode? GetSubsequent() 
        => HasSubsequent() ? _tree[Subsequent] : null;

    internal AbstractSyntaxTreeNode? GetAlternate() 
        => HasAlternate() ? _tree[Alternate] : null;

    internal AbstractSyntaxTreeNode? GetParent() 
        => ParentIndex >= 0 ? _tree[ParentIndex] : null;

    internal AbstractSyntaxTreeNode? GetLeftChild() 
        => LeftChild >= 0 ? _tree[LeftChild] : null;

    internal AbstractSyntaxTreeNode? GetRightChild() 
        => RightChild >= 0 ? _tree[RightChild] : null;

    internal bool IsLeftChild() => ChildType == NodeType.LEFT;
    
    internal bool IsRightChild() => ChildType == NodeType.RIGHT;

    public string DebugString()
    {
        return $"ASTNode: {SelfIndex:D3}  Self: {Self.ToString()?[15..],-30}  ChildType: {ChildType,-5}  ParentIndex: {ParentIndex.ToString(),4}  Left: {LeftChild.ToString(),4}  Right: {RightChild.ToString(),4}  Sub: {Subsequent.ToString(),4} Alt: {Alternate.ToString(),4}";
    }
}