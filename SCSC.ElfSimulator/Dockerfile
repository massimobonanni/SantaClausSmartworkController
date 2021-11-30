#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SCSC.ElfSimulator/SCSC.ElfSimulator.csproj", "SCSC.ElfSimulator/"]
COPY ["SCSC.APIClient/SCSC.APIClient.csproj", "SCSC.APIClient/"]
COPY ["SCSC.Core/SCSC.Core.csproj", "SCSC.Core/"]
RUN dotnet restore "SCSC.ElfSimulator/SCSC.ElfSimulator.csproj"
COPY . .
WORKDIR "/src/SCSC.ElfSimulator"
RUN dotnet build "SCSC.ElfSimulator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SCSC.ElfSimulator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SCSC.ElfSimulator.dll"]