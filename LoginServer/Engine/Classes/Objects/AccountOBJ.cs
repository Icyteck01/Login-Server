using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Util;
using JHSRanking;
using LoginServer.Engine.Managers;
using LoginServer.Engine.Service;
using LoginServer.MYSQL.Tables;
using LoginServer.Network.Data;
using LoginServer.Utils;
using Network.Server.Data;

namespace LoginServer.Database
{
    public class AccountOBJ : IDbEntity
    {
        #region DATA
        public uint connectionID = 0;
        public bool IsOnline = false;
        public bool InQueue = false;
        public bool LevelUpNotify = false;
        public LeagueStatus LegueStatus = LeagueStatus.STATIC;
        public byte LeaguePosition = 255;
        public uint SelectedCharacer = 0;
        public float RankRequestTime = 0;
        
        //RANK
        public int LeaguePoints { get; private set; }
        public int Deaths { get; private set; }
        public int Kills { get; private set; }
        public LeagueType League { get; private set; }
        public int GameCount { get; private set; }

        public Users Data = null;

        public SortedList<uint, CharacterOBJ> Skins = new SortedList<uint, CharacterOBJ>();

        public SortedList<int, ActivesOBJ> Actives = new SortedList<int, ActivesOBJ>();
        #endregion

        #region CONSTRUCTORS
        public AccountOBJ() { Data = new Users(); }

        public AccountOBJ(Users data)
        {
            Data = data;
        }

        public AccountOBJ(string userName)
        {
            Data = new Users();
            Username = userName;
        }
        #endregion

        #region MYSQL DATA EXTENSIONS
        public uint Id
        {
            get
            {
                return Data.UserId;
            }
            set
            {
                Data.UserId = value;
            }
        }

        public string Username
        {
            get
            {
                return Data.UserName;
            }
            set
            {
                Data.UserName = value;
            }
        }

        public string Password
        {
            get
            {
                return Data.password;
            }
            set
            {
                Data.password = value;
            }
        }

        public string PlayerName
        {
            get
            {
                return Data.PlayerName;
            }
            set
            {
                Data.PlayerName = value;
            }
        }

        public string Email
        {
            get
            {
                return Data.Email;
            }
            set
            {
                Data.Email = value;
            }
        }

        public string Ip
        {
            get
            {
                return Data.ip;
            }
            set
            {
                Data.ip = value;
            }
        }

        public string LastKnownIp
        {
            get
            {
                return Data.lastKnownIp;
            }
            set
            {
                Data.lastKnownIp = value;
            }
        }

        public DateTime CreateTime
        {
            get
            {
                return Data.createTime;
            }
            set
            {
                Data.createTime = value;
            }
        }

        public DateTime LoginTime
        {
            get
            {
                return Data.loginTime;
            }
            set
            {
                Data.loginTime = value;
            }
        }

        public DateTime LogoutTime
        {
            get
            {
                return Data.logoutTime;
            }
            set
            {
                Data.logoutTime = value;
            }
        }

        public int Gold
        {
            get
            {
                return Data.Gold;
            }
            set
            {
                Data.Gold = value;
            }
        }

        public int Silver
        {
            get
            {
                return Data.Silver;
            }
            set
            {
                Data.Silver = value;
            }
        }

        public int Level
        {
            get
            {
                return Data.Level;
            }
            set
            {
                Data.Level = value;
            }
        }

        public int Exp
        {
            get
            {
                return Data.Exp;
            }
            set
            {
                Data.Exp = value;
            }
        }

        public int Priviledge { get { return Data.Priviledge; } set { Data.Priviledge = value; } }

        #endregion

        #region INTERFACE
        protected string hAsh = null;

        public string GetId()
        {
            if (hAsh == null)
                hAsh = "AccountOBJ_" + Username.CreateMD5();

            return hAsh;
        }

        public object GetEntity()
        {
            return Data;
        }
        #endregion

        #region SERIALIZATION HELPER
        public ServerCharacter[] SerializeChars()
        {
            CharacterOBJ[] chars = Skins.Values.ToArray();
            List<ServerCharacter> ret = new List<ServerCharacter>();
            for (int i = 0; i < chars.Length; i++)
            {
                ret.Add(chars[i].GetServerChar());
            }
            return ret.ToArray();
        }

