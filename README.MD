# IP Show App
---

Project Evolution

1. **Base Template**  
   Started from the official minimal .NET Web API template

2. **Multi-stage Dockerfile**  
   - **Build stage**: Restored dependencies (dotnet restore) and published release artifacts (dotnet publish)
   - **Runtime stage**: Used the lightweight ASP.NET runtime image and copied only published output

3. **Support Scripts**  
   - **Healthcheck** (scripts/healthcheck.sh): Verifies that the web endpoint (/health) returns "200 OK"
   - **Restart** (scripts/restart.sh): Stops and restarts the Docker Compose services
   - **Log Rotation** (scripts/logrotate.sh): Archives and truncates logs/errors.log daily to prevent disk bloat

4. **PostgreSQL Integration**  
   Integrated PostgreSQL: added db/init.sql to create a visits(ip, visited_at) table
   On every GET / request, the app uses Npgsql to INSERT the client’s IP into that table
   Exposed a GET /stats endpoint that returns the total number of visits by running SELECT COUNT(*) FROM visits

5. **Docker Compose** (`docker-compose.yml`)  
   - **web**: .NET app on port `8000`, environment-driven DB connection string
   - **db**: PostgreSQL `15-alpine`, initialized from `init.sql`, with persistent volume `db_data`
   - **nginx**: Reverse proxy on port `80` forwarding to `web:8000`
   - **sonar**: SonarQube Community server on port `9000` for code quality

6. **CI/CD Pipeline** (.github/workflows/ci-cd.yml)  
   - **Trigger**: on `push` or `pull_request` to `main`
   - **Build job**:  
     1. Checkout code & cache NuGet
     2. Setup .NET 9.0 & restore, build, test
     3. Run SonarScanner via Docker CLI for code analysis 
     4. Build Docker image tagged with commit SHA 
     5. Push to Docker Hub
   - **Deploy job**:  
     1. Ssh into Linux server
     2. git pull, docker-compose pull, docker-compose up -d --build

7. **SonarQube Integration**  
   - Configured sonar-project.properties with "projectKey=ip-show-app"  
   - Defined a custom Quality Gate:  
     - **New Coverage ≥ 80%**  
     - **New Duplications ≤ 3%**  
     - **New Vulnerabilities = 0**  
   

