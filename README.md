**Daily Weather:**

The mod achieves this behaviour by taking a set of player-defined weather probabilities for the start, middle and end of each season. It then performs cubic spline interpolation for all the days in-between those time periods, essentially producing a "guess" for how easily the weather can change based on the numbers given.

There are two major advantage to this approach: 1) No two days have the same odds for all weather types, so that each day 'feels' slightly different 2) Weather can also seem to 'bleed over' from one period to the next, resulting in cool effects like witnessing snow at the end of Fall, similar to real-life!

If you would prefer a simpler weather approach, interpolation can also be disabled and the mod will treat the config values as fixed probabilities for each 1/3rd of its season. This is less realistic than using the interpolation but will still produce more varied weather than in the base game.

**Climate Templates:**

By default, the mod uses a pre-defined 'standard climate'. This is the climate most similar to vanilla, featuring gentle rain showers in Spring, brief thunderstorms in early Summer, dry, windy weather in Fall, and continous snowfall in Winter. However, when combined with cubic spline interpolation, this produces a smoother weather profile and more gradual season changes.

In the settings menu, you can define your own custom template or tweak the existing one. In future updates, you can expect more templates themed around various real-world biomes, each allowing for similar customisation.

#### Weather Models:

The weather model determines the likelihood of weather changes for each day of the year (e.g. the chance of rain, snow, thunderstorms etc.). You can make your own custom model or use one of the provided templates.

| Name | Value | Description |
|:---|:---|:---|
| **Model Choice** | ***standard**, custom* | *Determines the choice of weather model.* |
| **Daily Odds** | ***true**, false* | *If *true*, interpolation will be used to estimate the daily weather odds.* |

- Due to the way the game currently handles thunderstorms, an existing storm might continue whenever it is supposed to rain tomorrow. The issue will resolve itself at the start of the next sunny/windy day. This will be patched in a coming update. 
- The TV will misreport windy weather as snow in Summer and Winter, because it is not expecting it in these seasons. A patch will be investigated for this.

<details><summary>Editing weather models</summary>
<p>

In addition to selecting one of the model templates above, you can also manually edit the probabilities. If using [Generic Mod Config Menu][gmcm-link], then this can be done via the in-game menu, sorted by weather or by season. Otherwise, this can be done by manually editing the JSON files in the `models` folder.

You may assign any decimal value between 0 and 100 to days 1-9, 10-19 and 20-28 within each season for each type of weather (rain, storm, snow, wind). If interpolation is enabled, these numbers will be held fixed for days 5, 15 and 24 respectively. Otherwise, they will be treated as fixed for the entirety of each time period.

***For the curious:** You can see the effects of changing the settings by looking at the daily weather probabilities in the `data` folder. These will update in real-time when using [Generic Mod Config Menu][gmcm-link].*

</p>
</details>

<details><summary>Resetting custom models</summary>
<p>

Custom models are preserved when resetting with [Generic Mod Config Menu][gmcm-link]. If you want to reset any changes, you must delete `models/custom.json`. Alternatively, you can copy the *standard* values into the *custom* model by

1. Switching from *standard* to *custom*
2. Opening the values page
3. Clicking "Save" followed by "Save & Close"

</p>
</details>

#### Debug Logging:

When debug logging is enabled, SMAPI will output the dice rolls and other useful information to the terminal.

# Climate Control

This mod allows you to influence the weather throughout the year by customising the probabilities of each type of weather found in the game (rain, thunderstorms, snow, wind, sun), depending on the current season and the time of year.

This mod is part of a larger framework called ImmersiveWeathers (IW), which aims to create immersive experiences for players in Stardew Valley. Climate Control (CC) is the first mod in this framework and there are plans to expand this framework to a set of sister mods which each influence the weather/environment/ambience in different ways.

*NB: Right now, Climate Control is in very early (but active!) development. Currently, I am working on a rudimentary control of the weather probabilities before implementing more detailed features. Please be patient as I gradually flesh out the mod.*

## Feature plans

In the future, I am planning to add multiple different templates you can choose from. Each template will simulate a biome normally found on Earth (or at least, those that would make sense within the Valley!) and the different proportions of rain, snow, wind, thunderstorms found throughout the year. The richness of these templates will grow as features are added to this mod and as sister mods are integrated. The player can adjust these values manually if they so desire, but the templates provide a pre-built, hand-customised configuration which you can use as a starting point..

Over time, Climate Control's' weather system will also be expanded to become more complex, allowing for gradually-varying probabilities each day/week (to simulate the gradual passing of the seasons). For example, this might mean having more rainfall at the start of Summer, before it tapers off as Fall approaches. Or as Winter approaches, you might experience a higher likelihood of snow. Not only would this be customised for each biome, but the player will also be able to customise the behaviour of this variation (although, I haven't yet figured out how to make it completely intuitive!).

Ideally, this would make the climates feel more realistic and immersive and less 'modular' than they do right now. Especially when this is combined with planned features like random fluctuations (stochasticity) in the weather probabilities or slightly (!) inaccurate weather forecasts.

## Support for other mods

Unfortunately, I am not currently planning any integrations with Weathers of Ferngill.

While I am aware of the mod and it was part of my inspiration for making this mod, Weathers of Ferngill hasn't been properly updated in quite a while. It is also currently sitting in a very incomplete state. This makes it very difficult to provide long-term support while I add and expand on IW features, so it is highly unlikely an integration will ever occur. But who knows, maybe IW will one day implement similar features? It really depends on whether I have enough good ideas to justify doing this.

That said, if Weathers of Ferngill did start receiving regular updates again, I may reconsider my stance...

But let's not get ahead of ourselves. One step at a time...