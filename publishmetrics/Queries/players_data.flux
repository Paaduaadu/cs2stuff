rangeStart = -3y

from(bucket: "CS2")
  |> range(start: rangeStart)
  |> filter(fn: (r) => r["_measurement"] == "EventPlayerDeath")
  |> filter(fn: (r) => r["Attacker.SteamID"] != "")
  |> filter(fn: (r) => r["Attacker.SteamID"] != "None")
  |> group(columns: ["Attacker", "Attacker.SteamID"])
  |> distinct(column: "Attacker.SteamID")
  |> map(fn: (r) => ({SteamId: r["Attacker.SteamID"], Name: r["Attacker"]}))