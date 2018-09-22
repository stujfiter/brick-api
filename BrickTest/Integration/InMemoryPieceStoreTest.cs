using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.Sqlite;
using Brick.Data;
using Brick.Domain;

namespace BrickTest.Integration
{
    [TestClass]
    public class InMemoryPieceStoreTest
    {
        static InMemoryPieceStore store;
        static string tinyJpeg = "ABBKRklGAAEBAQBIAEgAAABDAAALCAABAAEBAREAABQQAQAAAAAAAAAAAAAAAAAAAAAACAEBAAE/EA==";

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            store = new InMemoryPieceStore();
        }

        [TestMethod]
        public void ShouldStoreAndRetrievePieceProperly()
        {
            Piece p = new Piece() {PartNumber="3002", Description="2x2", Image=tinyJpeg};
            store.Save(p);

            Piece retrievedPiece = store.FindPieces()[0];
            Assert.AreEqual(p.PartNumber, retrievedPiece.PartNumber);
            Assert.AreEqual(p.Description, retrievedPiece.Description);
            Assert.AreEqual(p.Image, retrievedPiece.Image);

        }

        [TestMethod]
        [ExpectedException(typeof(SqliteException))]
        public void ShouldRejectInvalidPartNumber()
        {
            Piece p = new Piece() {Description="2x2"};
            store.Save(p);
        }
    }
}
