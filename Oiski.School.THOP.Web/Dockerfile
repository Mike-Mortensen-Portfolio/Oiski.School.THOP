#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Oiski.School.THOP.Web/Oiski.School.THOP.Web.csproj", "Oiski.School.THOP.Web/"]
COPY ["Oiski.School.THOP.Services/Oiski.School.THOP.Services.csproj", "Oiski.School.THOP.Services/"]
RUN dotnet restore "Oiski.School.THOP.Web/Oiski.School.THOP.Web.csproj"
COPY . .
WORKDIR "/src/Oiski.School.THOP.Web"
RUN dotnet build "Oiski.School.THOP.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Oiski.School.THOP.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Oiski.School.THOP.Web.dll"]