using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer.Engine
{
    public class Settings
    {
        public static bool AutoCreate = false;
        public static short[] randomSkins;
        public static int PRICE_PER_COLOR_CHANGE = 10;
        public static int GOLD_EXCAHNGE_RATE = 10;
        public static int SILVER_EXCAHNGE_RATE = 1;
        public static uint BINDPASSWORD { get; set; }
        public static int GAMEVERSION { get; set; }
        public static int DEBUG_MODE { get; set; }
        public static bool USE_RANKING_QUEUES { get; set; }
        public static bool START_WITH_ONE_PLAYER { get; set; }
        public static int MIN_PLAYERS { get; set; }
        public static int CAN_REQUEST_RANK_UPDATE_TIME { get; set; }
        public static int MAX_PLAYERS { get; set; }
        public static int CHECK_CAN_START { get; set; }
        public static int MAX_WAIT_TIME { get; set; }
        public static int DEBUG_MAX_WAIT_TIME { get; set; }
        public static int MIN_GAMES_TO_DECIDE_RANKING_SKILLS { get; set; }
        public static bool DEBUG_GET_PLAYER = false;
        public static int RANKING_SEASON = 4;
    }
}
