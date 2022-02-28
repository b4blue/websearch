FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["BatchDataUploader/BatchDataUploader.csproj", "BatchDataUploader/"]
RUN dotnet restore "BatchDataUploader/BatchDataUploader.csproj"
COPY . .
WORKDIR "/src/BatchDataUploader"
RUN dotnet build "BatchDataUploader.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BatchDataUploader.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BatchDataUploader.dll"]
