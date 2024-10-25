FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish  ./exportevents/exportevents.csproj -c Release -o out
RUN ls

# Build runtime image
FROM joedwards32/cs2
COPY --from=build-env /App/out ./exportevents
COPY --from=build-env /App/pre.sh /etc/pre.sh
COPY --from=build-env /App/post.sh /etc/post.sh