using JHSNetProtocol;
using LoginServer.Database;
using LoginServer.Engine.Managers;
using LoginServer.Engine.Service;
using LoginServer.MYSQL.Tables;
using LoginServer.Network.Data;
using static CommonConstant;

namespace LoginServer.Network.CMD
{
    public class BuySkinHandler : IJHSInterface
    {
        protected AccountManager DbManager;
        public bool Execute(JHSNetworkMessage netMsg)
        {
            if (DbManager == null)
                DbManager = AccountManager.Instance;

            BuySkin packet = netMsg.ReadMessage<BuySkin>();
            if (packet != null)
            {
                uint connectionId = netMsg.conn.connectionId;
                AccountOBJ user = DbManager.GetOnlineByConnectionId(connectionId);
                if (user != null)
                {
                    ConfiModel model = ConfigManager.GetModel((int)packet.ModelId);
                    if(model == null)
                    {
                        netMsg.conn.Send(NetworkConstants.BUYCHARACTER, new BuySkinResponse() {STATUS = ITEM_CONFIG_WRONG });
                        return true;
                    }
                    //Silver Buy
                    if (packet.BuyType == 0)
                    {
                        //TO DO CHECK IF CHARACTER ALREADY HAS THE MODEL
                        lock(user)
                        {
                            if(user.Silver < model.SilverPrice)
                            {
                                netMsg.conn.Send(NetworkConstants.BUYCHARACTER, new BuySkinResponse() { STATUS = NOT_ENOUGH_SILVER });
                                return true;
                            }
                            Characters ccc = new Characters(user.Id, model.ModelId)
                            {
                                SkinColorId = model.SkinColorId,
                                EyeColorId = model.EyeColorId,
                                HairColorId = model.HairColorId,
                                ShirtColorId = model.ShirtColorId,
                                PantsColorId = model.PantsColorId,
                                BootsColorId = model.BootsColorId,
                                GlovesColorId = model.GlovesColorId
                            };
                            ccc.PlayerId = DbService.SaveEntity(ccc);
                            CharacterOBJ obj = new CharacterOBJ(ccc);
                            user.Skins.Add(obj.PlayerId, obj);
                            user.ResetNotification();
                            user.Silver -= model.SilverPrice;
                            DbService.SubmitUpdate2Queue(user);
                            netMsg.conn.Send(NetworkConstants.BUYCHARACTER, new BuySkinResponse() { STATUS = SUCCESS, Gold = (uint)user.Gold, Silver = (uint)user.Silver, character = obj.GetServerChar() });
                            return true;
                        }
                    }
                    else // GOLD BUY
                    {
                        
                        //TO DO CHECK IF CHARACTER ALREADY HAS THE MODEL
                        lock (user)
                        {
                            if (user.Gold < model.GoldPrice)
                            {
                                netMsg.conn.Send(NetworkConstants.BUYCHARACTER, new BuySkinResponse() { STATUS = NOT_ENOUGH_GOLD });
                                return true;
                            }
                            Characters ccc = new Characters(user.Id, model.ModelId)
                            {
                                SkinColorId = model.SkinColorId,
                                EyeColorId = model.EyeColorId,
                                HairColorId = model.HairColorId,
                                ShirtColorId = model.ShirtColorId,
                                PantsColorId = model.PantsColorId,
                                BootsColorId = model.BootsColorId,
                                GlovesColorId = model.GlovesColorId
                            };
                            ccc.PlayerId = DbService.SaveEntity(ccc);
                            CharacterOBJ obj = new CharacterOBJ(ccc);
                            user.Skins.Add(obj.PlayerId, obj);
                            user.Gold -= model.GoldPrice;
                            DbService.SubmitUpdate2Queue(user);
                            netMsg.conn.Send(NetworkConstants.BUYCHARACTER, new BuySkinResponse() { STATUS = SUCCESS, Gold = (uint)user.Gold, Silver = (uint)user.Silver, character = obj.GetServerChar() });
                            return true;
                        }
                    }
                }
            }

            return true;
        }
    }
}
