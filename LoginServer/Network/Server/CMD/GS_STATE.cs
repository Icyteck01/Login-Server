using JHSNetProtocol;
using LoginServer.Engine;
using LoginServer.Engine.Managers;
using Network.Server.Data;

namespace Network.Server.CMD
{
    public class GS_STATE : IJHSInterface
    {
        protected GameServerManager queueManager;
        public bool Execute(JHSNetworkMessage netMsg)
        {
            if (queueManager == null)
                queueManager = GameServerManager.Instance;

            GameServerLogin packet = netMsg.ReadMessage<GameServerLogin>();
            if (packet != null)
            {
                uint connectionId = netMsg.conn.connectionId;
                var server = queueManager.GetServerByConnectionId(connectionId);
                if (server == null)
                {
                    if (Settings.BINDPASSWORD == packet.PassWord)
                    {
                        if (queueManager.AddServer(connectionId, packet))
                        {
                            netMsg.conn.Send(GameServerOP.UPDATE_STATE, packet);
                        }
                    }
                }
                else
                {               
                    if(server != null)
                    {
                        lock (server)
                        {
                            server.status = packet.gameMatchState;
                            LOG.Info("OnChangeSession ServerId[" + connectionId + "] Status[" + packet.gameMatchState.ToString() + "]");
                        }
                    }
                }
            }
            return true;
        }
    }
}
