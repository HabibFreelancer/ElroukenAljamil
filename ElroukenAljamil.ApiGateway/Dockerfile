# Stage 1 : Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ElroukenAljamil.ApiGateway.csproj", "./"]
RUN dotnet restore "ElroukenAljamil.ApiGateway.csproj"

COPY . .
RUN dotnet publish "ElroukenAljamil.ApiGateway.csproj" -c Release -o /app/publish --no-restore

# Stage 2 : Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Securite : utilisateur non-root
RUN adduser --disabled-password --gecos "" appuser
USER appuser

COPY --from=build /app/publish .

# Copier les fichiers de configuration Ocelot
COPY --from=build /src/ocelot.json .
COPY --from=build /src/ocelot.Production.json .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ElroukenAljamil.ApiGateway.dll"]
