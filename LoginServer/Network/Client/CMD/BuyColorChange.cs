using JHSNetProtocol;
using LoginServer.Database;
using LoginServer.Engine;
using LoginServer.Engine.Service;
using LoginServer.Network.Data;
using static CommonConstant;

namespace LoginServer.Network.CMD
{
    public class BuyColorChange : IJHSInterface
    {
        protected AccountManager DbManager;

        public bool Execute(JHSNetworkMessage netMsg)
        {
            if (DbManager == null)
                DbManager = AccountManager.Instance;

            GWMainMenu packet = netMsg.ReadMessage<GWMainMenu>();
            if (packet != null)
            {
                uint connectionId = netMsg.conn.connectionId;
                AccountOBJ user = DbManager.GetOnlineByConnectionId(connectionId);
                if (user != null)
                {
                    int total = 0;
                    if (packet.SkinColorId_changed)
                        total += Settings.PRICE_PER_COLOR_CHANGE;
                    if (packet.HairColorId_changed)
                        total += Settings.PRICE_PER_COLOR_CHANGE;
                    if (packet.EyeColorId_changed)
                        total += Settings.PRICE_PER_COLOR_CHANGE;
                    if (packet.ShirtColorId_changed)
                        total += Settings.PRICE_PER_COLOR_CHANGE;
                    if (packet.PantsColorId_changed)
                        total += Settings.PRICE_PER_COLOR_CHANGE;
                    if (packet.BootsColorId_changed)
                        total += Settings.PRICE_PER_COLOR_CHANGE;
                    if (packet.GlovesColorId_changed)
                        total += Settings.PRICE_PER_COLOR_CHANGE;

                    if(total == 0)
                        return true;

                    lock (user)
                    {
                        if(user.Silver < total )
                        {
                            netMsg.conn.Send(NetworkConstants.BUYCOLORCHANGE, new HeroBuyColorResponse() { STATUS = NOT_ENOUGH_SILVER });
                            return true;
                        }
                        CharacterOBJ player = user.GetPlayer(packet.PlayerId);

                        if (packet.SkinColorId_changed)
                            player.SkinColorId = packet.SkinColorId;
                        if (packet.HairColorId_changed)
                            player.HairColorId = packet.HairColorId;
                        if (packet.EyeColorId_changed)
                            player.EyeColorId = packet.EyeColorId;
                        if (packet.ShirtColorId_changed)
                            player.ShirtColorId = packet.ShirtColorId;
                        if (packet.PantsColorId_changed)
                            player.PantsColorId = packet.PantsColorId;
                        if (packet.BootsColorId_changed)
                            player.BootsColorId = packet.BootsColorId;
                        if (packet.GlovesColorId_changed)
                            player.GlovesColorId = packet.GlovesColorId;

                        DbService.SubmitUpdate2Queue(player);
                        user.Silver = user.Silver - total;
                        user.ResetNotification();
                        DbService.SubmitUpdate2Queue(user);
                        HeroBuyColorResponse response = new HeroBuyColorResponse()
                        {
                            STATUS = 255,
                            PlayerId = packet.PlayerId,
                            SkinColorId = packet.SkinColorId,
                            SkinColorId_changed = packet.SkinColorId_changed,
                            HairColorId = packet.HairColorId,
                            HairColorId_changed = packet.HairColorId_changed,
                            EyeColorId = packet.EyeColorId,
                            EyeColorId_changed = packet.EyeColorId_changed,
                            ShirtColorId = packet.ShirtColorId,
                            ShirtColorId_changed = packet.ShirtColorId_changed,
                            PantsColorId = packet.PantsColorId,
                            PantsColorId_changed = packet.PantsColorId_changed,
                            BootsColorId = packet.BootsColorId,
                            BootsColorId_changed = packet.BootsColorId_changed,
                            GlovesColorId = packet.GlovesColorId,
                            GlovesColorId_changed = packet.GlovesColorId_changed,
                        };
                        response.STATUS = SUCCESS;
                        response.Silver = (uint)user.Silver;

                        netMsg.conn.Send(NetworkConstants.BUYCOLORCHANGE, response);
                        return true;
                    }
                }
                else
                {
                    netMsg.conn.Send(NetworkConstants.BUYCOLORCHANGE, new HeroBuyColorResponse() { STATUS = USER_NOT_FOUND });
                }
            }
            return true;
        }
    }
}
