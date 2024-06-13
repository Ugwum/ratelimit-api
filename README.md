# RateLimitDemoApi

This project demonstrates how to implement rate limiting in an ASP.NET Core Web API using Redis as a distributed cache. The rate limiting functionality is encapsulated in a middleware that checks if a client has exceeded the allowed number of requests within a specified time period.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Setup](#setup)
  - [Configuration](#configuration)
  - [Building and Running the Application](#building-and-running-the-application)
- [Project Structure](#project-structure)
- [Usage](#usage)
- [Docker Support](#docker-support)
- [License](#license)

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Docker](https://www.docker.com/get-started)
- [Redis](https://redis.io/)

## Setup

### Configuration

The application is configured using the `appsettings.json` file. You need to configure your Redis connection and rate limiting rules here.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "RedisConnection": {
    "Host": "redis",
    "Port": "6379",
    "Password": "password1",
    "UseSSL": "false"
  },
  "RateLimit": {
    "RateLimitRules": [
      {
        "Endpoint": "/api/v1/start",
        "Limit": 5,
        "Period": "00:01:00"
      },
      {
        "Endpoint": "/api/v1/start/recovery",
        "Limit": 10,
        "Period": "00:05:00"
      },
      {
        "Endpoint": "/poc/test/api/v1/public/key-exchange",
        "Limit": 10,
        "Period": "00:05:00"
      },
      {
        "Endpoint": "/weatherforecast",
        "Limit": 10,
        "Period": "00:00:03"
      }
    ]
  },
  "AllowedHosts": "*"
}
```

### Building and Running the Application

#### 1.  Clone the repository:
```sh
git clone https://github.com/your-repo/RateLimitDemoApi.git
cd RateLimitDemoApi
```
#### 2. Restore the dependencies:
```sh
dotnet restore
```
#### 3. Build the application:
```sh
dotnet build
```
#### 4. Run the application:
```sh
dotnet run
```
The application should now be running at http://localhost:5000.

## Project Structure
 - RateLimitDemoApi: The main API project.
 - Program.cs: Configures services and middleware.
 - appsettings.json: Contains configuration settings for the application.
 - RateLimitLib: Contains the rate limiting logic and interfaces.
 - Abstraction/ICacheProvider.cs: Interface for the cache provider.
 - Abstraction/IRateLimitConfiguration.cs: Interface for the rate limit configuration.
 - Cache/RedisCache.cs: Implementation of the ICacheProvider interface using Redis.
 - Configuration/RateLimitConfiguration.cs: Implementation of the IRateLimitConfiguration interface.
 - Middleware/RateLimitMiddleware.cs: Middleware for handling rate limiting.
 
## Usage
To test the rate limiting, make requests to the API endpoints defined in the RateLimit section of appsettings.json. For example, to test the /weatherforecast endpoint, you can use a tool like curl:
```sh
curl http://localhost:5000/weatherforecast
```
If the rate limit is exceeded, the API will return a 429 Too Many Requests status code.


## Docker Support
The project includes a Dockerfile and a docker-compose.yml file to facilitate running the application in a Docker container along with Redis.

##### Build and run the Docker containers:
```sh
docker-compose up --build
```
The API will be available at http://localhost:5001 and Redis at localhost:6379.


## License
This project is licensed under the MIT License. See the LICENSE file for details.
