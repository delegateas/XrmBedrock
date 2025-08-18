# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["XrmBedrock.sln", "./"]
COPY ["test/XrmMockupApi/XrmMockupApi.csproj", "test/XrmMockupApi/"]
COPY ["test/SharedTest/SharedTest.projitems", "test/SharedTest/"]
COPY ["test/SharedTest/SharedTest.shproj", "test/SharedTest/"]
COPY ["src/Azure/DataverseService/DataverseService.csproj", "src/Azure/DataverseService/"]
COPY ["src/Dataverse/PluginsNetCore/PluginsNetCore.csproj", "src/Dataverse/PluginsNetCore/"]
COPY ["src/Dataverse/SharedPluginLogic/SharedPluginLogic.projitems", "src/Dataverse/SharedPluginLogic/"]
COPY ["src/Dataverse/SharedPluginLogic/SharedPluginLogic.shproj", "src/Dataverse/SharedPluginLogic/"]
COPY ["src/Shared/SharedContext/SharedContext.projitems", "src/Shared/SharedContext/"]
COPY ["src/Shared/SharedContext/SharedContext.shproj", "src/Shared/SharedContext/"]
COPY ["src/Shared/SharedDataverseLogic/SharedDataverseLogic.projitems", "src/Shared/SharedDataverseLogic/"]
COPY ["src/Shared/SharedDataverseLogic/SharedDataverseLogic.shproj", "src/Shared/SharedDataverseLogic/"]
COPY ["src/Shared/SharedDomain/SharedDomain.projitems", "src/Shared/SharedDomain/"]
COPY ["src/Shared/SharedDomain/SharedDomain.shproj", "src/Shared/SharedDomain/"]
COPY ["src/Shared/NetCoreContext/NetCoreContext.csproj", "src/Shared/NetCoreContext/"]

# Restore dependencies
RUN dotnet restore "test/XrmMockupApi/XrmMockupApi.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/test/XrmMockupApi"
RUN dotnet build "XrmMockupApi.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "XrmMockupApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser

# Copy the published application
COPY --from=build /app/publish .
COPY --from=build /src/test/SharedTest/MetadataGenerated ./SharedTest/MetadataGenerated

# Change ownership to non-root user
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "XrmMockupApi.dll"]