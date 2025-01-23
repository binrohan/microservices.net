# Commands
## EF Core 
### Commands for CLI:
from Ordering.Infrastructure project
`dotnet ef migrations add InitialCreate --startup-project ../Ordering.API`
## Docker
Up:
`docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
`

Down:
`docker-compose -f docker-compose.yml -f docker-compose.override.yml down
`

Build:
`docker-compose -f docker-compose.yml -f docker-compose.override.yml up --build
`

## DOTNET CLI
### Api Gateway
Local: `dotnet run --launch-profile "OcelotApiGwLocal"`
