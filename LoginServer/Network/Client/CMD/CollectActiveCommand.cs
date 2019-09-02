using JHSNetProtocol;
using LoginServer.Database;
using LoginServer.Engine.Managers;
using LoginServer.Engine.Service;
using LoginServer.MYSQL.Tables;
using LoginServer.Network.Data;
using static CommonConstant;


namespace LoginServer.Network.CMD
{
    public class CollectActiveCommand : IJHSInterface
    {
        protected AccountManager DbManager;
        public bool Execute(JHSNetworkMessage netMsg)
        {
            if (DbManager == null)
                DbManager = AccountManager.Instance;

            CollectActive packet = netMsg.ReadMessage<CollectActive>();
            if (packet != null)
            {
                uint connectionId = netMsg.conn.connectionId;
                AccountOBJ user = DbManager.GetOnlineByConnectionId(connectionId);
                if (user != null)
                {
                    lock (user)
                    {
                        ActivesConfig model = ConfigManager.GetActive((int)packet.AciveId);
                        if(model == null)
                        {
                            netMsg.conn.Send(NetworkConstants.COLLECT_ACTIVE, new CollectActiveResponse() { STATUS = ITEM_CONFIG_WRONG });
                            return true;
                        }

                        ActivesOBJ active = user.GetActive((int)packet.AciveId);
                        if (active == null)
                        {
                            netMsg.conn.Send(NetworkConstants.COLLECT_ACTIVE, new CollectActiveResponse() { STATUS = ACTIVE_NOT_FOUND });
                            return true;
                        }

                        if (active.Collected)
                        {
                            netMsg.conn.Send(NetworkConstants.COLLECT_ACTIVE, new CollectActiveResponse() { STATUS = ACTIVE_ALREADY_COLLECTED });
                            return true;
                        }
                        if(model.Conditions != active.Value)
                        {
                            netMsg.conn.Send(NetworkConstants.COLLECT_ACTIVE, new CollectActiveResponse() { STATUS = ACTIVE_NOT_COMPLEATED });
                            return true;
                        }

                        active.Collected = true;
                        DbService.SubmitUpdate2Queue(active);

                        if (model.GoldReward > 0)
                            user.Gold += model.GoldReward;

                        if(model.SilverReward > 0)
                            user.Silver += model.SilverReward;

                        user.ResetNotification();
                        DbService.SubmitUpdate2Queue(user);

                        netMsg.conn.Send(NetworkConstants.COLLECT_ACTIVE, new CollectActiveResponse() { STATUS = SUCCESS, AciveId = packet.AciveId, Gold = (uint)user.Gold, Silver = (uint)user.Silver});
                    }

                }
            }
            return true;
        }


    }
}
