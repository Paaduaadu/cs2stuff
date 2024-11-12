import "join"
import "array"
import "internal/debug"

rangeStart = -168h

withAttackerSteamId = (tables=<-) => tables
    |> filter(fn: (r) => r["Attacker.SteamID"] != "")
    |> filter(fn: (r) => r["Attacker.SteamID"] != "None")

withPlayerSteamId = (tables=<-) => tables
    |> filter(fn: (r) => r["Player.SteamID"] != "")
    |> filter(fn: (r) => r["Player.SteamID"] != "None")

withAssisterSteamId = (tables=<-) => tables
    |> filter(fn: (r) => r["Assister.SteamID"] != "")
    |> filter(fn: (r) => r["Assister.SteamID"] != "None")

tEpd = from(bucket: "CS2")
    |> range(start: rangeStart)
    |> filter(fn: (r) => r["_measurement"] == "EventPlayerDeath")
    |> filter(fn: (r) => r["_field"] == "Count")

tMvp = from(bucket: "CS2")
    |> range(start: rangeStart)
    |> filter(fn: (r) => r["_measurement"] == "EventRoundMvp")
    |> filter(fn: (r) => r["_field"] == "Value")

deaths = union(tables:[
  tEpd
    |> withPlayerSteamId()
    |> group(columns:["Player", "Player.SteamID"])
    |> sum()
    |> group(),
    array.from(rows: [{_value:debug.null(type: "int"), Player:debug.null(type: "string"), "Player.SteamID": debug.null(type: "string")}])])

tKills = tEpd
    |> withAttackerSteamId()
    |> group(columns:["Attacker.SteamID", "Headshot"])

kills = union(tables:[
  tKills
    |> group(columns:["Attacker.SteamID"])
    |> sum()
    |> group(),
    array.from(rows: [{_value:debug.null(type: "int"), "Attacker.SteamID": debug.null(type: "string")}])])

hs = union(tables:[
    tKills
    |> filter(fn: (r) => r["Headshot"] == "True")
    |> group(columns:["Attacker.SteamID"])
    |> sum()
    |> group(),
    array.from(rows: [{_value:debug.null(type: "int"), "Attacker.SteamID": debug.null(type: "string")}])])

assists = union(tables:[
  tEpd
    |> withAssisterSteamId()
    |> group(columns:["Assister.SteamID"])
    |> sum()
    |> group(),
    array.from(rows: [{_value:debug.null(type: "int"), "Assister.SteamID": debug.null(type: "string")}])])

mvps = union(tables:[
  tMvp
    |> withPlayerSteamId()
    |> group(columns:["Player.SteamID"])
    |> count()
    |> group(),
      array.from(rows: [{_value:debug.null(type: "int"), "Player.SteamID":debug.null(type: "string")}])])

damage = union(tables:[
  from(bucket: "CS2")
    |> range(start: rangeStart)
    |> filter(fn: (r) => r["_measurement"] == "EventPlayerHurt")
    |> filter(fn: (r) => r["_field"] == "DmgHealth" or r["_field"] == "DmgArmor")
    |> withAttackerSteamId()
    |> group(columns:["Attacker.SteamID"])
    |> sum()
    |> group(),
    array.from(rows: [{_value:debug.null(type: "int"), "Attacker.SteamID": debug.null(type: "string")}])])

dk = join.left(
    left: deaths,
    right: kills,
    on: (l, r) => r["Attacker.SteamID"] == l["Player.SteamID"],
    as: (l, r) => ({"Player.SteamID": l["Player.SteamID"], Player: l.Player, Deaths:l["_value"], Kills: r["_value"], KDR: (float(v:r["_value"])/float(v:l["_value"])),"_value": (float(v:r["_value"])/float(v:l["_value"]))}),
) 

dk_a = join.left(
    left: dk,
    right: assists,
    on: (l, r) => r["Assister.SteamID"] == l["Player.SteamID"] ,
    as: (l, r) => ({l with Assists: r["_value"]}),
) 

dk_a_hs = join.left(
    left: dk_a,
    right: hs,
    on: (l, r) => r["Attacker.SteamID"] == l["Player.SteamID"] ,
    as: (l, r) => ({l with Headshots: r["_value"]}),
) 

dk_a_hs_mvp = join.left(
    left: dk_a_hs,
    right: mvps,
    on: (l, r) => r["Player.SteamID"] == l["Player.SteamID"],
    as: (l, r) => ({l with Mvp: r["_value"]}),
)

dk_a_hs_mvp_dmg = join.left(
    left: dk_a_hs_mvp,
    right: damage,
    on: (l, r) => r["Attacker.SteamID"] == l["Player.SteamID"],
    as: (l, r) => ({l with Damage: r["_value"]}),
)

dk_a_hs_mvp_dmg |> filter(fn: (r) => r["Player.SteamID"] != "")