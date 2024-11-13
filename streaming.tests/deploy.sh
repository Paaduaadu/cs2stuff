rm main.zip
rm -rf cs2stuff-main
wget https://github.com/Paaduaadu/cs2stuff/archive/refs/heads/main.zip
unzip main.zip
docker build --file /home/timo/appdata/cs2stuff-main/pushmetrics/Dockerfile -t cs2wolves/pushmetrics /home/timo/appdata/cs2stuff-main --no-cache
docker build --file /home/timo/appdata/cs2stuff-main/Dockerfile -t cs2wolves/cs2 /home/timo/appdata/cs2stuff-main --no-cache
docker build --file /home/timo/appdata/cs2stuff-main/publishmetrics/Dockerfile -t cs2wolves/publishmetrics /home/timo/appdata/cs2stuff-main --no-cache
cd cs2stuff-main
docker-compose up -d  pushmetrics
docker-compose up -d  cs2
docker-compose up -d  publishmetrics