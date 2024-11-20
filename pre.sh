
cssZip = "counterstrikesharp-with-runtime-build-287-linux-6cf124b.zip"
cssDownloadPath = "v287/$cssZip"
cssDownloadUrl = "https://github.com/roflmuffin/CounterStrikeSharp/releases/download/$cssDownloadPath"

cssPluginHome="/home/steam/exportevents"
csgoDir="/home/steam/cs2-dedicated/game/csgo"
addonsDir="$csgoDir/addons"
file="$addonsDir/counterstrikesharp/plugins/exporteventsplugin"
if [ ! -e "$file" ]; then
    mkdir -p $addonsDir
    
    wget https://mms.alliedmods.net/mmsdrop/2.0/mmsource-2.0.0-git1314-linux.tar.gz
    tar -xvzf mmsource-2.0.0-git1314-linux.tar.gz
    
    mv addons $csgoDir
    rm -rf mmsource-2.0.0-git1314-linux.tar.gz

    perl -i -pe 's/Game\s+csgo\r?\n/Game        csgo\/addons\/metamod
                Game    csgo
    /g' game/csgo/gameinfo.gi

    
    wget cssDownloadUrl
    unzip cssZip
    cp -R addons/* $addonsDir
    rm -rf addons
    rm -rf cssZip
else
    echo "pre.sh has been executed already once."
fi

mkdir $file
cp -R $cssPluginHome/* $file/
