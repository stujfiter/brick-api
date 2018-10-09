using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Brick.Domain;
using Microsoft.Data.Sqlite;
using System.Data.Common;
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
        public ActionResult Post([FromBody] Piece piece)
        {
            if (string.IsNullOrWhiteSpace(piece.PartNumber)) {
                return BadRequest("Invalid Part Number");
            }

            if (string.IsNullOrWhiteSpace(piece.Description)) {
                return BadRequest("Invalid Description");
            }

            piece.Image = imageProcessor.ResizeImage(piece.Image, 400, 400);

            store.Save(piece);
            return NoContent();
        }
    }
}