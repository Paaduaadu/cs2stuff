Ega midagi erilist pole, kõik tehtud selle baasil - https://github.com/joedwards32/CS2/

#Architecture
A data pipeline of the following shape:

CS2 
    -> Exportevents[CountersStrikeSharp Plugin] 
    -> Buffer[Redis Stream]
    -> Push metrics[Redis Stream Reader] 
    -> Time series database[Influxdb2]
    -> Dashboard[Grafana]

#Secrets
This is a quick and dirty solution for the statistics problem.
It is by no means meant to be very secure in this phase.
Hardening must follow later.

Give values for the following secrets:

Docker compose references the following secrets that must be filled on the host machine.

Required by influx in initial setup. Just put something here "admin":
influxdb2-admin-username:
file: ~/.env.influxdb2-admin-username

Required by influx in initial setup. Just put something of reasonable complexity here:
For example "admin-admin-124"
'influxdb2-admin-password:
file: ~/.env.influxdb2-admin-password

A shortcut pre generated access token for influxdb.
Fill with a base64 string of your choosing.
For example "HOE7FFYwTNTAE2zl3ddOqw2XomadVRPNAtp8vx--Bm9MdgzLUkfeqDfDkliYdcUlhHRyb-w-qXnRFqgaQi957A=="

influxdb2-admin-token:
file: ~/.env.influxdb2-admin-token

Used by pushmetrics container to write to influxdb.
Same file as previous, for convenience.
influxdb2-pushmetrics-token:
file: ~/.env.influxdb2-admin-token

#Grafana setup
For an unattended setup, before compose up, change the:
grafana/datasources/datasource.yml
Fill the field SecureJsonData.token with the contents of influxdb2-admin-token

This can be ofc done also later in UI.