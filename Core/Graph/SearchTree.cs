using System.Numerics;

namespace Core.Graph;

public class SearchTree<T>
{
    private class TreeNode<T1>
    {
        public SpatialGraphNode<T1> Data;
        public bool Vertical;
        public TreeNode<T1> Left;
        public TreeNode<T1> Right;

        public TreeNode(SpatialGraphNode<T1> data, bool vertical)
        {
            Data = data;
            Vertical = vertical;
        }
    }

    private TreeNode<T> _root;

    public SearchTree(IEnumerable<SpatialGraphNode<T>> nodes)
    {
        var nodeList = new List<SpatialGraphNode<T>>(nodes);
        _root = Build(nodeList, depth: 0);
    }

    private TreeNode<T> Build(List<SpatialGraphNode<T>> nodes, int depth)
    {
        if (nodes.Count == 0)
            return null;

        bool vertical = (depth % 2 == 0);
        nodes.Sort((a, b) => vertical
            ? a.Position.X.CompareTo(b.Position.X)
            : a.Position.Y.CompareTo(b.Position.Y));

        int medianIndex = nodes.Count / 2;
        var root = new TreeNode<T>(nodes[medianIndex], vertical)
        {
            Left = Build(nodes.GetRange(0, medianIndex), depth + 1),
            Right = Build(nodes.GetRange(medianIndex + 1, nodes.Count - medianIndex - 1), depth + 1)
        };

        return root;
    }

    public SpatialGraphNode<T> FindNearest(Vector2 target)
    {
        return FindNearest(_root, target, _root.Data, float.MaxValue);
    }

    private SpatialGraphNode<T> FindNearest(TreeNode<T> node, Vector2 target, SpatialGraphNode<T> best, float bestDist)
    {
        if (node == null)
            return best;

        float dist = Vector2.DistanceSquared(target, node.Data.Position);
        if (dist < bestDist)
        {
            bestDist = dist;
            best = node.Data;
        }

        float cmp = node.Vertical ? target.X - node.Data.Position.X : target.Y - node.Data.Position.Y;
        TreeNode<T> near = cmp < 0 ? node.Left : node.Right;
        TreeNode<T> far = cmp < 0 ? node.Right : node.Left;

        best = FindNearest(near, target, best, bestDist);

        // Check if we need to explore the other side
        if ((cmp * cmp) < bestDist)
        {
            best = FindNearest(far, target, best, Vector2.DistanceSquared(target, best.Position));
        }

        return best;
    }

    public List<SpatialGraphNode<T>> FindInRange(Vector2 target, float radius)
    {
        var results = new List<SpatialGraphNode<T>>();
        float radiusSquared = radius * radius;
        FindInRange(_root, target, radiusSquared, results);
        return results;
    }

    private void FindInRange(TreeNode<T> node, Vector2 target, float radiusSquared, List<SpatialGraphNode<T>> results)
    {
        if (node == null)
            return;

        float distSquared = Vector2.DistanceSquared(target, node.Data.Position);
        if (distSquared <= radiusSquared)
        {
            results.Add(node.Data);
        }

        // Check which side(s) to search
        float delta = node.Vertical ? target.X - node.Data.Position.X : target.Y - node.Data.Position.Y;
        float deltaSquared = delta * delta;

        if (delta < 0)
        {
            FindInRange(node.Left, target, radiusSquared, results);
            if (deltaSquared <= radiusSquared)
                FindInRange(node.Right, target, radiusSquared, results);
        }
        else
        {
            FindInRange(node.Right, target, radiusSquared, results);
            if (deltaSquared <= radiusSquared)
                FindInRange(node.Left, target, radiusSquared, results);
        }
    }
}