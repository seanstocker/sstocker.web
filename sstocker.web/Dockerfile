FROM mcr.microsoft.com/dotnet/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV VIRTUAL_HOST=seanstocker.com
ENV LETSENCRYPT_HOST=seanstocker.com
ENV LETSENCRYPT_EMAIL=seanstocker@gmail.com

FROM mcr.microsoft.com/dotnet/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["sstocker.web/sstocker.web.csproj", "sstocker.web/"]
COPY ["sstocker.budget/sstocker.budget.csproj", "sstocker.budget/"]
COPY ["sstocker.core/sstocker.core.csproj", "sstocker.core/"]
RUN dotnet restore "sstocker.web/sstocker.web.csproj"
COPY . .
WORKDIR "/src/sstocker.web"
RUN dotnet build "sstocker.web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "sstocker.web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "sstocker.web.dll"]