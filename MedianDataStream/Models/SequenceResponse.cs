namespace MedianDataStream.Models
{
    public class SequenceResponse
    {
        public int[] Sequence { get; set; } = Array.Empty<int>();
        public List<DetailedMedianResponse> Steps { get; set; } = new();
        public double FinalMedian { get; set; }
        public int TotalNumbers { get; set; }
    }
}
