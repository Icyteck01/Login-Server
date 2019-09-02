using JHSNetProtocol;
using LoginServer.Engine.Enums;

namespace LoginServer.Network.Data
{
    public class GWMainMenu : JHSMessageBase
    {
        public uint PlayerId;
        public byte SkinColorId = 0;
        public bool SkinColorId_changed = false;
        public byte HairColorId = 0;
        public bool HairColorId_changed = false;
        public byte EyeColorId = 0;
        public bool EyeColorId_changed = false;
        public byte ShirtColorId = 0;
        public bool ShirtColorId_changed = false;
        public byte PantsColorId = 0;
        public bool PantsColorId_changed = false;
        public byte BootsColorId = 0;
        public bool BootsColorId_changed = false;
        public byte GlovesColorId = 0;
        public bool GlovesColorId_changed = false;

        public override void Deserialize(JHSNetworkReader reader)
        {
            PlayerId = reader.ReadPackedUInt32();
            SkinColorId_changed = reader.ReadBoolean();
            HairColorId_changed = reader.ReadBoolean();
            EyeColorId_changed = reader.ReadBoolean();
            ShirtColorId_changed = reader.ReadBoolean();
            PantsColorId_changed = reader.ReadBoolean();
            BootsColorId_changed = reader.ReadBoolean();
            GlovesColorId_changed = reader.ReadBoolean();

            if (SkinColorId_changed)
                SkinColorId = reader.ReadByte();

            if (HairColorId_changed)
                HairColorId = reader.ReadByte();

            if (EyeColorId_changed)
                EyeColorId = reader.ReadByte();

            if (ShirtColorId_changed)
                ShirtColorId = reader.ReadByte();

            if (PantsColorId_changed)
                PantsColorId = reader.ReadByte();

            if (BootsColorId_changed)
                BootsColorId = reader.ReadByte();

            if (GlovesColorId_changed)
                GlovesColorId = reader.ReadByte();
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.WritePackedUInt32(PlayerId);
            writer.Write(SkinColorId_changed);
            writer.Write(HairColorId_changed);
            writer.Write(EyeColorId_changed);
            writer.Write(ShirtColorId_changed);
            writer.Write(PantsColorId_changed);
            writer.Write(BootsColorId_changed);
            writer.Write(GlovesColorId_changed);

            if (SkinColorId_changed)
                writer.Write(SkinColorId);

            if (HairColorId_changed)
                writer.Write(HairColorId);

            if (EyeColorId_changed)
                writer.Write(EyeColorId);

            if (ShirtColorId_changed)
                writer.Write(ShirtColorId);

            if (PantsColorId_changed)
                writer.Write(PantsColorId);

            if (BootsColorId_changed)
                writer.Write(BootsColorId);

            if (GlovesColorId_changed)
                writer.Write(GlovesColorId);
        }
    }

    public class HeroBuyColorResponse : JHSMessageBase
    {
        public byte STATUS = 0;
        public uint PlayerId;
        public uint Silver = 0;
        public byte SkinColorId = 0;
        public bool SkinColorId_changed = false;
        public byte HairColorId = 0;
        public bool HairColorId_changed = false;
        public byte EyeColorId = 0;
        public bool EyeColorId_changed = false;
        public byte ShirtColorId = 0;
        public bool ShirtColorId_changed = false;
        public byte PantsColorId = 0;
        public bool PantsColorId_changed = false;
        public byte BootsColorId = 0;
        public bool BootsColorId_changed = false;
        public byte GlovesColorId = 0;
        public bool GlovesColorId_changed = false;

