https://drive.google.com/drive/folders/1cBryzKH0TIu9YptK90Azrbfm6vv1ZIOo?usp=sharing <br>
-Batın KARADAĞ, İstanbul Rumeli Üniversitesi-<br>
Animations and development made by Batın KARADAĞ<br>
Sound effects and music obtained through Unity Asset Store<br>
<br>
-The game is currently being remade from ground up with slightly different mechanics than those shown in the downloaded demo. A complete list of changes made<br>
and planned can be seen at the end of this file.<br>
<br>
-The current demo lacks enemy and non-permanent object persistency, meaning that reloading an area reloads all dead enemies and broken objects (testflowers)<br>
	-intended behaviour is to reload such objects only when resting at an icon, however that is not implemented in this build<br>
-Bosses and unique unlockables persist between loads<br>
<br>
Due to the tutorials and in-game UI images being mostly placeholder objects for testing purposes, a more extensive tutorial is provided here.<br>
Please refer to this file if you have any questions regarding gameplay.<br>
<br>
//Controls<br>
A-D -> move left-right<br>
Left click -> stance attack<br>
Right click -> soul attack (costs energy)<br>
E -> execute stunned enemy / interact<br>
Hold shift -> run<br>
Press shift -> dodge (provides a small window of invincibility)<br>
Space -> jump (or double jump if unlocked)<br>
1-2-3-4 -> select stance (if another stance is unlocked, it is automatically equipped on an empty slot, slots can be managen through icons)<br>
f1-f2-f3-f4 -> select soul (if another soul is unlocked, ** ** **)<br>
Backspace -> respawn (after death)<br>
<br>
//Gameplay mechanics<br>
<br>
There are three status bars for player, health-stagger-energy.<br>
When health reaches 0, players becomes "Broken" and then possibly executed. Press backspace to respawn.<br>
When stagger reaches 0, player becomes stunned for a brief moment. Staggers cause a small gray particle effect and give a sound que.<br>
Energy is gained by attacking and is used by soul attacks.<br>
<br>
Enemies has health and staggers that function similarly, however they do not have any energy mechanics and solely use their abilities based on cooldowns.<br>
<br>
There are various damage types in the game, each with a multiplier associated to them. These damage types and their multipliers can be seen on the pause menu.<br>
(A multiplier of 0.5 means that the player will take only half damage from attacks coming from that source)<br>
While damage can be completely negated with a multiplier of 0 for a damage type, or even absorbed with a multiplier of -1, stagger damage will still be taken<br>
as its multiplier is limited to a minimum value of 0.3. Otherwise, stagger damage multiplier is always the same as the damage multiplier against the incoming damage's type.<br>
<br>
Enemies are also affected by such multipliers, however that information is unknown to the player.<br>
<br>
There are 5 enemies and 1 bossfight in the demo:<br>
Minimush<br>
Soldiermush<br>
Floatmush<br>
Mushgrowth<br>
Mushmage<br>
Minimush Elite (B)<br>
<br>
-Changelog for new version:<br>
<br>
Changes made:<br>
Removed "Souls" as a mechanic completely<br>
Added heavy attacks<br>
Added jump attacks (heavy and light)<br>
<br>
Animations created for following stances:<br>
+Aeromancer stance<br>
+Alchemist stance<br>
+Armblade stance<br>
+Artificer stance<br>
+Axe and shield stance<br>
+Axe stance<br>
+Beast stance<br>
+Bow stance<br>
+Chakram stance<br>
+Claw stance<br>
+Dagger stance<br>
+Flail stance<br>
+Greataxe stance<br>
+Greathammer stance<br>
+Greatsword stance<br>
+Gun stance<br>
+Halberd stance<br>
+Hammer and shield stance<br>
+Hammer stance<br>
+Kama stance<br>
+Katana stance<br>
+Pyromancer stance<br>
+Quarterstaff stance<br>
+Rapier stance<br>
+Scimitar stance<br>
+Scythe stance<br>
+Spear and shield stance<br>
+Spear stance<br>
+Sword and shield stance<br>
+Sword stance<br>
+Terramancer stance<br>
+Twinblade stance<br>
+Unarmed stance<br>
+Whip stance<br>
<br>
Planned:<br>
More stances<br>
Codex system to see enemy resistances and lore (requires defeating the enemy to unlock its codex entry)<br>
"Forest" zone to serve as the next demo area<br>
Object permanence (enemies and objects keeping their status between load screens unless x action is performed, like resting)<br>
New permanent passive abilities (like double jump etc.)<br>
New upgrade mechanics for each stance to make them more unique on later upgrade levels (partially done)<br>
New stats and stat upgrade system<br>
Inventory and consumable system (low priority)<br>
