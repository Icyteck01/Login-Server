using JHSNetProtocol;
using LoginServer.Database;
using LoginServer.Engine.Managers;
using LoginServer.Engine.Service;
using LoginServer.MYSQL.Tables;
using LoginServer.Network.Data;
using static CommonConstant;

namespace LoginServer.Network.CMD
{
    public class DeleteCharacterCommand : IJHSInterface
    {
        protected AccountManager DbManager;
        public bool Execute(JHSNetworkMessage netMsg)
        {
            if (DbManager == null)
                DbManager = AccountManager.Instance;

            DeleteCharacter packet = netMsg.ReadMessage<DeleteCharacter>();
            if (packet != null)
            {
                uint connectionId = netMsg.conn.connectionId;
                AccountOBJ user = DbManager.GetOnlineByConnectionId(connectionId);
                if (user != null)
                {
                    lock (user)
                    {
                        if(user.Skins.Count <= 1)
                        {
                            netMsg.conn.Send(NetworkConstants.DELETE_CHARACTER, new DeleteCharacterResponse() { STATUS = NOT_DELETE_LAST_CHARACTER });
                            return true;
                        }
                        CharacterOBJ character = user.GetPlayer(packet.PlayerId);
                        if (character != null)
                        {
                            ConfiModel model = ConfigManager.GetModel(character.SkinId);
                            if (model == null)
                            {
                                netMsg.conn.Send(NetworkConstants.DELETE_CHARACTER, new DeleteCharacterResponse() { STATUS = ITEM_CONFIG_WRONG });
                                return true;
                            }
                            uint playerId = character.PlayerId;
                            if (DbService.RemoveEntity(character.GetEntity()) && user.Skins.Remove(playerId))
                            {
                           
                                user.Silver += model.SilverPrice;
                                user.ResetNotification();
                                DbService.SubmitUpdate2Queue(user);
                                netMsg.conn.Send(NetworkConstants.DELETE_CHARACTER, new DeleteCharacterResponse() { STATUS = SUCCESS, PlayerId = playerId, Gold = (uint)user.Gold, Silver = (uint)user.Silver });
                            }
                            else
                            {
                                netMsg.conn.Send(NetworkConstants.DELETE_CHARACTER, new DeleteCharacterResponse() { STATUS = PLAYER_NOT_FOUND });
                            }

                        }
                        else
                        {
                            netMsg.conn.Send(NetworkConstants.DELETE_CHARACTER, new DeleteCharacterResponse() { STATUS = PLAYER_NOT_FOUND });
                        }
                    }
                }
            }
            return true;
        }
    }
}
