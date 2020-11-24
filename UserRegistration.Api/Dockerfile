#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["UserRegistration.Api/UserRegistration.Api.csproj", "UserRegistration.Api/"]
COPY ["Core.WebSocket/Core.WebSocket.csproj", "Core.WebSocket/"]
COPY ["UserRegistration.Bootstrapper/UserRegistration.Bootstrapper.csproj", "UserRegistration.Bootstrapper/"]
COPY ["UserRegistration.Data.Sql/UserRegistration.Data.Sql.csproj", "UserRegistration.Data.Sql/"]
COPY ["UserRegistration.Entities/UserRegistration.Entities.csproj", "UserRegistration.Entities/"]
COPY ["Core.Data.Sql/Core.Data.Sql.csproj", "Core.Data.Sql/"]
COPY ["Core.Data.MongoDb/Core.Data.MongoDb.csproj", "Core.Data.MongoDb/"]
COPY ["UserRegistration.Data.Contracts/UserRegistration.Data.Contracts.csproj", "UserRegistration.Data.Contracts/"]
COPY ["UserRegistration.Service.Contracts/UserRegistration.Service.Contracts.csproj", "UserRegistration.Service.Contracts/"]
COPY ["UserRegistration.Models/UserRegistration.Models.csproj", "UserRegistration.Models/"]
COPY ["Core.Common/Core.Common.csproj", "Core.Common/"]
COPY ["Core.Common.Contracts/Core.Common.Contracts.csproj", "Core.Common.Contracts/"]
COPY ["UserRegistration.Service/UserRegistration.Service.csproj", "UserRegistration.Service/"]
RUN dotnet restore "UserRegistration.Api/UserRegistration.Api.csproj"
COPY . .
WORKDIR "/src/UserRegistration.Api"
RUN dotnet build "UserRegistration.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserRegistration.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserRegistration.Api.dll"]