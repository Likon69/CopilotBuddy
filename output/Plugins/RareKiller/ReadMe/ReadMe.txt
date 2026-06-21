=======================================================================

                Rare Mob Killer -Plugin - Version
              Honorbuddy Plugin - www.thebuddyforum.com
                        Autor: katzerle

========================================================================

This Plugin will help you hunt Rare Mobs like TLPD
It can also hunt a Mob by ID or tame your Hunter his favourite Pet.
And Now also implemented some Features in Test Stage: 
--> Aeonaxx and Camel Figurine Interactor

If your WOW notices a hunted Mob, this Plugin will take control over your toon and flys or walks to the Mob and aggro him with an instant spell.
If falling down from the Mount (while hunting flying Mobs) the implemented Slowfall-Part, if enabled, will cast slowfall, so you wont die. Ones Aggro your CC will bring it to the end and loot the Mob. The Plugin also plays a Sound if found a hunted Mob. I implemented a little workaround to make sure your Rares will be lootet, even if you disable it in GB2. 
For your Security and to look more human while camping I also implemented
• a little Wisper-Alert with logoutput
• a Guildmessage-Alert
• a Keypresser which presses a Key every 3,5 - 5 Minutes
• a LogOut which logs you out if your Toon gets portet (notices if the Continent changes)

You could use this Plugin beside Gathering, Arching and Leveling.
If running with GB2 and Grindbot you also need a Profile.

Profiles:
• eXtOphius made a Nordend Mega Profile . It is perfect to hunt for the Archievement Frostbitten. It is a very long route, and so doesn't attract so much attention as small circles in one Area.
• I attached also two Profiles which circle around in Stormpeak for the Timelost Protodrake 
(One from Tony and one from Tvsian, thanks to you both  ) But don't use them 24/7 on a Account which you can't afford to lose.
• Also attached a GB2 Profile without Movement to camp the TLPD near Bor's Breath
• And for camping without Movement you could use the Camperprofile with the Grindbot.
With the No Movement - Profiles you could use the Keypresser to let your Char look humanlike while camping.

Possible settings for hunted Mobs:
• WOTLK-Rares for Archievement Frostbitten
• BC-Rares for Archievement Bloodlust
• all lootable Cataclysm-Rares for some nice Items
• Timelost Protodrake for Mount (including killing Vyragosa also)
• Classic Rare Mobs <= Level 60
• Hunt by ID (you give him a ID and he will kill all of them if seen)
• New: Collect Raptor Nests for little Raptor Pets
You can enable all of them at the same time

