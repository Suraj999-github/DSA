using MedianDataStream.Services;

namespace MedianDataStream.Services
{
    public interface IMedianFinder
    {
        void AddNum(int num);
        double FindMedian();
        void Clear();
        List<int> GetAllNumbers();
        List<int> GetMaxHeap();
        List<int> GetMinHeap();
        List<int> GetSortedNumbers();
    }
}
public class MedianFinder : IMedianFinder
{
    private readonly PriorityQueue<int, int> _lowerHalf; // Max-heap
    private readonly PriorityQueue<int, int> _upperHalf; // Min-heap

    public MedianFinder()
    {
        _lowerHalf = new PriorityQueue<int, int>(Comparer<int>.Create((x, y) => y.CompareTo(x)));
        _upperHalf = new PriorityQueue<int, int>();
    }

    public void AddNum(int num)
    {
        if (_lowerHalf.Count == 0 || num <= _lowerHalf.Peek())
        {
            _lowerHalf.Enqueue(num, num);
        }
        else
        {
            _upperHalf.Enqueue(num, num);
        }

        RebalanceHeaps();
    }

    public double FindMedian()
    {
        if (_lowerHalf.Count == 0 && _upperHalf.Count == 0)
            throw new InvalidOperationException("No numbers added yet");

        if (_lowerHalf.Count > _upperHalf.Count)
        {
            return _lowerHalf.Peek();
        }
        else if (_upperHalf.Count > _lowerHalf.Count)
        {
            return _upperHalf.Peek();
        }
        else
        {
            return (_lowerHalf.Peek() + _upperHalf.Peek()) / 2.0;
        }
    }

    public void Clear()
    {
        _lowerHalf.Clear();
        _upperHalf.Clear();
    }

    public List<int> GetAllNumbers()
    {
        var numbers = new List<int>();
        numbers.AddRange(GetMaxHeap());
        numbers.AddRange(GetMinHeap());
        return numbers;
    }

    public List<int> GetSortedNumbers()
    {
        var numbers = GetAllNumbers();
        numbers.Sort();
        return numbers;
    }

    // Get elements from max-heap (lower half) as sorted list
    public List<int> GetMaxHeap()
    {
        var result = new List<int>();
        var temp = new List<int>();

        while (_lowerHalf.Count > 0)
        {
            var num = _lowerHalf.Dequeue();
            result.Add(num);
            temp.Add(num);
        }

        // Restore heap
        foreach (var num in temp)
        {
            _lowerHalf.Enqueue(num, num);
        }

        // Max-heap elements sorted descending (for visualization)
        result.Sort((a, b) => b.CompareTo(a));
        return result;
    }

    // Get elements from min-heap (upper half) as sorted list
    public List<int> GetMinHeap()
    {
        var result = new List<int>();
        var temp = new List<int>();

        while (_upperHalf.Count > 0)
        {
            var num = _upperHalf.Dequeue();
            result.Add(num);
            temp.Add(num);
        }

        // Restore heap
        foreach (var num in temp)
        {
            _upperHalf.Enqueue(num, num);
        }

        // Min-heap elements sorted ascending (for visualization)
        result.Sort();
        return result;
    }

    private void RebalanceHeaps()
    {
        // Balance size difference
        while (Math.Abs(_lowerHalf.Count - _upperHalf.Count) > 1)
        {
            if (_lowerHalf.Count > _upperHalf.Count)
            {
                var num = _lowerHalf.Dequeue();
                _upperHalf.Enqueue(num, num);
            }
            else
            {
                var num = _upperHalf.Dequeue();
                _lowerHalf.Enqueue(num, num);
            }
        }

        // Ensure ordering property
        while (_lowerHalf.Count > 0 && _upperHalf.Count > 0 && _lowerHalf.Peek() > _upperHalf.Peek())
        {
            var lowerMax = _lowerHalf.Dequeue();
            var upperMin = _upperHalf.Dequeue();

            _lowerHalf.Enqueue(upperMin, upperMin);
            _upperHalf.Enqueue(lowerMax, lowerMax);
        }
    }
}

