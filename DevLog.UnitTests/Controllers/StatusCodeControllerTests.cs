using DevLog.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DevLog.UnitTests.Controllers
{
    [TestFixture]
    public class StatusCodeControllerTests
    {
        private StatusCodeController _controller;

        [SetUp]
        public void SetUp()
        {
            _controller = new StatusCodeController();
        }

        [TestCase(404)]
        [TestCase(500)]
        [TestCase(403)]
        public void Index_ReturnsViewWithStatusCodeModel(int code)
        {
            // Act
            var result = _controller.Index(code);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.AreEqual(code, viewResult?.Model);
        }
    }
}