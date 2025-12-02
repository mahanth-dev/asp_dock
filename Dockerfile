# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY ["wepapp/wepapp.csproj", "wepapp/"]

RUN dotnet restore "wepapp/wepapp.csproj"


COPY . .
WORKDIR "/source/wepapp"


RUN dotnet build "wepapp.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "wepapp.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
# -------------------------------------

ENTRYPOINT ["dotnet", "wepapp.dll"]
