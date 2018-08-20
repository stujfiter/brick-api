using System;
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

            var alterTableCommand = connection.CreateCommand();
            alterTableCommand.CommandText = @"ALTER TABLE piece ADD COLUMN partNumber TEXT";
            alterTableCommand.ExecuteNonQuery();

            var insertPieceCommand = connection.CreateCommand();
            insertPieceCommand.CommandText = @"INSERT INTO piece (description, partNumber) VALUES ('2x4', '3001')";
            insertPieceCommand.ExecuteNonQuery();

        }

        public List<Piece> FindPieces()
        {
            List<Piece> pieces = new List<Piece>();

            var selectPieceQuery = connection.CreateCommand();
            selectPieceQuery.CommandText = @"SELECT partNumber, description FROM piece";
            var reader = selectPieceQuery.ExecuteReader();

            while (reader.Read())
            {
                Piece p = new Piece();
                p.PartNumber = SafeGetString(reader, 0);
                p.Description = reader.GetString(1);
                pieces.Add(p);
            }

            return pieces;
        }

        public void Save(Piece piece)
        {
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO piece (partNumber, description) VALUES (@partNumber, @description)";
            
            SqliteParameter descriptionParameter = new SqliteParameter("@description", piece.Description);
            command.Parameters.Add(descriptionParameter);

            SqliteParameter partNumberParameter = new SqliteParameter("@partNumber", piece.PartNumber ?? (object)DBNull.Value);
            command.Parameters.Add(partNumberParameter);

            command.ExecuteNonQuery();
        }

        public static string SafeGetString(DbDataReader reader, int colIndex)
        {
            if(!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            return string.Empty;
        }
    }
}