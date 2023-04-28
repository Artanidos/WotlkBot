# WotlkBot
![](https://raw.githubusercontent.com/Artanidos/WotlkBot/master/Gui/screen.png)  
The WotlkBot shall be a possibilty to turn your WoW (World of Warcraft) charcater to become a bot and to join your party or raid and fight together with you.  
The user can create a Python script to control the bots behaviour.

This software shall only be used for your own private server like AzerothCore to be able to go into dungeons and raids alone.
It can also be used on other private servers, where botting is allowed. Maybe Solocraft could allow to use it, because they have also party-bots implemented.

## Motivation
I am playing Wow now since the first monsths. Since Blizzard has sold Wow to Activision, the retail version does not make much fun anymore, but the old version 3.3.5 Wrath of the Lich King was a very good extension.  
- 10 man raids where posible.  
- The dungeon finder has helped a lot to make many heroic dungeons every day.  
- The talent-tree was in god shape.  
- The Paladin, wich was my main char in Vanilla is now tanky since TBC.  
- The frost deathknight could tank dual-whield, which was real fun.  

I have played all classes at least to level 120. And what would be more fun and also timesaving if you level 5 chars at the same time?
You can decide which class to play in the group. And maybe you are a brilliant warlock, then you can write a brilliant bot-script for your warlock and stear another class as your main. Playing a tank with bots is real fun. I made my first experience with Solocraft and I know every single mistake these bots are making, so I can improve mine.
Botting is like multiboxing just more fascinating.

I have started to alter AzerothCore (private server) to make characters botable, but failed in some cases. I found no chance to equip the right gear to a bot.  
- Compiling the server took a long time and crashed often with heap memory allocation issues.  
- I was not able to loot when I had a bot in my party and I gave up after 3 days searching for that issue.  
 
Then I found the project (mClient) from Michał Kałużny and was able to make it running as a 3.3.5 compatible client.

## WoW-Killer
Developing this library could also lead to the possibility to create a new MMORPG based on the protokoll from Wow, the AzerothCore on the server and a Client based on Unreal engine 5, Unity or Godot 4. 

## Features Ready
- Login character
- Invite bot into a group

## Features Planned
- Bot shall follow the master (angle, distance)
- Bot shall be able to react on commands like STAY, MOVE, TABLE (for mages), LOCKPICK (for rogues)
- Bot shall buff
- Bot shall heal, dps or tank
- Bot shall accept same quest as master
- Bot shall roll for items and equip items
- And whatever you wish...

## Porting to C++
This project will be ported to C++.  
See: https://github.com/CrowdWare/AzerothClient

## Base
This project is based on https://github.com/justMaku/mClient from Michał Kałużny.
