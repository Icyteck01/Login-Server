
using JHSNetProtocol;
using JHSServer;
using LoginServer.Database;
using LoginServer.Engine;
using LoginServer.Engine.Managers;
using LoginServer.Engine.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Utils;

namespace LoginServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            float StartTime = JHSTime.Time;
            Console.Title = "LoginServer";
            AppDomain.CurrentDomain.DomainUnload += CleanupBeforeExit;
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            Properties props = new Properties(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs/Config.ini"));
            Settings.AutoCreate = bool.Parse(props.get("AUTO_CREATE"));
            Settings.BINDPASSWORD = uint.Parse(props.get("BINDPASSWORD"));
            Settings.GAMEVERSION = int.Parse(props.get("GAMEVERSION"));
            Settings.DEBUG_MODE = int.Parse(props.get("DEBUG_MODE"));

            Settings.MIN_PLAYERS = int.Parse(props.get("MIN_PLAYERS"));
            Settings.MAX_PLAYERS = int.Parse(props.get("MAX_PLAYERS"));
            Settings.CHECK_CAN_START = int.Parse(props.get("CHECK_CAN_START"));
            Settings.MAX_WAIT_TIME = int.Parse(props.get("MAX_WAIT_TIME"));
            Settings.RANKING_SEASON = int.Parse(props.get("RANKING_SEASON"));
            Settings.CAN_REQUEST_RANK_UPDATE_TIME = int.Parse(props.get("CAN_REQUEST_RANK_UPDATE_TIME"));
            Settings.MIN_GAMES_TO_DECIDE_RANKING_SKILLS = int.Parse(props.get("MIN_GAMES_TO_DECIDE_RANKING_SKILLS"));
            Settings.USE_RANKING_QUEUES = bool.Parse(props.get("USE_RANKING_QUEUES"));
            Settings.START_WITH_ONE_PLAYER = bool.Parse(props.get("START_WITH_ONE_PLAYER"));
            Settings.DEBUG_GET_PLAYER = bool.Parse(props.get("ALLOW_NOT_REGISTERED_GET_ROLE_INFO"));
            LOG.Error("Starting LoginServer");
            LOG.Info("Starting LoginServer");
            List<short> skins = new List<short>();
            string[] d = props.get("DEFAULT_SKIKINS").Split(',');
            foreach (string str in d)
            {
                skins.Add(short.Parse(str));
            }
            Settings.randomSkins = skins.ToArray();
            DbService.Instance.Start();
            ConfigManager.LoadConfigs();
            RankingManager.Init();
            NetworkServer.Instance.SetConfig(props);
            NetworkServer.Instance.InitHandlers();
            NetworkServer.Instance.Start();
            LOG.Info("Finished loading Server, time-consuming:" + (JHSTime.Time - StartTime) + " sec.");
            WATIREGION:

            string line = Console.ReadLine();
            
            if (line == "exit")
                goto EXITPROGRAM;
            if (line == "aadd")
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                AccountOBJ obj = DbService.Instance.GetAccountFromDB(2);
                if (obj != null)
                {
                    RankinngOBJ data = RankingManager.GetPlayer(obj);
                    RankingManager.UpdatePlayer(obj.Id, data.LeaguePoints + 100);
                }
            }
            if (line == "add") {
                AccountOBJ obj = DbService.Instance.GetAccountFromDB(1);
                if (obj != null)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    RankinngOBJ data = RankingManager.GetPlayer(obj);
                    if (data != null)
                    {
                        RankingManager.UpdatePlayer(obj.Id, data.LeaguePoints + 100);
                     
                    }
                }
            }
            if (line == "gamec")
            {
                AccountOBJ obj = DbService.Instance.GetAccountFromDB(1);
                if (obj != null)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    RankinngOBJ data = RankingManager.GetPlayer(obj);
                    if (data != null)
                    {
                        RankingManager.UpdatePlayer(obj.Id, data.LeaguePoints + 100, data.GameCount +1, data.Kills + 1, 0);
                    }
                }
            }
            if (line == "add2") {
                AccountOBJ obj = DbService.Instance.GetAccountFromDB(1);
                if (obj != null)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    RankinngOBJ data = RankingManager.GetPlayer(obj);
                    RankingManager.UpdatePlayer(obj.Id, data.LeaguePoints + 1000);
                }
            }
            if (line == "del")
            {
                AccountOBJ obj = DbService.Instance.GetAccountFromDB(1);
                if (obj != null)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    RankinngOBJ data = RankingManager.GetPlayer(obj);
                    RankingManager.UpdatePlayer(obj.Id, data.LeaguePoints - 100);
                }
            }

            goto WATIREGION;

            EXITPROGRAM:
            Console.WriteLine("Saving Database.");
            if (DbService.Instance.SaveAll())
            {
                Console.WriteLine("Server is now down.");
                Console.ReadKey();
            }
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("Saving Database.");
            if (DbService.Instance.SaveAll())
            {
                Console.WriteLine("Server is now down.");
                Console.ReadKey();
            }
            Console.ReadLine();
        }

        private static void CleanupBeforeExit(object sender, EventArgs e)
        {
           
            Console.WriteLine("Saving Database.");
            if (DbService.Instance.SaveAll())
            {
                Console.WriteLine("Server is now down.");
                Console.ReadKey();
            }
            Console.ReadLine();
        }

    }
}

