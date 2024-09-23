using Microsoft.VisualStudio.TestTools.UnitTesting;
using KC.DropIns.FrontierCore;
using System;

namespace FrontierTests
{
    [TestClass]
    public class FrontierTests
    {
     

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ThrowsArgumentNullException_WhenOptionsIsNull()
        {
            // Act
            new Frontier(null);
        }
    }
}