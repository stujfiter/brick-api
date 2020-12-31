Local Development
-----------------
1. `brew install postgres`
1. `docker-compose up -d postgres`
1. `psql -h localhost -U postgres`

To Run the Application
----------------------
`STORAGE_HOST=postgres POSTGRES_PASSWORD=<redacted> docker-compose up -d`