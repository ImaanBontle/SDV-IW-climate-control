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
- [Support](#support)
- [License](#license)

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

The following configuration options exist for ClimateControl. You can change these options in-game using [Generic Mod Config Menu][gmcm-link] or by manually editing the `config.json` in the mod's folder (generated after running SMAPI at least once). **Default values are shown in bold**.

### Weather Models:

The weather model determines the likelihood of weather changes tomorrow for each day of the year (e.g. the chance of rain, snow, thunderstorms etc.). You can make your own custom model or use one of the provided templates, as well as change the way daily odds are calculated.

| Name | Value | Description |
|:---|:---|:---|
| **Model Choice** | ***standard**, custom* | *Determines the choice of weather model.* |
| **Daily Odds** | ***true**, false* | *If *true*, interpolation will be used to estimate the daily weather odds.* |

In addition to selecting one of the model templates above, you can also manually edit the input probabilities. If using [Generic Mod Config Menu][gmcm-link], then this can be done via the in-game menu, sorted by weather or by season. Otherwise, this can be done by manually editing the JSON files available in the `models` folder. See the dropdown menus below for more details on manipulating the models.

<details><summary><i>Editing models</i></summary>
<p>

Each season is broken up into three time periods, covering days 1-9, days 10-19 and days 20-28. Each period may be assigned a unique probability for each type of weather. The probabilities must be decimal values between 0 and 100, and the allowed weather types are 'rain', 'storm', 'wind' and 'snow'. When none of these weather types occur, the game defaults to Sunny weather (this is also used for festival days and weddings).

When interpolation is enabled, the values for each time period are assigned to the middlemost day in that period (this corresponds to day 5, day 15 and day 24 respectively). Every other day in that period will then be given a value (determined by the interpolation), so that the overall change from period to period is smooth and gradual.

In contrast, when interpolation is disabled, every day in that time period will take on the same value (or put differently, the probability for that time period will be held fixed until that time period comes to an end).

The values for each time period can be edited in-game with the [Generic Mod Config Menu][gmcm-link] or by manually editing the files in the `models` folder. If you are comfortable with data arrays, you can also see the real-time effect of the interpolation after changing the settings by looking at the JSON files found in the `data` folder.

</p>
</details>

<details><summary><i>Copying models</i></summary>
<p>

Using [Generic Mod Config Menu][gmcm-link], it is possible to copy the values from *model A* directly into *model B*, by

1. Switching from *model A* to *model B*,
2. Opening the values page,
3. Then clicking "Save", followed by "Save & Close".

This can also be a handy way of creating a base for a *custom climate* by using a pre-existing template as your starting point.

</p>
</details>

<details><summary><i>Resetting models</i></summary>
<p>

Models can be reset at any time by using the [Generic Mod Config Menu][gmcm-link]. You can also find and delete the files manually. These files are found in the `models` folder and are named after each model. They will then be automatically regenerated and populated with the default values the next time you launch SMAPI.

*NB: Custom models must be deleted manually. By design, they are always preserved during a GMCM reset.*

</p>
</details>

### Debug Logging:

When debug logging is enabled, SMAPI will output the dice rolls and other useful information to the terminal.

<div align="right">

[[Back to top](#return-to-top)]

</div>

<!--Known Issues-->
## Known Issues <a id="issues"></a>

*This section contains known issues with Climate Control. For other mods, see the [mod documentation](#docs).*

- Existing thunderstorms will continue into the next day whenever it is supposed to rain instead. This is due to the way the game currently handles weather flags. Thankfully, the issue will resolve itself automatically at the start of the next sunny/windy day.
	- This bug will be patched in a coming update.
- The TV will misreport windy weather as snow in Summer, because it is not expecting this weather type during that season.
	- A patch is being investigated, but no ETA.
- Weather odds seem to be calculated one day sooner than anticipated, resulting in the incorrect predictions for festival days and weddings.
	- This is being fixed imminently.

<div align="right">

[[Back to top](#return-to-top)]

</div>

## Upcoming Features <a id="upcoming"></a>

*All upcoming features will be documented here. For feature progress, see [the latest changelog][changelog-link].*

### Upcoming:

- Translation support.
- Bug fixes.

### Planned:

- Add more templates.

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
### Suggestions <a id="suggestions"></a>

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

If you would like to support my work, you can always [buy me a coffee][ko-fi-link].

However, please note that this is entirely optional. My mods are available for free and entirely without expectation!

<div align="right">

[[Back to top](#return-to-top)]

</div>

<!--License-->
## License <a id="license"></a>

The source code for this mod is open-source and available under the [MIT license][license-link]. However, please do not host my official releases without my written consent.

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