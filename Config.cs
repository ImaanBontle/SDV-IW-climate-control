using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace IW_ClimateControl
{
    // Config file to generate
    public sealed class ModConfig
    {
        private const string V = "standard";

        // Which model to read from
        public string ModelChoice { get; set; } = V;

        // Preparing to copy across properties
        [MatchParent("Model")]
        public ModelDefinition WeatherModel { get; set; }

        public ModConfig()
        {
            // Copy across properties
            if (this.ModelChoice == "standard")
            {
                StandardModel standardModel = new();
                PropertyMatcher<StandardModel, ModConfig>.GenerateMatchedObject(standardModel, this);
            }
        }
    }

    // Class that cherry-picks relevant properties to copy: https://www.pluralsight.com/guides/property-copying-between-two-objects-using-reflection
    [AttributeUsage(AttributeTargets.Property)]
    public class MatchParentAttribute : Attribute
    {
        public readonly string ParentPropertyName;
        public MatchParentAttribute(string parentPropertyName)
        {
            ParentPropertyName = parentPropertyName;
        }
    }

    // Class that does the copying: https://www.pluralsight.com/guides/property-copying-between-two-objects-using-reflection
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
