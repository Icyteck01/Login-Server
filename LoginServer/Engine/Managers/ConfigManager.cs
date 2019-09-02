using JHSRanking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LoginServer.Engine.Managers
{
    public class ConfiModel
    {
        public short ModelId;
        public int GoldPrice;
        public int SilverPrice;
        public byte SkinColorId = 0;
        public byte EyeColorId = 0;
        public byte HairColorId = 0;
        public byte ShirtColorId = 0;
        public byte PantsColorId = 0;
        public byte BootsColorId = 0;
        public byte GlovesColorId = 0;
    }

    public enum ActiveType
    {
        KILL_PLAYERS,
        DIE,
        DEAL_DAMAGE,
        TAKE_DAMAGE,
        MATCH_COUNT,
        REACH_POSITION,
        PICKUP_ITEM,
        DROP_ITEM
    }

    public class ActivesConfig
    {
        public int ActiveId;
        public ActiveType Type;
        public uint Conditions;
        public short SilverReward = 0;
        public short GoldReward = 0;
    }

    public static class ConfigManager
    {
        private static Dictionary<int, ConfiModel> ListOfConfigs = new Dictionary<int, ConfiModel>();
        private static Dictionary<int, ActivesConfig> ListOfActives = new Dictionary<int, ActivesConfig>();
        private static Dictionary<LeagueType, RankConfig> IRankConfig = new Dictionary<LeagueType, RankConfig>();

        public static void LoadConfigs()
        {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs/LoginData.data");

            using (MemoryStream stream = new MemoryStream(File.ReadAllBytes(file)))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    reader.ReadBytes(5);
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        ConfiModel config = new ConfiModel()
                        {
                            ModelId = (short)reader.ReadInt32(),
                            GoldPrice = reader.ReadInt32(),
                            SilverPrice = reader.ReadInt32(),
                            SkinColorId = reader.ReadByte(),
                            EyeColorId = reader.ReadByte(),
                            HairColorId = reader.ReadByte(),
                            ShirtColorId = reader.ReadByte(),
                            PantsColorId = reader.ReadByte(),
                            BootsColorId = reader.ReadByte(),
                            GlovesColorId = reader.ReadByte(),
                        };
                        ListOfConfigs[config.ModelId] = config;
                    }

                    count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        ActivesConfig config = new ActivesConfig()
                        {
                            ActiveId = reader.ReadInt32(),
                            Type = (ActiveType)reader.ReadByte(),
                            Conditions = reader.ReadUInt32(),
                            SilverReward = reader.ReadInt16(),
                            GoldReward = reader.ReadInt16()
                        };
                        ListOfActives[config.ActiveId] = config;
                    }

                    count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        RankConfig config = new RankConfig()
                        {
                            RankId = (LeagueType)reader.ReadByte(),
                            RankName = reader.ReadString(),
                            PromotePoints = reader.ReadInt32(),
                            KillAward = reader.ReadInt32(),
                            DieAward = reader.ReadInt32()
                        };
                        IRankConfig[config.RankId] = config;
                    }
                }
            }

        }

        public static ConfiModel GetModel(int id)
        {
            if (ListOfConfigs.TryGetValue(id, out ConfiModel value))
                return value;

            return null;
        }

        public static ActivesConfig GetActive(int id)
        {
            if (ListOfActives.TryGetValue(id, out ActivesConfig value))
                return value;

            return null;
        }

        public static ActivesConfig[] GetActives()
        {
            return ListOfActives.Values.ToArray();
        }

        public static RankConfig GetRankConfig(LeagueType rankType)
        {
            if (IRankConfig.TryGetValue(rankType, out RankConfig baseItem))
                return baseItem;

            return null;
        }

        public static List<RankConfig> GetRankConfigs()
        {
            return IRankConfig.Values.ToList();
        }
    }
}
