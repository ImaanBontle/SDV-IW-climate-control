using Force.DeepCloner;
using IWClimateControl;
using ServiceStack.Text;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IW_ClimateControl
{
    /// <summary>
    /// The default user settings.
    /// </summary>
    public sealed class ModConfig : StandardModel
    {
        /// <summary>
        /// The choice of inherited weather model.
        /// </summary>
        /// <remarks>Defaults to the 'standard' model for generic climates.</remarks>
        public string ModelChoice { get; set; } = "standard";

        /// <summary>
        /// Generates the user configuration and inherits from the chosen model.
        /// </summary>
        public ModConfig()
        {
            LoadModel(this);
            ClimateControl.modelChoice = (IWAPI.WeatherModel)Enum.Parse(typeof(IWAPI.WeatherModel), this.ModelChoice);
        }

        public static void LoadModel(ModConfig Config)
        {
            // Copy across properties
            if (Config.ModelChoice == IWAPI.WeatherModel.standard.ToString())
            {
                // The standard model for generic climates.
                PropertyMatcher<StandardModel, ModConfig>.GenerateMatchedObject(ClimateControl.standardModel, Config);
                ClimateControl.standardModel = ClimateControl.standardModel.DeepClone();
            }
            else if (Config.ModelChoice == IWAPI.WeatherModel.custom.ToString())
            {
                // The custom model, with standard model as the base class.
                PropertyMatcher<StandardModel, ModConfig>.GenerateMatchedObject(ClimateControl.customModel, Config);
                ClimateControl.customModel = ClimateControl.customModel.DeepClone();
            }
        }

        public static void ResetModel(ModConfig Config, IModHelper Helper)
        {
            ClimateControl.eventLogger.SendToSMAPI("I was asked to reset all models", EventType.info);
            // Reset all models, except custom
            ClimateControl.standardModel = new();
            Helper.Data.WriteJsonFile("models/standard.json", ClimateControl.standardModel);
            //ClimateControl.customModel = new();
            //Helper.Data.WriteJsonFile("models/standard.json", ClimateControl.customModel);

            // Copy reset values into Config
            Config.ModelChoice = "standard";
            ClimateControl.modelChoice = (IWAPI.WeatherModel)Enum.Parse(typeof(IWAPI.WeatherModel), Config.ModelChoice);
            LoadModel(Config);
            Helper.WriteConfig(Config);
        }

        public static void ChangeModel(ModConfig Config, IModHelper Helper)
        {
            ClimateControl.eventLogger.SendToSMAPI("I was asked to refresh all models", EventType.info);
            // Refresh relevant models
            ClimateControl.eventLogger.SendToSMAPI($"Model was changed from {ClimateControl.modelChoice} to {Config.ModelChoice}. Changes will be applied to {ClimateControl.modelChoice}", EventType.info);
            // Save changes to old model.
            if (ClimateControl.modelChoice == IWAPI.WeatherModel.standard)
            {
                PropertyMatcher<ModConfig, StandardModel>.GenerateMatchedObject(Config, ClimateControl.standardModel);
                Helper.Data.WriteJsonFile("models/standard.json", ClimateControl.standardModel);
                ClimateControl.standardModel = Helper.Data.ReadJsonFile<StandardModel>("models/standard.json");
            }
            else if (ClimateControl.modelChoice == IWAPI.WeatherModel.custom)
            {
                PropertyMatcher<ModConfig, StandardModel>.GenerateMatchedObject(Config, ClimateControl.customModel);
                Helper.Data.WriteJsonFile("models/custom.json", ClimateControl.customModel);
                ClimateControl.customModel = Helper.Data.ReadJsonFile<StandardModel>("models/custom.json");
            }
            // Load new model.
            LoadModel(Config);
            Helper.WriteConfig(Config);
            ClimateControl.modelChoice = (IWAPI.WeatherModel)Enum.Parse(typeof(IWAPI.WeatherModel), Config.ModelChoice);
        }
    }

    /// <summary>
    /// Copies model properties from another class.
    /// </summary>
    /// <typeparam name="TParent">The class from which properties are inherited.</typeparam>
    /// <typeparam name="TChild">The class into which properties are copied.</typeparam>
    public class PropertyMatcher<TParent, TChild> where TParent : class
                                                  where TChild : class
    {
        /// <summary>
        /// Matches properties between two objects.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        /// <param name="child">The child object.</param>
        public static void GenerateMatchedObject(TParent parent, TChild child)
        {
            var childProperties = child.GetType().GetProperties();
            var parentProperties = parent.GetType().GetProperties();
            foreach (var childProperty in childProperties)
            {
                foreach (var parentProperty in parentProperties)
                {
                    if ((parentProperty.Name == childProperty.Name) && (parentProperty.PropertyType == childProperty.PropertyType))
                    {
                        childProperty.SetValue(child, parentProperty.GetValue(parent));
                    }
                }
            }
        }
    }
}
