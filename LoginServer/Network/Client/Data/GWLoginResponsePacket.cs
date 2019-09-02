using JHSNetProtocol;
using JHSRanking;
using System.Text;
using static CommonConstant;
namespace LoginServer.Network.Data
{
    public enum LeagueStatus
    {
        STATIC,
        UP,
        DOWN
    }
    public class GWLoginResponsePacket : JHSMessageBase
    {
        public ServerCharacter[] characters;
        public uint[] actives;
        public byte RESPONSE = 255;
        public uint Gold = 0;
        public uint Silver = 0;
        public uint LEVEL = 0;
        public uint EXP = 0;
        public LeagueType League = LeagueType.NONE;
        public byte LeaguePosition = 0;
        public LeagueStatus LEGUE_STATUS = LeagueStatus.STATIC;
        public byte SEASON = 0;
        public string PlayerNick;
        public byte Priviledge = 0;
        public byte GameCount = 0;
        public uint LoginTocken = 0;
        public bool IsPushLevelUp = false;

        public override void Deserialize(JHSNetworkReader reader)
        {
            RESPONSE = reader.ReadByte();
            if (RESPONSE == SUCCESS)
            {
                PlayerNick = Encoding.UTF8.GetString(reader.ReadBytesAndSize());
                LeaguePosition = reader.ReadByte();
                Gold = reader.ReadPackedUInt32();
                Silver = reader.ReadPackedUInt32();
                LoginTocken = reader.ReadPackedUInt32();
                GameCount = reader.ReadByte();
                LEVEL = reader.ReadPackedUInt32();
                EXP = reader.ReadPackedUInt32();
                League = (LeagueType)reader.ReadByte();
                SEASON = reader.ReadByte();
                LEGUE_STATUS = (LeagueStatus)reader.ReadByte();
                Priviledge = reader.ReadByte();
                int lenght = reader.ReadByte();
                characters = new ServerCharacter[lenght];
                for (int i = 0; i < lenght; i++)
                {
                    characters[i] = new ServerCharacter()
                    {
                        PlayerId = reader.ReadPackedUInt32(),
                        ModelId = reader.ReadPackedUInt32(),
                        IsChanged = reader.ReadBoolean(),
                    };
                    if (characters[i].IsChanged)
                    {
                        characters[i].SkinColorId = reader.ReadByte();
                        characters[i].HairColorId = reader.ReadByte();
                        characters[i].EyeColorId = reader.ReadByte();
                        characters[i].ShirtColorId = reader.ReadByte();
                        characters[i].PantsColorId = reader.ReadByte();
                        characters[i].BootsColorId = reader.ReadByte();
                        characters[i].GlovesColorId = reader.ReadByte();
                    }
                }
                int alenght = reader.ReadByte();
                actives = new uint[alenght];
                for (int i = 0; i < alenght; i++)
                {
                    actives[i] = reader.ReadPackedUInt32();
                }
                IsPushLevelUp = reader.ReadBoolean();
            }
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.Write(RESPONSE);
            if (RESPONSE == SUCCESS)
            {
                writer.WriteBytesFull(Encoding.UTF8.GetBytes(PlayerNick));
                writer.Write((byte)LeaguePosition);
                writer.WritePackedUInt32(Gold);
                writer.WritePackedUInt32(Silver);
                writer.WritePackedUInt32(LoginTocken);
                writer.Write(GameCount);
                writer.WritePackedUInt32(LEVEL);
                writer.WritePackedUInt32(EXP);
                writer.Write((byte)League);
                writer.Write(SEASON);
                writer.Write((byte)LEGUE_STATUS);
                writer.Write(Priviledge);

                if (characters != null && characters.Length > 0)
                {
                    int lenght = characters.Length;
                    writer.Write((byte)lenght);
                    for (int i = 0; i < lenght; i++)
                    {
                        writer.WritePackedUInt32(characters[i].PlayerId);
                        writer.WritePackedUInt32(characters[i].ModelId);
                        writer.Write(characters[i].IsChanged);
                        if (characters[i].IsChanged)
                        {
                            writer.Write(characters[i].SkinColorId);
                            writer.Write(characters[i].HairColorId);
                            writer.Write(characters[i].EyeColorId);
                            writer.Write(characters[i].ShirtColorId);
                            writer.Write(characters[i].PantsColorId);
                            writer.Write(characters[i].BootsColorId);
                            writer.Write(characters[i].GlovesColorId);
                        }
                    }
                }
                else
                {
                    writer.Write((byte)0);
                }

                if (actives != null && actives.Length > 0)
                {
                    int alenght = actives.Length;
                    writer.Write((byte)alenght);
                    for (int i = 0; i < alenght; i++)
                    {
                        writer.WritePackedUInt32(actives[i]);
                    }
                }
                else
                {
                    writer.Write((byte)0);
                }

                writer.Write(IsPushLevelUp);
            }
        }
    }
}