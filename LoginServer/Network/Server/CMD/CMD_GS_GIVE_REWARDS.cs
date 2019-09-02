using JHSNetProtocol;
using LoginServer.Database;
using LoginServer.Engine;
using LoginServer.Engine.Managers;
using LoginServer.Engine.Service;
using Network.Server.Data;
using static CommonConstant;

namespace Network.Server.CMD
{
    public class CMD_GS_GIVE_REWARDS : IJHSInterface
    {
        protected AccountManager DbManager;
        protected GameServerManager queueManager;
        protected DbService dbService;

        public bool Execute(JHSNetworkMessage netMsg)
        {
            if (DbManager == null) DbManager = AccountManager.Instance;
            if (queueManager == null) queueManager = GameServerManager.Instance;
            if (dbService == null) dbService = DbService.Instance;
            UpdateMatchResult packet = netMsg.ReadMessage<UpdateMatchResult>();
            if (packet != null)
            {
                uint connectionId = netMsg.conn.connectionId;
                var server = queueManager.GetServerByConnectionId(connectionId);
                if (server != null)
                {
                    AccountOBJ user = DbManager.GetOnlineByUserId(packet.UserId);
                    if (user == null)
                    {
                        user = dbService.GetAccountFromDB(packet.UserId);
                    }
                    if (user != null)
                    {
                        lock (user)
                        {
                            foreach (DBPlayerActive active in packet.Actives)
                            {
                                ActivesOBJ obj = user.GetActive((int)active.ActiveId);
                                obj.Value = (int)active.Value;
                                DbService.UpdateEntityIntime(obj);
                            }
                            user.Data.Exp += (int)packet.EXP;

                            RankinngOBJ RankData = RankingManager.GetPlayer(user);
                            if (RankData != null)
                            {
                                RankData.GameCount += 1;
                                RankData.Kills += (int)packet.KillCount;

                                if (!packet.HasWon)
                                    RankData.Deaths += 1;
                            }
                            user.OnGameResultRecieved();
                            DbService.UpdateEntityIntime(user);
                        }
                        LOG.Info(string.Format("SaveResult  ::  Server[{0}] userid[{1}]", connectionId, packet.UserId));
                    }
                    else
                    {
                        LOG.Info(string.Format("SaveResult Error unknown user ::  Server[{0}] userid[{1}]", connectionId, packet.UserId));
                    }
                    netMsg.conn.Send(GameServerOP.PUTROLE, new Empty());
                }
            }
             return true;
        }
    }
}
