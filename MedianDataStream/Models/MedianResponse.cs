namespace MedianDataStream.Models
{
    public class MedianResponse
    {
        public double Median { get; set; }
        public List<int> Numbers { get; set; } = new();
        public int Count { get; set; }
    }
}
