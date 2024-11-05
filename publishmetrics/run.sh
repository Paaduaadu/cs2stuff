#!/bin/bash

mkdir -p /App/Results
# Run your dotnet console app
dotnet /App/publishmetrics.dll 

cd ~
git clone https://github.com/Paaduaadu/cs2stuff.git ~/repo
mkdir -p ~/repo/Results
cp -rf /App/Results/* ~/repo/Results

cd ~/repo
git add Results/*.json
git commit -m "Stats update"
git push