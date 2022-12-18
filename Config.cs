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
        public string ModelChoice { get; set; }

        /// <summary>
        /// Whether to enable interpolation of probabilities onto a daily grid.
        /// </summary>
        /// <remarks>Defaults to <see langword="true"/>.</remarks>
        public bool EnableInterpolation { get; set; }

        /// <summary>
        /// Whether debug logs should be sent to the terminal.
        /// </summary>
        public bool EnableDebugLogging { get; set; }

        /// <summary>
        /// Generates the user configuration and inherits from the chosen model.
        /// </summary>
        public ModConfig()
        {
            ModelChoice = "standard";
            EnableInterpolation = true;
            EnableDebugLogging = false;
            ClimateControl.s_modelChoice = (IIWAPI.WeatherModel)Enum.Parse(typeof(IIWAPI.WeatherModel), this.ModelChoice);
            LoadModel(this);
        }

        public static void LoadModel(ModConfig Config)
        {
            // Copy across properties
            if (Config.ModelChoice == IIWAPI.WeatherModel.standard.ToString())
            {
                // The standard model for generic climates.
                PropertyMatcher<StandardModel, ModConfig>.GenerateMatchedObject(ClimateControl.s_standardModel, Config);
                ClimateControl.s_standardModel = ClimateControl.s_standardModel.DeepClone();
            }
            else if (Config.ModelChoice == IIWAPI.WeatherModel.custom.ToString())
            {
                // The custom model, with standard model as the base class.
                PropertyMatcher<StandardModel, ModConfig>.GenerateMatchedObject(ClimateControl.s_customModel, Config);
                ClimateControl.s_customModel = ClimateControl.s_customModel.DeepClone();
            }

            // Set SMAPI log levels
            if (Config.EnableDebugLogging)
            {
                ClimateControl.s_logLevel = LogLevel.Info;
            }
            else
            {
                ClimateControl.s_logLevel = LogLevel.Trace;
            }
        }

        public static void ResetModel(IModHelper Helper)
        {
            ClimateControl.s_eventLogger.SendToSMAPI("I was asked to reset all models");
            // Reset all models, except custom
            ClimateControl.s_standardModel = new();
            Helper.Data.WriteJsonFile("models/standard.json", ClimateControl.s_standardModel);

            // Copy reset values into Config
            ClimateControl.s_config = new ModConfig();
            Helper.WriteConfig(ClimateControl.s_config);
            if (ClimateControl.s_config.EnableInterpolation)
            {
                ClimateControl.InterpolateModel(Helper);
            }
        }

        public static void ChangeModel(ModConfig Config, IModHelper Helper)
        {
            ClimateControl.s_eventLogger.SendToSMAPI("I was asked to refresh all models");
            // Refresh relevant models
            ClimateControl.s_eventLogger.SendToSMAPI($"Model was changed from {ClimateControl.s_modelChoice} to {Config.ModelChoice}. Changes will be applied to {ClimateControl.s_modelChoice}");
            // Save changes to old model.
            if (ClimateControl.s_modelChoice == IIWAPI.WeatherModel.standard)
            {
                PropertyMatcher<ModConfig, StandardModel>.GenerateMatchedObject(Config, ClimateControl.s_standardModel);
                Helper.Data.WriteJsonFile("models/standard.json", ClimateControl.s_standardModel);
                ClimateControl.s_standardModel = Helper.Data.ReadJsonFile<StandardModel>("models/standard.json");
            }
            else if (ClimateControl.s_modelChoice == IIWAPI.WeatherModel.custom)
            {
                PropertyMatcher<ModConfig, StandardModel>.GenerateMatchedObject(Config, ClimateControl.s_customModel);
                Helper.Data.WriteJsonFile("models/custom.json", ClimateControl.s_customModel);
                ClimateControl.s_customModel = Helper.Data.ReadJsonFile<StandardModel>("models/custom.json");
            }
            // Load new model.
            LoadModel(Config);
            Helper.WriteConfig(Config);
            ClimateControl.s_modelChoice = (IIWAPI.WeatherModel)Enum.Parse(typeof(IIWAPI.WeatherModel), Config.ModelChoice);
            if (Config.EnableInterpolation)
            {
                ClimateControl.InterpolateModel(Helper);
            }
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
