using GenericModConfigMenu;
using IWClimateControl;
using ServiceStack.Text;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW_ClimateControl
{
    internal class GMCMHelper
    {
        internal static void Register(ModConfig Config, IGenericModConfigMenuApi gMCM, StardewModdingAPI.IManifest ModManifest, IModHelper Helper)
        {
            // Register mod
            gMCM.Register(
                mod: ModManifest,
                reset: () => ModConfig.ResetModel(Config, Helper),
                save: () => ModConfig.ChangeModel(Config, Helper)
            );

            // Add section title
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Weather Models",
                tooltip: () => "The weather model determines the likelihood of weather changes for each day of the year (e.g. the chance of rain, snow, thunderstorms etc.). You can make your own custom model or use one of the provided templates."
            );

            // Add model choices
            gMCM.AddTextOption(
                mod: ModManifest,
                getValue: () => Config.ModelChoice,
                setValue: value => Config.ModelChoice = value,
                name: () => "Model Choice:",
                allowedValues: new string[] { "standard", "custom" }
            );

            // Add description
            gMCM.AddParagraph(
                mod: ModManifest,
                text: () => "Each morning, the mod will choose a weather for tomorrow based on the probabilities below. You can either use these models exactly as provided, customize their values separately, or create your own custom model.\n\nNOTE: When editing these values, you must FIRST (!) select the model you want to edit and then \"Save & Close\" the config BEFORE making any changes. Otherwise, changes will apply to the previous model.\n\n(The 'custom' model is always preserved when resetting to \"Default\". If you want to reset this too, you can either delete the 'custom.json' file in your mod folder and relaunch the game, or, after clicking \"Default\", switch from 'standard' to 'custom' and then \"Save\" twice and close the config. This will copy the standard model across to the custom model.)"
            );

            // Add seasons
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Spring",
                tooltip: () => "Change the Spring probabilities"
            );

            // Add rain section
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Rain",
                tooltip: () => "The chance that it will rain tomorrow."
            );

            // Add probabilities
            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float) Config.Spring.Rain.Early,
                setValue: value => Config.Spring.Rain.Early = (double)value,
                name: () => "Early",
                tooltip: () => "Days 1-9",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );
        }
    }
}
