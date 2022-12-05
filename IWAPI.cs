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
        Tuple<string, string> GetWeatherInfo();
        string TranslateTomorrowStates(int integerState);
        int TranslateTomorrowStates(string stringState);
        double RollTheDice();
        int RollTheDiceInt();
        public enum FollowTheWhiteRabbit
        {
            ClimateControl = 0
        }
        public void WakeUpNeo_TheyreWatchingYou(string messageForNeo, int thisIsMyName);
        public enum WeatherModel
        {
            custom = 0,
            standard = 1
        }
    }
}
