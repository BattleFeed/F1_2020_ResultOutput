# F1_2020_ResultOutput
(Work in progress)  
A simple command line program to export a match result as an Excel file.  
![](https://github.com/BattleFeed/F1_2020_ResultOutput/blob/master/Annotation.jpg)  

.NET Version: .NET framework 4.8  
To use this, run the program(or .exe) before/during the game session, and just put it there. It'll automatically export the result to the folder where the EXE file is as soon as the "RACE RESULT" interface pops up after the race. Try switching to "Race Director" and switch back to let the game resend a UDP packet if you forgot to open the program.  
Currently player names are not displayed, since the game itself is not able to send a player's steamID...  

Based on @TimHanewich 's hard work, thanks a lot really :D (I was having a hard time trying to deal with the packets)  
https://github.com/TimHanewich/Codemasters.F1_2020  