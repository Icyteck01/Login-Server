namespace LoginServer.MYSQL.Tables
{
    public class Actives
    {
        private uint _Id;
        [Index(true)]
        public virtual uint Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        private uint _userId;
        public virtual uint UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        private int _aID;
        public virtual int ActiveId
        {
            get { return _aID; }
            set { _aID = value; }
        }

        private int _value;
        public virtual int value
        {
            get { return _value; }
            set { _value = value; }
        }

        private bool _collected;
        public virtual bool collected
        {
            get { return _collected; }
            set { _collected = value; }
        }
    }
}
