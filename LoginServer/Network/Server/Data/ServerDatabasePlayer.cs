using JHSNetProtocol;
using static CommonConstant;

namespace Network.Server.Data
{
    public enum InfoType
    {
        NONE,
        CUSTOMIZATIONS,
        STATUS,
        ACTIVE,
        ACTIVE_AND_STATUS,
        FULL
    }

    public enum GameMatchState
    {
        LOBY,
        INGAME,
        OVER,
        WAITING
    }

    public class DBPlayer
    {
        public DBPlayerBase Base;
        public DBCustomisation Custom;
        public DBStatus Status;
        public DBPlayerActive[] Actives;
    }

    public struct DBPlayerBase
    {
        public uint UserId; //FOR EASE OF COMMUNICATIONS
        public uint PlayerId; //FOR EASE OF COMMUNICATIONS
        public byte ModelId;
    }

    public struct DBCustomisation
    {
        public string PlayerName; //FOR NOTIFICATIONS
        public byte SkinColorId;
        public byte HairColorId;
        public byte EyeColorId;
        public byte ShirtColorId;
        public byte PantsColorId;
        public byte BootsColorId;
        public byte GlovesColorId;
    }

    public struct DBStatus
    {
        public uint Level;
        public uint Exp;
        public uint MMR;
        public uint Kills;
        public uint Deaths;
        public uint Golden;
        public uint Silver;
    }

    public struct DBPlayerActive
    {
        public uint ActiveId;
        public uint Value;
    }

    public class LoginDataBasePlayer : JHSMessageBase
    {
        public byte STATUS = 0;
        public DBPlayer player;
        public uint REQ = 0;

        public override void Deserialize(JHSNetworkReader reader)
        {
            STATUS = reader.ReadByte();
            if (STATUS == SUCCESS)
            {
                REQ = reader.ReadPackedUInt32();
                player = new DBPlayer
                {
                    Base = new DBPlayerBase
                    {
                        UserId = reader.ReadPackedUInt32(),
                        PlayerId = reader.ReadPackedUInt32(),
                        ModelId = reader.ReadByte()
                    },
                    Custom = new DBCustomisation
                    {
                        PlayerName = reader.ReadString(),
                        SkinColorId = reader.ReadByte(),
                        HairColorId = reader.ReadByte(),
                        EyeColorId = reader.ReadByte(),
                        ShirtColorId = reader.ReadByte(),
                        PantsColorId = reader.ReadByte(),
                        BootsColorId = reader.ReadByte(),
                        GlovesColorId = reader.ReadByte()
                    }
                };
                int lenght = reader.ReadByte();
                player.Actives = new DBPlayerActive[lenght];
                for (int i = 0; i < lenght; i++)
                {
                    player.Actives[i] = new DBPlayerActive
                    {
                        ActiveId = reader.ReadPackedUInt32(),
                        Value = reader.ReadPackedUInt32()
                    };
                }
            }
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.Write((byte)STATUS);
            if (STATUS == SUCCESS)
            {
                writer.WritePackedUInt32(REQ);
                writer.WritePackedUInt32(player.Base.UserId);
                writer.WritePackedUInt32(player.Base.PlayerId);
                writer.WritePackedUInt32(player.Base.ModelId);

                writer.Write(player.Custom.PlayerName);
                writer.Write(player.Custom.SkinColorId);
                writer.Write(player.Custom.HairColorId);
                writer.Write(player.Custom.EyeColorId);
                writer.Write(player.Custom.ShirtColorId);
                writer.Write(player.Custom.PantsColorId);
                writer.Write(player.Custom.BootsColorId);
                writer.Write(player.Custom.GlovesColorId);

                int lenghxt = player.Actives.Length;
                writer.Write((byte)lenghxt);
                for (int i = 0; i < lenghxt; i++)
                {
                    writer.WritePackedUInt32(player.Actives[i].ActiveId);
                    writer.WritePackedUInt32(player.Actives[i].Value);
                }
            }
        }
    }

    public class GameServerLogin : JHSMessageBase
    {
        public uint PassWord;
        public uint Port;
        public string IP;
        public GameMatchState gameMatchState;

        public override void Deserialize(JHSNetworkReader reader)
        {
            PassWord = reader.ReadPackedUInt32();
            Port = reader.ReadPackedUInt32();
            IP = reader.ReadString();
            gameMatchState = (GameMatchState)reader.ReadByte();
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.WritePackedUInt32(PassWord);
            writer.WritePackedUInt32(Port);
            writer.Write(IP);
            writer.Write((byte)gameMatchState);
        }
    }

    public class UpdateRole : JHSMessageBase
    {
        public byte STATUS = 0; // SUCCESS OR NOT FOUND
        public DBPlayer player;
        public InfoType TYPE = InfoType.NONE;

