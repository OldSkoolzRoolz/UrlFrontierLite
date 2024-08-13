

namespace KC.Dropins.FrontierCore;

/// <summary>
/// A class that implements a priority queue.
/// </summary>
/// <typeparam name="UrlItem"></typeparam>
public class PriorityQueue<UrlItem>
{


    /// <summary>
    /// The dictionary of priority queues. The key is the priority level, and the value is the queue for that priority level.
    /// <param name="T">The type of the items in the queue. <see cref="UrlItem"/> is used.</param>
    /// </summary>
    private readonly SortedDictionary<int, Queue<UrlItem>> _queueDict = new SortedDictionary<int, Queue<UrlItem>>();

    /// <summary>
    /// Enqueues an item with the specified priority to the priority queue.
    /// If the priority level(key) does not exist in the dictionary, it creates a new queue for that priority level.
    /// Then it adds the item to the newly created queue.
    /// </summary>
    /// <param name="item">The UrlItem to enqueue.</param>
    /// <param name="priority">The priority level of the item.</param>
    public void Enqueue(UrlItem item, int priority)
    {
        // Check if the priority level already exists in the queue dictionary
        if (!_queueDict.ContainsKey(priority))
        {
            // If the priority level does not exist, create a new queue for that priority level
            _queueDict[priority] = new Queue<UrlItem>();
        }

        // Add the item to the queue for the specified priority level
        _queueDict[priority].Enqueue(item);
    }




    /// <summary>
    /// Dequeues an item with the highest priority from the priority queue.
    /// It finds the queue with the highest priority, dequeues the item from that queue,
    /// and removes the queue if it becomes empty.
    /// </summary>
    /// <returns>The item with the highest priority.</returns>
    public UrlItem? Dequeue()
    {
        if (_queueDict.Count == 0) // If the queue dictionary is empty
        {
            return default(UrlItem); // Return null
        }

        // Find the queue with the highest priority
        using (var queueEnumerator = _queueDict.GetEnumerator())
        {
            if (!queueEnumerator.MoveNext())
            {
                throw new Exception("Failed to move to the next item in the queue dictionary.");
            }

            var queueWithHighestPriority = queueEnumerator.Current.Value; // Get the queue associated with the highest priority

            if (queueWithHighestPriority == null)
            {
                throw new Exception("The queue associated with the highest priority is null.");
            }

            UrlItem item = queueWithHighestPriority.Dequeue(); // Dequeue the item from the queue with the highest priority

            if (queueWithHighestPriority.Count == 0) // If the queue becomes empty after dequeue
            {
                if (!_queueDict.Remove(queueEnumerator.Current.Key))
                {
                    throw new Exception("Failed to remove the queue from the SortedDictionary.");
                }
            }

            return item; // Return the item with the highest priority
        }
    }



















    public int Count
    {
        get { return _queueDict.Values.Sum(queue => queue.Count); }
    }





}