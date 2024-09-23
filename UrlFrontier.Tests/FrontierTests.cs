 using Microsoft.VisualStudio.TestTools.UnitTesting;
using KC.Dropins.FrontierCore;

namespace KC.DropIns.FrontierCore.Tests
{
    [TestClass]
    public class FrontierTests
    {
        [TestMethod]
        public void EnqueueUrlAsync_AddsUrlToQueue()
        {
            // Arrange
            var frontier = new Frontier(new FrontierOptions());

            // Act
            frontier.EnqueueUrlAsync("https://example.com");

            // Assert
            Assert.AreEqual(1, frontier.QueCount);
        }

        [TestMethod]
        public void DequeueUrlAsync_ReturnsUrlFromQueue()
        {
            // Arrange
            var frontier = new Frontier(new FrontierOptions());
            frontier.EnqueueUrlAsync("https://example.com");

            // Act
            UrlItem result = frontier.DequeueUrlAsync().Result;

            // Assert
            Assert.AreEqual("https://example.com", result.Address);
        }

        [TestMethod]
        public void EnqueueUrlAsync_DoesNotEnqueueDuplicateUrls()
        {
            // Arrange
            var frontier = new Frontier(new FrontierOptions());

            // Act
            frontier.EnqueueUrlAsync("https://example.com");
            frontier.EnqueueUrlAsync("https://example.com");

            // Assert
            Assert.AreEqual(1, frontier.QueCount);
        }

        [TestMethod]
        public void EnqueueUrlAsync_DoesNotEnqueueUrlIfQueueIsFull()
        {
            // Arrange
            var options = new FrontierOptions
            {
                QueueMaxCapacity = 1
            };
            var frontier = new Frontier(options);

            // Act
            frontier.EnqueueUrlAsync("https://example.com");
            frontier.EnqueueUrlAsync("https://example.com/2");

            // Assert
            Assert.AreEqual(1, frontier.QueCount);
        }

        // Add more test methods here...
    }
}