        public override void Deserialize(JHSNetworkReader reader)
        {
            STATUS = reader.ReadByte();
            if (STATUS == SUCCESS)
            {
                TYPE = (InfoType)reader.ReadByte();
                player = new DBPlayer
                {
                    Base = new DBPlayerBase
                    {
                        UserId = reader.ReadPackedUInt32()
                    }
                };
                switch (TYPE)
                {
                    case InfoType.ACTIVE:
                        int lenght = reader.ReadByte();
                        player.Actives = new DBPlayerActive[lenght];
                        for (int i = 0; i < lenght; i++)
                        {
                            player.Actives[i] = new DBPlayerActive
                            {
                                ActiveId = reader.ReadPackedUInt32(),
                                Value = reader.ReadPackedUInt32()
                            };
                        }
                        break;
                    case InfoType.CUSTOMIZATIONS:
                        player.Custom = new DBCustomisation
                        {
                            PlayerName = reader.ReadString(),
                            SkinColorId = reader.ReadByte(),
                            HairColorId = reader.ReadByte(),
                            EyeColorId = reader.ReadByte(),
                            ShirtColorId = reader.ReadByte(),
                            PantsColorId = reader.ReadByte(),
                            BootsColorId = reader.ReadByte(),
                            GlovesColorId = reader.ReadByte()
                        };
                        break;
                    case InfoType.STATUS:
                        player.Status = new DBStatus
                        {
                            Level = reader.ReadPackedUInt32(),
                            Exp = reader.ReadPackedUInt32(),
                            MMR = reader.ReadPackedUInt32(),
                            Kills = reader.ReadPackedUInt32(),
                            Deaths = reader.ReadPackedUInt32(),
                            Golden = reader.ReadPackedUInt32(),
                            Silver = reader.ReadPackedUInt32()
                        };
                        break;
                    case InfoType.ACTIVE_AND_STATUS:
                        player.Status = new DBStatus
                        {
                            Level = reader.ReadPackedUInt32(),
                            Exp = reader.ReadPackedUInt32(),
                            MMR = reader.ReadPackedUInt32(),
                            Kills = reader.ReadPackedUInt32(),
                            Deaths = reader.ReadPackedUInt32(),
                            Golden = reader.ReadPackedUInt32(),
                            Silver = reader.ReadPackedUInt32()
                        };
                        int lenghxt = reader.ReadByte();
                        player.Actives = new DBPlayerActive[lenghxt];
                        for (int i = 0; i < lenghxt; i++)
                        {
                            player.Actives[i] = new DBPlayerActive
                            {
                                ActiveId = reader.ReadPackedUInt32(),
                                Value = reader.ReadPackedUInt32()
                            };
                        }
                        break;
                    case InfoType.FULL:
                        player.Base.PlayerId = reader.ReadPackedUInt32();
                        player.Base.ModelId = reader.ReadByte();
                        player.Custom = new DBCustomisation
                        {
                            PlayerName = reader.ReadString(),
                            SkinColorId = reader.ReadByte(),
                            HairColorId = reader.ReadByte(),
                            EyeColorId = reader.ReadByte(),
                            ShirtColorId = reader.ReadByte(),
                            PantsColorId = reader.ReadByte(),
                            BootsColorId = reader.ReadByte(),
                            GlovesColorId = reader.ReadByte()
                        };
                        player.Status = new DBStatus
                        {
                            Level = reader.ReadPackedUInt32(),
                            Exp = reader.ReadPackedUInt32(),
                            MMR = reader.ReadPackedUInt32(),
                            Kills = reader.ReadPackedUInt32(),
                            Deaths = reader.ReadPackedUInt32(),
                            Golden = reader.ReadPackedUInt32(),
                            Silver = reader.ReadPackedUInt32()
                        };
                        int lenghxty = reader.ReadByte();
                        player.Actives = new DBPlayerActive[lenghxty];
                        for (int i = 0; i < lenghxty; i++)
                        {
                            player.Actives[i] = new DBPlayerActive
                            {
                                ActiveId = reader.ReadPackedUInt32(),
                                Value = reader.ReadPackedUInt32()
                            };
                        }
                        break;
                }
            }
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.Write((byte)STATUS);
            if (STATUS == SUCCESS)
            {
                writer.Write((byte)TYPE);
                writer.WritePackedUInt32(player.Base.UserId);
                switch (TYPE)
                {
                    case InfoType.ACTIVE:
                        int lenght = player.Actives.Length;
                        writer.Write((byte)lenght);
                        for (int i = 0; i < lenght; i++)
                        {
                            writer.WritePackedUInt32(player.Actives[i].ActiveId);
                            writer.WritePackedUInt32(player.Actives[i].Value);
                        }
                        break;
                    case InfoType.CUSTOMIZATIONS:
                        writer.Write(player.Custom.PlayerName);
                        writer.Write(player.Custom.SkinColorId);
                        writer.Write(player.Custom.HairColorId);
                        writer.Write(player.Custom.EyeColorId);
                        writer.Write(player.Custom.ShirtColorId);
                        writer.Write(player.Custom.PantsColorId);
                        writer.Write(player.Custom.BootsColorId);
                        writer.Write(player.Custom.GlovesColorId);
                        break;
                    case InfoType.STATUS:
                        writer.WritePackedUInt32(player.Status.Level);
                        writer.WritePackedUInt32(player.Status.Exp);
                        writer.WritePackedUInt32(player.Status.MMR);
                        writer.WritePackedUInt32(player.Status.Kills);
                        writer.WritePackedUInt32(player.Status.Deaths);
                        writer.WritePackedUInt32(player.Status.Golden);
                        writer.WritePackedUInt32(player.Status.Silver);
                        break;
                    case InfoType.ACTIVE_AND_STATUS:
                        writer.WritePackedUInt32(player.Status.Level);
                        writer.WritePackedUInt32(player.Status.Exp);
                        writer.WritePackedUInt32(player.Status.MMR);
                        writer.WritePackedUInt32(player.Status.Kills);
                        writer.WritePackedUInt32(player.Status.Deaths);
                        writer.WritePackedUInt32(player.Status.Golden);
                        writer.WritePackedUInt32(player.Status.Silver);
                        int lenghxt = player.Actives.Length;
                        writer.Write((byte)lenghxt);
                        for (int i = 0; i < lenghxt; i++)
                        {
                            writer.WritePackedUInt32(player.Actives[i].ActiveId);
                            writer.WritePackedUInt32(player.Actives[i].Value);
                        }
                        break;
                    case InfoType.FULL:
                        writer.WritePackedUInt32(player.Base.PlayerId);
                        writer.Write(player.Base.ModelId);
                        writer.Write(player.Custom.PlayerName);
                        writer.Write(player.Custom.SkinColorId);
                        writer.Write(player.Custom.HairColorId);
                        writer.Write(player.Custom.EyeColorId);
                        writer.Write(player.Custom.ShirtColorId);
                        writer.Write(player.Custom.PantsColorId);
                        writer.Write(player.Custom.BootsColorId);
                        writer.Write(player.Custom.GlovesColorId);
                        writer.WritePackedUInt32(player.Status.Level);
                        writer.WritePackedUInt32(player.Status.Exp);
                        writer.WritePackedUInt32(player.Status.MMR);
                        writer.WritePackedUInt32(player.Status.Kills);
                        writer.WritePackedUInt32(player.Status.Deaths);
                        writer.WritePackedUInt32(player.Status.Golden);
                        writer.WritePackedUInt32(player.Status.Silver);
                        int lenghxty = player.Actives.Length;
                        writer.Write((byte)lenghxty);
                        for (int i = 0; i < lenghxty; i++)
                        {
                            writer.WritePackedUInt32(player.Actives[i].ActiveId);
                            writer.WritePackedUInt32(player.Actives[i].Value);
                        }
                        break;
                }

            }
        }
    }

