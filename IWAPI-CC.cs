using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW_ClimateControl
{
    /// <summary>
    /// Internal API for ImmersiveWeathers.
    /// </summary>
    public interface IWAPI
    { 
        /// <summary>
        ///     All possible weathers recognised by Stardew Valley.
        /// </summary>
        public enum WeatherType
        {
            sunny = 0,
            raining = 1,
            windy = 2,
            storming = 3,
            festival = 4,
            snowing = 5,
            wedding = 6,
            unknown = 7
        }

        /// <summary>
        /// All possible seasons recognised by Stardew Valley.
        /// </summary>
        public enum SeasonType
        {
            spring = 0,
            summer = 1,
            fall = 2,
            winter = 3
        }

        /// <summary>
        ///     All possible weather models used by this mod.
        /// </summary>
        public enum WeatherModel
        {
            none = 0,
            custom = 1,
            standard = 2
        }

        /// <summary>
        ///     All sister mods recognised by the Framework.
        /// </summary>
        public enum FollowTheWhiteRabbit
        {
            ClimateControl = 0
        }

        /// <summary>
        ///     All possible updates sent by sister mods to the Framework.
        /// </summary>
        /// <remarks>
        ///     Useful for responding differently based on when in a session the various features are triggered.
        /// </remarks>
        public enum TypeOfMessage
        {
            saveLoaded = 0,
            titleReturned = 1,
            dayStarted = 2
        }

        /// <summary>
        ///     Weather forecast for today and tomorrow.
        /// </summary>
        /// <returns>
        ///     Tuple: Weather today and weather tomorrow.
        /// </returns>
        public Tuple<string, string> GetWeatherInfo();

        /// <summary>
        ///     Converts an integer into the respective weather type recognised by Stardew Valley.
        /// </summary>
        /// <param name="integerState">
        ///     See <see cref="WeatherType"/> for possible values.
        /// </param>
        /// <returns>
        ///     String version of a weather type.
        /// </returns>
        public string TranslateTomorrowStates(int integerState);

        /// <summary>
        ///     Converts a weather string into the respective integer recognised by Stardew Valley.
        /// </summary>
        /// <param name="stringState">
        ///     See <see cref="WeatherType"/> for possible values.
        /// </param>
        /// <returns>
        ///     Integer value for a weather type.
        /// </returns>
        public int TranslateTomorrowStates(string stringState);

        /// <summary>
        ///     Request a random double from the Framework. Will reference EvenBetterRNG if possible.
        /// </summary>
        /// <returns>
        ///     A random double between 0 and 1.
        /// </returns>
        public double RollTheDice();

        /// <summary>
        ///     Request a random integer from the Framework. Will reference EvenBetterRNG if possible.
        /// </summary>
        /// <returns>
        ///     A random integer.
        /// </returns>
        public int RollTheDiceInt();

        /// <summary>
        ///     Send a message to the Framework and receive a response.
        /// </summary>
        /// <param name="Message">
        ///     Container for both a message and a response when communicating with the Framework.
        /// </param>
        public void ProcessMessage(ImmersiveWeathers.MessageContainer Message);
    }
}
