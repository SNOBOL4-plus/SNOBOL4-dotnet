namespace Snobol4.Common;

internal class AbstractSyntaxTree
{
    private readonly List<AbstractSyntaxTreeNode> _nodes = [];
    private AbstractSyntaxTreeNode? _startNode;

    public AbstractSyntaxTreeNode StartNode => _startNode 
        ?? throw new InvalidOperationException("AST not built");

    public int Count => _nodes.Count;

    public AbstractSyntaxTreeNode this[int index] => _nodes[index];

    public static AbstractSyntaxTree Build(Pattern rootPattern)
    {
        var ast = new AbstractSyntaxTree();
        ast.BuildFromPattern(rootPattern);
        return ast;
    }

    private void BuildFromPattern(Pattern rootPattern)
    {
        // Always rebuild fresh: do NOT reuse cached AST nodes.
        // Cached AbstractSyntaxTreeNode objects hold a _tree reference to the list
        // they were created in. If reused via AddRange into a new list (e.g. inside
        // UnevaluatedPattern's child Scanner), GetSubsequent/GetAlternate follow
        // _tree[index] back into the original list — not the new copy — causing the
        // child scanner to jump to wrong nodes or fail entirely.
        // Rebuilding is slightly more work but is correct and re-entrant safe.
        BuildNodeList(rootPattern);
        LinkParentChildren();
        ComputeSubsequentsAndAlternates();
        FindStartNode();
    }

    private void BuildNodeList(Pattern rootPattern)
    {
        var nodeStack = new Stack<AbstractSyntaxTreeNode>();
        var currentIndex = 0;
        nodeStack.Push(new AbstractSyntaxTreeNode(rootPattern, 0, AbstractSyntaxTreeNode.NodeType.NONE, -1, _nodes));

        while (nodeStack.Count > 0)
        {
            var currentNode = nodeStack.Pop();
            var parentIndex = currentIndex;
            currentNode.SelfIndex = currentIndex++;
            _nodes.Add(currentNode);

            if (currentNode.IsTerminal())
                continue;

            nodeStack.Push(new AbstractSyntaxTreeNode(currentNode.Self.RightPattern!, -99, AbstractSyntaxTreeNode.NodeType.RIGHT, parentIndex, _nodes));
            nodeStack.Push(new AbstractSyntaxTreeNode(currentNode.Self.LeftPattern!, -99, AbstractSyntaxTreeNode.NodeType.LEFT, parentIndex, _nodes));
        }
    }

    private void LinkParentChildren()
    {
        for (var i = 1; i < _nodes.Count; i++)
        {
            var currentNode = _nodes[i];
            var parent = _nodes[currentNode.ParentIndex];

            switch (currentNode.ChildType)
            {
                case AbstractSyntaxTreeNode.NodeType.LEFT:
                    parent.LeftChild = i;
                    break;
                case AbstractSyntaxTreeNode.NodeType.RIGHT:
                    parent.RightChild = i;
                    break;
            }
        }
    }

    private void ComputeSubsequentsAndAlternates()
    {
        for (var i = 0; i < _nodes.Count; ++i)
        {
            if (!_nodes[i].IsTerminal())
                continue;
            _nodes[i].Subsequent = ComputeSubsequent(i);
            _nodes[i].Alternate = ComputeAlternate(i);
        }
    }

    private void FindStartNode()
    {
        var node = _nodes[0];
        while (!node.IsTerminal())
            node = node.GetLeftChild()!;
        _startNode = node;
    }

    private int ComputeSubsequent(int index) => ComputeNext(index, true);

    private int ComputeAlternate(int index) => ComputeNext(index, false);

    private int ComputeNext(int index, bool concatenate)
    {
        if (_nodes.Count == 1)
            return -1;

        var currentIndex = index;

        while (true)
        {
            var currentNode = _nodes[currentIndex];
            var parentNode = _nodes[currentNode.ParentIndex];

            if (currentNode.IsLeftChild() && 
                (concatenate ? parentNode.Self is ConcatenatePattern : parentNode.Self is AlternatePattern))
                break;

            if (parentNode.SelfIndex == 0)
                return -1;

            currentIndex = currentNode.ParentIndex;
        }

        var parentNode2 = _nodes[_nodes[currentIndex].ParentIndex];
        var currentNode2 = _nodes[parentNode2.RightChild];

        while (!currentNode2.IsTerminal())
            currentNode2 = currentNode2.GetLeftChild()!;

        return currentNode2.SelfIndex;
    }

    /// <summary>
    /// Graft a sub-pattern's nodes into this AST at runtime (used by UnevaluatedPattern).
    /// Builds a fresh sub-AST for <paramref name="subPattern"/>, offsets all its node
    /// indices by the current node count, patches terminal nodes whose Subsequent is -1
    /// to point to <paramref name="successorNodeIndex"/> (the node that follows *X in the
    /// outer pattern), then appends the new nodes to this AST's node list.
    /// Returns the index of the grafted sub-tree's start node in the extended AST.
    /// </summary>
    internal int Graft(Pattern subPattern, int successorNodeIndex)
    {
        // Build a standalone sub-AST for the evaluated pattern
        var subAst = new AbstractSyntaxTree();
        subAst.BuildNodeList(subPattern);
        subAst.LinkParentChildren();
        subAst.ComputeSubsequentsAndAlternates();
        subAst.FindStartNode();

        int offset = _nodes.Count;

        // Append sub-AST nodes, remapping all index references by offset
        foreach (var n in subAst._nodes)
        {
            // Remap structural indices (parent, children) — these are intra-sub-AST
            int newParent  = n.ParentIndex >= 0  ? n.ParentIndex  + offset : n.ParentIndex;
            int newLeft    = n.LeftChild  >= 0   ? n.LeftChild    + offset : n.LeftChild;
            int newRight   = n.RightChild >= 0   ? n.RightChild   + offset : n.RightChild;

            // Remap traversal edges: Subsequent and Alternate.
            // -1 on Subsequent means "end of pattern" → wire to successorNodeIndex.
            // -1 on Alternate  means "no alternate in sub-pattern" → leave as -1
            //   so the outer scanner's alternate stack handles backtracking.
            int newSub  = n.Subsequent >= 0 ? n.Subsequent + offset : successorNodeIndex;
            int newAlt  = n.Alternate  >= 0 ? n.Alternate  + offset : -1;

            var grafted = new AbstractSyntaxTreeNode(
                n.Self,
                n.SelfIndex + offset,
                n.ChildType,
                newParent,
                _nodes)
            {
                LeftChild  = newLeft,
                RightChild = newRight,
                Subsequent = newSub,
                Alternate  = newAlt
            };
            _nodes.Add(grafted);
        }

        // Return index of the sub-AST's start node in the extended _nodes list
        return subAst.StartNode.SelfIndex + offset;
    }

    public void Dump()
    {
        Console.Error.WriteLine(@"=============================================================");
        Console.Error.WriteLine(@"Root:");
        if (_startNode is not null)
            Console.Error.WriteLine(_startNode.DebugAst());
        Console.Error.WriteLine(@"-------------------------------------------------------------");
        foreach (var node in _nodes)
        {
            Console.Error.WriteLine(node.DebugAst());
        }
        Console.Error.WriteLine(@"=============================================================");
    }
}