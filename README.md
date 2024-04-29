# Graduation's Phase Five Project
Post-graduation fifth phase project

## The project
The project is about a role Tech Blog system, with multiples microservices, databases and more.

## Build and Test
```bash
# [IMPORTANT]:
# You need to have .NET 6 installed in your computer
# You also need to be at the same folder as the solution file (.sln)

# First, restore the dependencies
dotnet restore

# After, build the application
dotnet build

# If want to execute the application tests
dotnet test
```

## Running the project
Follow these steps to run the project locally.
```bash
# [IMPORTANT]:
# You need to have Docker installed in your computer
	
# Execute all APIs and workers for production
docker-compose -f docker/docker-compose.yml up --build 

# Execute an instance of RabbitMq and all APIs and workers for staging
docker-compose -f docker/docker-compose-staging.yml up --build

# Execute the instance of RabbitMq for development tests
docker-compose -f docker/docker-compose-develop.yml up --build
```

## Help links
| Link | Description |
|------|-------------|
| [Technologies Used](/docs/tech-used.md) | A list of all key-words of the technologies used on the project |
| [Web-Queue-Worker architecture style](https://learn.microsoft.com/en-us/azure/architecture/guide/architecture-styles/web-queue-worker) | Explains the client-worker relation
| [Worker services in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/workers?pivots=dotnet-6-0) | Structure for workers
| [Email sending flow](/docs/send-email-flow.png) | The drawing of the email sending flow 

## Wiki
Access our [Wiki](https://dev.azure.com/fiap-net-group/FIAP/_wiki/wikis/FIAP.wiki/43/Graduation-Phase-Five) for more information!