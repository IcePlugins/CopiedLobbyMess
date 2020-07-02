# CopiedLobbyMess
CopiedLobbyMess is a simple Rocket plugin that messes with the lobby information the server gives you. It is a copy of AviLobbyMess so for more information you can read up on that.

https://iceplugins.xyz/CopiedLobbyMess

# Features

 - Invisible Rocket - The server is still rocket however users that filter with Vanilla still see the server
 - Hide Plugins - This hides the plugin list of the server
 - Mess Plugins - Changes the plugin list to whatever you specify in Plugins(new line requires to be in a new `<string></string>`)
 - Hide Workshop - This simply hides the workshop and makes it seem as if the server doesn't have workshop items(by default Unturned filters out servers with workshop items so this makes you server more visible to new users)
 - Mess Workshop - Allows you to add fake workshop items to the workshop list(every item needs to be an ID)
 - Mess Gamemode - Change the game mode of the server to any text specified in Gamemode
 - Hide Config - Hides the configuration tab that shows if you server is set to anything besides Normal difficulty
 - Mess Config - Allows you to fake the config of the server
 - Is PVP(mess config) - Fakes if the server is PVP or PVE
 - Has Cheats(mess config) - Fakes if the server has cheats enabled
 - Difficulty(mess config) - Fakes the server difficulty
 - Camera mode(mess config) - Fakes the allowed camera mode on the server
 - Gold Only(mess config) - Makes the server appear to be gold only
 - Has Battleye(mess config) - Fakes if the server has battleye or not
 - Is Vanilla - Makes the server look like it is vanilla by removing the plugin list and faking the filter check

# Difficulty

Must be one of the following, case insensitive:

- "NRM" "Normal"
- "HRD" "Hard"
- "EZY" "Easy"

If you enter something else, it will default to NRM (Normal)


# Camera Mode

Must be one of the following, case insensitive:

- "First" "1Pp"
- "Both" "2Pp"
- "Third" "3Pp"
- "Vehicle" "4Pp"

If you enter something else, it will default to 2Pp (Both)

# Installation

 1. Download the zip file from Releases
 2. Extract the files inside the zip to your Plugins folder of your server
 3. Done

Created by AtiLion
