namespace LoginServer.MYSQL.Tables
{
    public class Rankings
    {
        private uint _id = 0;
        [Index]
        public virtual uint UserId
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _PlayerName;
        public virtual string PlayerName
        {
            get { return _PlayerName; }
            set { _PlayerName = value; }
        }

        private int _League;
        public virtual int League
        {
            get { return _League; }
            set { _League = value; }
        }

        private int _LeguePoints;
        public virtual int LeaguePoints
        {
            get { return _LeguePoints; }
            set { _LeguePoints = value; }
        }

        private int _GameCount;
        public virtual int GameCount
        {
            get { return _GameCount; }
            set { _GameCount = value; }
        }

        private int _kills;
        public virtual int Kills
        {
            get { return _kills; }
            set { _kills = value; }
        }

        private int _deaths;
        public virtual int Deaths
        {
            get { return _deaths; }
            set { _deaths = value; }
        }

    }
}
