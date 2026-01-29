

## Heap Operations Flow Detail

```
graph TB
    subgraph "Add Number Process"
        A1[Start] --> A2[Receive num]
        A2 --> A3{Which heap?}
        
        A3 --> A4[Max-Heap if:<br/>1. Empty<br/>2. num ≤ Max-Heap root]
        A3 --> A5[Min-Heap if:<br/>num > Max-Heap root]
        
        A4 --> A6[Insert into Max-Heap]
        A5 --> A7[Insert into Min-Heap]
        
        A6 --> A8[Rebalance Heaps]
        A7 --> A8
    end
    
    subgraph "Rebalance Process"
        B1[Start Rebalance] --> B2[Check sizes]
        B2 --> B3{Size difference > 1?}
        
        B3 -->|Yes| B4[Move from larger<br/>to smaller heap]
        B3 -->|No| B5[Check heap property]
        
        B4 --> B5
        B5 --> B6{Max root > Min root?}
        B6 -->|Yes| B7[Swap heap roots]
        B6 -->|No| B8[End Rebalance]
        B7 --> B8
    end
    
    subgraph "Find Median Process"
        C1[Start Find Median] --> C2[Get heap sizes]
        C2 --> C3{Compare sizes}
        
        C3 -->|Max-Heap larger| C4[Return Max-Heap root]
        C3 -->|Min-Heap larger| C5[Return Min-Heap root]
        C3 -->|Equal size| C6[Return average<br/>of both roots]
        
        C4 --> C7[End: Return median]
        C5 --> C7
        C6 --> C7
    end
    
    A8 --> B1
    B8 --> C1

```
---

## Complete API Request-Response Flow

```
sequenceDiagram
    participant Client as API Client
    participant Controller as MedianController
    participant Service as MedianFinder
    participant MaxHeap as Max-Heap
    participant MinHeap as Min-Heap
    
    Client->>Controller: POST /api/median/add-single
    Note over Controller: {number: 42}
    
    Controller->>Service: AddNum(42)
    
    Service->>MaxHeap: Check if empty or 42 ≤ root
    alt Add to Max-Heap
        Service->>MaxHeap: Enqueue(42)
    else Add to Min-Heap
        Service->>MinHeap: Enqueue(42)
    end
    
    loop Rebalance Heaps
        Service->>MaxHeap: Get Count()
        Service->>MinHeap: Get Count()
        Service->>Service: Check balance condition
        alt Need to move from Max to Min
            Service->>MaxHeap: Dequeue()
            Service->>MinHeap: Enqueue(value)
        else Need to move from Min to Max
            Service->>MinHeap: Dequeue()
            Service->>MaxHeap: Enqueue(value)
        end
        
        Service->>MaxHeap: Peek()
        Service->>MinHeap: Peek()
        alt Max root > Min root
            Service->>MaxHeap: Dequeue()
            Service->>MinHeap: Dequeue()
            Service->>MaxHeap: Enqueue(minValue)
            Service->>MinHeap: Enqueue(maxValue)
        end
    end
    
    Service->>Controller: Return success
    
    Controller->>Service: GetMaxHeap()
    Service->>MaxHeap: Copy all elements
    MaxHeap-->>Service: Return heap array
    
    Controller->>Service: GetMinHeap()
    Service->>MinHeap: Copy all elements
    MinHeap-->>Service: Return heap array
    
    Controller->>Service: FindMedian()
    Service->>MaxHeap: Get Count()
    Service->>MinHeap: Get Count()
    
    alt Max-Heap larger
        Service->>MaxHeap: Peek()
        MaxHeap-->>Service: Root value
    else Min-Heap larger
        Service->>MinHeap: Peek()
        MinHeap-->>Service: Root value
    else Equal size
        Service->>MaxHeap: Peek()
        Service->>MinHeap: Peek()
        Service->>Service: Calculate average
    end
    
    Service-->>Controller: Return median
    
    Controller->>Service: GetAllNumbers()
    Service-->>Controller: Combined numbers
    
    Controller-->>Client: Return detailed response<br/>{maxHeap, minHeap, median, allNumbers}

```

