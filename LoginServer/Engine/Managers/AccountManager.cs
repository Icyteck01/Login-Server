using JHUI.Utils;
using LoginServer.Engine.Service;
using System.Collections.Generic;
using System.Linq;

namespace LoginServer.Database
{
    public sealed class AccountManager : JBehavor
    {
        static volatile AccountManager s_Instance;
        public static AccountManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    lock (s_Sync)
                    {
                        if (s_Instance == null)
                        {
                            s_Instance = new AccountManager();
                        }
                    }
                }
                return s_Instance;
            }
        }
        private Dictionary<uint, AccountOBJ> online = new Dictionary<uint, AccountOBJ>();
        private Dictionary<uint, uint> online_ref = new Dictionary<uint, uint>();
        private static object s_Sync = new object();

        private AccountManager()
        {

        }

        public int TotalPlayersOnline()
        {
           return online.Count;
        }

        public AccountOBJ GetAccountFromDbByUserName(string username)
        {
            return DbService.Instance.GetAccount(new AccountOBJ(username));
        }

        public AccountOBJ GetOnlineByUserId(uint userId)
        {
            if(online_ref.TryGetValue(userId, out uint id))
            {
                if (online.TryGetValue(id, out AccountOBJ value))
                    return value;
            }
            return null;
        }

        public bool IsOnline(uint connectionId)
        {
            lock (online)
            {
                return online.ContainsKey(connectionId);
            }
        }

        public bool IsOnline(string username)
        {
            return GetAccountOnlineByUserName(username) != null;
        }

        public void RegisterOnline(uint connectionId, AccountOBJ account)
        {
            lock (online)
            {
                lock (online_ref)
                {
                    account.IsOnline = true;
                    online_ref[account.Id] = connectionId;
                    online[connectionId] = account;               
                    LOG.Info("LoginHandler:: User Login :[" + account.Username + "] Connection ID:[" + connectionId + "] IP:[" + account.Ip + "]");
                }
            }
        }

        public AccountOBJ GetOnlineByConnectionId(uint connectionId)
        {
            lock (online)
            {
                if (online.TryGetValue(connectionId, out AccountOBJ account))
                    return account;
            }

            return null;
        }

        public AccountOBJ GetAccountOnlineByUserName(string username)
        {
            lock (online)
            {
                return online.FirstOrDefault(x => x.Value.Username == username).Value;
            }
        }

        public AccountOBJ GetAccountByUserName(string userName)
        {
            return GetAccountFromDbByUserName(userName);
        }

        public void RemoveOnline(uint connectionId)
        {
            lock (online)
            {
                lock (online_ref)
                {
                    if (online.TryGetValue(connectionId, out AccountOBJ user))
                    {
                        online_ref.Remove(user.Id);
                        online.Remove(connectionId);
                        LOG.Info("LoginHandler:: User Logout:[" + user.Username + "] Connection ID:[" + connectionId + "]");
                    }
                }
            }
           
        }
    }
}
