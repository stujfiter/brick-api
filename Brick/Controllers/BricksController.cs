using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Brick.Domain;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using Brick.Data;

namespace Brick.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class BricksController : ControllerBase 
    {

        private PieceStore store;

        public BricksController(PieceStore store)
        {
            this.store = store;
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

            store.Save(piece);
            return NoContent();
        }
    }
}