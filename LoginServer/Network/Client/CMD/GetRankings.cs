using JHSEngine.net.GateWay.Data;
using JHSNetProtocol;
using JHSRanking;
using LoginServer.Database;
using LoginServer.Engine;
using LoginServer.Engine.Managers;
using LoginServer.Engine.Service;
using LoginServer.MYSQL.Tables;
using LoginServer.Network.Data;
using Network.Server.Data;
using System;
using System.Collections.Generic;
using static CommonConstant;

namespace LoginServer.Network.CMD
{
    public class GetRankings : IJHSInterface
    {
        protected AccountManager DbManager;
        protected RankingManager rankingManager;

        public bool Execute(JHSNetworkMessage netMsg)
        {
            if (DbManager == null) DbManager = AccountManager.Instance;
            if (rankingManager == null) rankingManager = RankingManager.Instance;

            Empty packet = netMsg.ReadMessage<Empty>();
            if (packet != null)
            {
                uint connectionId = netMsg.conn.connectionId;
                AccountOBJ account = DbManager.GetOnlineByConnectionId(connectionId);
                if (account != null)
                {
                    

                    if (account.RankRequestTime > JHSTime.Time)
                    {
                        netMsg.conn.Send(NetworkConstants.UPDATE_RANKINGS, new RankingPacket()
                        {
                            PayLoadType = RankingPacketType.ERROR
                        });
                        return true;
                    }
                    account.RankRequestTime = JHSTime.Time + Settings.CAN_REQUEST_RANK_UPDATE_TIME;
                    RankinngOBJ rankings = rankingManager._GetPlayer(account);
                    if (rankings != null)
                    {
                        if(rankings.GameCount < Settings.MIN_GAMES_TO_DECIDE_RANKING_SKILLS)
                        {
                            netMsg.conn.Send(NetworkConstants.UPDATE_RANKINGS, new RankingPacket()
                            {
                                GameCount = rankings.GameCount < Settings.MIN_GAMES_TO_DECIDE_RANKING_SKILLS ? (byte)(Settings.MIN_GAMES_TO_DECIDE_RANKING_SKILLS - rankings.GameCount) : (byte)0,
                                PayLoadType = RankingPacketType.GAME_COUNT,
                            });
                            return true;
                        }
                       
                        LeagueData[] data = rankingManager._GetLegue(rankings.League);

                        netMsg.conn.Send(NetworkConstants.UPDATE_RANKINGS, new RankingPacket() {
                            CurrentLeague = rankings.League,
                            GameCount = (byte)rankings.GameCount,
                            LeaguePosition = (byte) (rankings.RankNo > 100 ? 255: rankings.RankNo),
                            PayLoadType = RankingPacketType.UPDATE_DATA,
                            rankingData = data,
                            LeaguePoints = (uint)rankings.LeaguePoints
                        });
                    }
                    
                }

            }
            return true;
        }
    }
}
