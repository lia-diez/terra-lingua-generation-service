# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and projects
COPY InCell.sln ./
COPY Api/Api.csproj ./Api/
COPY Core/Core.csproj ./Core/

# Restore
RUN dotnet restore

# Copy the rest of the source code
COPY . ./

# Publish the API project
WORKDIR /src/Api
RUN dotnet publish -c Release -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app


# Copy build output
COPY --from=build /app/publish .

# Expose HTTP port
EXPOSE 80

ENTRYPOINT ["dotnet", "Api.dll"]
