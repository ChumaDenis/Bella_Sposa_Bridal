FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
WORKDIR /src

COPY . .

RUN dotnet publish Bella_Sposa_Bridal_api/Bella_Sposa_Bridal_api.csproj \
    -c Release -o /app/publish --no-restore || \
    dotnet restore && \
    dotnet publish Bella_Sposa_Bridal_api/Bella_Sposa_Bridal_api.csproj \
    -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Bella_Sposa_Bridal_api.dll"]
