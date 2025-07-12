#Stage 1 (building SDK)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /App
COPY minimal-ip-app.csproj ./
RUN dotnet restore "minimal-ip-app.csproj"
COPY . ./
RUN dotnet publish "minimal-ip-app.csproj" -c Release -o out

#Stage 2 - runtime (lightweight image for running)
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /App
COPY --from=build /App/out ./
ENV ASPNETCORE_URLS=http://+:8000
EXPOSE 8000
ENTRYPOINT ["dotnet", "minimal-ip-app.dll"]
