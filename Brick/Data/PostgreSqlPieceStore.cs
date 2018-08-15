using System.Collections.Generic;
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

        }

        public List<Piece> FindPieces()
        {
            List<Piece> results = new List<Piece>();
            
            using (var conn = getConnection(applicationDataStore))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT description FROM piece", conn))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        Piece piece = new Piece();
                        piece.Description = reader.GetString(0);
                        results.Add(piece);
                    }
            }

            return results;
        }

        public void Save(Piece piece)
        {
            
        }

        NpgsqlConnection getConnection(string database)
        {
            var connString = $"Host=localhost;Username=postgres;Password=changeme;Database={database}";
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
    }
}