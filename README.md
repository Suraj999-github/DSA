# DSA

 - Implement LRU Cache
 - Implement LFU Cache
 - Find Median from Data Stream
 - Merge K Sorted Lists
 - Word Ladder (shortest transformation)
 - Detect Cycle in a Directed Graph
 - Kth Largest Element in a Stream
 - Top K Frequent Elements
 - Maximum Subarray Sum (Kadane + variations)
 - Sliding Window Maximum
 - Longest Substring Without Repeating Characters
 - Minimum Window Substring
 - Lowest Common Ancestor (Binary Tree)
 - Serialize & Deserialize Binary Tree
 - Number of Islands
 - Course Schedule (Topological Sort)
 - Coin Change (DP)
 - Design a Task Scheduler with Priorities
 
 ### **Quick Reference for LRU Cache and LFU Cache **

| **Choose LRU When:** | **Choose LFU When:** |
|----------------------|----------------------|
| • Recent access matters | • Frequency matters |
| • Sequential patterns | • Stable access patterns |
| • Memory constrained | • Can tolerate memory overhead |
| • Simple implementation needed | • Popular content needs to stay |


 
 ## **Summary of Importance Finding Median from Data Stream**

1. **Real-time Analytics**: Essential for streaming data where immediate insights are needed
2. **Anomaly Detection**: Perfect for identifying outliers in continuous data streams
3. **Resource Efficiency**: O(1) median retrieval with O(log n) updates
4. **Statistical Robustness**: Median provides better central tendency with skewed data
5. **Scalability**: Handles millions of data points with minimal memory footprint
6. **Industry Standard**: Used across finance, IoT, healthcare, and tech industries

This pattern is particularly valuable in modern applications where:
- Data arrives continuously
- Immediate decisions are required
- System resources are constrained
- Data may contain extreme values that shouldn't skew analysis