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
            connection = new SqliteConnection("Data Source=InMemorySample;Mode=Memory;Cache=Private");
            connection.Open();

            var createTableCommand = connection.CreateCommand();
            createTableCommand.CommandText = @"CREATE TABLE piece (description TEXT not null, partNumber TEXT not null, image BLOB)";
            createTableCommand.ExecuteNonQuery();

            createTableCommand.CommandText = @"CREATE TABLE inventory (partnumber TEXT not null, quantity INTEGER not null)";
            createTableCommand.ExecuteNonQuery();
        }

        public List<Piece> FindPieces()
        {
            List<Piece> pieces = new List<Piece>();

            var selectPieceQuery = connection.CreateCommand();
            selectPieceQuery.CommandText = @"SELECT partNumber, description, image FROM piece";
            var reader = selectPieceQuery.ExecuteReader();

            while (reader.Read())
            {
                Piece p = new Piece();
                p.PartNumber = SafeGetString(reader, 0);
                p.Description = reader.GetString(1);
                p.Image = SafeGetString(reader, 2);
                pieces.Add(p);
            }

            return pieces;
        }

        public void Save(Piece piece)
        {
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO piece (partNumber, description, image) VALUES (@partNumber, @description, @image)";
            
            SqliteParameter descriptionParameter = new SqliteParameter("@description", piece.Description);
            command.Parameters.Add(descriptionParameter);

            SqliteParameter partNumberParameter = new SqliteParameter("@partNumber", piece.PartNumber ?? (object)DBNull.Value);
            command.Parameters.Add(partNumberParameter);

            SqliteParameter imageParameter = new SqliteParameter("@image", piece.Image ?? (object)DBNull.Value);
            command.Parameters.Add(imageParameter);

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