FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /coderoom

COPY CodeRoom.sln .
COPY Src/Domain/Domain.csproj Src/Domain/Domain.csproj
COPY Src/Application/Application.csproj Src/Application/Application.csproj
COPY Src/Infrastructure/Infrastructure.csproj Src/Infrastructure/Infrastructure.csproj
COPY Src/WebUI/WebUI.csproj Src/WebUI/WebUI.csproj

COPY Tests/Application.Test/Application.Test.csproj Tests/Application.Test/Application.Test.csproj
COPY Tests/Infrastructure.Tests/Infrastructure.Tests.csproj Tests/Infrastructure.Tests/Infrastructure.Tests.csproj
COPY Tests/WebUI.Integration.Tests/WebUI.Integration.Tests.csproj Tests/WebUI.Integration.Tests/WebUI.Integration.Tests.csproj
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS runtime
RUN mkdir -p /usr/share/man/man1 /usr/share/man/man2
RUN apt-get update && apt-get -y install --no-install-recommends openjdk-11-jdk-headless && apt-get clean && rm -r /var/lib/apt/lists/*
RUN apt-get -q update && apt-get -qy install netcat
WORKDIR /app
COPY --from=build /app .
COPY wait-for .
RUN chmod +x wait-for


