using JHSNetProtocol;
using LoginServer.Database;
using LoginServer.Engine;
using LoginServer.Engine.Service;
using LoginServer.Network.Data;
using Network.Server.Data;
using static CommonConstant;

namespace LoginServer.Network.Client.CMD
{
    public class RequestActives : IJHSInterface
    {
        protected AccountManager DbManager;

        public bool Execute(JHSNetworkMessage netMsg)
        {
            if (DbManager == null)
                DbManager = AccountManager.Instance;

            Empty packet = netMsg.ReadMessage<Empty>();
            if (packet != null)
            {
                uint connectionId = netMsg.conn.connectionId;
                AccountOBJ account = DbManager.GetOnlineByConnectionId(connectionId);
                if (account != null)
                {
                    netMsg.conn.Send(NetworkConstants.REFRESH_ACTIVES, new RefreshActives() { actives = account.SerializeActives() });
                }
            }

            return true;
        }
    }
}
