using DevLog.Areas.Panel.Extensions;
using System.Security.Claims;

namespace DevLog.UnitTests.Areas.Panel.Extensions
{
    [TestFixture]
    public class ClaimPrincipalExtensionsTests
    {
        [Test]
        public void GetUserId_WhenPrincipalIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            ClaimsPrincipal principal = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => principal.GetUserId());
        }

        [Test]
        public void GetUserId_WhenNameIdentifierClaimExists_ReturnsUserId()
        {
            // Arrange
            var claim = new Claim(ClaimTypes.NameIdentifier, "user123");
            var identity = new ClaimsIdentity(new[] { claim });
            var principal = new ClaimsPrincipal(identity);

            // Act
            var userId = principal.GetUserId();

            // Assert
            Assert.That(userId, Is.EqualTo("user123"));
        }

        [Test]
        public void GetUserId_WhenNameIdentifierClaimDoesNotExist_ReturnsNull()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);

            // Act
            var userId = principal.GetUserId();

            // Assert
            Assert.IsNull(userId);
        }
    }
}
