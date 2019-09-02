using LoginServer.Engine.Managers;
using LoginServer.MYSQL.Tables;
using LoginServer.Network.Data;

namespace LoginServer.Database
{
    public class CharacterOBJ : IDbEntity
    {
        private Characters Data;
        private ConfiModel conf;

        public CharacterOBJ(Characters character)
        {
            Data = character;
        }

        public uint connectionID = 0;

        public uint PlayerId { get { return Data.PlayerId; } set { Data.PlayerId = value; } }

        public uint UserId { get { return Data.UserId; } set { Data.UserId = value; } }

        public short SkinId { get { return Data.SkinId; } set { Data.SkinId = value; } }

        public byte SkinColorId { get { return Data.SkinColorId; } set { Data.SkinColorId = value; } }
        public byte EyeColorId { get { return Data.EyeColorId; } set { Data.EyeColorId = value; } }
        public byte HairColorId { get { return Data.HairColorId; } set { Data.HairColorId = value; } }
        public byte ShirtColorId { get { return Data.ShirtColorId; } set { Data.ShirtColorId = value; } }
        public byte PantsColorId { get { return Data.PantsColorId; } set { Data.PantsColorId = value; } }
        public byte BootsColorId { get { return Data.BootsColorId; } set { Data.BootsColorId = value; } }
        public byte GlovesColorId { get { return Data.GlovesColorId; } set { Data.GlovesColorId = value; } }

        public ServerCharacter GetServerChar()
        {
            return new ServerCharacter()
            {
                PlayerId = PlayerId,
                ModelId = (uint)SkinId,
                IsChanged = Changed,
                SkinColorId = SkinColorId,
                HairColorId = HairColorId,
                EyeColorId = EyeColorId,
                ShirtColorId = ShirtColorId,
                PantsColorId = PantsColorId,
                BootsColorId = BootsColorId,
                GlovesColorId = GlovesColorId,

            };
        }

        public string GetId()
        {
            return "CharacterOBJ" + PlayerId;
        }

        public object GetEntity()
        {
            return Data;
        }

        public bool Changed
        {
            get
            {
                if (conf == null)
                    conf = ConfigManager.GetModel(Data.SkinId);

                return conf.SkinColorId != SkinColorId
                    || conf.HairColorId != HairColorId
                    || conf.EyeColorId != EyeColorId
                    || conf.ShirtColorId != ShirtColorId
                    || conf.PantsColorId != PantsColorId
                    || conf.BootsColorId != BootsColorId
                    || conf.GlovesColorId != GlovesColorId;

            }
        }
    }
}
