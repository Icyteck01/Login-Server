using JHSEngine.net.GateWay.Data;
using JHSNetProtocol;
using JHSRanking;
using LoginServer.Database;
using LoginServer.Engine.Service;
using LoginServer.MYSQL.Tables;
using System;
using System.Collections.Generic;

namespace LoginServer.Engine.Managers
{
    public sealed class RankingManager
    {
        private static object s_Sync = new object();
        static volatile RankingManager s_Instance;
        public static RankingManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    lock (s_Sync)
                    {
                        if (s_Instance == null)
                        {
                            s_Instance = new RankingManager();
                        }
                    }
                }
                return s_Instance;
            }
        }
        private Dictionary<uint, RankinngOBJ> KnownPlayers = new Dictionary<uint, RankinngOBJ>();
        private DbService dbService;
        private LeagueManager leagueManager;

        public static void Init()
        {
            Instance._Init();
        }

        public void _Init()
        {
            float start = JHSTime.Time;
            dbService = DbService.Instance;
            leagueManager = LeagueManager.Instance;
            leagueManager.RequiredGames = Settings.MIN_GAMES_TO_DECIDE_RANKING_SKILLS;
            List<RankConfig> configs = ConfigManager.GetRankConfigs();
            leagueManager._Configure(configs);
            leagueManager.OnPlayerChanged = OnPlayerChanged;
            leagueManager.OnPlayerDemoted = OnPlayerDemoted;
            leagueManager.OnPlayerPromoted = OnPlayerPromoted;
            leagueManager.OnPlayerRemoved = OnPlayerRemoved;
            foreach (RankConfig config in configs)
            {
                LeagueType rank = config.RankId;

                IList<Rankings> usr = DbService.Instance.GetRank(rank);
                foreach (Rankings usrrank in usr)
                {
                    RankinngOBJ obj = dbService.GetRankinData(new RankinngOBJ(usrrank.UserId, usrrank.PlayerName));
                    if (obj != null)
                    {
                        KnownPlayers.Add(usrrank.UserId, obj);
                        leagueManager._AddPlayer(obj.RefLeagueData);
                    }
                }
            }
            LOG.Info("Finished loading ranking, time-consuming:" + (JHSTime.Time - start) + " sec.");
            LOG.Info(leagueManager.ToString());

        }

        public LeagueData[] _GetLegue(LeagueType league)
        {
            return leagueManager._ToArray(league);
        }

        public static RankinngOBJ GetPlayer(AccountOBJ user)
        {
            return Instance._GetPlayer(user);
        }

        public RankinngOBJ _GetPlayer(AccountOBJ user)
        {
            if (KnownPlayers.TryGetValue(user.Id, out RankinngOBJ obj))
                return obj;
            else
            {
                RankinngOBJ objx = dbService.GetRankinData(new RankinngOBJ(user.Id, user.PlayerName));
                if (objx != null)
                {
                    KnownPlayers.Add(objx.UserId, objx);
                    leagueManager._AddPlayer(objx.RefLeagueData);
                    return objx;
                }
            }
            return null;
        }

        public static void UpdatePlayer(uint PlayerId, int LeaguePoints)
        {
            Instance._UpdatePlayer(PlayerId, LeaguePoints);
        }

        public void _UpdatePlayer(uint PlayerId, int LeaguePoints)
        {
            if(KnownPlayers.TryGetValue(PlayerId, out RankinngOBJ player))
            {
                player.LeaguePoints = LeaguePoints;
                if (leagueManager._UpdatePlayer(player.RefLeagueData))
                {
                    LeagueData updatedPlayer = leagueManager._GetPlayer(player.UserId);
                    if (updatedPlayer != null)
                    {
                        player.League = updatedPlayer.League;
                    }
                }
                DbService.SubmitUpdate2Queue(player);
            }
        }

        public static void UpdatePlayer(uint PlayerId, int TotalLeaguePoints, int TotalGamePlays, int TotalKills, int TotalDeaths)
        {
            Instance._UpdatePlayer(PlayerId, TotalLeaguePoints, TotalGamePlays, TotalKills, TotalDeaths);
        }

        public void _UpdatePlayer(uint PlayerId, int TotalLeaguePoints, int TotalGamePlays, int TotalKills, int TotalDeaths)
        {
            if (KnownPlayers.TryGetValue(PlayerId, out RankinngOBJ player))
            {
                player.LeaguePoints = TotalLeaguePoints;
                player.Kills = TotalKills;
                player.Deaths = TotalDeaths;
                player.GameCount = TotalGamePlays;               
                if (leagueManager._UpdatePlayer(player.RefLeagueData))
                {
                    LeagueData updatedPlayer = leagueManager._GetPlayer(player.UserId);
                    if (updatedPlayer != null)
                    {
                        player.League = updatedPlayer.League;
                    }
                }
                DbService.SubmitUpdate2Queue(player);
            }
        }
        private void OnPlayerRemoved(LeagueData Player)
        {
            if (Player != null && KnownPlayers.TryGetValue(Player.UserId, out RankinngOBJ obj))
            {
                obj.Index = Player.Index;
                obj.League = Player.League;
                DbService.SubmitUpdate2Queue(obj);
            }
        }

        private void OnPlayerPromoted(LeagueData Player)
        {
            if (Player != null && KnownPlayers.TryGetValue(Player.UserId, out RankinngOBJ obj))
            {
                obj.Index = Player.Index;
                obj.League = Player.League;
                DbService.SubmitUpdate2Queue(obj);
            }
        }

        private void OnPlayerDemoted(LeagueData Player)
        {
            if (Player != null && KnownPlayers.TryGetValue(Player.UserId, out RankinngOBJ obj))
            {
                obj.Index = Player.Index;
                obj.League = Player.League;
                DbService.SubmitUpdate2Queue(obj);
            }
        }

        private void OnPlayerChanged(LeagueData Player)
        {
            if (Player != null && KnownPlayers.TryGetValue(Player.UserId, out RankinngOBJ obj))
            {
                obj.Index = Player.Index;
                obj.League = Player.League;
                DbService.SubmitUpdate2Queue(obj);
            }
        }
    }
}
