FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ElroukenAljamil.Common/ElroukenAljamil.Common.csproj ElroukenAljamil.Common/
COPY ElroukenAljamil.EventBus/ElroukenAljamil.EventBus.csproj ElroukenAljamil.EventBus/
COPY ElroukenAljamil.Notification.Domain/ElroukenAljamil.Notification.Domain.csproj ElroukenAljamil.Notification.Domain/
COPY ElroukenAljamil.Notification.Application/ElroukenAljamil.Notification.Application.csproj ElroukenAljamil.Notification.Application/
COPY ElroukenAljamil.Notification.Infrastructure/ElroukenAljamil.Notification.Infrastructure.csproj ElroukenAljamil.Notification.Infrastructure/
COPY ElroukenAljamil.Notification.API/ElroukenAljamil.Notification.API.csproj ElroukenAljamil.Notification.API/

RUN dotnet restore "ElroukenAljamil.Notification.API/ElroukenAljamil.Notification.API.csproj"

COPY . .
RUN dotnet publish "ElroukenAljamil.Notification.API/ElroukenAljamil.Notification.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "ElroukenAljamil.Notification.API.dll"]