        public ServerActives[] SerializeActives()
        {
            ActivesOBJ[] chars = Actives.Values.ToArray();
            List<ServerActives> ret = new List<ServerActives>();

            for (int i = 0; i < chars.Length; i++)
            {
                if (!chars[i].Collected)
                {
                    ServerActives act = chars[i].GetActive();
                    ret.Add(act);
                }
            }
            return ret.ToArray();
        }

        public uint[] GetActiveCompleate()
        {
            ActivesOBJ[] chars = Actives.Values.ToArray();
            List<uint> ret = new List<uint>();
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i].Compleated)
                {
                    ret.Add((uint)chars[i].ActiveId);
                }
            }
            return ret.ToArray();
        }
        #endregion

        #region FAST ACCESS HELPERS
        public CharacterOBJ GetPlayer(uint roleid)
        {
            if (Skins.TryGetValue(roleid, out CharacterOBJ chara))
                return chara;

            return null;
        }

        public ActivesOBJ GetActive(int activeId)
        {
            if (Actives.TryGetValue(activeId, out ActivesOBJ active))
            {
                return active;
            }
            return null;
        }

        public DBPlayer GetRole(uint roleid)
        {
            CharacterOBJ c = GetPlayer(roleid);
            if (c != null)
            {
                DBPlayer player = new DBPlayer
                {
                    Base = new DBPlayerBase()
                    {
                        UserId = Id,
                        PlayerId = roleid,
                        ModelId = (byte)c.SkinId
                    },
                    Custom = new DBCustomisation()
                    {
                        PlayerName = PlayerName,
                        SkinColorId = c.SkinColorId,
                        HairColorId = c.HairColorId,
                        EyeColorId = c.EyeColorId,
                        ShirtColorId = c.ShirtColorId,
                        PantsColorId = c.PantsColorId,
                        BootsColorId = c.BootsColorId,
                        GlovesColorId = c.GlovesColorId
                    },
                    Status = new DBStatus()
                    {
                        Level = (uint)Level,
                        Exp = (uint)Exp,
                        MMR = (uint)LeaguePoints,
                        Kills = (uint)Kills,
                        Deaths = (uint)Deaths,
                        Golden = (uint)Gold,
                        Silver = (uint)Silver
                    }
                };

                List<DBPlayerActive> activx = new List<DBPlayerActive>();
                foreach (KeyValuePair<int, ActivesOBJ> a in Actives)
                {
                    if (a.Value.CanNetSend())
                    {
                        activx.Add(new DBPlayerActive() { ActiveId = (uint)a.Value.ActiveId, Value = (uint)a.Value.Value });
                    }
                }

                player.Actives = activx.ToArray();
                return player;
            }

            return default(DBPlayer);
        }

        public CharacterOBJ FirstRole()
        {
            return Skins.First().Value;
        }
        #endregion

        #region EVENTS
        public void OnLogin()
        {
            int curExp = Data.Exp;
            int NeedExp = RewardHelper.EXPNeeded(Data.Level);
            bool LevelUp = curExp >= NeedExp;
            if (LevelUp)
            {
                Data.Exp = 0;
                int NextLevel = Data.Level + 1;
                if (NextLevel < 100)
                {
                    Data.Level = NextLevel;
                    LevelUpNotify = true;
                }
            }
            RankinngOBJ Rdata = RankingManager.GetPlayer(this);
            if (Rdata != null)
            {
                Deaths = Rdata.Deaths;
                Kills = Rdata.Deaths;
                League = Rdata.League;
                LeaguePoints = Rdata.LeaguePoints;
                GameCount = Rdata.GameCount;
                LeaguePosition = Rdata.RankNo <= 100 ? (byte)Rdata.RankNo : (byte)101;
            }
        }

        public void OnLogout()
        {
            connectionID = 0;
            LogoutTime = DateTime.Now;
            SelectedCharacer = 0;
            IsOnline = false;
            DbService.SubmitUpdate2Queue(this);
        }

        public void OnGameResultRecieved()
        {
            RankinngOBJ Rdata = RankingManager.GetPlayer(this);
            if (Rdata != null)
            {
                Deaths = Rdata.Deaths;
                Kills = Rdata.Deaths;
                League = Rdata.League;
                LeaguePoints = Rdata.LeaguePoints;
                LeaguePosition = Rdata.RankNo <= 100 ? (byte)Rdata.RankNo : (byte)101;
            }
        }

        public void ResetNotification()
        {
            LevelUpNotify = false;
        }
        #endregion
    }
}
