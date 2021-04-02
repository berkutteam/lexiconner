FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

# Heroku fails on port 80.
# The web process must listen for HTTP traffic on $PORT, which is set by Heroku. 
# EXPOSE in Dockerfile is not respected by Heroku, but can be used for local testing. Only HTTP requests are supported.

#EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Lexiconner.Api/Lexiconner.Api.csproj", "Lexiconner.Api/"]
COPY ["Lexiconner.Persistence/Lexiconner.Persistence.csproj", "Lexiconner.Persistence/"]
COPY ["Lexiconner.Application/Lexiconner.Application.csproj", "Lexiconner.Application/"]
COPY ["Lexiconner.Domain/Lexiconner.Domain.csproj", "Lexiconner.Domain/"]
RUN dotnet restore "Lexiconner.Api/Lexiconner.Api.csproj"
#COPY "Lexiconner/Lexiconner.Api/"  .
#COPY "Lexiconner/"  .
COPY "./"  .
WORKDIR "/src/Lexiconner.Api"
RUN dotnet build "Lexiconner.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Lexiconner.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "Lexiconner.Api.dll"]
#Because Heroku doesn’t work nicely with ENTRYPOINT command we should change this
CMD dotnet Lexiconner.Api.dll