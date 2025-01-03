#!/bin/bash

# Set environment variables copied from container
source /App/set_env.sh;

mkdir -p /App/Results
# Run your dotnet console app
dotnet /App/publishmetrics.dll 

cp $GIT_CREDENTIALS_FILE ~/.git-credentials
cd ~
git clone $PUBLISHMETRICS_REPO ~/repo
cd ~/repo
git pull

mkdir -p ~/repo/public/results
cp -rf /App/Results/* ~/repo/public/results
git add public/results/*.json
git commit -m "Stats update"
git push