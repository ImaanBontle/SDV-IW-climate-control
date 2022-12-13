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
                tooltip: () => "The weather models determine the likelihood of weather on each day (e.g. rain, snow, thunderstorm etc.)."
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
                text: () => "Each morning, the mod will choose a weather for tomorrow based on the probabilities below. You can create your own custom model or you can use the templates provided (currently includes only the standard model, others will be added in the future.). \n\nNOTE: You will need to sleep or reload before changes will have an effect."
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
