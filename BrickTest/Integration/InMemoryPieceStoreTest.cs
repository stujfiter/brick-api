using Microsoft.VisualStudio.TestTools.UnitTesting;
using Brick.Data;
using Brick.Domain;

namespace BrickTest.Integration
{
    [TestClass]
    public class InMemoryPieceStoreTest
    {
        [TestMethod]
        public void ShouldStoreAndRetrievePieceProperly()
        {
            InMemoryPieceStore store = new InMemoryPieceStore();
            Piece p = new Piece() {PartNumber="3002", Description="2x2"};
            store.Save(p);

            Piece retrievedPiece = store.FindPieces()[0];
            Assert.AreEqual(p.PartNumber, retrievedPiece.PartNumber);
            Assert.AreEqual(p.Description, retrievedPiece.Description);
        }
    }
}
