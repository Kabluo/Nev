//TODO
-overall refactoring of all scripts for improved readability, currently the scripts don't follow any specific rules (high priority)
  -implement the "this." notation to scripts that don't contain it
  -remove obselete code and leftover comments
  -merge some scripts to reduce repetition
  -move "Soul" object variables to SoulSOs (stat multipliers and damage affinities)
  -remove hardcoded values, use [SerializeField] for easier editor-level edits
  
-overhaul UI elements, minor graphical fixes (low priority)
-implement additional animations (low priority)
  -implement the DialogueIdle, StanceSwap and SoulSwap animations
-implement additional enemies (low priority)
  -complete the "mushroom" type enemies (animations finished)

-overhaul enemy behaviour and player detection, current detection is hacky (high priority)
-overhaul the way attack hitboxes are handled (low priority)
-overhaul the current layer system and remove obsolete layers (low priority)
-overhaul OnStandby, make overall behaviour similar to MinimushElite ManageStandby() function (high priority)
-overhaul after-grab placement, get the grab position at the time of grab, currently enemy after-grab spawn can be manipulated by player (low priority)
-minor fixes for Parallax script, currently works but throws errors when returning to menu as the camera component it tracks gets destroyed right before the scene
swap, harmless but annoying (low priority)
