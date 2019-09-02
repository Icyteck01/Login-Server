using System;

namespace LoginServer.MYSQL.Tables
{
    public class Characters
    {
        private uint _id = 0;
        [Index(true)]
        public virtual uint PlayerId
        {
            get { return _id; }
            set { _id = value; }
        }

        private uint _userId;
        public virtual uint UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        private short _skinId;
        public virtual short SkinId
        {
            get { return _skinId; }
            set { _skinId = value; }
        }

        private byte _SkinColorId;
        public virtual byte SkinColorId
        {
            get { return _SkinColorId; }
            set { _SkinColorId = value; }
        }
        private byte _EyeColorId;
        public virtual byte EyeColorId
        {
            get { return _EyeColorId; }
            set { _EyeColorId = value; }
        }
        private byte _HairColorId;
        public virtual byte HairColorId
        {
            get { return _HairColorId; }
            set { _HairColorId = value; }
        }
        private byte _ShirtColorId;
        public virtual byte ShirtColorId
        {
            get { return _ShirtColorId; }
            set { _ShirtColorId = value; }
        }
        private byte _PantsColorId;
        public virtual byte PantsColorId
        {
            get { return _PantsColorId; }
            set { _PantsColorId = value; }
        }
        private byte _BootsColorId;
        public virtual byte BootsColorId
        {
            get { return _BootsColorId; }
            set { _BootsColorId = value; }
        }
        private byte _GlovesColorId;
        public virtual byte GlovesColorId
        {
            get { return _GlovesColorId; }
            set { _GlovesColorId = value; }
        }
        public Characters()
        {

        }

        public Characters(uint user, short shipId)
        {

            UserId = user;
            SkinId = shipId;
        }
    }
}
