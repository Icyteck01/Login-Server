using JHSNetProtocol;
using Network.Server.Data;

namespace Network.Server
{
    public class GameServerOP
    {
        public const short PUTROLE = 20000;
        public const short GETROLE = 20001;
        public const short UPDATE_STATE = 20002;
        public const short GET_ROLE2 = 20003;
    }


    public class ReqeuestUser : JHSMessageBase
    {
        public uint userId = 0;
        public uint Req = 0;
        public InfoType Type = InfoType.NONE;
        public override void Deserialize(JHSNetworkReader reader)
        {
            Type = (InfoType)reader.ReadByte();
            userId = reader.ReadPackedUInt32();
            Req = reader.ReadPackedUInt32();
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.Write((byte)Type);
            writer.WritePackedUInt32(userId);
            writer.WritePackedUInt32(Req);
        }
    }
}
