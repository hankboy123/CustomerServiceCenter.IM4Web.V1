FROM microsoft/aspnetcore:latest
COPY ./ IM4Web
WORKDIR /IM4Web
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet","CustomerServiceCenter.IM4Web.dll"]