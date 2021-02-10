# F1_2020_ResultOutput v0.3
A simple console application to export a match result as an Excel file.
  
![](https://github.com/BattleFeed/F1_2020_ResultOutput/blob/master/Annotation-Excel.jpg)  
![](https://github.com/BattleFeed/F1_2020_ResultOutput/blob/master/Annotation-Console.jpg)

How to use this:
1. DOTNET framework 4.8 is required
2. Run the app(.exe) before/during a game session, and just put it there.
3. Check telemetry setting where IP: 127.0.0.1, port: 20777.(which is default in the game)
4. The excel file is saved at the same folder with the EXE file, when the "RESULTS" panel pops up. (file would be something like "20210204-2107.xlsx")

Note:
1. Every time you switch back to "RESULTS" panel, a UDP packet is sent out. You can do so in order to let the app receive the result for the race(but no qualifying time) if you forgot to open it earlier.
2. Player names are not displayed properly in online multiplayer session since Codemasters failed to provide it...
(Availble in Single player and LAN game though)
3. Does not work with practice session and full qualifying session. (I haven't coded for them yet)

Based on @TimHanewich 's work, thanks a lot :D
https://github.com/TimHanewich/Codemasters.F1_2020
