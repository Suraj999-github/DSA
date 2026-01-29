namespace MedianDataStream.Models
{
    public class InteractiveResponse
    {
        public int[] InputNumbers { get; set; } = Array.Empty<int>();
        public List<DetailedMedianResponse> Steps { get; set; } = new();
        public object Summary { get; set; } = new();
    }
}