## **Memory State Diagram (Step-by-Step)**

```
INITIAL STATE:
┌─────────────┬─────────────┐
│   MAX-HEAP  │   MIN-HEAP  │
│  (Lower)    │  (Upper)    │
├─────────────┼─────────────┤
│     []      │     []      │
└─────────────┴─────────────┘
Median: N/A

STEP 1: Add 5
┌─────────────┬─────────────┐
│   MAX-HEAP  │   MIN-HEAP  │
│  (Lower)    │  (Upper)    │
├─────────────┼─────────────┤
│     [5]     │     []      │
└─────────────┴─────────────┘
          5
Median = ─── = 5.0
          1

STEP 2: Add 15 (15 > 5 → goes to Min-Heap)
┌─────────────┬─────────────┐
│   MAX-HEAP  │   MIN-HEAP  │
│  (Lower)    │  (Upper)    │
├─────────────┼─────────────┤
│     [5]     │    [15]     │
└─────────────┴─────────────┘
        5 + 15
Median = ────── = 10.0
           2

STEP 3: Add 1 (1 < 5 → goes to Max-Heap)
┌─────────────┬─────────────┐
│   MAX-HEAP  │   MIN-HEAP  │
│  (Lower)    │  (Upper)    │
├─────────────┼─────────────┤
│   [5, 1]    │    [15]     │
└─────────────┴─────────────┘
⚠ UNBALANCED: 2 vs 1 ⚠
REBALANCE: Move 5 to Min-Heap
┌─────────────┬─────────────┐
│   MAX-HEAP  │   MIN-HEAP  │
│  (Lower)    │  (Upper)    │
├─────────────┼─────────────┤
│     [1]     │   [5, 15]   │
└─────────────┴─────────────┘
Median = 5.0

STEP 4: Add 3 (3 > 1 → goes to Min-Heap)
┌─────────────┬─────────────┐
│   MAX-HEAP  │   MIN-HEAP  │
│  (Lower)    │  (Upper)    │
├─────────────┼─────────────┤
│     [1]     │ [3, 5, 15]  │
└─────────────┴─────────────┘
⚠ UNBALANCED: 1 vs 3 ⚠
REBALANCE: Move 3 to Max-Heap
┌─────────────┬─────────────┐
│   MAX-HEAP  │   MIN-HEAP  │
│  (Lower)    │  (Upper)    │
├─────────────┼─────────────┤
│   [3, 1]    │   [5, 15]   │
└─────────────┴─────────────┘
        3 + 5
Median = ────── = 4.0
           2

FINAL SORTED: [1, 3, 5, 15]
```

## **Time Complexity Flow**

```
ADD OPERATION (O(log n)):
┌─────────────────────────────────────┐
│            Add Number               │
├─────────────────────────────────────┤
│ 1. Decide which heap: O(1)          │
│ 2. Insert into heap: O(log n)       │
│ 3. Rebalance (max 2 operations):    │
│    - Remove from heap: O(log n)     │
│    - Insert to heap: O(log n)       │
│ 4. Total: 3 × O(log n) = O(log n)   │
└─────────────────────────────────────┘

FIND MEDIAN (O(1)):
┌─────────────────────────────────────┐
│           Find Median               │
├─────────────────────────────────────┤
│ 1. Check heap sizes: O(1)           │
│ 2. Access heap root(s): O(1)        │
│ 3. Calculate (if needed): O(1)      │
│ 4. Total: O(1)                      │
└─────────────────────────────────────┘

MEMORY (O(n)):
┌─────────────────────────────────────┐
│          Memory Usage               │
├─────────────────────────────────────┤
│ 1. Max-Heap stores ~n/2 elements    │
│ 2. Min-Heap stores ~n/2 elements    │
│ 3. Total: O(n/2 + n/2) = O(n)       │
└─────────────────────────────────────┘
```