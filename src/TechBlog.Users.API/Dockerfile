FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ./src . 

WORKDIR "/src/TechBlog.Users.API"
RUN dotnet restore "TechBlog.Users.API.csproj"
RUN dotnet build "TechBlog.Users.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TechBlog.Users.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "TechBlog.Users.API.dll"]