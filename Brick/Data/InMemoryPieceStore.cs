using System.Collections.Generic;
using Brick.Domain;
using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace Brick.Data 
{
    public class InMemoryPieceStore : PieceStore
    {

        DbConnection connection;

        public InMemoryPieceStore() {
            connection = new SqliteConnection("Data Source=InMemorySample;Mode=Memory;Cache=Shared");
            connection.Open();

            var createTableCommand = connection.CreateCommand();
            createTableCommand.CommandText = @"CREATE TABLE piece (description TEXT)";
            createTableCommand.ExecuteNonQuery();

            var insertPieceCommand = connection.CreateCommand();
            insertPieceCommand.CommandText = @"INSERT INTO piece (description) VALUES ('2x4')";
            insertPieceCommand.ExecuteNonQuery();

        }

        public List<Piece> FindPieces()
        {
            List<Piece> pieces = new List<Piece>();

            var selectPieceQuery = connection.CreateCommand();
            selectPieceQuery.CommandText = @"SELECT description FROM piece";
            var reader = selectPieceQuery.ExecuteReader();

            while (reader.Read())
            {
                Piece p = new Piece();
                p.Description = reader.GetString(0);
                pieces.Add(p);
            }

            return pieces;
        }

        public void Save(Piece piece)
        {
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO piece (description) VALUES (@description)";
            SqliteParameter descriptionParameter = new SqliteParameter("@description", piece.Description);
            command.Parameters.Add(descriptionParameter);
            command.ExecuteNonQuery();
        }
    }
}