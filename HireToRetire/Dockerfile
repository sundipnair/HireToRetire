FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["HireToRetire.csproj", "HireToRetire/"]
RUN dotnet restore "HireToRetire/HireToRetire.csproj"
WORKDIR "/src/HireToRetire"
COPY . .
RUN dotnet build "HireToRetire.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "HireToRetire.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "HireToRetire.dll"]