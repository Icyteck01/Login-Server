using Network.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoginServer.Engine.Managers
{
    public class Server
    {
        public uint connectionId;
        public GameMatchState status;
        public string IP = "";
        public short port;
    }

    public sealed class GameServerManager
    {
        private static object s_Sync = new object();
        static volatile GameServerManager s_Instance;
        public static GameServerManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    lock (s_Sync)
                    {
                        if (s_Instance == null)
                        {
                            s_Instance = new GameServerManager();
                        }
                    }
                }
                return s_Instance;
            }
        }

        private Dictionary<uint, Server> servers = new Dictionary<uint, Server>();

        public bool AddServer(uint connectionId, GameServerLogin packet)
        {
            lock (servers)
            {
                Server server = new Server() { connectionId = connectionId, status = packet.gameMatchState, IP = packet.IP, port = (short)packet.Port };
                servers[connectionId] = server;
               
                CheckServer(servers[connectionId]);

                LOG.Info("OnAddSession ServerId[" + connectionId + "] Status[" + packet.gameMatchState.ToString() + "]");
                return true;
            }
        }

        public void RemoveServer(uint connectionId)
        {
            lock (servers)
            {
                if (!servers.ContainsKey(connectionId))
                {
                    return;
                }
                servers.Remove(connectionId);
                LOG.Info("OnDelSession ServerId[" + connectionId + "]");
            }
        }

        public void UpdateServer(uint connectionId, GameMatchState status)
        {
            lock(servers)
            {
                if(servers.TryGetValue(connectionId, out Server server))
                {
                    servers[connectionId].status = status;
                }
            }
        }

        public Server GetServerByConnectionId(uint connectionId)
        {
            servers.TryGetValue(connectionId, out Server serv);
            return serv;
        }

        public Server GetFreeServer()
        {
            lock(servers)
            {
                return servers.Values.FirstOrDefault(x => x.status == GameMatchState.OVER || x.status == GameMatchState.LOBY);
            }
        }

        public void CheckServer(Server server)
        {

        }
    }
}
