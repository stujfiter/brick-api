using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Brick.Domain;
using Brick.Controllers;

namespace BrickTest.Controllers
{
    [TestClass]
    public class BricksControllerTest
    {
        BricksController controller;
        Piece piece;

        [TestInitialize]
        public void Initialize()
        {
            controller = new BricksController(null);
            piece = new Piece() { PartNumber="1234", Description="1x5" };
        }

        [TestMethod]
        public void ShouldRejectBlankPartNumber()
        {
            piece.PartNumber = "";
            ActionResult result = controller.Post(piece);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            BadRequestObjectResult badReqestResult = result as BadRequestObjectResult;
            Assert.AreEqual(badReqestResult.Value, "Invalid Part Number");
        }

        [TestMethod]
        public void ShouldRejectBlankDescription()
        {
            piece.Description = "";

            ActionResult result = controller.Post(piece);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            BadRequestObjectResult badReqestResult = result as BadRequestObjectResult;
            Assert.AreEqual(badReqestResult.Value, "Invalid Description");
        }
    }
}