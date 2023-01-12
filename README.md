<div align="center">

# Climate Control <a id="return-to-top"></a>

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

- [About](#about)
	- [Daily Weather](#about-weather)
	- [Climate Templates](#about-templates)
- [Configuration](#config)
	- [Weather Models](#config-models)
	- [Debug Logging](#config-debug)
- [Compatibility](#compatibility)
	- [Supported](#supported)
	- [Recommended](#recommended)
	- [Incompatible](#incompatible)
- [Known Issues](#issues)
- [Upcoming Features](#upcoming)
- [Contributing](#contribute)
	- [Bug Reports](#bugs)
	- [Feature Suggestions](#suggestions)
	- [Translations](#translations)
- [Support](#support)
- [License](#license)

<!--About ClimateControl-->
## About Climate Control <a id="about"></a>

*To get started, see [installation][install-link].*

Climate Control allows you to define unique weather odds for each day of the year.

Since every day is unique, the weather changes gradually alongside the seasons. This might mean small chances of snow in late Fall or some light rain in mid-Winter, the choice is yours! Either use one of the provided climates* for an easier setup or craft your own for complete creative freedom.

Read below for feature overviews.

**Currently, the mod includes only the 'standard' and 'custom' climates. More templates will be added in the future. See [upcoming features](#upcoming) for more information.*

### Daily Weather <a id="about-weather"></a>

In the vanilla game, each type of weather (rain, thunderstorms, snow, wind, sun) follows fixed rules within each season. These rules determine the likelihood that that weather type will occur on the following day. With very few exceptions, these rules change only when the next season begins.

While this gives each season a unique identity, it also keeps them unrealistically distinct from each other. For example, in-game, you go to sleep on Fall 28 and on Winter 1, everything's suddenly covered in snow! Meanwhile, in the real world, you likely would have seen some warning that Winter Is Coming, such as light snowfall. This abrupt change is what Climate Control attempts to address.

First, the mod assigns fixed probabilities to the start, middle and end of each season. These values can either be provided by the player or grabbed from a pre-existing template. After the values are assigned, the mod then performs cubic spline interpolation for all of the days in-between. This produces unique odds for every day of the year, meaning they change gradually as the days pass, rather than all at once.

There are two major advantages to this:

1. No two days will have the same odds, so each week feels slightly different to the one before.
2. Weather now 'bleeds over' from season to season, allowing for things like snow at the end of Fall, all while keeping each season's identity intact.

If you would prefer a simpler approach, the interpolation can be disabled. In this case, the mod will treat the config values as fixed probabilities for each 1/3rd of the season. This means the same rules apply for roughly ten days at a time. While this is less realistic than using the interpolation, it is still more realistic and varied than in vanilla.

### Climate Templates <a id="about-templates"></a>

*For details on each of the templates, check out the [model breakdowns][models-link].*

By default, this mod uses a pre-defined *standard* climate. This is most similar to vanilla, featuring gentle Spring rain, brief thunderstorms in early Summer, a dry and windy in Fall, and plenty of snowfall in Winter. When combined with cubic spline interpolation, this produces a smoother weather profile than the basegame with more gradual season changes.

There also exists a *custom* climate, to allow players to design their own template. This climate is kept completely safe from resets with [Generic Mod Config Menu][gmcm-link], meaning players can safely restore their defaults without losing their creations. Further, each of the existing templates can be tweaked, should players wish to change some of the values.

In the future, I am planning on adding more templates beyond the *standard*, each based on a biome found on Earth. These climates will be inspired by real-world data so they reproduce the correct proportions of rain, snow, wind, and storms.

Players can also [submit their templates for future inclusion][discussion-template-link] (no promises, though!).

<div align="right">

[[Back to top](#return-to-top)]

</div>

<!--Config Options-->
## Configuration <a id="config"></a>

*This section covers the available config options. **Default values are shown in bold**.*

*For other mods in the series, see the [mod documentation][main-doc-link].*

You can change these options in-game using [Generic Mod Config Menu][gmcm-link] or by manually editing the `config.json` in the mod's folder (generated after running SMAPI at least once).

### Weather Models:

The weather model determines the likelihood of weather changes for each day of the year (e.g. the chance of rain, snow, thunderstorms etc.). You can make your own custom model or use one of the provided templates.

| Name | Value | Description |
|:---|:---|:---|
| **Model Choice** | ***standard**, custom* | *Determines the choice of weather model.* |
| **Daily Odds** | ***true**, false* | *If *true*, interpolation will be used to estimate the daily weather odds.* |

In addition to selecting one of the model templates above, you can also manually edit the probabilities. If using [Generic Mod Config Menu][gmcm-link], then this can be done via the in-game menu, sorted by weather or by season. Otherwise, this can be done by manually editing the JSON files in the `models` folder.

<details><summary><i>Editing models</i></summary>
<p>

Each season is broken up into three time periods, covering days 1-9, days 10-19 and days 20-28. Each period may be assigned a unique probability for each type of weather. The probabilities must be decimal values between 0 and 100, and the allowed weather types are rain, storm, wind and snow. When no other weather occurs, Sunny weather happens by default.

When interpolation is enabled, the values for each time period are assigned to the middlemost day in that period. This corresponds to day 5, day 15 and day 24 respectively. Every other day in that period will then be given a value (determined by the interpolation) so that the overall change is smooth and gradual from one time period to the next. In contrast, when interpolation is disabled, every day in that time period will take on the same value (or put differently, the probability for that time period will be held fixed until that time period comes to an end).

The values for each time period can be edited with the [Generic Mod Config Menu][gmcm-link] or by manually editing the files in the `models` folder. If you are comfortable with data arrays, you can also see the real-time effect on the interpolation after you've changed the settings by looking at the files in the `data` folder.

</p>
</details>

<details><summary><i>Copying models</i></summary>
<p>

Using [Generic Mod Config Menu][gmcm-link], it is possible to copy the values from *model A* directly into *model B* by

1. Switching from *model A* to *model B*,
2. Opening the values page,
3. Clicking "Save", then clicking "Save & Close".

This can also be a handy way of creating a base for another model by using a pre-existing template.

</p>
</details>

<details><summary><i>Resetting models</i></summary>
<p>

Models can be reset at any time with the [Generic Mod Config Menu][gmcm-link]. You can also find and delete the files named after each model in the `models` folder. These files will be automatically generated with the default values the next time you launch SMAPI.

***NOTE:** Custom models must be deleted manually. By design, they are always preserved during a reset.*

</p>
</details>

### Debug Logging:

When debug logging is enabled, SMAPI will output the dice rolls and other useful information to the terminal.

<div align="right">

[[Back to top](#return-to-top)]

</div>

<!--Known Issues-->
## Known Issues <a id="issues"></a>

*This section contains updates on any known issues. For mod-specific issues, see the [mod documentation](#docs).*

- Due to the way the game currently handles thunderstorms, an existing storm might continue whenever it is supposed to rain tomorrow. The issue will resolve itself at the start of the next sunny/windy day. This will be patched in a coming update. 
- The TV will misreport windy weather as snow in Summer and Winter, because it is not expecting it in these seasons. A patch will be investigated for this.

<div align="right">

[[Back to top](#return-to-top)]

</div>

<!--Compatibility-->
## Mod Compatibility <a id="compatibility"></a>

*The following is a list of all supported, recommended and incompatible mods.*

<!--Supported-->
### Supported Mods <a id="supported"></a>

The following mods are officially supported by Immersive Weathers. When used in-game, you will see special functionality.

- **[Generic Mod Config Menu][gmcm-link]** - allows editing of the configuration in-game.
- **[Even Better RNG][even-better-rng-link]** - enables more accurate dice rolls for tomorrow's weather.

<!--Recommended-->
### Recommended Mods <a id="recommended"></a>

The following mods are entirely optional. You don't need these to enjoy Immersive Weathers but they may enhance your experience. I don't use them all myself, so please only install those which sound interesting to you.

- **[Thunder and Frog Sounds][thunder-frog-link]** (requires [Custom Music][custom-music-link]) - for a more relaxing, cozy thunderstorm ambience.
- **[Stardew Survival Project][survival-link]** - for those who enjoy survival gameplay and like the idea of micro-managing the farmer's body temperature.

<!--Incompatible-->
### Incompatible Mods <a id="incompatible"></a>

The following is a list of all mods which are not supported or are incompatible with *one or more* mods in the series. For mod-specific compatibility, see the [mod documentation](#docs). In general, any mod which alters the weather is going to cause unpredictable behaviour. You should be cautious when using these. However, [Content Patcher packs][content-patcher-link] should still be fine.

- **[More Rain][more-rain-link]** - *incompatible*. Alters the weather probabilities.
- **[Rain Plus][rain-plus-link]** - *incompatible*. Forces rain on certain days of the week.
- **[Winter Rain][winter-rain-link]** - *incompatible*. Changes the winter weather probabilities.
- **[Climates of Ferngill][climates-ferngill-link]** - *use with caution*. No problems yet, but this will break in the future.
- **[Weather Machine][weather-machine-link] / [Real Weather][real-weather-link]** - *incompatible*. Changes the way weather is calculated and adds new weather types. Use this if you prefer using live weather data rather than generated weather.
- **[Extreme Weather][extreme-weather-link]** - *incompatible*. Hilarious mod. But lol, why would you use these together?

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

For more substantial contributions, please fork the develop repo and submit a pull request using the https://github.com/ImaanBontle/SDV-immersive-weathers/labels/contribution label. You can also attempt to contact me via [NexusMods][nexus-profile] or by [opening an issue][issues-link].

Please be patient if I haven't responded immediately. I am likely busy with my studies.

<!--Bugs-->
### Bug Fixes/Reports <a id="bugs"></a>

If you encounter any bugs, please first remove any [incompatible mods](#incompatible) and re-run SMAPI to check if the issue resolves itself. If the bug persists or you do not see your mod included in the list, you can [submit a bug report][bugs-link]. Please answer the prompts to the best of your ability and mention any suspected mod conflicts. You should provide a link to your [SMAPI log][smapi-log] in the report.

If you would like submit a bugfix, you can do so by submitting a pull request using the https://github.com/ImaanBontle/SDV-immersive-weathers/labels/fix and https://github.com/ImaanBontle/SDV-immersive-weathers/labels/contribution labels.

<!--Feature Suggestions-->
### Suggestions <a id="suggestions"></a>

If you would like to suggest a feature for this project, please [submit a feature request][request-features-link]. While I can't guarantee these will be included in future releases, I would love to hear from you!

You will be credited for any suggestions that get implemented.

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

<!--Acknowledgements-->
## Special Thanks <a id="acknowledgements"></a>

I'd like to extend a **huge thanks** to [ConcernedApe][concernedape] for creating this masterpiece. Without your imagination, generosity and dedication, we wouldn't be here. You deserve all of your success and more. Thank you from the bottom of my heart.

I'd additionally like to thank [Pathoschild][pathoschild] for creating SMAPI and, through it, enabling the modding commmunity to thrive. I am very grateful to the modders who contributed to its success, who built this community, and who collectively curated the [amazing resources][stardew-modding-wiki] that taught me how to mod in C#.

I would like to thank the following individuals by name:

- [spacechase0][spacechase0] for creating the [Generic Mod Config Menu][gmcm-link] and its API.
- [Pepoluan][Pepoluan] for providing the API for [Even Better RNG][even-better-rng-link].

I would also like to thank my loving family and friends, for believing in my project and for your encouragement since its inception. Your love means the world to me.

To all the players who have downloaded and enjoyed my mods: Thank you. I am eternally grateful for your support. May you have many happy years in the Valley.

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

<!--Compatibility Links-->
[custom-music-link]: <https://www.nexusmods.com/stardewvalley/mods/3043?tab=files&BH=2> "Custom Music on NexusMods"
[content-patcher-link]: <https://www.nexusmods.com/stardewvalley/mods/1915> "Content Patcher on NexusMods"

<!--Supported Mods-->
[gmcm-link]: <https://www.nexusmods.com/stardewvalley/mods/5098> "Generic Mod Config Menu on NexusMods"
[even-better-rng-link]: <https://www.nexusmods.com/stardewvalley/mods/8680> "Even Better RNG on NexusMods"

<!--Recommended Mods-->
[thunder-frog-link]: <https://www.nexusmods.com/stardewvalley/mods/7450> "Thunder and Frog Sounds on NexusMods"
[survival-link]: <https://www.nexusmods.com/stardewvalley/mods/14183> "Stardew Survival Project on NexusMods"

<!--Incompatible Mods-->
[more-rain-link]: <https://www.nexusmods.com/stardewvalley/mods/441> "More Rain on NexusMods"
[rain-plus-link]: <https://www.nexusmods.com/stardewvalley/mods/441> "Rain Plus on NexusMods"
[winter-rain-link]: <https://www.nexusmods.com/stardewvalley/mods/13767> "Winter Rain on NexusMods"
[weather-machine-link]: <https://www.nexusmods.com/stardewvalley/mods/3203> "Weather Machine on NexusMods"
[real-weather-link]: <https://www.nexusmods.com/stardewvalley/mods/5773> "Real Weather on NexusMods"
[extreme-weather-link]: <https://www.nexusmods.com/stardewvalley/mods/12334> "Extreme Weather on NexusMods"
[climates-ferngill-link]: <https://www.moddrop.com/stardew-valley/mods/664033-climates-of-ferngill> "Climates of Ferngill on ModDrop"

<!--Translations-->
[framework-translation]: <https://github.com/ImaanBontle/SDV-immersive-weathers/tree/develop/i18n> "Framework i18n folder"
[climatecontrol-translation]: <https://github.com/ImaanBontle/SDV-IW-climate-control/tree/develop/i18n> "ClimateControl i18n folder"

<!--Contact Links-->
[nexus-profile]: <https://forums.nexusmods.com/index.php?showuser=54975162> "NexusMods Profile"
[ko-fi-link]: <https://ko-fi.com/msbontle> "Donate"

<!--Acknowledgements-->
[concernedape]: <https://twitter.com/ConcernedApe> "ConcernedApe on Twitter"
[pathoschild]: <https://www.nexusmods.com/stardewvalley/users/1552317> "Pathoschild on NexusMods"
[stardew-modding-wiki]: <https://stardewvalleywiki.com/Modding:Index> "Stardew Valley Wiki"
[spacechase0]: <https://www.nexusmods.com/stardewvalley/users/34250790> "spacechase0 on NexusMods"
[Pepoluan]: <https://www.nexusmods.com/stardewvalley/users/27024274> "Pepoluan on NexusMods"



# Climate Control



## Support for other mods

Unfortunately, I am not currently planning any integrations with Weathers of Ferngill.

While I am aware of the mod and it was part of my inspiration for making this mod, Weathers of Ferngill hasn't been properly updated in quite a while. It is also currently sitting in a very incomplete state. This makes it very difficult to provide long-term support while I add and expand on IW features, so it is highly unlikely an integration will ever occur. But who knows, maybe IW will one day implement similar features? It really depends on whether I have enough good ideas to justify doing this.

That said, if Weathers of Ferngill did start receiving regular updates again, I may reconsider my stance...

But let's not get ahead of ourselves. One step at a time...