FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["PortfolioCMS.csproj", "./"]

RUN dotnet restore "PortfolioCMS.csproj"

COPY . .

RUN dotnet publish "PortfolioCMS.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

ENTRYPOINT ["sh", "-c", "dotnet PortfolioCMS.dll --urls http://0.0.0.0:${PORT:-10000}"]