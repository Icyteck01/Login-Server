using System;
using Assets.Util;
using JHSEngine.net.GateWay.Data;
using JHSNetProtocol;
using LoginServer.Database;
using LoginServer.Engine;
using LoginServer.Engine.Managers;
using LoginServer.Engine.Service;
using LoginServer.Network.Data;
using static CommonConstant;

namespace LoginServer.Network.CMD
{
    public class UserLogin : IJHSInterface
    {
        protected AccountManager DbManager;

        public bool Execute(JHSNetworkMessage netMsg)
        {
            if (DbManager == null) DbManager = AccountManager.Instance;

            GWLoginPacket packet = netMsg.ReadMessage<GWLoginPacket>();
            if(packet != null)
            {
                uint connectionId = netMsg.conn.connectionId;

                AccountOBJ user = DbManager.GetAccountFromDbByUserName(packet.LoginName);
                if(user != null)
                {
                    lock (user)
                    {
                        string userName = user.Username;
                        if (userName.VerifyPassword(user.Password, packet.Password))
                        {

                            //RESPONSE LOGIC
                            lock (user)
                            {
                                user.connectionID = connectionId;
                                user.LoginTime = DateTime.Now;
                                user.LastKnownIp = user.Ip;
                                user.Ip = netMsg.conn.IP;
                                user.OnLogin();
                                DbService.SubmitUpdate2Queue(user);
                            }
                          //  RankinData ranking = rankingManager.GetUser(user);

                            GWLoginResponsePacket dat = new GWLoginResponsePacket
                            {
                                actives = user.GetActiveCompleate(),
                                characters = user.SerializeChars(),
                                RESPONSE = SUCCESS,
                                League = user.League,
                                LeaguePosition = user.LeaguePosition,
                                LEGUE_STATUS = user.LegueStatus,
                                PlayerNick = user.PlayerName,
                                Gold = (uint)user.Gold,
                                Silver = (uint)user.Silver,
                                Priviledge = (byte)user.Priviledge,
                                LoginTocken = user.Id,
                                LEVEL = (uint)user.Data.Level,
                                EXP = (uint)user.Data.Exp,
                                IsPushLevelUp = user.LevelUpNotify,
                                SEASON = (byte)Settings.RANKING_SEASON,
                                GameCount = user.GameCount < Settings.MIN_GAMES_TO_DECIDE_RANKING_SKILLS ? (byte)(Settings.MIN_GAMES_TO_DECIDE_RANKING_SKILLS - user.GameCount)  : (byte)0
                                
                            };

                            DbManager.RegisterOnline(connectionId, user);
                            JHSNetworkServer.Send(connectionId, NetworkConstants.LOGIN, dat);
                        }
                        else
                        {
                            JHSNetworkServer.Send(connectionId, NetworkConstants.LOGIN, new GWLoginResponsePacket() { RESPONSE = WRONG_PASSWORD_OR_LOGIN });
                        }
                    }
                }
                else
                {
                    if(Settings.AutoCreate)
                    {
                        user = CreateAccount(packet);
                        lock(user)
                        {
                            user.connectionID = connectionId;
                            user.LoginTime = DateTime.Now;
                            user.LastKnownIp = user.Ip;
                            user.Ip = netMsg.conn.IP;
                            user.OnLogin();
                            DbService.UpdateEntityIntime(user);
                        }
                        user = DbManager.GetAccountFromDbByUserName(packet.LoginName);
                        if (user != null)
                        {
                            GWLoginResponsePacket dat = new GWLoginResponsePacket
                            {
                                actives = user.GetActiveCompleate(),
                                characters = user.SerializeChars(),
                                RESPONSE = SUCCESS,
                                League = user.League,
                                LeaguePosition = user.LeaguePosition,
                                LEGUE_STATUS = user.LegueStatus,
                                PlayerNick = user.PlayerName,
                                Gold = (uint)user.Gold,
                                Silver = (uint)user.Silver,
                                Priviledge = (byte)user.Priviledge,
                                LoginTocken = user.Id,
                                LEVEL = (uint)user.Data.Level,
                                EXP = (uint)user.Data.Exp,
                                IsPushLevelUp = user.LevelUpNotify,
                                SEASON = (byte)Settings.RANKING_SEASON,
                                GameCount = user.GameCount < 5 ? (byte)(5 - user.GameCount) : (byte)0

                            };
                            DbManager.RegisterOnline(connectionId, user);
                            JHSNetworkServer.Send(connectionId, NetworkConstants.LOGIN, dat);
                        }
                    }
                    else
                    {
                        JHSNetworkServer.Send(connectionId, NetworkConstants.LOGIN, new GWLoginResponsePacket() { RESPONSE = USER_NOT_FOUND });
                    }
                }
            }

            return true;
        }

        private AccountOBJ CreateAccount(GWLoginPacket packet)
        {
            AccountOBJ user = new AccountOBJ()
            {
                Username = packet.LoginName,
                Password = (packet.LoginName + packet.Password).GetPassword(),
                PlayerName = "Test",
                Email = "EEEE",
                CreateTime = DateTime.Now
            };        
            return user;
        }
    }
}
