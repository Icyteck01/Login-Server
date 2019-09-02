using System;

namespace LoginServer.MYSQL.Tables
{
    public class Users
    {
        private uint _id = 0;
        [Index(true)]
        public virtual uint UserId
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _UserName;
        public virtual string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        private string _PlayerName;
        public virtual string PlayerName
        {
            get { return _PlayerName; }
            set { _PlayerName = value; }
        }

        private string _PassWord;
        public virtual string password
        {
            get { return _PassWord; }
            set { _PassWord = value; }
        }

        private string _email;
        public virtual string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        private string _ip;
        public virtual string ip
        {
            get { return _ip; }
            set { _ip = value; }
        }

        private string _lastKnownIp;
        public virtual string lastKnownIp
        {
            get { return _lastKnownIp; }
            set { _lastKnownIp = value; }
        }

        private DateTime _createTime;
        public virtual DateTime createTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }

        private DateTime _loginTime;
        public virtual DateTime loginTime
        {
            get { return _loginTime; }
            set { _loginTime = value; }
        }

        private DateTime _logoutTime;
        public virtual DateTime logoutTime
        {
            get { return _logoutTime; }
            set { _logoutTime = value; }
        }

        private int _level;
        public virtual int Level
        {
            get { return _level; }
            set { _level = value; }
        }

        private int _exp;
        public virtual int Exp
        {
            get { return _exp; }
            set { _exp = value; }
        }

        private int _gold;
        public virtual int Gold
        {
            get { return _gold; }
            set { _gold = value; }
        }

        private int _silver;
        public virtual int Silver
        {
            get { return _silver; }
            set { _silver = value; }
        }

        private int _Priviledge;
        public virtual int Priviledge
        {
            get { return _Priviledge; }
            set { _Priviledge = value; }
        }


        public Users()
        {
        }
    }
}
