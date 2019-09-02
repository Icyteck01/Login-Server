using JHSNetProtocol;
using JHSRanking;
using LoginServer.Utils;
using System;

namespace JHSEngine.net.GateWay.Data
{
    public enum RankingPacketType
    {
        UPDATE_DATA,
        GAME_COUNT,
        ERROR
    }
    public class RankingPacket : JHSMessageBase
    {
        public RankingPacketType PayLoadType = RankingPacketType.UPDATE_DATA;
        public LeagueData[] rankingData;
        public LeagueType CurrentLeague = LeagueType.NONE;
        public uint LeaguePoints = 0;
        public byte GameCount = 0;
        public byte LeaguePosition = 0;

        public override void Deserialize(JHSNetworkReader reader)
        {
            PayLoadType = (RankingPacketType)reader.ReadByte();
            if (PayLoadType == RankingPacketType.UPDATE_DATA)
            {
                int dataLenght = reader.ReadByte();
                rankingData = new LeagueData[dataLenght];
                for (int i = 0; i < dataLenght; i++)
                {
                    rankingData[i] = new LeagueData()
                    {
                        PlayerName = reader.ReadString(),
                        LeaguePoints = (int)reader.ReadPackedUInt32(),
                        Kills = (int)reader.ReadPackedUInt32(),
                        Deaths = (int)reader.ReadPackedUInt32()
                    };
                }
                LeaguePosition = reader.ReadByte();
                GameCount = reader.ReadByte();
                LeaguePoints = reader.ReadPackedUInt32();
                CurrentLeague = (LeagueType)reader.ReadByte();
            }
            if (PayLoadType == RankingPacketType.GAME_COUNT)
            {
                GameCount = reader.ReadByte();
            }
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.Write((byte)PayLoadType);

            if (PayLoadType == RankingPacketType.UPDATE_DATA)
            {
                int dataLenght = rankingData.Length;
                writer.Write((byte)dataLenght);

                for (int i = 0; i < dataLenght; i++)
                {
                    writer.Write(rankingData[i].PlayerName); //INDEX :D
                    writer.WritePackedUInt32((uint)rankingData[i].LeaguePoints);
                    writer.WritePackedUInt32((uint)rankingData[i].Kills);
                    writer.WritePackedUInt32((uint)rankingData[i].Deaths);
                }
                writer.Write((byte)LeaguePosition);
                writer.Write((byte)GameCount);
                writer.WritePackedUInt32(LeaguePoints);
                writer.Write((byte)CurrentLeague);
            }
            if (PayLoadType == RankingPacketType.GAME_COUNT)
            {
                writer.Write((byte)GameCount);
            }
        }
    }
}