    public class UpdateMatchResult : JHSMessageBase
    {
        public DBPlayerActive[] Actives; //THE ACTIVES PLAYER UNLOCKED OR UPDATED
        public bool HasWon = false; //LAS MAN STANDING
        public uint KillCount = 0; //HOW MANY PLAYERS PLAYER KILLED
        public uint UserId = 0; //HOW MANY PLAYERS PLAYER KILLED
        public uint EXP = 0; //EXP WON THIS MATCH

        public override void Deserialize(JHSNetworkReader reader)
        {
            HasWon = reader.ReadBoolean();
            UserId = reader.ReadPackedUInt32();
            KillCount = reader.ReadPackedUInt32();
            EXP = reader.ReadPackedUInt32();
            int lenght = reader.ReadByte();
            Actives = new DBPlayerActive[lenght];
            for (int i = 0; i < lenght; i++)
            {
                Actives[i] = new DBPlayerActive
                {
                    ActiveId = reader.ReadPackedUInt32(),
                    Value = reader.ReadPackedUInt32()
                };
            }
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.Write(HasWon);
            writer.WritePackedUInt32(UserId);
            writer.WritePackedUInt32(KillCount);
            writer.WritePackedUInt32(EXP);
            int lenghxty = Actives.Length;
            writer.Write((byte)lenghxty);
            for (int i = 0; i < lenghxty; i++)
            {
                writer.WritePackedUInt32(Actives[i].ActiveId);
                writer.WritePackedUInt32(Actives[i].Value);
            }
        }
    }
}
