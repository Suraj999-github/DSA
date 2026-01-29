namespace MedianDataStream.Models
{
    public class DetailedMedianResponse
    {
        public int? AddedNumber { get; set; }
        public List<int> MaxHeap { get; set; } = new();
        public List<int> MinHeap { get; set; } = new();
        public double Median { get; set; }
        public List<int> AllNumbers { get; set; } = new();
        public List<int> SortedNumbers { get; set; } = new();
        public int MaxHeapCount { get; set; }
        public int MinHeapCount { get; set; }
        public int TotalNumbers { get; set; }
        public int? Step { get; set; }
        public string Status { get; set; } = "Success";
        public string Message { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public object? MaxHeapProperties { get; set; }
        public object? MinHeapProperties { get; set; }
    }
}
