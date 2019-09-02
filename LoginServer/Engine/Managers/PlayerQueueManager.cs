using JHSNetProtocol;
using JHSRanking;
using LoginServer.Database;
using LoginServer.Network;
using LoginServer.Network.Data;
using LoginServer.Utils;
using System.Collections.Generic;

namespace LoginServer.Engine.Managers
{
    public class MatchPlayer
    {
        public uint userId;
        public uint ConnectionId;
        public uint PlayerId;
        public LeagueType League;
    }

    public sealed class PlayerQueueManager
    {
        private static object s_Sync = new object();
        private static object s_SyncStart = new object();
        static volatile PlayerQueueManager s_Instance;
        public static PlayerQueueManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    lock (s_Sync)
                    {
                        if (s_Instance == null)
                        {
                            s_Instance = new PlayerQueueManager();
                        }
                    }
                }
                return s_Instance;
            }
        }
        private float LastCheckTime = 0;
        private UniqueQueue<MatchPlayer> m_queue = new UniqueQueue<MatchPlayer>();
        private GameServerManager gameServerManager = null;
        private AccountManager DbManager = null;
        private float LastStartTime = 0;
        private float Starterd => JHSTime.Time - LastStartTime;
        private bool IsForceStart => Starterd >= Settings.MAX_WAIT_TIME;
        private bool CanStart => m_queue.Count >= Settings.MAX_PLAYERS || m_queue.Count >= Settings.MIN_PLAYERS && IsForceStart || Starterd >= Settings.DEBUG_MAX_WAIT_TIME && Settings.START_WITH_ONE_PLAYER;

        public void AddPlayer(MatchPlayer player)
        {
            lock (m_queue)
            {
                m_queue.Enqueue(player);
            }
        }

        public void CheckStart()
        {
            if (LastCheckTime < JHSTime.Time)
            {
                LastCheckTime = JHSTime.Time + Settings.CHECK_CAN_START;
                InternalCheckStart();
            }
        }

        private bool InternalCheckStart()
        {
            lock (s_SyncStart)
            {
                if (gameServerManager == null) gameServerManager = GameServerManager.Instance;
                Server server = gameServerManager.GetFreeServer();
                if (server != null)
                {
                    if (CanStart)
                    {
                        StartNow(GetCurrentPlayers(), server);
                    }
                }
                return false;
            }
        }

        private void StartNow(List<MatchPlayer> m_players, Server server)
        {
            LastStartTime = JHSTime.Time;
            if (DbManager == null) DbManager = AccountManager.Instance;
            for (int i = 0; i < m_players.Count; i++)
            {
                MatchPlayer parti = m_players[i];
                if (parti != null)
                {
                    AccountOBJ user = DbManager.GetOnlineByConnectionId(parti.ConnectionId);
                    if (user != null && user.InQueue && user.IsOnline)
                    {
                        lock (user)
                            user.InQueue = false;
                        JHSNetworkServer.Send(user.connectionID, NetworkConstants.START_SEARCH_MATCH, new SearchMatch() { op = Enums.SearchMatchOperations.START, IP = server.IP, port = server.port });
                    }
                }
            }

        }

        public int AvregeWaitTime()
        {
            return 2;
        }

        public List<MatchPlayer> GetCurrentPlayers(LeagueType RANK = LeagueType.NONE)
        {
            lock (m_queue)
            {
                List<MatchPlayer> pl = new List<MatchPlayer>();
                if (RANK == LeagueType.NONE || !Settings.USE_RANKING_QUEUES)
                {
                    while (m_queue.Count > 0)
                    {
                        if (pl.Count < Settings.MAX_PLAYERS)
                        {
                            pl.Add(m_queue.Dequeue());
                        }
                        else
                        {
                            break;
                        }
                    }
                    return pl;
                }
            }

            return null;
        }
    }
}
