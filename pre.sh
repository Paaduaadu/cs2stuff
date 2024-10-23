cssPluginHome="/home/steam/exportevents"

file="game/csgo/addons/counterstrikesharp/plugins/exportevents"
if [ ! -e "$file" ]; then
    wget https://mms.alliedmods.net/mmsdrop/2.0/mmsource-2.0.0-git1314-linux.tar.gz

    tar -xvzf mmsource-2.0.0-git1314-linux.tar.gz
    mv addons game/csgo
    rm -rf mmsource-2.0.0-git1314-linux.tar.gz

    perl -i -pe 's/Game\s+csgo\r?\n/Game        csgo\/addons\/metamod
                Game    csgo
    /g' game/csgo/gameinfo.gi

    wget https://github.com/roflmuffin/CounterStrikeSharp/releases/download/v276/counterstrikesharp-with-runtime-build-276-linux-42dd270.zip

    unzip counterstrikesharp-with-runtime-build-276-linux-42dd270.zip
    cp -R addons/* game/csgo/addons
    rm -rf addons
    rm -rf counterstrikesharp-with-runtime-build-276-linux-42dd270.zip

    mkdir game/csgo/addons/counterstrikesharp/plugins/exportevents
    cp -R $cssPluginHome/* $file
else
    echo "pre.sh has been executed already once."
fi