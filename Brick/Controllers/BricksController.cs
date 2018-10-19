using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Brick.Domain;
using Brick.Data;
using Brick.Images;

namespace Brick.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class BricksController : ControllerBase 
    {

        private PieceStore store;
        private ImageProcessor imageProcessor;

        public BricksController(PieceStore store, ImageProcessor imageProcessor)
        {
            this.store = store;
            this.imageProcessor = imageProcessor;
        }

        // Get api/bricks
        [HttpGet]
        public ActionResult<List<Piece>> Get()
        {
            return store.FindPieces();
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public ActionResult Post([FromBody] Piece piece, [FromHeader] string auth = null)
        {
            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AUTH_CODE")) 
                && !Environment.GetEnvironmentVariable("AUTH_CODE").Equals(auth))
            {
                return BadRequest("Not Authorized");
            }

            if (string.IsNullOrWhiteSpace(piece.PartNumber)) 
            {
                return BadRequest("Invalid Part Number");
            }

            if (string.IsNullOrWhiteSpace(piece.Description)) 
            {
                return BadRequest("Invalid Description");
            }

            if (!string.IsNullOrWhiteSpace(piece.Image))
            {
                piece.Image = imageProcessor.ResizeImage(piece.Image, 400, 400);
            }

            store.Save(piece);
            return NoContent();
        }
    }
}