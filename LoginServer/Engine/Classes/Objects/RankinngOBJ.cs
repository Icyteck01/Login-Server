using System;
using Assets.Util;
using JHSRanking;
using LoginServer.Engine;
using LoginServer.MYSQL.Tables;

namespace LoginServer.Database
{
    public class RankinngOBJ : IDbEntity
    {
        private Rankings Data;
        public uint UserId { get { return Data.UserId; } set { Data.UserId = value; } }
        public string PlayerName { get { return Data.PlayerName; } set { Data.PlayerName = value; } }
        public int Kills { get { return Data.Kills; } set { Data.Kills = value; } }
        public int Deaths { get { return Data.Deaths; } set { Data.Deaths = value; } }
        public int LeaguePoints { get { return Data.LeaguePoints; } set { Data.LeaguePoints = value; } }
        public int GameCount { get { return Data.GameCount; } set { Data.GameCount = value; } }
        public int Index = 0;
        public int RankNo => Index + 1;
        public LeagueType League = LeagueType.NONE;
        public LeagueData RefLeagueData => new LeagueData() { UserId = UserId, LeaguePoints = LeaguePoints, PlayerName = PlayerName, Deaths = Deaths, Kills = Kills, GamePlayed = GameCount, League = League, AssignedLeague = LeagueType.NONE, Index = -1 };

        protected string hAsh = null;

        public RankinngOBJ(uint UserId, string PlayerName)
        {
            Data = new Rankings();
            this.UserId = UserId;
            this.PlayerName = PlayerName;
        }

        public RankinngOBJ(Rankings ob)
        {
            Data = ob;
        }

        public object GetEntity()
        {
            Data.League = (int)League;
            return Data;
        }

        public string GetId()
        {
            if (hAsh == null)
                hAsh = "RankOBJJ_" + PlayerName.CreateMD5();

            return hAsh;
        }

        public override string ToString()
        {
            return RankNo + "   -   " + PlayerName + "   -   " + LeaguePoints;
        }
    }
}
