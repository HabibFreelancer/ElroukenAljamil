FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ElroukenAljamil.Common/ElroukenAljamil.Common.csproj ElroukenAljamil.Common/
COPY ElroukenAljamil.EventBus/ElroukenAljamil.EventBus.csproj ElroukenAljamil.EventBus/
COPY ElroukenAljamil.Security/ElroukenAljamil.Security.csproj ElroukenAljamil.Security/
COPY ElroukenAljamil.Search.Domain/ElroukenAljamil.Search.Domain.csproj ElroukenAljamil.Search.Domain/
COPY ElroukenAljamil.Search.Application/ElroukenAljamil.Search.Application.csproj ElroukenAljamil.Search.Application/
COPY ElroukenAljamil.Search.Infrastructure/ElroukenAljamil.Search.Infrastructure.csproj ElroukenAljamil.Search.Infrastructure/
COPY ElroukenAljamil.Search.API/ElroukenAljamil.Search.API.csproj ElroukenAljamil.Search.API/

RUN dotnet restore "ElroukenAljamil.Search.API/ElroukenAljamil.Search.API.csproj"

COPY . .
RUN dotnet publish "ElroukenAljamil.Search.API/ElroukenAljamil.Search.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "ElroukenAljamil.Search.API.dll"]
