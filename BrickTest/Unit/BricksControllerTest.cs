using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Brick.Domain;
using Brick.Controllers;
using Brick.Data;
using Brick.Images.ImageSharp;

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
            controller = new BricksController(new InMemoryPieceStore(), new ImageSharpProcessor());
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

        [TestMethod]
        public void ShouldRejectPostsWithoutAuth()
        {
            Environment.SetEnvironmentVariable("AUTH_CODE", "blah");
            ActionResult result = controller.Post(piece);
            BadRequestObjectResult badRequestResult = result as BadRequestObjectResult;

            Environment.SetEnvironmentVariable("AUTH_CODE", null);
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(badRequestResult.Value, "Not Authorized");
        }

        [TestMethod]
        public void ShouldAcceptPostsWhenAuthIsNotSpecified()
        {
            Environment.SetEnvironmentVariable("AUTH_CODE", null);

            ActionResult result = controller.Post(piece);
            Assert.IsTrue(result is NoContentResult);
        }

        [TestMethod]
        public void ShouldAcceptPostsWhenAuthenticated()
        {
            Environment.SetEnvironmentVariable("AUTH_CODE", "foobar");

            ActionResult result = controller.Post(piece, "foobar");
            Assert.IsTrue(result is NoContentResult);
        }
    }
}