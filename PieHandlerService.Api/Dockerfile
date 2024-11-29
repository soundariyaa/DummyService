FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

ADD ./cert/oefactory_pvo_qa.json /app/certificates/oefactory_pvo_qa.json
ADD ./cert/oefactory_pvo_test.json /app/certificates/oefactory_pvo_test.json
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG NUGET_PAT
WORKDIR /src

COPY ["nuget.config", "."]
COPY ["src/PieHandlerService.Api/PieHandlerService.Api.csproj", "src/PieHandlerService.Api/"]
COPY ["src/PieHandlerService.Application/PieHandlerService.Application.csproj", "src/PieHandlerService.Application/"]
COPY ["src/PieHandlerService.Core/PieHandlerService.Core.csproj", "src/PieHandlerService.Core/"]
COPY ["src/PieHandlerService.Infrastructure/PieHandlerService.Infrastructure.csproj", "src/PieHandlerService.Infrastructure/"]
COPY ["test/PieHandlerService.Application.Test/PieHandlerService.Application.Test.csproj", "test/PieHandlerService.Application.Test/"]
COPY ["test/PieHandlerService.Infrastructure.Test/PieHandlerService.Infrastructure.Test.csproj", "test/PieHandlerService.Infrastructure.Test/"]

RUN dotnet restore "./src/PieHandlerService.Api/./PieHandlerService.Api.csproj"
COPY . .
WORKDIR "/src/src/PieHandlerService.Api"
RUN dotnet build "./PieHandlerService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS testrunner
WORKDIR /src
RUN dotnet test --no-restore -c $BUILD_CONFIGURATION ./test/PieHandlerService.Application.Test/PieHandlerService.Application.Test.csproj
RUN dotnet test --no-restore -c $BUILD_CONFIGURATION ./test/PieHandlerService.Infrastructure.Test/PieHandlerService.Infrastructure.Test.csproj

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PieHandlerService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY  --chown=nonroot:nonroot --from=publish /app/publish .
RUN mkdir /app/logs
RUN chmod -R 777 .
#RUN mkdir /app/logs
#RUN chown -R 0 /app/logs && chmod -R g=u /app/logs
#RUN mkdir /.dotnet
#RUN chgrp -R 0 /.dotnet && chmod -R g=u /.dotnet 
ENTRYPOINT ["dotnet", "PieHandlerService.Api.dll"]