Now also the Tamer is implemented in this Plugin:
If activated he will Tame per default:
• Ghostcrawler (Hint: for him you need livetime or flying plugin because of the Movement in Vashjir)
• Jadefang (You have to camp him in his cave because the bot can't go there)
• Karoma
• Madexx (all Colors)
• Terrorpene
• Loque'nahak
• Sambas
• Aotona
• Skoll
• Gondria
• Arcturis
• King Krush
• Nuramoc
• Goretooth
Other Pets will Follow with one of the next Versions. Till then use Tame by ID if you want to camp and tame another special Mob.
Hint: you have to be sure that your Hunter has an empty slot when activating this Pluginpart, there is no check implemented.

In Teststage because of the really rare Testobjects:
• Aeonaxx 
(only testet in Theorie with Chillmaw, I really don't know if it works or if HB and its CC (who must handle the fight) just says " WTF")
• Camel Figurine interactor 
(Interact with Figurine works but there where some Problems with Movement in the steam pool area from Dormus. Hopefully I fixed them by using Movement with Keys, but this isn't testet there.)

How to use:
1. Make sure you have the latest version of Honorbuddy
2. Extract the ZIP in your HonorBuddy/Plugins folder and enable it
3. Config the Plugin with the Config Window and press Save. (Description of the Window in Post 2)
4. Enable Automatically Dismount in WOW
6. If hunting Dragons: Make sure you have enough Slowfall Items in your inventory so the bot can waste some of them in worst cases
....6a. Please test if the Slowfall-Plugin works correctly by manually dismounting above the ground (maybe over a lake) and maybe change the slowfalltimer, that it works best for you.
8. If you made changes in the Configwindow, you always have to save them before the Bot will use them.

Special Thanks to: 
• highvoltz (I was using his NetherwingCollector as basement to write this Plugin)
• CodenameG (His Tips and Plugins helped with the GUI)
• Bobby (... for his really amazing CC ShamWOW. It helped me with the Config save and load)
• Tony (... for his Timelost Protodrake-Profile)
• Tvsian (... for the Safer Loop Timelost Protodrake-Profile)
• Kickazz (... for some Tipps)
• SMcCloud (... for the original Slowfall-Plugin)
• ZapMan for his Zapsound 
and thanks to all the brave Tester of the early Versions of these Plugin

Some more helpfull hints:
• You can't hunt Timelost Protodrake, Aeonaxx and Vyragosa in Groundmount Mode
• You may die a couple of times, if you hunt Timelost Protodrake or Vyragosa without using Slowfall
• The Bot will not try to kill Rare Mobs whose Level is 5+ higher then yours
• Rogues, Warriors and sometimes also the caster classes have some Problems to kill Vyragosa because she is a Flying Range Mob. 
If you don't want your toon trying to kill her just check: don't kill Vyragosa. You also can ues "shoot down Vyra" if you have a Gun or Thrown
• In some cases the workaround to loot Vyragosa don't work. Hopefully this is just a Problem of Vyragosa because she is a Range Mob while TLPD is a Melee. I can't give you a garantee.
If you want to play it safe use the Camperprofile and camp on a Place where no chasms and holes are around.

Bot Save:
If you use this Plugin with your Main to camp Aeonaxx and TLPD - Bot Safe:
• don't get slaphappy if you use this Plugin with your Main Account.
• Circle around 24/7 the same Route for days is very risky. Take some breaks between camping.
• If you want to play very save, don't leave your Bot alone at home.
• I use a Camperprofile with only one fix hotspot to camp TLPD and Aeonaxx, this looks more human in my opinion.
Sometimes I also let him fly around 1-2 rounds, but then I every time switch back to the camperprofile

Useful Hints for No-Movement-Camping:
If you use - instant Logout - and you get a suspencion because of a GM-Port, you could tell Blizz something like:
"I was just camping/searching for the Dragon/farming some Herbs as surprisedly I get a Relogg Screen and my Toon stands in xyz 
and I get a WoW Error. My WoW gots broken and I have to reinstall it with all the additional Shit :(. It takes hours to search for all the Addons 
I use. And now I'm not able to logg in. Please fix this, I don't know what happens."
And if they ask you why you don't answer to GM-Wispers just say:
"I told my little Brother/Son/etc to press a Movekey every few Minutes so my char don't get AFK and let me know if the NPC-Scan shows 
something. He didn't know how to answer Questions. And what they look like"

Easy Setup for hunting TLPD:
1. enable the Plugin
2. open the Configwindow Setup and then press Save
3. buy some Snowfall Lager (if you don't play on a english client you have ot replace it in the Config Window in your language)
4. Enable Automatically Dismount in WOW
6. Load the CamperProfile --> Link Here and choose Grindbot
6a ... or Load the TLPD-Bors-Breath Profile and choose Gatherbuddy2
7. Place your toon somewhere to Bor's Breath and press start.

If you want to Hunt TLPD its a good idea to test the Plugin before going AFK:
Especially for TLPD-Hunters it makes sense to test this Plugin on a dummy Mob before using it afk.
Please enable Hunt by ID and write 33687 in the Textbox and Save. Now the Plugin will also hunt Chillmaw in Icecrown.
Setup your Slowfall, Pullrange/-spell, Slowfalltimer and all the other stuff. 
Use the Camperprofile (link above) with Grindbot and start. Then fly manually near the point were Chillmaw is.
If everything works right the bot will fly to him, pull him and use the slowfall if he is higher above the ground and then finally kills him.
If this works you can go hunting for other Drakes. 

Special Setups for some Classes:
• Rogues have to set the Range for Pullspell to 5 if using the Default Pullspell "Fan of Knifes" 
- Will be automatically Set when pressed Save
• Paladins have to set the Range for Pullspell to 3 and config the Pull Spell with "Cruisader Strike" 
- Will be automatically Set when pressed Save
• Hunter should set the Range to 25 - Its implemented as Default Range
• Don't use Paladin Bubble as Slowfall Ability, because you might lose the Aggro of the Drake while Falling down. 
