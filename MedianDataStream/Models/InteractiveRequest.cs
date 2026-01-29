namespace MedianDataStream.Models
{
    public class InteractiveRequest
    {
        public int[] Numbers { get; set; } = Array.Empty<int>();
        public bool ClearFirst { get; set; } = true;
    }
}
