FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
#EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Lexiconner.IdentityServer4/Lexiconner.IdentityServer4.csproj", "Lexiconner.IdentityServer4/"]
COPY ["Lexiconner.Persistence/Lexiconner.Persistence.csproj", "Lexiconner.Persistence/"]
COPY ["Lexiconner.Domain/Lexiconner.Domain.csproj", "Lexiconner.Domain/"]
RUN dotnet restore "Lexiconner.IdentityServer4/Lexiconner.IdentityServer4.csproj"
#COPY . .
COPY "Lexiconner/"  .
WORKDIR "/src/Lexiconner.IdentityServer4"
RUN dotnet build "Lexiconner.IdentityServer4.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Lexiconner.IdentityServer4.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "Lexiconner.IdentityServer4.dll"]
#Because Heroku doesn’t work nicely with ENTRYPOINT command we should change this
CMD dotnet Lexiconner.IdentityServer4.dll