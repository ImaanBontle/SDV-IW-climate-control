<div align="center">

# ClimateControl <a id="return-to-top"></a>

Snow in Fall? C'est impossible!

<!--Badges-->
[![License][license-shield]][license-link]
[![Release][release-shield]][release-link]
[![Pre-release][pre-release-shield]][release-link]
[![Release date][release-date-shield]][release-link]
<br>
[![Contributors][contributors-shield]][contributors-link]
[![Open issues][issues-shield]][issues-link]
[![Pull requests][pulls-shield]][pulls-link]
[![Commits since latest release][commits-shield]][commits-link]
[![Last commit][last-commit-shield]][commits-link]

<!--Links-->
[Nexus][nexus-link]
 &#183; 
[ModDrop][moddrop-link]
 &#183; 
[Report Bug][bugs-link]
 &#183; 
[Request a Feature][request-features-link]

</div>

<br>

<!--Table of Contents-->
# Table of Contents

- [About ClimateControl](#about)
- [Feature Overview](#features)
	- [Daily Weather](#about-weather)
	- [Climate Templates](#about-templates)
- [Configuration](#config)
	- [Weather Models](#config-models)
	- [Debug Logging](#config-debug)
- [Known Issues](#issues)
- [Upcoming Features](#upcoming)
- [Contributing](#contribute)
	- [Bug Reports](#bugs)
	- [Feature Suggestions](#suggestions)
	- [Translations](#translations)
- [Support My Work](#support)
- [License](#license)
- [Contributors](#contributors)

<!--About ClimateControl-->
## About ClimateControl <a id="about"></a>

*To get started, refer to the [installation guide][install-link].*

ClimateControl allows you to define unique weather odds for each day of the year. With this mod, the weather changes gradually in sync with the seasons. For example, this might mean a chance of light rain in mid-Winter or a touch of snow in late Fall, all depending on your specific configuration. Whether you want to craft your own climate for complete creative freedom or opt for one of the provided templates* for a simpler setup, the choice is yours!

Read below for a [detailed breakdown of each feature](#features).

**At the moment, the mod includes 'standard' and 'custom' climates only. I'm actively working on adding more templates in the future. Find out more about the [upcoming features](#upcoming).*

<div align="right">

[[Back to top](#return-to-top)]

</div>

## Feature Overview <a id="features"></a>

*This section delves into the intricacies of ClimateControl's capabilities. If you're new to the mod, be sure to check out the [installation guide][install-link] to get started.*

### Daily Weather <a id="about-weather"></a>

In the vanilla game, each type of weather (rain, thunderstorms, snow, wind, sunny) follows fixed rules for every day of the season. These rules determine the likelihood of weather occurring on the following day and are applied each night after you go to sleep. Moreover, these rules only change when the next season begins and, with very few exceptions, remain identical for all 28 days. This means that the same logic is applied every day.

While this approach gives each season a unique gameplay identity, it also keeps them unrealistically distinct from each other. For instance, in the game, you wake up on Winter 1 to find everything suddenly covered in snow, even though it was dry and parched on Fall 28. In the real world, there would likely have been some light snowfall before the land was blanketed in white.

This abrupt change is what ClimateControl aims to address. The mod accomplishes this by initially assigning pre-defined fixed probabilities to the start, middle, and end of each season (on the 5th, 15th, and 24th days, respectively). Players can set these values in the config or use the values from a pre-existing template. Subsequently, the mod employs cubic spline interpolation for the days without assigned probabilities (those between the 'days-of-fixed-probability'), utilizing the pre-defined odds as anchor points.

This approach results in unique odds for each day throughout the year, with gradual changes as the season progresses. It avoids the sudden shift of weather that occurs all at once on day 1 of a new season. This method offers two key advantages:

1. Each day presents a distinct combination of odds, creating a sense of weekly variety.
2. Weather now transitions between seasons, allowing phenomena like increased snowfall in the last days of Fall or impending thunderstorms as Summer approaches, while preserving each season's unique identity.

This approach fosters a more immersive and realistic weather experience. However, players who prefer a simpler weather system more akin to the base game can disable interpolation in the config. In this case, the mod will treat the config values as fixed probabilities for each third of the season (approximately ten days at a time). This maintains a level of realism and variety greater than that of the vanilla game while still approximating the original game's flavour.

### Climate Templates <a id="about-templates"></a>

<!--*For details on each of the templates, check out the [model breakdowns][models-link].*-->

By default, this mod employs a pre-defined *standard climate*. This climate closely resembles the vanilla game's weather patterns, featuring gentle Spring rain, transitioning into brief but intense thunderstorms in early Summer, followed by dry and windy conditions in Fall, and finally, abundant snowfall in Winter. When used alongside cubic spline interpolation, this combination produces a smoother weather profile compared to the base game, with more gradual shifts between seasons while maintaining the original seasons' distinct identities.

Looking ahead, additional templates beyond the *standard climate* will be introduced. Each template will be based on Earth's biomes and inspired by real-world data. These climate presets will replicate the accurate proportions of rain, snow, wind, and storms for a specific biome. Players can also [submit their own templates][discussion-template-link] for potential future inclusion (though no promises are made).

Additionally, the config menu includes a *custom climate* feature, allowing players to design their own weather templates. Notably, this *custom climate* remains independent from [Generic Mod Config Menu's][gmcm-link] *reset-to-default* function, ensuring players can restore defaults without losing their custom creations. Furthermore, each existing template can be manually adjusted if players prefer to modify only specific values rather than creating an entirely new climate.

<div align="right">

[[Back to top](#return-to-top)]

</div>

<!--Config Options-->
## Configuration <a id="config"></a>

*This section covers the settings for ClimateControl. For other mods in the series, see the [mod documentation][main-doc-link].*

The following configuration options exist for ClimateControl. Modify these settings either in-game using the [Generic Mod Config Menu][gmcm-link] or by manually editing the `config.json` file located in the mod's folder (created after running SMAPI at least once). **Default values are indicated in bold**.

### Weather Models:

The weather model determines the likelihood of weather changes for each day of the year, including rain, snow, thunderstorms, and windy weather. Create your own custom model or use one of the provided templates. You can also adjust how daily odds are calculated.

| Name | Value | Description |
|:---|:---|:---|
| **Model Choice** | ***standard**, custom* | *Choose the weather model to use.* |
| **Daily Odds** | ***true**, false* | *When set to *true*, interpolation will be used to estimate daily weather odds.* |

Aside from selecting one of the model templates above, you can manually edit the input probabilities. In [Generic Mod Config Menu][gmcm-link], this can be done via the in-game menu, organized by weather type or by season. Otherwise, you can modify the JSON files in the `models` folder. Refer to the dropdown menus below for comprehensive instructions on manipulating the models.

<details><summary><i>Editing models</i></summary>
<p>

Each season is divided into three time periods: days 1-9, days 10-19, and days 20-28. Each period can have unique probabilities for different weather types. Probabilities are decimal values between 0 and 100, with allowable weather types being 'rain', 'storm', 'wind', and 'snow'. In the absence of these types, the mod defaults to sunny weather (also used for festivals and weddings).

With interpolation enabled, values for each period are assigned to the middle day of that period (day 5, day 15, and day 24). Other days in that period receive values determined by the interpolation, ensuring smooth transitions.

Without interpolation, all days within a period have the same value (probability remains constant throughout the period).

Edit time period values in-game with [Generic Mod Config Menu][gmcm-link] or by manually editing the files in the `models` folder. If comfortable with data arrays, you can observe interpolation's real-time effect by examining the JSON files in the `data` folder after changing settings.

</p>
</details>

<details><summary><i>Copying models</i></summary>
<p>

Using [Generic Mod Config Menu][gmcm-link], you can copy values from *model A* directly to *model B*:

1. Switch to *model A* from *model B*.
2. Open the values page.
3. Click "Save," then "Save & Close."

This method can be useful for establishing a base for a *custom climate* using an existing template.

</p>
</details>

<details><summary><i>Resetting models</i></summary>
<p>

Models can be reset anytime using [Generic Mod Config Menu][gmcm-link]. You can also manually locate and delete the JSON files. Files reside in the `models` folder and are named after each model. Deleted files are regenerated with default values upon SMAPI launch.

*Note: Custom models require manual deletion and are always preserved during GMCM resets.*

</p>
</details>

### Debug Logging:

When debug logging is enabled, SMAPI will provide terminal outputs of dice rolls and other useful information.

<div align="right">

[[Back to top](#return-to-top)]

</div>

<!--Known Issues-->
## Known Issues <a id="issues"></a>

*This section highlights known issues related to ClimateControl. For information on other mods, refer to the [mod documentation](#docs).*

- In the current version, existing thunderstorms persist into the next day when rain is expected. This behavior is due to the way the game handles weather flags. Fortunately, the issue resolves itself automatically at the start of the next sunny or windy day.
	- This bug will be addressed in an upcoming update.
- During Summer, the TV incorrectly reports windy weather as snow. This discrepancy arises because the game doesn't anticipate this weather type during that season.
	- I'm investigating a patch for this issue, though no estimated time of arrival is available.
- There's an issue with weather odds being calculated a day sooner than expected, leading to inaccurate predictions for festival days and weddings.
	- I am in the process of fixing this, and the solution will be implemented shortly.

<div align="right">

[[Back to top](#return-to-top)]

</div>

## Upcoming Features <a id="upcoming"></a>

*This section outlines upcoming additions and enhancements. To track feature progress, refer to [the latest changelog][changelog-link].*

### Upcoming:

- Introduction of translation support.
- Addressing bug fixes.

### Planned:

- Expanding the selection of templates to choose from.

<div align="right">

[[Back to top](#return-to-top)]

</div>

<!--Contributing-->
## Contributing <a id="contribute"></a>

*Please be patient if I haven't responded immediately. I am likely busy with my studies.*

This project is open-source and contributions are welcome, particularly in the form of [bug fixes](#bugs), [feature suggestions](#suggestions) and [translation support](#translations). For more substantial contributions, please fork the develop repo and submit a pull request using the https://github.com/ImaanBontle/SDV-IW-climate-control/labels/contribution label. You can also attempt to contact me via [NexusMods][nexus-profile] or by [opening an issue][issues-link].

<!--Bugs-->
### Bug Fixes/Reports <a id="bugs"></a>

If you encounter any bugs, please remove any [incompatible mods](#incompatible) and then re-run SMAPI to check if the issue resolves itself. If the bug persists or if you do not see your mod included in the list, then you should [submit a bug report][bugs-link]. Please answer the prompts to the best of your ability and mention any suspected mod conflicts. You should also provide a link to your [SMAPI log][smapi-log] in the report.

***Please only submit bug reports if you have confirmed that the bug is not present in the vanilla game itself!***

Alternatively, f you would like submit a bugfix, you can do so by submitting a pull request using the https://github.com/ImaanBontle/SDV-IW-climate-control/labels/fix and https://github.com/ImaanBontle/SDV-IW-climate-control/labels/contribution labels.

<!--Feature Suggestions-->
### Feature Suggestions <a id="suggestions"></a>

If you would like to suggest a feature for this project, please [submit a feature request][request-features-link]. I do my best to read these and I would love to hear from you. While I can't guarantee that these suggestions will be included in future releases, you will be credited for any features that do get implemented.

<!--Translations-->
### Translations <a id="translations"></a>

*Translation support will be added in the next minor release. In anticipation, I am adding the following table of translations. Please note that the associated `default.json` files are currently empty and should be ignored.*

(❑ = untranslated, ↻ = partly translated, ✓ = fully translated)

&nbsp;     | [ClimateControl][climatecontrol-translation] | [Framework][framework-translation]
:--------- | :--------------------------------- | :----------------------------
Chinese    | ❑                                  | ❑
French     | ❑                                  | ❑
German     | ❑                                  | ❑
Hungarian  | ❑                                  | ❑
Italian    | ❑                                  | ❑
Japanese   | ❑                                  | ❑
Korean     | ❑                                  | ❑
Portuguese | ❑                                  | ❑
Russian    | ❑                                  | ❑
Spanish    | ❑                                  | ❑
Turkish    | ❑                                  | ❑

<div align="right">

[[Back to top](#return-to-top)]

</div>

<!--Support-->
## Support My Work <a id="support"></a>

If you would like to support my work, you have the option to [buy me a coffee][ko-fi-link]. However, please note that this is completely voluntary. My mods are available for free and entirely without expectation!

<div align="right">

[[Back to top](#return-to-top)]

</div>

<!--License-->
## License <a id="license"></a>

The source code for this mod is open-source and is released under the [MIT license][license-link]. However, please refrain from hosting my official releases without obtaining my written consent.

<div align="right">

[[Back to top](#return-to-top)]

</div>

<!--Contributors-->
## Contributors <a id="contributors"></a>

Thank you to all the contributors who helped with this project!

[![Image of all contributors][contributors-image]][contributors-link]

<div align="right">

[[Back to top](#return-to-top)]

</div>

<!--Markdown Links, Images and Abbreviations-->
<!--
REFERENCES FOR INSPIRATION LAYOUTS
[best-readme]: https://github.com/othneildrew/Best-README-Template
[awesome-readme]: https://github.com/Louis3797/awesome-readme-template
[readme-article]: https://www.freecodecamp.org/news/how-to-write-a-good-readme-file/
[translation-table]: https://github.com/Pathoschild/StardewMods/#translating-the-mods
[translation-script]: https://gist.github.com/Pathoschild/040ff6c8dc863ed2a7a828aa04447033
-->

<!--Shields-->
[license-shield]: <https://img.shields.io/github/license/ImaanBontle/SDV-IW-climate-control>
[license-link]: <https://github.com/ImaanBontle/SDV-IW-climate-control/blob/main/LICENSE> "License"
[release-shield]: <https://img.shields.io/github/v/release/ImaanBontle/SDV-IW-climate-control>
[release-link]: <https://github.com/ImaanBontle/SDV-IW-climate-control/releases> "Latest releases"
[pre-release-shield]: <https://img.shields.io/github/v/release/ImaanBontle/SDV-IW-climate-control?include_prereleases&label=pre-release>
[release-date-shield]: <https://img.shields.io/github/release-date/ImaanBontle/SDV-IW-climate-control>
[contributors-shield]: <https://img.shields.io/github/contributors/ImaanBontle/SDV-IW-climate-control>
[contributors-link]: <https://github.com/ImaanBontle/SDV-IW-climate-control/graphs/contributors> "Contributors"
[commits-shield]: <https://img.shields.io/github/commits-since/ImaanBontle/SDV-IW-climate-control/latest?include_prereleases>
[commits-link]: <https://github.com/ImaanBontle/SDV-IW-climate-control/commits> "Commit history"
[issues-shield]: <https://img.shields.io/github/issues-raw/ImaanBontle/SDV-IW-climate-control>
[issues-link]: <https://github.com/ImaanBontle/SDV-IW-climate-control/issues> "Issues"
[pulls-shield]: <https://img.shields.io/github/issues-pr/ImaanBontle/SDV-IW-climate-control>
[pulls-link]: <https://github.com/ImaanBontle/SDV-IW-climate-control/pulls> "Open pull requests"
[last-commit-shield]: <https://img.shields.io/github/last-commit/ImaanBontle/SDV-IW-climate-control>

<!--Immersive Weathers-->
[main-mod-link]: <https://github.com/ImaanBontle/SDV-immersive-weathers> "Main GitHub Page"
[install-link]: <https://github.com/ImaanBontle/SDV-immersive-weathers/blob/develop/README.md#getting-started> "Mod Installation"
[main-doc-link]: <https://github.com/ImaanBontle/SDV-immersive-weathers/blob/develop/README.md#docs> "Mod Documentation"

<!--Repo Links-->
[nexus-link]: <https://www.nexusmods.com/stardewvalley/mods/14659> "ClimateControl on NexusMods"
[moddrop-link]: <> "ClimateControl on ModDrop"
[github-link]: <https://github.com/ImaanBontle/SDV-IW-climate-control> "ClimateControl on GitHub"
[bugs-link]: <https://github.com/ImaanBontle/SDV-IW-climate-control/issues/new?assignees=ImaanBontle&labels=bug&template=bug_report.md&title=%5BBUG%5D%3A+> "Report a Bug/Problem"
[smapi-log]: <https://smapi.io/log> "SMAPI Log Parser"
[request-features-link]: <https://github.com/ImaanBontle/SDV-IW-climate-control/issues/new?assignees=ImaanBontle&labels=enhancement&template=feature_request.md&title=%5BFEATURE%5D%3A+> "Request a New Feature"
[discussions-tab]: <https://github.com/ImaanBontle/SDV-immersive-weathers/discussions> "Start a Discussion"
[discussion-template-link]: <https://github.com/ImaanBontle/SDV-immersive-weathers/discussions/new?category=weather-templates> "Discussions: Weather Templates"

<!--Dependency Links-->
[stardew-link]: <https://store.steampowered.com/app/413150/Stardew_Valley/> "Get Stardew Valley on Steam"
[smapi-link]: <https://smapi.io/> "Download SMAPI"
[smapi-mod-wiki]: <https://stardewvalleywiki.com/Modding:Player_Guide/Getting_Started#Install_mods> "SMAPI Modding Guide"
[smapi-instructions]: <https://stardewvalleywiki.com/Modding:Player_Guide/Getting_Started#Getting_started> "SMAPI Installation Guide"

<!--Documentation-->
[changelog-link]: <https://github.com/ImaanBontle/SDV-IW-climate-control/blob/develop/CHANGELOG.md> "Latest CHANGELOG"

<!--Supported Mods-->
[gmcm-link]: <https://www.nexusmods.com/stardewvalley/mods/5098> "Generic Mod Config Menu on NexusMods"
[even-better-rng-link]: <https://www.nexusmods.com/stardewvalley/mods/8680> "Even Better RNG on NexusMods"

<!--Translations-->
[framework-translation]: <https://github.com/ImaanBontle/SDV-immersive-weathers/tree/develop/i18n> "Framework i18n folder"
[climatecontrol-translation]: <https://github.com/ImaanBontle/SDV-IW-climate-control/tree/develop/i18n> "ClimateControl i18n folder"

<!--Contact Links-->
[nexus-profile]: <https://forums.nexusmods.com/index.php?showuser=54975162> "NexusMods Profile"
[ko-fi-link]: <https://ko-fi.com/msbontle> "Donate"

<!--Contributor Links-->
[contributors-graph]: <https://github.com/ImaanBontle/SDV-IW-climate-control/graphs/contributors> "List of contributors"
[contributors-image]: <https://contrib.rocks/image?repo=ImaanBontle/SDV-IW-climate-control> "Thank you to all the contributors!"