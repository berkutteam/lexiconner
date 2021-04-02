#!!!this dockerfile is changed manually
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

# Heroku fails on port 80.
# The web process must listen for HTTP traffic on $PORT, which is set by Heroku. 
# EXPOSE in Dockerfile is not respected by Heroku, but can be used for local testing. Only HTTP requests are supported.

#EXPOSE 8080
#ENV ASPNETCORE_URLS=http://*:8080
#ENV ASPNETCORE_URLS=http://*:$PORT

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Lexiconner.Web/Lexiconner.Web.csproj", "Lexiconner.Web/"]
COPY ["Lexiconner.Application/Lexiconner.Application.csproj", "Lexiconner.Application/"]
RUN dotnet restore "Lexiconner.Web/Lexiconner.Web.csproj"
#WORKDIR "/src/Lexiconner.Web"
#COPY "Lexiconner/Lexiconner.Web/" .
#COPY "Lexiconner/"  .
COPY "./"  .
WORKDIR "/src/Lexiconner.Web"
RUN dotnet build "Lexiconner.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Lexiconner.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "Lexiconner.Web.dll"]
#Because Heroku doesn’t work nicely with ENTRYPOINT command we should change this
CMD dotnet Lexiconner.Web.dll