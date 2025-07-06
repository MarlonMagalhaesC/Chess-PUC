# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copia tudo para a imagem
COPY . ./

# Publica a aplicação
RUN dotnet publish -c Release -o out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

# Copia os arquivos publicados da etapa de build
COPY --from=build /app/out .

# Define a porta que o Render irá usar
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

# Inicia o app
ENTRYPOINT ["dotnet", "ChessAPI.dll"]
