using Microsoft.VisualStudio.TestTools.UnitTesting;
using KC.Dropins.FrontierCore;
using System;
using System.Collections.Generic;

namespace KC.Dropins.FrontierCore.Tests
{
    [TestClass]
    public class HostAccessTrackerTests
    {
        [TestMethod]
        public void Constructor_ValidParameters_InitializesProperties()
        {
            // Arrange
            int maxAccessCount = 10;
            TimeSpan minTimeSinceLastAccess = TimeSpan.FromMinutes(1);

            // Act
            var tracker = new HostAccessTracker(maxAccessCount, minTimeSinceLastAccess);

            // Assert
            Assert.AreEqual(maxAccessCount, tracker._maxAccessCount);
            Assert.AreEqual(minTimeSinceLastAccess, tracker._minTimeSinceLastAccess);
        }

        [TestMethod]
        public void RecordAccess_NewHost_AddsHostToAccessData()
        {
            // Arrange
            var tracker = new HostAccessTracker(10, TimeSpan.FromMinutes(1));
            string host = "example.com";

            // Act
            tracker.RecordAccess(host);

            // Assert
            Assert.IsTrue(tracker.AccessData.ContainsKey(host));
            Assert.AreEqual(1, tracker.AccessData[host].AccessCount);
        }

        [TestMethod]
        public void RecordAccess_ExistingHost_IncrementsAccessCount()
        {
            // Arrange
            var tracker = new HostAccessTracker(10, TimeSpan.FromMinutes(1));
            string host = "example.com";
            tracker.RecordAccess(host);

            // Act
            tracker.RecordAccess(host);

            // Assert
            Assert.AreEqual(2, tracker.AccessData[host].AccessCount);
        }

        [TestMethod]
        public void GetAccessCount_HostExists_ReturnsAccessCount()
        {
            // Arrange
            var tracker = new HostAccessTracker(10, TimeSpan.FromMinutes(1));
            string host = "example.com";
            tracker.RecordAccess(host);
            tracker.RecordAccess(host);

            // Act
            int accessCount = tracker.GetAccessCount(host);

            // Assert
            Assert.AreEqual(2, accessCount);
        }

        [TestMethod]
        public void GetAccessCount_HostDoesNotExist_ReturnsZero()
        {
            // Arrange
            var tracker = new HostAccessTracker(10, TimeSpan.FromMinutes(1));
            string host = "example.com";

            // Act
            int accessCount = tracker.GetAccessCount(host);

            // Assert
            Assert.AreEqual(0, accessCount);
        }

        [TestMethod]
        public void GetTimeSinceLastAccess_HostExists_ReturnsTimeSpan()
        {
            // Arrange
            var tracker = new HostAccessTracker(10, TimeSpan.FromMinutes(1));
            string host = "example.com";
            tracker.RecordAccess(host);

            // Act
            TimeSpan timeSinceLastAccess = tracker.GetTimeSinceLastAccess(host);

            // Assert
            Assert.IsTrue(timeSinceLastAccess > TimeSpan.Zero);
        }

        [TestMethod]
        public void GetTimeSinceLastAccess_HostDoesNotExist_ReturnsZero()
        {
            // Arrange
            var tracker = new HostAccessTracker(10, TimeSpan.FromMinutes(1));
            string host = "example.com";

            // Act
            TimeSpan timeSinceLastAccess = tracker.GetTimeSinceLastAccess(host);

            // Assert
            Assert.AreEqual(TimeSpan.Zero, timeSinceLastAccess);
        }

        [TestMethod]
        public void IsAccessCountExceeded_HostExistsAndAccessCountExceeded_ReturnsTrue()
        {
            // Arrange
            var tracker = new HostAccessTracker(1, TimeSpan.FromMinutes(1));
            string host = "example.com";
            tracker.RecordAccess(host);

            // Act
            bool isAccessCountExceeded = tracker.IsAccessCountExceeded(host);

            // Assert
            Assert.IsTrue(isAccessCountExceeded);
        }

        [TestMethod]
        public void IsAccessCountExceeded_HostExistsButAccessCountNotExceeded_ReturnsFalse()
        {
            // Arrange
            var tracker = new HostAccessTracker(2, TimeSpan.FromMinutes(1));
            string host = "example.com";
            tracker.RecordAccess(host);

            // Act
            bool isAccessCountExceeded = tracker.IsAccessCountExceeded(host);

            // Assert
            Assert.IsFalse(isAccessCountExceeded);
        }

        [TestMethod]
        public void IsTimeSinceLastAccessExceeded_HostExistsAndTimeSinceLastAccessExceeded_ReturnsTrue()
        {
            // Arrange
            var tracker = new HostAccessTracker(10, TimeSpan.FromMinutes(1));
            string host = "example.com";
            tracker.RecordAccess(host);
            System.Threading.Thread.Sleep(TimeSpan.FromMinutes(2).Milliseconds);

            // Act
            bool isTimeSinceLastAccessExceeded = tracker.IsTimeSinceLastAccessExceeded(host);

            // Assert
            Assert.IsTrue(isTimeSinceLastAccessExceeded);
        }
    }
}


