<div align="center">

# ClimateControl <a id="return-to-top"></a>

Snow in Fall? If you say so...

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

*To get started, see [installation][install-link].*

Climate Control allows you to define unique weather odds for each day of the year. Since every day is unique, the weather now changes gradually alongside the seasons. This might mean small chances of snow in late Fall or even light rain in mid-Winter, depending on the exact configuration. Either craft your own climate for complete creative freedom or use one of the provided templates* for an easier setup, the choice is yours!

Read below for a [detailed breakdown of each feature](#features).

**Currently, the mod includes only the 'standard' and 'custom' climates. More templates will be added in the future. See [upcoming features](#upcoming) for more information.*

## Feature Overview <a id="features"></a>

### Daily Weather <a id="about-weather"></a>

In the vanilla game, each type of weather (rain, thunderstorms, snow, wind, sunny) follows fixed rules for every day of the season. These rules determine the likelihood that the weather will occur on the following day and are applied each night after you go to sleep. Additionally, these rules only change when the next season begins, and with very few exceptions, are identical for all 28 days, which means the same logic applies every day.

This approach gives each season a unique identity, which is great from a gameplay perspective. However, it also keeps them unrealistically distinct from each other, so that seasons seem to behave independently. For example, in-game, you wake up on Winter 1 to find that everything's suddenly covered in snow, even though it was completely dry and parched on Fall 28! In the real world, you probably would've seen some light snowfall before the land was blanketed in white!

This abrupt change is what ClimateControl attempts to address. To do this, the mod first assigns some pre-defined, fixed probabilities to the start, middle and end of each season (respectively, the 5th, 15th and 24th day). These values can be provided by the player in the config or alternatively grabbed from a pre-existing template. Then, after these values are assigned, the mod performs cubic spline interpolation for all of the remaining days which do not have a probability (i.e. those that lie between the 'days-of-fixed-probability'), using the pre-defined odds as anchor points.

What this ultimately means is that we get unique odds for every day of the year which change smoothly as the season progresses, and transition gradually from season to season, rather than all at once at day 1. There are two major advantages to this approach:

1. No two days have the same combination of odds. This results in each week feeling slightly different to the one prior.
2. Weather now 'bleeds over' from season to season. This allows for phenomena like increased snowfall in the last days of Fall, or impending thunderstorms as Summer approachs, all while ensuring each season's identity is kept intact.

This should allow for a more realistic and immersive weather experience. However, if players would prefer a simpler approach to the weather, more akin to the basegame, then interpolation can be disabled in the config. In this case, the mod will instead default to treating the config values as fixed probabilities for each chunk of the season (roughly ten days at a time). This means the same rules will apply for each period, rather than shifting gradually over time. While this approach remains less realistic than simply using the interpolative method, it may approximate the style of the basegame but will still be more realistic and varied than in vanilla.

### Climate Templates <a id="about-templates"></a>

<!--*For details on each of the templates, check out the [model breakdowns][models-link].*-->

By default, this mod uses a pre-defined *standard climate*. This climate is most similar to vanilla, featuring gentle Spring rain, transitioning into brief but intense thunderstorms in early Summer, followed by a dry and windy in Fall, and then plenty of snowfall in Winter. When combined with cubic spline interpolation, this produces a smoother weather profile than the basegame with more gradual season changes, but retains the original seasons' identities.

In the future, more templates will be added beyond the *standard climate*, each based around a biome found on Earth. These climates will be inspired by real-world data (unlike the *standard climate*), so they will reproduce the correct proportions of rain, snow, wind, and storms. As part of this, players can also [submit their own templates][discussion-template-link] for possible future inclusion (no promises, though!).

There also exists a *custom climate* in the config menu, which allows players to design their own templates. By design, this *custom climate* is kept completely separate from [Generic Mod Config Menu's][gmcm-link] *reset-to-default* feature, meaning players can safely restore their defaults without losing their custom creations. Furthermore, each of the existing templates can be tweaked manually should players wish to change only some of the values, rather than building an all-new climate.

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

- Existing storms may continue into the next day whenever it is supposed to rain. This is due to the way the game currently handles thunderstorms, but will resolve itself at the start of the next sunny/windy day.
	- This will be patched in a coming update. 
- The TV will misreport windy weather as snow in Summer and Winter, because it is not expecting it during these seasons. A patch is being investigated.

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
## Contribute to this Project <a id="contribute"></a>

This project is open-source and contributions are welcome, particularly in the form of [bug fixes](#bugs), [feature suggestions](#suggestions) and [translation support](#translations).

For more substantial contributions, please fork the develop repo and submit a pull request using the https://github.com/ImaanBontle/SDV-IW-climate-control/labels/contribution label. You can also attempt to contact me via [NexusMods][nexus-profile] or by [opening an issue][issues-link].

Please be patient if I haven't responded immediately. I am likely busy with my studies.

<!--Bugs-->
### Bug Fixes/Reports <a id="bugs"></a>

If you encounter any bugs, please first remove any [incompatible mods](#incompatible) and re-run SMAPI to check if the issue resolves itself. If the bug persists or you do not see your mod included in the list, you can [submit a bug report][bugs-link]. Please answer the prompts to the best of your ability and mention any suspected mod conflicts. You should provide a link to your [SMAPI log][smapi-log] in the report.

If you would like submit a bugfix, you can do so by submitting a pull request using the https://github.com/ImaanBontle/SDV-IW-climate-control/labels/fix and https://github.com/ImaanBontle/SDV-IW-climate-control/labels/contribution labels.

<!--Feature Suggestions-->
### Suggestions <a id="suggestions"></a>

If you would like to suggest a feature for this project, please [submit a feature request][request-features-link]. While I can't guarantee these will be included in future releases, I would still love to hear from you. You will be credited for any suggestions that get implemented.

<!--Translations-->
### Translations <a id="translations"></a>

*Translation support will be added in the next minor release. In anticipation, I am adding the following table of translations.*

***Please note that the associated `default.json` files are currently empty and should be ignored.***

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
## Support <a id="support"></a>

If you would like to support my work, you can [buy me a coffee][ko-fi-link]. However, this is entirely optional. My mods are available for free and without expectation.

<div align="right">

[[Back to top](#return-to-top)]

</div>

<!--License-->
## License <a id="license"></a>

The source code for this mod is available under the [MIT license][license-link]. Please do not host my official releases without my written consent.

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