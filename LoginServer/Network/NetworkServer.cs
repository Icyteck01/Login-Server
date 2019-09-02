using System.Collections.Generic;
using Utils;
using JHSNetProtocol;
using System;
using LoginServer.Network;
using LoginServer.Network.CMD;
using LoginServer.Database;
using LoginServer.Engine.Service;
using Network.Server;
using Network.Server.CMD;
using LoginServer.Engine.Managers;
using LoginServer.Utils;
using LoginServer.Engine;
using LoginServer.Network.Client.CMD;

namespace JHSServer
{
    public sealed class NetworkServer
    {
        public static readonly NetworkServer Instance = new NetworkServer();
        Properties config;
        private AccountManager DbManager;
        private PlayerQueueManager matchQueue;
        private GameServerManager queueManager;

        #region HANDLERS
        private Dictionary<short, IJHSInterface> m_MessageHandlersDict = new Dictionary<short, IJHSInterface>();

        public void RegisterHandeler(short id, IJHSInterface handler)
        {
            if (!m_MessageHandlersDict.ContainsKey(id))
            {
                m_MessageHandlersDict[id] = handler;
            }
            else
                LOG.Error("Network Handler " + (object)id + " already exist.");
        }

        public void UnregisterHandler(short id)
        {
            if (m_MessageHandlersDict.ContainsKey(id))
                m_MessageHandlersDict.Remove(id);
            else
                LOG.Error("Network Handler " + (object)id + " already exist.");
        }
        #endregion

        private NetworkServer() { }

        public void SetConfig(Properties config)
        {
            DbManager = AccountManager.Instance;
            matchQueue = PlayerQueueManager.Instance;
            queueManager = GameServerManager.Instance;
            this.config = config;
            JHSDebug.LogReciver = new JHSLoggingOverite();
            NetConfig.logFilter = (JHSLogFilter)Settings.DEBUG_MODE;
            NetConfig.IP = this.config.get("HOST_IP");
            NetConfig.Port = int.Parse(this.config.get("TCP_port"));
        }

        public void InitHandlers()
        {
            JHSNetworkServer.RegisterHandler(InternalMessages.DISCONNECT, OnDisconnect);
            JHSNetworkServer.RegisterHandler(InternalMessages.RECIEVE, OnNetworkReceive);           
            RegisterHandeler(NetworkConstants.LOGIN, new UserLogin());
            RegisterHandeler(NetworkConstants.BUYCOLORCHANGE, new BuyColorChange());
            RegisterHandeler(NetworkConstants.EXCHANGE, new ExchangeCurrency());
            RegisterHandeler(NetworkConstants.BUYCHARACTER, new BuySkinHandler());
            RegisterHandeler(NetworkConstants.GM, new GMComands());
            RegisterHandeler(NetworkConstants.DELETE_CHARACTER, new DeleteCharacterCommand());
            RegisterHandeler(NetworkConstants.START_SEARCH_MATCH, new SearchMatchCommand());
            RegisterHandeler(NetworkConstants.COLLECT_ACTIVE, new CollectActiveCommand());
            RegisterHandeler(NetworkConstants.REFRESH_ACTIVES, new RequestActives());
            RegisterHandeler(NetworkConstants.UPDATE_RANKINGS, new GetRankings());

            //GAME SERVER PACKETS
            RegisterHandeler(GameServerOP.PUTROLE, new CMD_GS_GIVE_REWARDS());
            RegisterHandeler(GameServerOP.GETROLE, new CMD_GETROLE());          
            RegisterHandeler(GameServerOP.UPDATE_STATE, new GS_STATE());
           
        }

        private void OnNetworkReceive(JHSNetworkMessage netMsg)
        {
            if (m_MessageHandlersDict.TryGetValue(netMsg.msgType, out IJHSInterface handler))
            {
                lock (handler)
                {
                    handler.Execute(netMsg);
                }
            }
        }

        public void Start()
        {
            foreach (KeyValuePair<short, IJHSInterface> that in m_MessageHandlersDict)
            {
                JHSNetworkServer.RegisterHandler(that.Key, OnNetworkReceive);
            }

            JHSNetworkServer.Start();
        }

        public void OnDisconnect(JHSNetworkMessage netMsg)
        {
            AccountOBJ user = DbManager.GetOnlineByConnectionId(netMsg.conn.connectionId);
            if(user != null)
            {
                lock(user)
                {
                    user.OnLogout();
                    DbManager.RemoveOnline(netMsg.conn.connectionId);
                    user.InQueue = false;
                }
            }
            queueManager.RemoveServer(netMsg.conn.connectionId);
        }

        public void SendToClients(int[] v, short msgId, JHSNetworkMessage sr, bool reliable = true)
        {
            /*
            if (NetworkGameServer == null)
                return;
            for (int index = 0; index < v.Length; ++index)
            {
                TcpPlayer player = NetworkGameServer.GetPlayer(v[index]);
                if (player != null)
                    player.Send(msgId, (DRMessage)sr, reliable);
            }
            */
        }
    }
}
