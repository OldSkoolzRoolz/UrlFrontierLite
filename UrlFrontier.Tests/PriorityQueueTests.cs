using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using KC.Dropins.FrontierCore.Helpers;

namespace KC.Dropins.FrontierCore.Tests.Helpers
{
    [TestClass]
    public class PriorityQueueTests
    {
        [TestMethod]
        public void Enqueue_ItemWithHigherPriority_IsDequeuedFirst()
        {
            // Arrange
            var priorityQueue = new PriorityQueue<UrlItem>();
            var item1 = new UrlItem ("https://example.com/1",null );
            var item2 = new UrlItem ("https://example.com/2",null);

            // Act
            priorityQueue.Enqueue(item1, 1);
            priorityQueue.Enqueue(item2, 2);

            // Assert
            var dequeuedItem = priorityQueue.Dequeue();
            Assert.AreEqual(item2, dequeuedItem);
        }

        [TestMethod]
        public void Enqueue_ItemWithLowerPriority_IsDequeuedLast()
        {
            // Arrange
            var priorityQueue = new PriorityQueue<UrlItem>();
            var item1 = new UrlItem("https://example.com/1", null);
            var item2 = new UrlItem("https://example.com/2", null);

            // Act
            priorityQueue.Enqueue(item1, 2);
            priorityQueue.Enqueue(item2, 1);

            // Assert
            var dequeuedItem = priorityQueue.Dequeue();
            Assert.AreEqual(item2, dequeuedItem);
        }

        [TestMethod]
        public void Enqueue_MultipleItemsWithSamePriority_AreDequeuedInOrder()
        {
            // Arrange
            var priorityQueue = new PriorityQueue<UrlItem>();
            var item1 = new UrlItem ( "https://example.com/1",null );
            var item2 = new UrlItem ( "https://example.com/2",null );
            var item3 = new UrlItem ( "https://example.com/3", null );

            // Act
            priorityQueue.Enqueue(item1, 1);
            priorityQueue.Enqueue(item2, 1);
            priorityQueue.Enqueue(item3, 1);

            // Assert
            var dequeuedItem1 = priorityQueue.Dequeue();
            Assert.AreEqual(item1, dequeuedItem1);

            var dequeuedItem2 = priorityQueue.Dequeue();
            Assert.AreEqual(item2, dequeuedItem2);

            var dequeuedItem3 = priorityQueue.Dequeue();
            Assert.AreEqual(item3, dequeuedItem3);
        }

        [TestMethod]
        public void Dequeue_EmptyQueue_ReturnsNull()
        {
            // Arrange
            var priorityQueue = new PriorityQueue<UrlItem>();

            // Act
            var dequeuedItem = priorityQueue.Dequeue();

            // Assert
            Assert.IsNull(dequeuedItem);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Dequeue_QueueWithNoItems_ThrowsException()
        {
            // Arrange
            var priorityQueue = new PriorityQueue<UrlItem>();

            // Act
            priorityQueue.Dequeue();
        }
    }
}
