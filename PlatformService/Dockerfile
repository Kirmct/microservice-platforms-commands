FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
EXPOSE 8080

# Copia o projeto e restaura as dependências
COPY *.csproj ./
RUN dotnet restore

# Copia o restante dos arquivos e publica a aplicação
COPY . ./
RUN dotnet publish -c Release -o out

# Imagem final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Define o comando de execução
ENTRYPOINT [ "dotnet", "PlatformService.dll" ]