FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ElroukenAljamil.Common/ElroukenAljamil.Common.csproj ElroukenAljamil.Common/
COPY ElroukenAljamil.EventBus/ElroukenAljamil.EventBus.csproj ElroukenAljamil.EventBus/
COPY ElroukenAljamil.Media.Domain/ElroukenAljamil.Media.Domain.csproj ElroukenAljamil.Media.Domain/
COPY ElroukenAljamil.Media.Application/ElroukenAljamil.Media.Application.csproj ElroukenAljamil.Media.Application/
COPY ElroukenAljamil.Media.Infrastructure/ElroukenAljamil.Media.Infrastructure.csproj ElroukenAljamil.Media.Infrastructure/
COPY ElroukenAljamil.Media.API/ElroukenAljamil.Media.API.csproj ElroukenAljamil.Media.API/

RUN dotnet restore "ElroukenAljamil.Media.API/ElroukenAljamil.Media.API.csproj"

COPY . .
RUN dotnet publish "ElroukenAljamil.Media.API/ElroukenAljamil.Media.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "ElroukenAljamil.Media.API.dll"]
