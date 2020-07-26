FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /coderoom

COPY CodeRoom.sln .
COPY Src/Domain/Domain.csproj Src/Domain/Domain.csproj
COPY Src/Application/Application.csproj Src/Application/Application.csproj
COPY Src/Infrastructure/Infrastructure.csproj Src/Infrastructure/Infrastructure.csproj
COPY Src/WebUI/WebUI.csproj Src/WebUI/WebUI.csproj

COPY Tests/Application.Test/Application.Test.csproj Tests/Application.Test/Application.Test.csproj
COPY Tests/Infrastructure.Tests/Infrastructure.Tests.csproj Tests/Infrastructure.Tests/Infrastructure.Tests.csproj
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
RUN apt-get -q update && apt-get -qy install netcat
WORKDIR /app
COPY --from=build /app .
COPY wait-for .
RUN chmod +x wait-for


