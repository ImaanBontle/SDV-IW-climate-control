using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW_ClimateControl
{
    // Internal interface for IWAPI
    public interface IWAPI
    { 
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
        public enum SeasonType
        {
            spring = 0,
            summer = 1,
            fall = 2,
            winter = 3
        }
        public enum WeatherModel
        {
            none = 0,
            custom = 1,
            standard = 2
        }
        public enum FollowTheWhiteRabbit
        {
            ClimateControl = 0
        }
        public enum TypeOfMessage
        {
            saveLoaded = 0,
            titleReturned = 1,
            dayStarted = 2
        }
        public Tuple<string, string> GetWeatherInfo();
        public string TranslateTomorrowStates(int integerState);
        public int TranslateTomorrowStates(string stringState);
        public double RollTheDice();
        public int RollTheDiceInt();
        public void ProcessMessage(ImmersiveWeathers.MessageContainer Message);
    }
}
