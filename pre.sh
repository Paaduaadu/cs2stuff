
cssZip="counterstrikesharp-with-runtime-build-305-linux-e99d27c.zip"
cssDownloadPath="v305/$cssZip"
cssDownloadUrl="https://github.com/roflmuffin/CounterStrikeSharp/releases/download/$cssDownloadPath"
metamodPackage="mmsource-2.0.0-git1319-linux.tar.gz"

cssPluginHome="/home/steam/exportevents"
csgoDir="/home/steam/cs2-dedicated/game/csgo"
addonsDir="$csgoDir/addons"
file="$addonsDir/counterstrikesharp/plugins/exporteventsplugin"


    mkdir -p $addonsDir
    
    wget https://mms.alliedmods.net/mmsdrop/2.0/$metamodPackage
    tar -xvzf $metamodPackage
    
    mv addons $csgoDir
    rm -rf $metamodPackage

    perl -i -pe 's/Game\s+csgo\r?\n/Game        csgo\/addons\/metamod
                Game    csgo
    /g' game/csgo/gameinfo.gi

    
    wget $cssDownloadUrl
    unzip $cssZip
    cp -R addons/* $addonsDir
    rm -rf addons
    rm -rf $cssZip

mkdir $file
cp -R $cssPluginHome/* $file/
