# 1.5.0.0
- Added support for Patch 7.4 and Dalamud API 14.

# 1.4.0.0
- Added support for Patch 7.3 and Dalamud API 13.
- Added a setting to change the glow direction CW or CCW.
- Fixed config window being shrunk and in the corner of the screen.
- Fixed issues with Dancer's Technical Step.

# 1.3.0.0
- Added support for Patch 7.2 and Dalamud API 12.

# 1.2.0.1
- Added keybind label tag for Actions and Items.
- Fixed Dark Knight's Darkside duration.
- Fixed issues with invalid icon identifiers:
  * A lot of the game's icons game files where moved around in 7.1.
  * If you have auras that are not showing, you might need to manually re-enter the Action/Status names/ids so DelvCD can find the right icons again.

# 1.2.0.0
- Added support for Patch 7.1 and Dalamud API 11.
- Fixed inverted swipe.

# 1.1.1.0
- Added Dynamic Groups that automatically layout their elements.
- Updated Monk's Job Gauge triggers.
- Fixed fonts sometimes not loading.
- Fixed input text for items, status and cooldown triggers being limited to 32 characters.

# 1.1.0.4
- Updated Monk's Fury Stacks Bar for 7.01 patch changes.

# 1.1.0.3
- Fixed Monk's Coeurl Stacks.
- Fixed Dragoon's Life of the Dragon duration.

# 1.1.0.2
- Added "Action Highlighted" condition to Cooldown Triggers.
- Updated Monk's Job Gauge data.
- Updated Summoner's Job Gauge data.
- Fixed Samurai's Combo Ready not working for some actions.

# 1.1.0.1
- Fixed charges and cooldowns not working properly for some abilities.
- Fixed issues when drawing elements pointing to invalid icon ids.

# 1.1.0.0
- Added support for Dawntrail and Dalamud API 10.

# Version 1.0.1.1
- Fixed ImGui crash.

# Version 1.0.1.0
- Added indicators on trigger config windows to show if the Action or Status entered is valid:
  * It is no longer necessary to press Enter after typing the name or ID.
  * A checkmark or a cross shows to indicate if the entered name or ID is valid.
  * If the Action or Status is found, a preview of the icon will show.

- Added Shield data to Character State triggers.
- Fixed some icons not working.

# Version 1.0.0.2
- Fixed Position setting not appearing when No Icon option is selected.
- Improved calculation of distance to target in CharacterState triggers.
- Implemented text fonts with new Dalamud FontAtlas API.

# Version 1.0.0.1
- Fixed bars border thickness not working.
- Fixed glows not working properly for chunked bars.
- Fixed issues that caused the Conditions window to not load properly sometimes.

# Version 1.0.0.0
- Moved from testing.
- Added Chunk Styles for bars.
- Fixed stackable statuses icons.
- Fixed names not working correctly when using other plugins like XIVCombo.

# Version 0.5.2.0
- Added Source Type for Status Triggers:
  * This allows to make triggers that only work on enemies or allies.

- Added option to configure HP, MP, CP and GP triggers with percentages instead of raw values.

- Fixed Monk's Master Gauge Chakra not working correctly:
  * Opo-opo and Raptor chakras were inverted.

- Fixed the Conditions tab sometimes not rendering properly.

# Version 0.5.1.1
- Fixed Hide When Sheathed logic.

# Version 0.5.1.0
- Improved config migration code that allows to use a very old config file.
- Fixed weird artifacts in some parts of the config window.

# Version 0.5.0.0
- Added support for patch 6.5 and Dalamud API 9.
- Added new trigger condition for Red Mage's Job Gauge that compares White and Black Mana.

# Version 0.4.0.1
- Fixed Summoner's Garuda related conditions not working properly.

# Version 0.4.0.0
- Conditions data options are now dynamically populated depending on the triggers in that element:
  * The condition properties were fixed before which resulted in confusing behavior if the element didn't contain the appropriate trigger types.
  * The "Dynamic" option was removed.

- Updated how the data from triggers is used for Icon swipe animations:
  * The trigger and its property must be explicitly set to determine what progress is being tracked.
  * Some of your Icons might need to be manually corrected.

- Updated how text tags work:
  * Text tags are now more explicit which provides more control on which trigger the label will use to get the data from.
  * For example to show the cooldown of an action, instead of `[value]`, you would now do `[cooldown_timer]`.
  * Existing text tags will be automatically changed when updating to this version.
  * Some of your Labels might need to be manually corrected.
  * Use the 'Tags' button next to the Text Format input field in Labels to see all the available text tags for each type of trigger.

- Added JobGauge triggers:
  * These allow you to use data from your job gauges to make conditions.
  * They only work when playing on the corresponding job.

- Added Bar elements:
  * Similar to the Icon's swipe animations, these need to be linked to a trigger property to track its progress.

- Added automatic settings backup system:
  * Every time DelvCD's is updated, a backup of your settings will be created in `%APPDATA%\XIVLauncher\pluginConfigs\DelvCD\Backups\<previous_version>`.

- Added `/dcd` command to open DelvCD's settings.

# Version 0.3.0.0
- Fixed config windows not adjusting to Dalamud's Global Font Scale.

# Version 0.2.3.1
- Fixed issue with elements tracking status on Target of Target or Focus Target.
- Fixed issue with status Icons not showing in certain conditions.

# Version 0.2.3.0
- Added PvP Action cooldown support.
- Added Item cooldown tracking.
- Added ability to resize all icons in a group under Group settings.
- Added Visibility options for root DelvCD group.
- Added Window Clipping option in root Visibility options.
- Added ability to select specific trigger for each condition row.
- Lots of bug fixes and performance improvements.

# Version 0.2.2.1
- Updated plugin for patch 6.1.

# Version 0.2.2.0
- Updated plugin for patch 6.1.

# Version 0.2.1.1
- Fixed issue where elements would show during some cutscenes and quest dialogue.
- Added option to select the trigger source for individual conditions.
- Conditional appearances are now previewed while being edited.
- When a new condition is created, the conditional appearance will copy the current icon configuration.

# Version 0.2.1.0
- Added Visibility option 'Hide While Weapon Sheathed'.
- Added Visibility option to Hide by Player Level.
- Added Icon "Glow" option to mimic default hotbar proc indicators.
- Added buttons to move Elements/Triggers/Conditions up or down in lists.
- Added option to show GCD swipe when cooldown is inactive.
- Added Level as data source for conditions.
- Added new Number Formatting options to Labels.

# Version 0.2.0.2
- Fixed some issues that were causing performance issues with large amounts of elements.
- Fixed issue where elements positions would change if the game resolution changed after startup.
- Fixed issue where progress swipe was not showing during preview.

# Version 0.2.0.1
- New fonts are now automatically added to the font list on import.
- Added 'Solid Color' Icon type.
- Added Ability to dynamically track 'Adjusted Actions':
  * An adjusted action is an ability that changes/upgrades during combat.
  * Examples: Standard Step -> Standard Finish, Gallows -> Cross Reaping, etc.
- Fixed labels appearing on top of other Icons when stacked.

# Version 0.2.0.0
- Re-designed the entire trigger system.
- Tons of new features, performance inprovements, and bug fixes.

# Version 0.1.4.0
- Improved font management.
- Updated About page.
- Added import from clipboard for Labels.
- Added changelog.