        public override void Deserialize(JHSNetworkReader reader)
        {
            STATUS = reader.ReadByte();
            if (STATUS == 0)
            {
                PlayerId = reader.ReadPackedUInt32();
                Silver = reader.ReadPackedUInt32();
                SkinColorId_changed = reader.ReadBoolean();
                HairColorId_changed = reader.ReadBoolean();
                EyeColorId_changed = reader.ReadBoolean();
                ShirtColorId_changed = reader.ReadBoolean();
                PantsColorId_changed = reader.ReadBoolean();
                BootsColorId_changed = reader.ReadBoolean();
                GlovesColorId_changed = reader.ReadBoolean();

                if (SkinColorId_changed)
                    SkinColorId = reader.ReadByte();

                if (HairColorId_changed)
                    HairColorId = reader.ReadByte();

                if (EyeColorId_changed)
                    EyeColorId = reader.ReadByte();

                if (ShirtColorId_changed)
                    ShirtColorId = reader.ReadByte();

                if (PantsColorId_changed)
                    PantsColorId = reader.ReadByte();

                if (BootsColorId_changed)
                    BootsColorId = reader.ReadByte();

                if (GlovesColorId_changed)
                    GlovesColorId = reader.ReadByte();
            }
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.Write(STATUS);
            if (STATUS == 0)
            {
                writer.WritePackedUInt32(PlayerId);
                writer.WritePackedUInt32(Silver);
                writer.Write(SkinColorId_changed);
                writer.Write(HairColorId_changed);
                writer.Write(EyeColorId_changed);
                writer.Write(ShirtColorId_changed);
                writer.Write(PantsColorId_changed);
                writer.Write(BootsColorId_changed);
                writer.Write(GlovesColorId_changed);

                if (SkinColorId_changed)
                    writer.Write(SkinColorId);

                if (HairColorId_changed)
                    writer.Write(HairColorId);

                if (EyeColorId_changed)
                    writer.Write(EyeColorId);

                if (ShirtColorId_changed)
                    writer.Write(ShirtColorId);

                if (PantsColorId_changed)
                    writer.Write(PantsColorId);

                if (BootsColorId_changed)
                    writer.Write(BootsColorId);

                if (GlovesColorId_changed)
                    writer.Write(GlovesColorId);
            }
        }
    }

    public class ExchangeCur : JHSMessageBase
    {
        public uint Value = 0;
        /// <summary>
        /// 0 = SILVER
        /// 1 = GOLD
        /// </summary>
        public byte CurencyType = 0;

