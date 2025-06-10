using System.Numerics;

namespace Core.Graph;

public class SpatialGraphNode<T>
{
    public T? Value { get; }
    public Vector2 Position { get; }

    private readonly HashSet<SpatialGraphNode<T>> _neighbours;
    private readonly object _lock = new();
    public IReadOnlyCollection<SpatialGraphNode<T>> Neighbours
    {
        get
        {
            lock (_lock) return _neighbours.ToList();
        }
    }


    public SpatialGraphNode(Vector2 position, T? value = default)
    {
        Position = position;
        _neighbours = [];
        Value = value;
    }

    public void AddNeighbour(SpatialGraphNode<T> neighbour)
    {
        if (!_neighbours.Add(neighbour))
            return;
        if (neighbour == this)
            return;
        _neighbours.Add(neighbour);
    }
    
    public void ConcurrentAddNeighbour(SpatialGraphNode<T> neighbour)
    {
        if (neighbour == this) return;

        var first = this;
        var second = neighbour;

        // Ensure consistent lock order using object identity hash
        // Some vibe-coded magic, idk how it works
        var hash1 = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);
        var hash2 = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(neighbour);

        if (hash1 > hash2)
        {
            first = neighbour;
            second = this;
        }

        lock (first._lock)
        lock (second._lock)
        {
            if (_neighbours.Add(neighbour))
                neighbour._neighbours.Add(this);
        }
    }
    
    public void RemoveNeighbour(SpatialGraphNode<T> neighbour)
    {
        if (!_neighbours.Remove(neighbour))
            return;
        if (neighbour == this)
            return;
        neighbour._neighbours.Remove(this);
    }
    
    public void ClearNeighbours()
    {
        foreach (var neighbour in _neighbours)
        {
            neighbour._neighbours.Remove(this);
        }
        _neighbours.Clear();
    }
}