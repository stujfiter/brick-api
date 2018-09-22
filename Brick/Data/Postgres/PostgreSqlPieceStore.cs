using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using Brick.Domain;
using Npgsql;

namespace Brick.Data {
    public class PostgreSqlPieceStore : PieceStore 
    {
        string applicationDataStore = "brick";
        string systemDataStore = "template1";

        public PostgreSqlPieceStore() {

            string[] pathElements = {"Data", "Postgres", "deltas"};
            string path = Path.Combine(pathElements);

            createDatabaseIfNotExists();
            migrate(path, "0000_InitializeDB.psql");
            migrate(path, "0001_AddPiecePartNumber.psql");
            migrate(path, "0002_AddNotNullConstraints.psql");
            migrate(path, "0003_AddPieceImage.psql");
        }

        public List<Piece> FindPieces()
        {
            List<Piece> results = new List<Piece>();
            
            using (var conn = getConnection(applicationDataStore))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT partnumber, description, image FROM piece", conn))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        Piece piece = new Piece();

                        piece.PartNumber = SafeGetString(reader, 0);
                        piece.Description = SafeGetString(reader, 1);
                        piece.Image = SafeGetString(reader, 2);
                        results.Add(piece);
                    }
            }

            return results;
        }

        public void Save(Piece piece)
        {
            using (var conn = getConnection(applicationDataStore))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand($"INSERT INTO piece (partnumber, description, image) VALUES (@partNumber, @description, @image)", conn))
                {
                    cmd.Parameters.AddWithValue("partNumber", piece.PartNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("description", piece.Description);
                    cmd.Parameters.AddWithValue("image", piece.Image ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static string SafeGetString(DbDataReader reader, int colIndex)
        {
            if(!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            return string.Empty;
        }

        NpgsqlConnection getConnection(string database)
        {
            var hostName = Environment.GetEnvironmentVariable("STORAGE_HOST") ?? "localhost";
            var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            var connString = $"Host={hostName};Username=postgres;Password={password};Database={database}";
            return new NpgsqlConnection(connString);
        }

        void createDatabaseIfNotExists()
        {
            var databaseExists = false;
            using (var conn = getConnection(systemDataStore))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname='{applicationDataStore}';", conn))
                using (var reader = cmd.ExecuteReader())
                    databaseExists = reader.Read();

                if (!databaseExists)
                {
                    using (var cmd = new NpgsqlCommand($"CREATE DATABASE {applicationDataStore};", conn))
                        cmd.ExecuteNonQuery();
                }
            }
        }

        void migrate(string path, string fileName)
        {   
            var initDB = File.ReadAllText(Path.Combine(path, fileName));
            using (var conn = getConnection(applicationDataStore))
            {
                conn.Open();

                var cmd = new NpgsqlCommand(initDB, conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}