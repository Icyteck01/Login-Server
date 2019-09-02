using JHSNetProtocol;
using LoginServer.Database;
using LoginServer.Engine.Managers;
using LoginServer.Engine.Service;
using LoginServer.Network.Data;

namespace LoginServer.Network.CMD
{
    public class GMComands : IJHSInterface
    {
        protected AccountManager DbManager;
        public bool Execute(JHSNetworkMessage netMsg)
        {
            if (DbManager == null)
                DbManager = AccountManager.Instance;

            ConsoleCMD packet = netMsg.ReadMessage<ConsoleCMD>();
            if (packet != null)
            {
                uint connectionId = netMsg.conn.connectionId;
                AccountOBJ account = DbManager.GetOnlineByConnectionId(connectionId);
                if (account != null)
                {
                    if (account.Priviledge > 1)
                    {
                        switch((GMCOMMAND)packet.Command)
                        {
                            case GMCOMMAND.ADD_GOLD:
                                account.Gold += (int)packet.Value;
                                netMsg.conn.Send(NetworkConstants.EXCHANGE, new ExchangeCurResp() { STATUS = 0, SilverValue = (uint)account.Silver, GoldValue = (uint)account.Gold });
                                break;
                            case GMCOMMAND.ADD_SILVER:
                                account.Silver += (int)packet.Value;
                                netMsg.conn.Send(NetworkConstants.EXCHANGE, new ExchangeCurResp() { STATUS = 0, SilverValue = (uint)account.Silver, GoldValue = (uint)account.Gold });
                                break;
                            case GMCOMMAND.FINISH_ACTIVES:
                                if (packet.Value == 1)
                                {
                                    ActivesConfig[] allConfig = ConfigManager.GetActives();
                                    for (int i = 0; i < allConfig.Length; i++)
                                    {
                                        ActivesOBJ active = account.GetActive(allConfig[i].ActiveId);
                                        if (active != null)
                                        {
                                            active.Value = (int)allConfig[i].Conditions;
                                            active.Collected = false;
                                            DbService.SubmitUpdate2Queue(active);
                                        }
                                    }
                                    netMsg.conn.Send(NetworkConstants.REFRESH_ACTIVES, new RefreshActives() { actives = account.SerializeActives() });
                                }
                                else
                                {
                                    ActivesConfig[] allConfig = ConfigManager.GetActives();
                                    for (int i = 0; i < allConfig.Length; i++)
                                    {
                                        ActivesOBJ active = account.GetActive(allConfig[i].ActiveId);
                                        if (active != null)
                                        {
                                            active.Value = 0;
                                            active.Collected = false;
                                            DbService.SubmitUpdate2Queue(active);
                                        }
                                    }
                                    netMsg.conn.Send(NetworkConstants.REFRESH_ACTIVES, new RefreshActives() { actives = account.SerializeActives() });
                                }
                                break;
                        }
                    }
                }
            }
            return true;
        }
    }
}
