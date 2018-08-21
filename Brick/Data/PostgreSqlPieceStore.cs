using System;
using System.Collections.Generic;
using System.Data.Common;
using Brick.Domain;
using Npgsql;

namespace Brick.Data {
    public class PostgreSqlPieceStore : PieceStore 
    {
        string applicationDataStore = "brick";
        string systemDataStore = "template1";


        public PostgreSqlPieceStore() {
            createDatabaseIfNotExists();
            createTableIfNotExits();
            createColumnIfNotExists("piece", "partnumber");

        }

        public List<Piece> FindPieces()
        {
            List<Piece> results = new List<Piece>();
            
            using (var conn = getConnection(applicationDataStore))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT partnumber, description FROM piece", conn))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        Piece piece = new Piece();

                        piece.PartNumber = SafeGetString(reader, 0);
                        piece.Description = reader.GetString(1);
                        results.Add(piece);
                    }
            }

            return results;
        }

        public static string SafeGetString(DbDataReader reader, int colIndex)
        {
            if(!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            return string.Empty;
        }

        public void Save(Piece piece)
        {
            using (var conn = getConnection(applicationDataStore))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand($"INSERT INTO piece (partnumber, description) VALUES (@partNumber, @description)", conn))
                {
                    cmd.Parameters.AddWithValue("partNumber", piece.PartNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("description", piece.Description);
                    cmd.ExecuteNonQuery();
                }
            }
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

                using (var cmd = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname='brick';", conn))
                using (var reader = cmd.ExecuteReader())
                    databaseExists = reader.Read();

                if (!databaseExists)
                {
                    using (var cmd = new NpgsqlCommand("CREATE DATABASE brick;", conn))
                        cmd.ExecuteNonQuery();
                }
            }
        }

        void createTableIfNotExits()
        {
            var tableExists = false;
            using (var conn = getConnection(applicationDataStore))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT 1 FROM pg_tables WHERE schemaname = 'public' AND tablename = 'piece';", conn))
                using (var reader = cmd.ExecuteReader())
                    tableExists = reader.Read();

                if (!tableExists)
                {
                    using (var cmd = new NpgsqlCommand("CREATE TABLE piece (description varchar(100) not null);", conn))
                        cmd.ExecuteNonQuery();
                }
            }
        }

        void createColumnIfNotExists(string tableName, string columnName)
        {
            var columnExists = false;
            using (var conn = getConnection(applicationDataStore))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT attname FROM pg_attribute WHERE attrelid = (SELECT oid FROM pg_class WHERE relname = 'piece') AND attname = 'partnumber';", conn))
                using (var reader = cmd.ExecuteReader())
                    columnExists = reader.Read();

                if (!columnExists)
                {
                    using(var cmd = new NpgsqlCommand("ALTER TABLE piece ADD COLUMN partnumber varchar(100);",conn))
                        cmd.ExecuteNonQuery();
                }
            }
        }
    }
}