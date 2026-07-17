FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ElroukenAljamil.Common/ElroukenAljamil.Common.csproj ElroukenAljamil.Common/
COPY ElroukenAljamil.Security/ElroukenAljamil.Security.csproj ElroukenAljamil.Security/
COPY ElroukenAljamil.Identity.Domain/ElroukenAljamil.Identity.Domain.csproj ElroukenAljamil.Identity.Domain/
COPY ElroukenAljamil.Identity.Application/ElroukenAljamil.Identity.Application.csproj ElroukenAljamil.Identity.Application/
COPY ElroukenAljamil.Identity.Infrastructure/ElroukenAljamil.Identity.Infrastructure.csproj ElroukenAljamil.Identity.Infrastructure/
COPY ElroukenAljamil.Identity.API/ElroukenAljamil.Identity.API.csproj ElroukenAljamil.Identity.API/

RUN dotnet restore "ElroukenAljamil.Identity.API/ElroukenAljamil.Identity.API.csproj"

COPY . .
RUN dotnet publish "ElroukenAljamil.Identity.API/ElroukenAljamil.Identity.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "ElroukenAljamil.Identity.API.dll"]
