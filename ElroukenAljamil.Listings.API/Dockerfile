FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ElroukenAljamil.Common/ElroukenAljamil.Common.csproj ElroukenAljamil.Common/
COPY ElroukenAljamil.EventBus/ElroukenAljamil.EventBus.csproj ElroukenAljamil.EventBus/
COPY ElroukenAljamil.Security/ElroukenAljamil.Security.csproj ElroukenAljamil.Security/
COPY ElroukenAljamil.Listings.Domain/ElroukenAljamil.Listings.Domain.csproj ElroukenAljamil.Listings.Domain/
COPY ElroukenAljamil.Listings.Application/ElroukenAljamil.Listings.Application.csproj ElroukenAljamil.Listings.Application/
COPY ElroukenAljamil.Listings.Infrastructure/ElroukenAljamil.Listings.Infrastructure.csproj ElroukenAljamil.Listings.Infrastructure/
COPY ElroukenAljamil.Listings.API/ElroukenAljamil.Listings.API.csproj ElroukenAljamil.Listings.API/

RUN dotnet restore "ElroukenAljamil.Listings.API/ElroukenAljamil.Listings.API.csproj"

COPY . .
RUN dotnet publish "ElroukenAljamil.Listings.API/ElroukenAljamil.Listings.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "ElroukenAljamil.Listings.API.dll"]
