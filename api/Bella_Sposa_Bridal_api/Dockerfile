FROM mcr.microsoft.com/dotnet/nightly/sdk:11.0-preview AS build
WORKDIR /src

COPY . .

RUN dotnet restore Bella_Sposa_Bridal_api/Bella_Sposa_Bridal_api.csproj

RUN dotnet publish Bella_Sposa_Bridal_api/Bella_Sposa_Bridal_api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/nightly/aspnet:11.0-preview AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Bella_Sposa_Bridal_api.dll"]
