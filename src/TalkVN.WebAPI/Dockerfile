# Stage 1: Base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore dependencies
COPY ["src/TalkVN.WebAPI/TalkVN.WebAPI.csproj", "TalkVN.WebAPI/"]
COPY ["src/TalkVN.Infrastructure/TalkVN.Infrastructure.csproj", "TalkVN.Infrastructure/"]
COPY ["src/TalkVN.Application/TalkVN.Application.csproj", "TalkVN.Application/"]
COPY ["src/TalkVN.DataAccess/TalkVN.DataAccess.csproj", "TalkVN.DataAccess/"]
COPY ["src/TalkVN.Domain/TalkVN.Domain.csproj", "TalkVN.Domain/"]
RUN dotnet restore "TalkVN.WebAPI/TalkVN.WebAPI.csproj"

# Copy the remaining source code and build
COPY src/ . 
WORKDIR "/src/TalkVN.WebAPI"
RUN dotnet build "TalkVN.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish
FROM build AS publish
RUN dotnet publish "TalkVN.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy SSL certificate if Production
ARG ENVIRONMENT=Development
COPY ["src/TalkVN.WebAPI/chitchat.pfx", "/app/chitchat.pfx"]
RUN if [ "$ENVIRONMENT" = "Production" ]; then cp /app/chitchat.pfx /app/chitchat.pfx; fi

ENTRYPOINT ["dotnet", "TalkVN.WebAPI.dll"]
