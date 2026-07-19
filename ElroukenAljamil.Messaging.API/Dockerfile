FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ElroukenAljamil.Common/ElroukenAljamil.Common.csproj ElroukenAljamil.Common/
COPY ElroukenAljamil.EventBus/ElroukenAljamil.EventBus.csproj ElroukenAljamil.EventBus/
COPY ElroukenAljamil.Security/ElroukenAljamil.Security.csproj ElroukenAljamil.Security/
COPY ElroukenAljamil.Messaging.Domain/ElroukenAljamil.Messaging.Domain.csproj ElroukenAljamil.Messaging.Domain/
COPY ElroukenAljamil.Messaging.Application/ElroukenAljamil.Messaging.Application.csproj ElroukenAljamil.Messaging.Application/
COPY ElroukenAljamil.Messaging.Infrastructure/ElroukenAljamil.Messaging.Infrastructure.csproj ElroukenAljamil.Messaging.Infrastructure/
COPY ElroukenAljamil.Messaging.API/ElroukenAljamil.Messaging.API.csproj ElroukenAljamil.Messaging.API/

RUN dotnet restore "ElroukenAljamil.Messaging.API/ElroukenAljamil.Messaging.API.csproj"

COPY . .
RUN dotnet publish "ElroukenAljamil.Messaging.API/ElroukenAljamil.Messaging.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "ElroukenAljamil.Messaging.API.dll"]
