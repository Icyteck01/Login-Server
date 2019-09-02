using JHSNetProtocol;
using LoginServer.Database;
using LoginServer.Engine;
using LoginServer.Engine.Managers;
using LoginServer.Engine.Service;
using Network.Server.Data;
using static CommonConstant;

namespace Network.Server.CMD
{
    public class CMD_GETROLE : IJHSInterface
    {
        protected AccountManager DbManager;
        protected GameServerManager queueManager;
        protected DbService dbService;

        public bool Execute(JHSNetworkMessage netMsg)
        {
            if (DbManager == null) DbManager = AccountManager.Instance;
            if (queueManager == null) queueManager = GameServerManager.Instance;
            if (dbService == null) dbService = DbService.Instance;

            ReqeuestUser packet = netMsg.ReadMessage<ReqeuestUser>();
            if (packet != null)
            {
                uint connectionId = netMsg.conn.connectionId;
                var server = queueManager.GetServerByConnectionId(connectionId);
                if(server != null || Settings.DEBUG_GET_PLAYER)
                {
                    AccountOBJ user = DbManager.GetOnlineByUserId(packet.userId);
                    if(Settings.DEBUG_GET_PLAYER && user == null)
                    {
                        user = dbService.GetAccountFromDB(packet.userId);
                        user.SelectedCharacer = user.FirstRole().PlayerId;
                    }
                    if (user != null)
                    {
                        if(user.SelectedCharacer != 0)
                        {
                            DBPlayer role = user.GetRole(user.SelectedCharacer);
                            if (role != null)
                            {
                                if (packet.Type == InfoType.NONE)
                                {
                                    netMsg.conn.Send(GameServerOP.GETROLE, new LoginDataBasePlayer()
                                    {
                                        STATUS = SUCCESS,
                                        player = role,
                                        REQ = packet.Req
                                    });
                                }
                                else
                                {
                                    netMsg.conn.Send(GameServerOP.GET_ROLE2, new UpdateRole()
                                    {
                                        STATUS = SUCCESS,
                                        player = role,
                                        TYPE = packet.Type
                                    });
                                }
                                LOG.Info(string.Format("GETRole :: REQ[{3}] id[{0}] userid[{1}] serverid[{2}]", role.Base.PlayerId, role.Base.UserId, connectionId, packet.Req));
                                return true;
                            }
                        }
                       
                    }

                    netMsg.conn.Send(GameServerOP.GETROLE, new LoginDataBasePlayer() { STATUS = PLAYER_NOT_FOUND });
                }
            }
            return true;
        }
    }
}
