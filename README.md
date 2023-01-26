//TODO<br>
-add other asset files (Animations, Audio, Fonts, Materials, Prefabs, Scenes, Sprites, TMPro and Tiles) to branch
-overall refactoring of all scripts for improved readability, currently the scripts don't follow any specific rules (high priority)<br>
*implement the "this." notation to scripts that don't contain it<br>
*remove obselete code and leftover comments<br>
*merge some scripts to reduce repetition<br>
*move "Soul" object variables to SoulSOs (stat multipliers and damage affinities)<br>
*remove hardcoded values, use [SerializeField] for easier editor-level edits<br>
  
-overhaul UI elements, minor graphical fixes (low priority)<br>
-implement additional animations (low priority)<br>
*implement the DialogueIdle, StanceSwap and SoulSwap animations<br>
-implement additional enemies (low priority)<br>
*complete the "mushroom" type enemies (animations finished)<br>

-overhaul enemy behaviour and player detection, current detection is hacky (high priority)<br>
-overhaul the way attack hitboxes are handled (low priority)<br>
-overhaul the current layer system and remove obsolete layers (low priority)<br>
-overhaul OnStandby, make overall behaviour similar to MinimushElite ManageStandby() function (high priority)<br>
-overhaul after-grab placement, get the grab position at the time of grab, currently enemy after-grab spawn can be manipulated by player (low priority)<br>
-minor fixes for Parallax script, currently works but throws errors when returning to menu as the camera component it tracks gets destroyed right before the scene
swap, harmless but annoying (low priority)<br>
