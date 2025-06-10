using System.Numerics;
using Core.Map.Cells;

namespace Core.Graph;

public class SpatialGraph<T>
{
    public Vector2 Size { get; }
    private ConcurrentHashSet<SpatialGraphNode<T>> _nodes;
    public IReadOnlyCollection<SpatialGraphNode<T>> NodesList => _nodes;
    private SearchTree<T>? _searchTree;
    private bool _stateChangedSinceLastSearch = true;


    public SpatialGraph(Vector2 size, IEnumerable<SpatialGraphNode<T>>? nodes = null)
    {
        Size = size;
        _nodes = nodes != null ? [..nodes] : [];
    }

    public SpatialGraphNode<T> GetNearest(Vector2 target)
    {
        if (_stateChangedSinceLastSearch || _searchTree == null)
        {
            _searchTree = new SearchTree<T>(_nodes);
            _stateChangedSinceLastSearch = false;
        }

        var nearest = _searchTree.FindNearest(target);
        return nearest;
    }

    public List<SpatialGraphNode<T>> GetInRange(Vector2 target, float radius)
    {
        if (_stateChangedSinceLastSearch || _searchTree == null)
        {
            _searchTree = new SearchTree<T>(_nodes);
            _stateChangedSinceLastSearch = false;
        }

        return _searchTree.FindInRange(target, radius);
    }

    public void AddNode(SpatialGraphNode<T> node)
    {
        if (node.Position.X < 0 || node.Position.X > Size.X || node.Position.Y < 0 || node.Position.Y > Size.Y)
            throw new ArgumentOutOfRangeException(nameof(node), "Node position is out of bounds.");
        if (!_nodes.Add(node))
            return;
        _stateChangedSinceLastSearch = true;
    }

    public void RemoveNode(SpatialGraphNode<T> node)
    {
        if (!_nodes.Contains(node))
            return;
        _nodes.Remove(node);
        _stateChangedSinceLastSearch = true;
    }

    public void Clear()
    {
        _nodes.Clear();
        _stateChangedSinceLastSearch = true;
    }

    public void AutoLink(Vector2? targetPrecision = null)
    {
        var precision = targetPrecision ?? new Vector2(1, 1);
        var x = (int)(Size.X / precision.X);
        var y = (int)(Size.Y / precision.Y);
        var nearest = new SpatialGraphNode<T>[x, y];
        for (var i = 0; i < x; i++)
        {
            for (var j = 0; j < y; j++)
            {
                nearest[i, j] = GetNearest(new Vector2(i * precision.X, j * precision.Y));
            }
        }

        for (var i = 0; i < x; i++)
        {
            for (var j = 0; j < y; j++)
            {
                var node = nearest[i, j];
                if (i > 0)
                {
                    node.AddNeighbour(nearest[i - 1, j]);
                }

                if (i < x - 1)
                {
                    node.AddNeighbour(nearest[i + 1, j]);
                }

                if (j > 0)
                {
                    node.AddNeighbour(nearest[i, j - 1]);
                }

                if (j < y - 1)
                {
                    node.AddNeighbour(nearest[i, j + 1]);
                }
            }
        }
    }
    
    public void ConcurrentAutoLink(Vector2? targetPrecision = null)
    {
        var precision = targetPrecision ?? new Vector2(1, 1);
        var x = (int)(Size.X / precision.X);
        var y = (int)(Size.Y / precision.Y);
        var nearest = new SpatialGraphNode<T>[x, y];

        // First: parallelize filling nearest
        Parallel.For(0, x, i =>
        {
            for (var j = 0; j < y; j++)
            {
                nearest[i, j] = GetNearest(new Vector2(i * precision.X, j * precision.Y));
            }
        });

        // Second: optionally parallelize linking if AddNeighbour is thread-safe
        Parallel.For(0, x, i =>
        {
            for (var j = 0; j < y; j++)
            {
                var node = nearest[i, j];
                if (i > 0) node.ConcurrentAddNeighbour(nearest[i - 1, j]);
                if (i < x - 1) node.ConcurrentAddNeighbour(nearest[i + 1, j]);
                if (j > 0) node.ConcurrentAddNeighbour(nearest[i, j - 1]);
                if (j < y - 1) node.ConcurrentAddNeighbour(nearest[i, j + 1]);
            }
        });
    }

    public void Unlink()
    {
        foreach (var node in _nodes)
        {
            node.ClearNeighbours();
        }
    }

    public void Squash(float threshold, int steps)
    {
        while (steps > 0)
        {
            var toDelete = new HashSet<SpatialGraphNode<T>>();
            foreach (var node in _nodes)
            {
                if (toDelete.Contains(node))
                {
                    continue;
                }
                var toAddNeighbours = new HashSet<SpatialGraphNode<T>>();
                foreach (var other in node.Neighbours)
                {
                    if (other == node)
                    {
                        continue;
                    }
                    if (Vector2.Distance(node.Position, other.Position) < threshold)
                    {
                        foreach (var neighbour in other.Neighbours)
                        {
                            toAddNeighbours.Add(neighbour);
                        }

                        other.ClearNeighbours();
                        toDelete.Add(other);
                    }
                }

                foreach (var neighbour in toAddNeighbours)
                {
                    node.ConcurrentAddNeighbour(neighbour);
                }

                
            }

            foreach (var node in toDelete)
            {
                RemoveNode(node);
            }

            steps--;
        }

        _stateChangedSinceLastSearch = true;
    }
}