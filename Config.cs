using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace IW_ClimateControl
{
    /// <summary>
    ///     The default user settings.
    /// </summary>
    public sealed class ModConfig
    {
        /// <summary>
        ///     The choice of inherited weather model.
        /// </summary>
        /// <remarks>
        ///     Defaults to the 'standard' model for generic climates.
        /// </remarks>
        public string ModelChoice { get; set; } = "standard";

        /// <summary>
        ///     The inherited model properties.
        /// </summary>
        /// <remarks>
        ///     Defaults to the 'standard' model for generic climates.
        /// </remarks>
        [MatchParent("Model")]
        public ModelDefinition WeatherModel { get; set; }

        /// <summary>
        ///     Generates the user configuration and
        ///     inherits from the chosen model.
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
    ///     Cherry-picks relevant properties to inherit from another class.
    /// </summary>
    /// <remarks>
    ///     See the
    ///     <see href="https://www.pluralsight.com/guides/property-copying-between-two-objects-using-reflection">original article</see>
    ///     for more information.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class MatchParentAttribute : Attribute
    {
        public readonly string ParentPropertyName;
        public MatchParentAttribute(string parentPropertyName)
        {
            ParentPropertyName = parentPropertyName;
        }
    }

    /// <summary>
    ///     Performs the cherry-picking of class properties.
    /// </summary>
    /// <remarks>
    ///     See the
    ///     <see href="https://www.pluralsight.com/guides/property-copying-between-two-objects-using-reflection">original article</see>
    ///     for more information.
    /// </remarks>
    /// <typeparam name="TParent"></typeparam>
    /// <typeparam name="TChild"></typeparam>
    public class PropertyMatcher<TParent, TChild> where TParent : class
                                                  where TChild : class
    {
        public static void GenerateMatchedObject(TParent parent, TChild child)
        {
            var childProperties = child.GetType().GetProperties();
            foreach (var childProperty in childProperties)
            {
                var attributesForProperty = childProperty.GetCustomAttributes(typeof(MatchParentAttribute), true);
                var isOfTypeMatchParentAttribute = false;

                MatchParentAttribute currentAttribute = null;

                foreach (var attribute in attributesForProperty)
                {
                    if (attribute.GetType() == typeof(MatchParentAttribute))
                    {
                        isOfTypeMatchParentAttribute = true;
                        currentAttribute = (MatchParentAttribute)attribute;
                        break;
                    }
                }

                if (isOfTypeMatchParentAttribute)
                {
                    var parentProperties = parent.GetType().GetProperties();
                    object parentPropertyValue = null;
                    foreach (var parentProperty in parentProperties)
                    {
                        if (parentProperty.Name == currentAttribute.ParentPropertyName)
                        {
                            if (parentProperty.PropertyType == childProperty.PropertyType)
                            {
                                parentPropertyValue = parentProperty.GetValue(parent);
                            }
                        }
                    }
                    childProperty.SetValue(child, parentPropertyValue);
                }
            }
        }
    }
}
