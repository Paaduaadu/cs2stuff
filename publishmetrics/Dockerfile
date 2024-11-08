FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Build and publish a release
RUN dotnet publish ./publishmetrics/publishmetrics.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:6c4df091e4e531bb93bdbfe7e7f0998e7ced344f54426b7e874116a3dc3233ff
WORKDIR /App
COPY --from=build-env /App/out .

RUN apt-get update -qq && apt-get -y install cron -qq --force-yes
RUN apt-get -y install git -qq --force-yes
RUN git config --global credential.helper store

# Add export environment variable script and schedule
COPY publishmetrics/*.sh ./
COPY publishmetrics/schedule /etc/cron.d/schedule
RUN sed -i 's/\r//' export_env.sh \
    && sed -i 's/\r//' run.sh \
    && sed -i 's/\r//' /etc/cron.d/schedule \
    && chmod +x export_env.sh run.sh \
    && chmod 0644 /etc/cron.d/schedule

# Create log file
RUN touch /var/log/cron.log
RUN chmod 0666 /var/log/cron.log

# Volume required for tail command
VOLUME /var/log

# Run Cron
CMD /App/export_env.sh && /usr/sbin/cron && tail -f /var/log/cron.log