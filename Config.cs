using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
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
            // Copy across properties
            if (this.ModelChoice == "standard")
            {
                // The standard model for generic climates.
                StandardModel standardModel = new();
                PropertyMatcher<StandardModel, ModConfig>.GenerateMatchedObject(standardModel, this);
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
