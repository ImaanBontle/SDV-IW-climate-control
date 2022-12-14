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

            // ---------
            // MAIN PAGE
            // ---------
            // Add Weather Model title
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
                text: () => "Each morning, the mod will choose a weather for tomorrow based on the probabilities below. You can either use these models exactly as provided, customize their values separately, or create your own custom model.\n\nNOTE: When editing these values, you must FIRST (!) select the model you want to edit and then \"Save\" the config BEFORE making any changes. Otherwise, changes will apply to the previous model.\n\n(The 'custom' model is always preserved when resetting to \"Default\". If you want to reset this too, you can either delete the 'custom.json' file in your mod folder and relaunch the game, or, after clicking \"Default\", switch from 'standard' to 'custom', open the values page, then click \"Save\" followed by \"Save & Close\". This will copy the standard model across to the custom model. You can also use this to copy between models if you like.)"
            );

            // Add season pages
            gMCM.AddPageLink(
                mod: ModManifest,
                pageId: "IWConfig-EditSeasons",
                text: () => "Edit values (by season)",
                tooltip: () => "Change the weather probabilities for this model, grouped by season."
            );

            // Add probabilities page (by season)
            gMCM.AddPage(
                mod: ModManifest,
                pageId: "IWConfig-EditSeasons",
                pageTitle: () => "Probabilities, By Season"
            );

            // ---------------
            // SEASONAL VALUES
            // ---------------
            // ****Spring*****
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Spring"
            );

            // Rain
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Rainfall:",
                tooltip: () => "The chance that it will rain tomorrow."
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float) Config.Spring.Rain.Early,
                setValue: value => Config.Spring.Rain.Early = (double)value,
                name: () => "Early (Days 1-9)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Spring.Rain.Mid,
                setValue: value => Config.Spring.Rain.Mid = (double)value,
                name: () => "Mid (Days 10-19)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Spring.Rain.Late,
                setValue: value => Config.Spring.Rain.Late = (double)value,
                name: () => "Late (Days 20-28)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            // Storms
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Thunderstorms:",
                tooltip: () => "The chance that it will storm tomorrow."
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Spring.Storm.Early,
                setValue: value => Config.Spring.Storm.Early = (double)value,
                name: () => "Early (Days 1-9)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Spring.Storm.Mid,
                setValue: value => Config.Spring.Storm.Mid = (double)value,
                name: () => "Mid (Days 10-19)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Spring.Storm.Late,
                setValue: value => Config.Spring.Storm.Late = (double)value,
                name: () => "Late (Days 20-28)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            // Wind
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Windy Weather:",
                tooltip: () => "The chance that it will be windy tomorrow."
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Spring.Wind.Early,
                setValue: value => Config.Spring.Wind.Early = (double)value,
                name: () => "Early (Days 1-9)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Spring.Wind.Mid,
                setValue: value => Config.Spring.Wind.Mid = (double)value,
                name: () => "Mid (Days 10-19)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Spring.Wind.Late,
                setValue: value => Config.Spring.Wind.Late = (double)value,
                name: () => "Late (Days 20-28)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            // Snow
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Snowfall:",
                tooltip: () => "The chance that it will snow tomorrow."
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Spring.Snow.Early,
                setValue: value => Config.Spring.Snow.Early = (double)value,
                name: () => "Early (Days 1-9)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Spring.Snow.Mid,
                setValue: value => Config.Spring.Snow.Mid = (double)value,
                name: () => "Mid (Days 10-19)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Spring.Snow.Late,
                setValue: value => Config.Spring.Snow.Late = (double)value,
                name: () => "Late (Days 20-28)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            // ****Summer*****
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Summer"
            );

            // Rain
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Rainfall:",
                tooltip: () => "The chance that it will rain tomorrow."
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Summer.Rain.Early,
                setValue: value => Config.Summer.Rain.Early = (double)value,
                name: () => "Early (Days 1-9)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Summer.Rain.Mid,
                setValue: value => Config.Summer.Rain.Mid = (double)value,
                name: () => "Mid (Days 10-19)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Summer.Rain.Late,
                setValue: value => Config.Summer.Rain.Late = (double)value,
                name: () => "Late (Days 20-28)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            // Storms
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Thunderstorms:",
                tooltip: () => "The chance that it will storm tomorrow."
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Summer.Storm.Early,
                setValue: value => Config.Summer.Storm.Early = (double)value,
                name: () => "Early (Days 1-9)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Summer.Storm.Mid,
                setValue: value => Config.Summer.Storm.Mid = (double)value,
                name: () => "Mid (Days 10-19)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Summer.Storm.Late,
                setValue: value => Config.Summer.Storm.Late = (double)value,
                name: () => "Late (Days 20-28)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            // Wind
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Windy Weather:",
                tooltip: () => "The chance that it will be windy tomorrow."
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Summer.Wind.Early,
                setValue: value => Config.Summer.Wind.Early = (double)value,
                name: () => "Early (Days 1-9)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Summer.Wind.Mid,
                setValue: value => Config.Summer.Wind.Mid = (double)value,
                name: () => "Mid (Days 10-19)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Summer.Wind.Late,
                setValue: value => Config.Summer.Wind.Late = (double)value,
                name: () => "Late (Days 20-28)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            // Snow
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Snowfall:",
                tooltip: () => "The chance that it will snow tomorrow."
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Summer.Snow.Early,
                setValue: value => Config.Summer.Snow.Early = (double)value,
                name: () => "Early (Days 1-9)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Summer.Snow.Mid,
                setValue: value => Config.Summer.Snow.Mid = (double)value,
                name: () => "Mid (Days 10-19)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Summer.Snow.Late,
                setValue: value => Config.Summer.Snow.Late = (double)value,
                name: () => "Late (Days 20-28)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            // ****Fall*****
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Fall"
            );

            // Rain
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Rainfall:",
                tooltip: () => "The chance that it will rain tomorrow."
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Fall.Rain.Early,
                setValue: value => Config.Fall.Rain.Early = (double)value,
                name: () => "Early (Days 1-9)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Fall.Rain.Mid,
                setValue: value => Config.Fall.Rain.Mid = (double)value,
                name: () => "Mid (Days 10-19)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Fall.Rain.Late,
                setValue: value => Config.Fall.Rain.Late = (double)value,
                name: () => "Late (Days 20-28)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            // Storms
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Thunderstorms:",
                tooltip: () => "The chance that it will storm tomorrow."
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Fall.Storm.Early,
                setValue: value => Config.Fall.Storm.Early = (double)value,
                name: () => "Early (Days 1-9)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Fall.Storm.Mid,
                setValue: value => Config.Fall.Storm.Mid = (double)value,
                name: () => "Mid (Days 10-19)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Fall.Storm.Late,
                setValue: value => Config.Fall.Storm.Late = (double)value,
                name: () => "Late (Days 20-28)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            // Wind
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Windy Weather:",
                tooltip: () => "The chance that it will be windy tomorrow."
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Fall.Wind.Early,
                setValue: value => Config.Fall.Wind.Early = (double)value,
                name: () => "Early (Days 1-9)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Fall.Wind.Mid,
                setValue: value => Config.Fall.Wind.Mid = (double)value,
                name: () => "Mid (Days 10-19)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Fall.Wind.Late,
                setValue: value => Config.Fall.Wind.Late = (double)value,
                name: () => "Late (Days 20-28)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            // Snow
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Snowfall:",
                tooltip: () => "The chance that it will snow tomorrow."
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Fall.Snow.Early,
                setValue: value => Config.Fall.Snow.Early = (double)value,
                name: () => "Early (Days 1-9)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Fall.Snow.Mid,
                setValue: value => Config.Fall.Snow.Mid = (double)value,
                name: () => "Mid (Days 10-19)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Fall.Snow.Late,
                setValue: value => Config.Fall.Snow.Late = (double)value,
                name: () => "Late (Days 20-28)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            // ****Winter*****
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Winter"
            );

            // Rain
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Rainfall:",
                tooltip: () => "The chance that it will rain tomorrow."
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Winter.Rain.Early,
                setValue: value => Config.Winter.Rain.Early = (double)value,
                name: () => "Early (Days 1-9)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Winter.Rain.Mid,
                setValue: value => Config.Winter.Rain.Mid = (double)value,
                name: () => "Mid (Days 10-19)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Winter.Rain.Late,
                setValue: value => Config.Winter.Rain.Late = (double)value,
                name: () => "Late (Days 20-28)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            // Storms
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Thunderstorms:",
                tooltip: () => "The chance that it will storm tomorrow."
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Winter.Storm.Early,
                setValue: value => Config.Winter.Storm.Early = (double)value,
                name: () => "Early (Days 1-9)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Winter.Storm.Mid,
                setValue: value => Config.Winter.Storm.Mid = (double)value,
                name: () => "Mid (Days 10-19)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Winter.Storm.Late,
                setValue: value => Config.Winter.Storm.Late = (double)value,
                name: () => "Late (Days 20-28)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            // Wind
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Windy Weather:",
                tooltip: () => "The chance that it will be windy tomorrow."
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Winter.Wind.Early,
                setValue: value => Config.Winter.Wind.Early = (double)value,
                name: () => "Early (Days 1-9)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Winter.Wind.Mid,
                setValue: value => Config.Winter.Wind.Mid = (double)value,
                name: () => "Mid (Days 10-19)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Winter.Wind.Late,
                setValue: value => Config.Winter.Wind.Late = (double)value,
                name: () => "Late (Days 20-28)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            // Snow
            gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Snowfall:",
                tooltip: () => "The chance that it will snow tomorrow."
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Winter.Snow.Early,
                setValue: value => Config.Winter.Snow.Early = (double)value,
                name: () => "Early (Days 1-9)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Winter.Snow.Mid,
                setValue: value => Config.Winter.Snow.Mid = (double)value,
                name: () => "Mid (Days 10-19)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );

            gMCM.AddNumberOption(
                mod: ModManifest,
                getValue: () => (float)Config.Winter.Snow.Late,
                setValue: value => Config.Winter.Snow.Late = (double)value,
                name: () => "Late (Days 20-28)",
                min: 0.0f,
                max: 100.0f,
                interval: 0.1f
            );
        }
    }
}
