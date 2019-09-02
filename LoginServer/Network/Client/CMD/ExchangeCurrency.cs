using JHSNetProtocol;
using LoginServer.Database;
using LoginServer.Engine;
using LoginServer.Engine.Service;
using LoginServer.Network.Data;
using static CommonConstant;

namespace LoginServer.Network.CMD
{
    public class ExchangeCurrency : IJHSInterface
    {
        protected AccountManager DbManager;
        public const int PRICE_PER_COLOR_CHANGE = 10;

        public bool Execute(JHSNetworkMessage netMsg)
        {
            if (DbManager == null)
                DbManager = AccountManager.Instance;

            ExchangeCur packet = netMsg.ReadMessage<ExchangeCur>();
            if (packet != null)
            {
                uint connectionId = netMsg.conn.connectionId;
                AccountOBJ user = DbManager.GetOnlineByConnectionId(connectionId);
                if (user != null)
                {
                    switch(packet.CurencyType)
                    {
                        case 0:// SILVER TO GOLD
                            lock (user)
                            {
                                int Total = (int)packet.Value * Settings.SILVER_EXCAHNGE_RATE;
                                if (user.Silver < packet.Value)
                                {
                                    netMsg.conn.Send(NetworkConstants.EXCHANGE, new ExchangeCurResp() { STATUS = NOT_ENOUGH_SILVER });
                                    return true;
                                }
                                user.Silver -= (int)packet.Value;
                                user.Gold += Total;
                                DbService.SubmitUpdate2Queue(user);
                                user.ResetNotification();
                                netMsg.conn.Send(NetworkConstants.EXCHANGE, new ExchangeCurResp() { STATUS = SUCCESS, SilverValue = (uint)user.Silver, GoldValue = (uint)user.Gold});
                                return true;
                            }
                        case 1:// GOLD TO SILVER
                            lock(user)
                            {
                                int Total = (int)packet.Value * Settings.GOLD_EXCAHNGE_RATE;
                                if (user.Gold < packet.Value)
                                {
                                    netMsg.conn.Send(NetworkConstants.EXCHANGE, new ExchangeCurResp() { STATUS = NOT_ENOUGH_GOLD });
                                    return true;
                                }
                                user.Gold -= (int)packet.Value;
                                user.Silver += Total;
                                user.ResetNotification();
                                DbService.SubmitUpdate2Queue(user);
                                netMsg.conn.Send(NetworkConstants.EXCHANGE, new ExchangeCurResp() { STATUS = SUCCESS, SilverValue = (uint)user.Silver, GoldValue = (uint)user.Gold });
                                return true;
                            }
                    }
                }
                else
                {
                    //DO NOTHING
                    return true;
                }

            }
            return true;
        }
    }
}
