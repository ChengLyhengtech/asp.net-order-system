FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["aps.net order-system.csproj", "./"]
RUN dotnet restore "aps.net order-system.csproj"

COPY . .
RUN dotnet publish "aps.net order-system.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/uploads

EXPOSE 8080

ENTRYPOINT ["dotnet", "aps.net order-system.dll"]
