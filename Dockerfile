#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src

COPY ["Source/Tcc.Sigo.Normas.Acl/Tcc.Sigo.Normas.Acl.csproj", "Tcc.Sigo.Normas.Acl/"]

COPY . ./
RUN dotnet restore "Source/Tcc.Sigo.Normas.Acl/Tcc.Sigo.Normas.Acl.csproj"
RUN dotnet publish "Source/Tcc.Sigo.Normas.Acl" -c Release -o /app/publish 

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Tcc.Sigo.Normas.Acl.dll"]