        public override void Deserialize(JHSNetworkReader reader)
        {
            Value = reader.ReadPackedUInt32();
            CurencyType = reader.ReadByte();
        }
        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.WritePackedUInt32(Value);
            writer.Write(CurencyType);
        }
    }

    public class ExchangeCurResp : JHSMessageBase
    {
        public uint GoldValue = 0;
        public uint SilverValue = 0;
        public byte STATUS = 0;

        public override void Deserialize(JHSNetworkReader reader)
        {
            STATUS = reader.ReadByte();
            if (STATUS == 0)
            {
                GoldValue = reader.ReadPackedUInt32();
                SilverValue = reader.ReadPackedUInt32();
            }
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.Write(STATUS);
            if (STATUS == 0)
            {
                writer.WritePackedUInt32(GoldValue);
                writer.WritePackedUInt32(SilverValue);
            }
        }
    }

    public class BuySkin : JHSMessageBase
    {
        public uint ModelId;
        /// <summary>
        /// 0 BUY WITH SILVER
        /// 1 BUY WITH GOLD
        /// </summary>
        public byte BuyType;

        public override void Deserialize(JHSNetworkReader reader)
        {
            ModelId = reader.ReadPackedUInt32();
            BuyType = reader.ReadByte();
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.WritePackedUInt32(ModelId);
            writer.Write(BuyType);
        }
    }

    public class BuySkinResponse : JHSMessageBase
    {
        public byte STATUS;
        public uint Gold;
        public uint Silver;
        public ServerCharacter character;

        public override void Deserialize(JHSNetworkReader reader)
        {
            STATUS = reader.ReadByte();
            if (STATUS == 0)
            {
                Gold = reader.ReadPackedUInt32();
                Silver = reader.ReadPackedUInt32();
                character = new ServerCharacter()
                {
                    PlayerId = reader.ReadPackedUInt32(),
                    ModelId = reader.ReadPackedUInt32(),
                    SkinColorId = reader.ReadByte(),
                    HairColorId = reader.ReadByte(),
                    EyeColorId = reader.ReadByte(),
                    ShirtColorId = reader.ReadByte(),
                    PantsColorId = reader.ReadByte(),
                    BootsColorId = reader.ReadByte(),
                    GlovesColorId = reader.ReadByte()
                };
            }
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.Write(STATUS);
            if (STATUS == 0)
            {
                writer.WritePackedUInt32(Gold);
                writer.WritePackedUInt32(Silver);
                writer.WritePackedUInt32(character.PlayerId);
                writer.WritePackedUInt32(character.ModelId);
                writer.Write(character.SkinColorId);
                writer.Write(character.HairColorId);
                writer.Write(character.EyeColorId);
                writer.Write(character.ShirtColorId);
                writer.Write(character.PantsColorId);
                writer.Write(character.BootsColorId);
                writer.Write(character.GlovesColorId);
            }
        }
    }

    public class DeleteCharacter : JHSMessageBase
    {
        public uint PlayerId = 0;
        public override void Deserialize(JHSNetworkReader reader)
        {
            PlayerId = reader.ReadPackedUInt32();
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.WritePackedUInt32(PlayerId);
        }
    }

    public class DeleteCharacterResponse : JHSMessageBase
    {
        public byte STATUS = 0;
        public uint PlayerId = 0;
        public uint Gold = 0;
        public uint Silver = 0;

        public override void Deserialize(JHSNetworkReader reader)
        {
            STATUS = reader.ReadByte();
            if (STATUS == 0)
            {
                PlayerId = reader.ReadPackedUInt32();
                Gold = reader.ReadPackedUInt32();
                Silver = reader.ReadPackedUInt32();
            }
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.Write(STATUS);
            if (STATUS == 0)
            {
                writer.WritePackedUInt32(PlayerId);
                writer.WritePackedUInt32(Gold);
                writer.WritePackedUInt32(Silver);
            }
        }
    }

    public class CollectActive : JHSMessageBase
    {
        public uint AciveId;
        public override void Deserialize(JHSNetworkReader reader)
        {
            AciveId = reader.ReadPackedUInt32();
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.WritePackedUInt32(AciveId);
        }
    }

    public class CollectActiveResponse : JHSMessageBase
    {
        public byte STATUS = 0;
        public uint Gold = 0;
        public uint Silver = 0;
        public uint AciveId;
        public override void Deserialize(JHSNetworkReader reader)
        {
            STATUS = reader.ReadByte();
            if (STATUS == 0)
            {
                AciveId = reader.ReadPackedUInt32();
                Gold = reader.ReadPackedUInt32();
                Silver = reader.ReadPackedUInt32();
            }

        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.Write(STATUS);
            if (STATUS == 0)
            {
                writer.WritePackedUInt32(AciveId);
                writer.WritePackedUInt32(Gold);
                writer.WritePackedUInt32(Silver);
            }
        }
    }

    public class RefreshActives : JHSMessageBase
    {
        public ServerActives[] actives;
        public override void Deserialize(JHSNetworkReader reader)
        {
            int alenght = reader.ReadByte();
            actives = new ServerActives[alenght];
            for (int i = 0; i < alenght; i++)
            {
                actives[i] = new ServerActives()
                {
                    Id = reader.ReadPackedUInt32(),
                    Value = reader.ReadPackedUInt32(),
                    collected = reader.ReadBoolean()
                };
            }
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            if (actives != null && actives.Length > 0)
            {
                int alenght = actives.Length;
                writer.Write((byte)alenght);
                for (int i = 0; i < alenght; i++)
                {
                    writer.WritePackedUInt32(actives[i].Id);
                    writer.WritePackedUInt32(actives[i].Value);
                    writer.Write(actives[i].collected);
                }
            }
            else
            {
                writer.Write((byte)0);
            }
        }
    }

    public class SearchMatch : JHSMessageBase
    {
        public SearchMatchOperations op;
        public uint value;
        public string IP = "";
        public short port = 0;

        public override void Deserialize(JHSNetworkReader reader)
        {
            op = (SearchMatchOperations)reader.ReadByte();
            if (op == SearchMatchOperations.Search || op == SearchMatchOperations.SEARCHING_INIT)
            {
                value = reader.ReadPackedUInt32();
            }
            if (op == SearchMatchOperations.START)
            {
                IP = reader.ReadString();
                port = reader.ReadInt16();
            }
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.Write((byte)op);
            if (op == SearchMatchOperations.Search || op == SearchMatchOperations.SEARCHING_INIT)
            {
                writer.WritePackedUInt32(value);
            }

            if (op == SearchMatchOperations.START)
            {
                writer.Write(IP);
                writer.Write(port);
            }
        }
    }

}
