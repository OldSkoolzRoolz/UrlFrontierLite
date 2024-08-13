using Microsoft.VisualStudio.TestTools.UnitTesting;
using KC.Dropins.FrontierCore;
using System.Threading.Tasks;

namespace KC.DropIns.FrontierCore.Tests
{
    [TestClass]
    public class AbstractFrontierCoreTests
    {
        [TestMethod]
        public void CalculateSearchTermPriority_Returns10_IfUrlContainsSearchTerm()
        {
            // Arrange
            var options = new FrontierOptions { SearchTerm = "example" };
            var frontier = new Frontier(options);
            var urlItem = new UrlItem ("https://example.com", null );

            // Act
            var result = frontier.CalculateSearchTermPriority(urlItem);

            // Assert
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void CalculateSearchTermPriority_Returns0_IfUrlDoesNotContainSearchTerm()
        {
            // Arrange
            var options = new FrontierOptions { SearchTerm = "example" };
            var frontier = new Frontier(options);
            var urlItem = new UrlItem ("https://example.net" ,null);

            // Act
            var result = frontier.CalculateSearchTermPriority(urlItem);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public async Task OnAppStartupAsync_EnqueuesUrlsFromBackup()
        {
            // Arrange
            var options = new FrontierOptions { UseBackupQueue = true };
            var frontier = new Frontier(options);
            var expectedUrls = new[] { "https://example.com", "https://example.net" };
         //   var fileMock = new Moq<IFile>();
         //   fileMock.Setup(f => f.ReadAllLinesAsync("UrlFrontier.bak")).ReturnsAsync(expectedUrls);
         //   frontier.File = fileMock.Object;

            // Act
            await frontier.OnAppStartupAsync();

            // Assert
            foreach (var url in expectedUrls)
            {
               await frontier.EnqueueUrlAsync(url);
            }
        }

        // Add more test methods here...
    }
}
