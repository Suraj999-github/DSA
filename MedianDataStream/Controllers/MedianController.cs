using MedianDataStream.Models;
using MedianDataStream.Services;
using Microsoft.AspNetCore.Mvc;

namespace MedianDataStream.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedianController : ControllerBase
    {
        private readonly IMedianFinder _medianFinder;
        private readonly ILogger<MedianController> _logger;

        public MedianController(IMedianFinder medianFinder, ILogger<MedianController> logger)
        {
            _medianFinder = medianFinder;
            _logger = logger;
        }

        [HttpPost("add-single")]
        public IActionResult AddSingleNumber([FromBody] AddNumberRequest request)
        {
            try
            {
                // Add the number
                _medianFinder.AddNum(request.Number);

                // Get all data for response
                var maxHeap = _medianFinder.GetMaxHeap();
                var minHeap = _medianFinder.GetMinHeap();
                var median = _medianFinder.FindMedian();
                var allNumbers = _medianFinder.GetAllNumbers();
                var sortedNumbers = _medianFinder.GetSortedNumbers();

                // Log for debugging
                _logger.LogInformation(
                    "Added {Number}. MaxHeap: {MaxHeapCount}, MinHeap: {MinHeapCount}, Median: {Median}",
                    request.Number, maxHeap.Count, minHeap.Count, median);

                return Ok(new DetailedMedianResponse
                {
                    AddedNumber = request.Number,
                    MaxHeap = maxHeap,
                    MinHeap = minHeap,
                    Median = median,
                    AllNumbers = allNumbers,
                    SortedNumbers = sortedNumbers,
                    MaxHeapCount = maxHeap.Count,
                    MinHeapCount = minHeap.Count,
                    TotalNumbers = allNumbers.Count,
                    Status = "Success",
                    Message = $"Number {request.Number} added successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding number {Number}", request.Number);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("add-sequence")]
        public IActionResult AddSequence([FromBody] AddSequenceRequest request)
        {
            try
            {
                var results = new List<DetailedMedianResponse>();

                foreach (var num in request.Numbers)
                {
                    _medianFinder.AddNum(num);

                    results.Add(new DetailedMedianResponse
                    {
                        AddedNumber = num,
                        MaxHeap = _medianFinder.GetMaxHeap(),
                        MinHeap = _medianFinder.GetMinHeap(),
                        Median = _medianFinder.FindMedian(),
                        AllNumbers = _medianFinder.GetAllNumbers(),
                        SortedNumbers = _medianFinder.GetSortedNumbers(),
                        MaxHeapCount = _medianFinder.GetMaxHeap().Count,
                        MinHeapCount = _medianFinder.GetMinHeap().Count,
                        TotalNumbers = _medianFinder.GetAllNumbers().Count,
                        Step = results.Count + 1,
                        Status = "Success",
                        Timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new SequenceResponse
                {
                    Sequence = request.Numbers,
                    Steps = results,
                    FinalMedian = _medianFinder.FindMedian(),
                    TotalNumbers = _medianFinder.GetAllNumbers().Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding sequence");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("current-state")]
        public IActionResult GetCurrentState()
        {
            try
            {
                var maxHeap = _medianFinder.GetMaxHeap();
                var minHeap = _medianFinder.GetMinHeap();
                var median = _medianFinder.FindMedian();
                var allNumbers = _medianFinder.GetAllNumbers();
                var sortedNumbers = _medianFinder.GetSortedNumbers();

                return Ok(new DetailedMedianResponse
                {
                    MaxHeap = maxHeap,
                    MinHeap = minHeap,
                    Median = median,
                    AllNumbers = allNumbers,
                    SortedNumbers = sortedNumbers,
                    MaxHeapCount = maxHeap.Count,
                    MinHeapCount = minHeap.Count,
                    TotalNumbers = allNumbers.Count,
                    Status = "Current State",
                    Message = "Current state retrieved successfully",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (InvalidOperationException ex)
            {
                return Ok(new
                {
                    Status = "Empty",
                    Message = "No numbers added yet",
                    MaxHeap = new List<int>(),
                    MinHeap = new List<int>(),
                    AllNumbers = new List<int>(),
                    SortedNumbers = new List<int>(),
                    MaxHeapCount = 0,
                    MinHeapCount = 0,
                    TotalNumbers = 0
                });
            }
        }

        [HttpPost("add-interactive")]
        public IActionResult AddInteractive([FromBody] InteractiveRequest request)
        {
            try
            {
                var results = new List<DetailedMedianResponse>();

                if (request.ClearFirst)
                {
                    _medianFinder.Clear();
                }

                foreach (var num in request.Numbers)
                {
                    _medianFinder.AddNum(num);

                    var maxHeap = _medianFinder.GetMaxHeap();
                    var minHeap = _medianFinder.GetMinHeap();
                    var median = _medianFinder.FindMedian();

                    // Calculate heap properties
                    var maxHeapProperties = GetHeapProperties(maxHeap, "Max-Heap");
                    var minHeapProperties = GetHeapProperties(minHeap, "Min-Heap");

                    results.Add(new DetailedMedianResponse
                    {
                        AddedNumber = num,
                        MaxHeap = maxHeap,
                        MinHeap = minHeap,
                        Median = median,
                        AllNumbers = _medianFinder.GetAllNumbers(),
                        SortedNumbers = _medianFinder.GetSortedNumbers(),
                        MaxHeapCount = maxHeap.Count,
                        MinHeapCount = minHeap.Count,
                        TotalNumbers = _medianFinder.GetAllNumbers().Count,
                        Step = results.Count + 1,
                        Status = "Success",
                        Message = $"After adding {num}",
                        Timestamp = DateTime.UtcNow,
                        MaxHeapProperties = maxHeapProperties,
                        MinHeapProperties = minHeapProperties
                    });
                }

                return Ok(new InteractiveResponse
                {
                    InputNumbers = request.Numbers,
                    Steps = results,
                    Summary = new
                    {
                        FinalMedian = _medianFinder.FindMedian(),
                        FinalMaxHeapSize = _medianFinder.GetMaxHeap().Count,
                        FinalMinHeapSize = _medianFinder.GetMinHeap().Count,
                        FinalTotalNumbers = _medianFinder.GetAllNumbers().Count,
                        IsBalanced = Math.Abs(_medianFinder.GetMaxHeap().Count - _medianFinder.GetMinHeap().Count) <= 1
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in interactive mode");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("test-algorithm")]
        public IActionResult TestAlgorithm()
        {
            _medianFinder.Clear();

            // Test case: [5, 15, 1, 3]
            var testNumbers = new[] { 5, 15, 1, 3 };
            var steps = new List<DetailedMedianResponse>();

            foreach (var num in testNumbers)
            {
                _medianFinder.AddNum(num);

                steps.Add(new DetailedMedianResponse
                {
                    AddedNumber = num,
                    MaxHeap = _medianFinder.GetMaxHeap(),
                    MinHeap = _medianFinder.GetMinHeap(),
                    Median = _medianFinder.FindMedian(),
                    AllNumbers = _medianFinder.GetAllNumbers(),
                    SortedNumbers = _medianFinder.GetSortedNumbers(),
                    MaxHeapCount = _medianFinder.GetMaxHeap().Count,
                    MinHeapCount = _medianFinder.GetMinHeap().Count,
                    TotalNumbers = _medianFinder.GetAllNumbers().Count,
                    Step = steps.Count + 1,
                    Message = GetAlgorithmExplanation(steps.Count, num, _medianFinder)
                });
            }

            return Ok(new
            {
                TestName = "Two-Heap Median Algorithm Demo",
                TestNumbers = testNumbers,
                Steps = steps,
                FinalState = new
                {
                    Median = _medianFinder.FindMedian(),
                    AllNumbersSorted = _medianFinder.GetSortedNumbers(),
                    HeapBalance = $"{_medianFinder.GetMaxHeap().Count}:{_medianFinder.GetMinHeap().Count}"
                }
            });
        }

        // Keep existing endpoints (add, median, clear, etc.)
        [HttpPost("add")]
        public IActionResult AddNumber([FromBody] AddNumberRequest request)
        {
            _medianFinder.AddNum(request.Number);
            return Ok(new { message = $"Number {request.Number} added successfully" });
        }

        [HttpGet("median")]
        public IActionResult GetMedian()
        {
            try
            {
                var median = _medianFinder.FindMedian();
                var numbers = _medianFinder.GetAllNumbers();

                return Ok(new MedianResponse
                {
                    Median = median,
                    Numbers = numbers,
                    Count = numbers.Count
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("add-batch")]
        public IActionResult AddNumbers([FromBody] AddBatchRequest request)
        {
            foreach (var num in request.Numbers)
            {
                _medianFinder.AddNum(num);
            }

            return Ok(new
            {
                message = $"Added {request.Numbers.Length} numbers",
                count = request.Numbers.Length
            });
        }

        [HttpDelete("clear")]
        public IActionResult Clear()
        {
            _medianFinder.Clear();
            return Ok(new { message = "All numbers cleared" });
        }

        [HttpGet("numbers")]
        public IActionResult GetAllNumbers()
        {
            var numbers = _medianFinder.GetAllNumbers();
            return Ok(new { numbers });
        }

        // Helper methods
        private object GetHeapProperties(List<int> heap, string heapName)
        {
            if (heap.Count == 0)
                return new { Name = heapName, IsEmpty = true };

            return new
            {
                Name = heapName,
                Count = heap.Count,
                Root = heap.Count > 0 ? heap[0] : (int?)null,
                Min = heap.Min(),
                Max = heap.Max(),
                Average = heap.Average(),
                IsEmpty = false
            };
        }

        private string GetAlgorithmExplanation(int step, int addedNum, IMedianFinder finder)
        {
            var maxHeap = finder.GetMaxHeap();
            var minHeap = finder.GetMinHeap();

            return step switch
            {
                0 => $"Added {addedNum}. Max-Heap: [{string.Join(", ", maxHeap)}], Min-Heap: []",
                1 => $"Added {addedNum}. Comparing with Max-Heap root ({maxHeap[0]})",
                2 => $"Added {addedNum}. Rebalancing heaps...",
                3 => $"Added {addedNum}. Heaps balanced: Max-Heap({maxHeap.Count}), Min-Heap({minHeap.Count})",
                _ => $"Step {step + 1}: Added {addedNum}"
            };
        }
    }
}
