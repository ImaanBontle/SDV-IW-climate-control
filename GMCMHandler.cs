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
        internal static void Register(IManifest ModManifest, IModHelper Helper)
        {
            // Register mod
            ClimateControl.s_gMCM.Register(
                mod: ModManifest,
                reset: () => ModConfig.ResetModel(ClimateControl.s_config, Helper),
                save: () => ModConfig.ChangeModel(ClimateControl.s_config, Helper)
            );

            // ---------
            // MAIN PAGE
            // ---------
            // Add Weather Model title
            ClimateControl.s_gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Weather Models",
                tooltip: () => "The weather model determines the likelihood of weather changes for each day of the year (e.g. the chance of rain, snow, thunderstorms etc.). You can make your own custom model or use one of the provided templates."
            );

            // Add model choices
            ClimateControl.s_gMCM.AddTextOption(
                mod: ModManifest,
                getValue: () => ClimateControl.s_config.ModelChoice,
                setValue: value => ClimateControl.s_config.ModelChoice = value,
                name: () => "Model Choice:",
                allowedValues: new string[] { "standard", "custom" },
                tooltip: () => "The 'custom' model is always preserved when resetting to \"Default\". If you want to reset this too, you can either delete the 'custom.json' file in your mod folder and relaunch the game, or, after clicking \"Default\", switch from 'standard' to 'custom', open the values page, then click \"Save\" followed by \"Save & Close\". This will copy the standard model across to the custom model. You can also use this to copy between models if you like."
            );

            // Add model interpolation
            ClimateControl.s_gMCM.AddBoolOption(
                mod: ModManifest,
                getValue: () => ClimateControl.s_config.EnableInterpolation,
                setValue: value => ClimateControl.s_config.EnableInterpolation = value,
                name: () => "Enable Daily Odds:",
                tooltip: () => "If enabled, this mod will try to guess the daily odds of each weather type by using 'cubic spline interpolation'. This fits a smooth line through the probabilities, resulting in gradual changes during the year and a more immersive experience (e.g. you might see increasing chances of snow towards the end of Fall). For a simpler approach, disable this and the mod will treat the config values as fixed for each date range. This is closer to how Stardew Valley treats the weather but might result in more abrupt changes."
            );

            // Add Probabilities title
            ClimateControl.s_gMCM.AddSectionTitle(
                mod: ModManifest,
                text: () => "Weather Probabilities",
                tooltip: () => "These values determine the likelihood of the weather changing to different types, either by using interpolation to guess the daily odds or by using the values as fixed probabilities for each date range."
            );

            // Add description
            ClimateControl.s_gMCM.AddParagraph(
                mod: ModManifest,
                text: () => "Each morning, the mod will choose a weather for tomorrow based on the probabilities below. You can either use these models exactly as provided, customize their values separately, or create your own custom model.\n\nNOTE: When editing these values, you must FIRST (!) select the model you want to edit and then \"Save\" the config BEFORE making any changes. Otherwise, changes will apply to the previous model."
            );

            // Add season page
            ClimateControl.s_gMCM.AddPageLink(
                mod: ModManifest,
                pageId: "IWConfig-EditSeasons",
                text: () => "Edit values (by season)",
                tooltip: () => "Change the weather probabilities, grouped by season."
            );

            // Add type page
            ClimateControl.s_gMCM.AddPageLink(
                mod: ModManifest,
                pageId: "IWConfig-EditTypes",
                text: () => "Edit values (by type)",
                tooltip: () => "Change the weather probabilities, grouped by weather type."
            );

            // ---------------
            // SEASONAL VALUES
            // ---------------
            {
                // Add probabilities page (by season)
                ClimateControl.s_gMCM.AddPage(
                    mod: ModManifest,
                    pageId: "IWConfig-EditSeasons",
                    pageTitle: () => "Probabilities, By Season"
                );

                // ****Spring*****
                {
                    ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Spring"
                    );

                    // Rain
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Rainfall:",
                        tooltip: () => "The chance that it will rain tomorrow."
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Rain.Early,
                        setValue: value => ClimateControl.s_config.Spring.Rain.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Rain.Mid,
                        setValue: value => ClimateControl.s_config.Spring.Rain.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Rain.Late,
                        setValue: value => ClimateControl.s_config.Spring.Rain.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Storms
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Thunderstorms:",
                        tooltip: () => "The chance that it will storm tomorrow."
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Storm.Early,
                        setValue: value => ClimateControl.s_config.Spring.Storm.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Storm.Mid,
                        setValue: value => ClimateControl.s_config.Spring.Storm.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Storm.Late,
                        setValue: value => ClimateControl.s_config.Spring.Storm.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Wind
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Windy Weather:",
                        tooltip: () => "The chance that it will be windy tomorrow."
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Wind.Early,
                        setValue: value => ClimateControl.s_config.Spring.Wind.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Wind.Mid,
                        setValue: value => ClimateControl.s_config.Spring.Wind.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Wind.Late,
                        setValue: value => ClimateControl.s_config.Spring.Wind.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Snow
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Snowfall:",
                        tooltip: () => "The chance that it will snow tomorrow."
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Snow.Early,
                        setValue: value => ClimateControl.s_config.Spring.Snow.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Snow.Mid,
                        setValue: value => ClimateControl.s_config.Spring.Snow.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Snow.Late,
                        setValue: value => ClimateControl.s_config.Spring.Snow.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );
                }

                // ****Summer*****
                {
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Summer"
                    );

                    // Rain
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Rainfall:",
                        tooltip: () => "The chance that it will rain tomorrow."
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Rain.Early,
                        setValue: value => ClimateControl.s_config.Summer.Rain.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Rain.Mid,
                        setValue: value => ClimateControl.s_config.Summer.Rain.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Rain.Late,
                        setValue: value => ClimateControl.s_config.Summer.Rain.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Storms
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Thunderstorms:",
                        tooltip: () => "The chance that it will storm tomorrow."
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Storm.Early,
                        setValue: value => ClimateControl.s_config.Summer.Storm.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Storm.Mid,
                        setValue: value => ClimateControl.s_config.Summer.Storm.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Storm.Late,
                        setValue: value => ClimateControl.s_config.Summer.Storm.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Wind
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Windy Weather:",
                        tooltip: () => "The chance that it will be windy tomorrow."
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Wind.Early,
                        setValue: value => ClimateControl.s_config.Summer.Wind.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Wind.Mid,
                        setValue: value => ClimateControl.s_config.Summer.Wind.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Wind.Late,
                        setValue: value => ClimateControl.s_config.Summer.Wind.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Snow
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Snowfall:",
                        tooltip: () => "The chance that it will snow tomorrow."
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Snow.Early,
                        setValue: value => ClimateControl.s_config.Summer.Snow.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Snow.Mid,
                        setValue: value => ClimateControl.s_config.Summer.Snow.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Snow.Late,
                        setValue: value => ClimateControl.s_config.Summer.Snow.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );
                }

                // ****Fall*****
                {
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Fall"
                    );

                    // Rain
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Rainfall:",
                        tooltip: () => "The chance that it will rain tomorrow."
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Rain.Early,
                        setValue: value => ClimateControl.s_config.Fall.Rain.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Rain.Mid,
                        setValue: value => ClimateControl.s_config.Fall.Rain.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Rain.Late,
                        setValue: value => ClimateControl.s_config.Fall.Rain.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Storms
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Thunderstorms:",
                        tooltip: () => "The chance that it will storm tomorrow."
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Storm.Early,
                        setValue: value => ClimateControl.s_config.Fall.Storm.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Storm.Mid,
                        setValue: value => ClimateControl.s_config.Fall.Storm.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Storm.Late,
                        setValue: value => ClimateControl.s_config.Fall.Storm.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Wind
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Windy Weather:",
                        tooltip: () => "The chance that it will be windy tomorrow."
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Wind.Early,
                        setValue: value => ClimateControl.s_config.Fall.Wind.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Wind.Mid,
                        setValue: value => ClimateControl.s_config.Fall.Wind.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Wind.Late,
                        setValue: value => ClimateControl.s_config.Fall.Wind.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Snow
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Snowfall:",
                        tooltip: () => "The chance that it will snow tomorrow."
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Snow.Early,
                        setValue: value => ClimateControl.s_config.Fall.Snow.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Snow.Mid,
                        setValue: value => ClimateControl.s_config.Fall.Snow.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Snow.Late,
                        setValue: value => ClimateControl.s_config.Fall.Snow.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );
                }

                // ****Winter*****
                {
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Winter"
                    );

                    // Rain
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Rainfall:",
                        tooltip: () => "The chance that it will rain tomorrow."
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Rain.Early,
                        setValue: value => ClimateControl.s_config.Winter.Rain.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Rain.Mid,
                        setValue: value => ClimateControl.s_config.Winter.Rain.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Rain.Late,
                        setValue: value => ClimateControl.s_config.Winter.Rain.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Storms
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Thunderstorms:",
                        tooltip: () => "The chance that it will storm tomorrow."
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Storm.Early,
                        setValue: value => ClimateControl.s_config.Winter.Storm.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Storm.Mid,
                        setValue: value => ClimateControl.s_config.Winter.Storm.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Storm.Late,
                        setValue: value => ClimateControl.s_config.Winter.Storm.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Wind
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Windy Weather:",
                        tooltip: () => "The chance that it will be windy tomorrow."
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Wind.Early,
                        setValue: value => ClimateControl.s_config.Winter.Wind.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Wind.Mid,
                        setValue: value => ClimateControl.s_config.Winter.Wind.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Wind.Late,
                        setValue: value => ClimateControl.s_config.Winter.Wind.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Snow
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Snowfall:",
                        tooltip: () => "The chance that it will snow tomorrow."
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Snow.Early,
                        setValue: value => ClimateControl.s_config.Winter.Snow.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Snow.Mid,
                        setValue: value => ClimateControl.s_config.Winter.Snow.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Snow.Late,
                        setValue: value => ClimateControl.s_config.Winter.Snow.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );
                }
            }

            // --------------
            // WEATHER VALUES
            // --------------
            {
                // Add probabilities page (by weather type)
               ClimateControl.s_gMCM.AddPage(
                    mod: ModManifest,
                    pageId: "IWConfig-EditTypes",
                    pageTitle: () => "Probabilities, By Type"
                );

                // ****Rainfall*****
                {
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Rainfall",
                        tooltip: () => "The chance that it will rain tomorrow."
                    );

                    // Spring
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Spring:"
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Rain.Early,
                        setValue: value => ClimateControl.s_config.Spring.Rain.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Rain.Mid,
                        setValue: value => ClimateControl.s_config.Spring.Rain.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Rain.Late,
                        setValue: value => ClimateControl.s_config.Spring.Rain.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Summer
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Summer:"
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Rain.Early,
                        setValue: value => ClimateControl.s_config.Summer.Rain.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Rain.Mid,
                        setValue: value => ClimateControl.s_config.Summer.Rain.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Rain.Late,
                        setValue: value => ClimateControl.s_config.Summer.Rain.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Fall
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Fall:"
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Rain.Early,
                        setValue: value => ClimateControl.s_config.Fall.Rain.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Rain.Mid,
                        setValue: value => ClimateControl.s_config.Fall.Rain.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Rain.Late,
                        setValue: value => ClimateControl.s_config.Fall.Rain.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Winter
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Winter:"
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Rain.Early,
                        setValue: value => ClimateControl.s_config.Winter.Rain.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Rain.Mid,
                        setValue: value => ClimateControl.s_config.Winter.Rain.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Rain.Late,
                        setValue: value => ClimateControl.s_config.Winter.Rain.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );
                }

                // ****Thunderstorms*****
                {
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Thunderstorms",
                        tooltip: () => "The chance that it will storm tomorrow."
                    );

                    // Spring
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Spring:"
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Storm.Early,
                        setValue: value => ClimateControl.s_config.Spring.Storm.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Storm.Mid,
                        setValue: value => ClimateControl.s_config.Spring.Storm.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Storm.Late,
                        setValue: value => ClimateControl.s_config.Spring.Storm.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Summer
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Summer:"
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Storm.Early,
                        setValue: value => ClimateControl.s_config.Summer.Storm.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Storm.Mid,
                        setValue: value => ClimateControl.s_config.Summer.Storm.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Storm.Late,
                        setValue: value => ClimateControl.s_config.Summer.Storm.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Fall
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Fall:"
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Storm.Early,
                        setValue: value => ClimateControl.s_config.Fall.Storm.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Storm.Mid,
                        setValue: value => ClimateControl.s_config.Fall.Storm.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Storm.Late,
                        setValue: value => ClimateControl.s_config.Fall.Storm.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Winter
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Winter:"
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Storm.Early,
                        setValue: value => ClimateControl.s_config.Winter.Storm.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Storm.Mid,
                        setValue: value => ClimateControl.s_config.Winter.Storm.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Storm.Late,
                        setValue: value => ClimateControl.s_config.Winter.Storm.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );
                }

                // ****Wind*****
                {
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Windy Weather:",
                        tooltip: () => "The chance that it will be windy tomorrow."
                    );

                    // Spring
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Spring:"
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Wind.Early,
                        setValue: value => ClimateControl.s_config.Spring.Wind.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Wind.Mid,
                        setValue: value => ClimateControl.s_config.Spring.Wind.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Wind.Late,
                        setValue: value => ClimateControl.s_config.Spring.Wind.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Summer
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Summer:"
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Wind.Early,
                        setValue: value => ClimateControl.s_config.Summer.Wind.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Wind.Mid,
                        setValue: value => ClimateControl.s_config.Summer.Wind.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Wind.Late,
                        setValue: value => ClimateControl.s_config.Summer.Wind.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Fall
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Fall:"
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Wind.Early,
                        setValue: value => ClimateControl.s_config.Fall.Wind.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Wind.Mid,
                        setValue: value => ClimateControl.s_config.Fall.Wind.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Wind.Late,
                        setValue: value => ClimateControl.s_config.Fall.Wind.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Winter
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Winter:"
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Wind.Early,
                        setValue: value => ClimateControl.s_config.Winter.Wind.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Wind.Mid,
                        setValue: value => ClimateControl.s_config.Winter.Wind.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Wind.Late,
                        setValue: value => ClimateControl.s_config.Winter.Wind.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );
                }

                // ****Snow*****
                {
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Snowfall",
                        tooltip: () => "The chance that it will snow tomorrow."
                    );

                    // Snow
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Spring:"
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Snow.Early,
                        setValue: value => ClimateControl.s_config.Spring.Snow.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Snow.Mid,
                        setValue: value => ClimateControl.s_config.Spring.Snow.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Spring.Snow.Late,
                        setValue: value => ClimateControl.s_config.Spring.Snow.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Summer
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Summer:"
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Snow.Early,
                        setValue: value => ClimateControl.s_config.Summer.Snow.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Snow.Mid,
                        setValue: value => ClimateControl.s_config.Summer.Snow.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Summer.Snow.Late,
                        setValue: value => ClimateControl.s_config.Summer.Snow.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Fall
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Fall:"
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Snow.Early,
                        setValue: value => ClimateControl.s_config.Fall.Snow.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Snow.Mid,
                        setValue: value => ClimateControl.s_config.Fall.Snow.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Fall.Snow.Late,
                        setValue: value => ClimateControl.s_config.Fall.Snow.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                    // Winter
                   ClimateControl.s_gMCM.AddSectionTitle(
                        mod: ModManifest,
                        text: () => "Winter:"
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Snow.Early,
                        setValue: value => ClimateControl.s_config.Winter.Snow.Early = (double)value,
                        name: () => "Early (Days 1-9)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Snow.Mid,
                        setValue: value => ClimateControl.s_config.Winter.Snow.Mid = (double)value,
                        name: () => "Mid (Days 10-19)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );

                   ClimateControl.s_gMCM.AddNumberOption(
                        mod: ModManifest,
                        getValue: () => (float)ClimateControl.s_config.Winter.Snow.Late,
                        setValue: value => ClimateControl.s_config.Winter.Snow.Late = (double)value,
                        name: () => "Late (Days 20-28)",
                        min: 0.0f,
                        max: 100.0f,
                        interval: 0.1f
                    );
                }
            }
        }
    }
}
