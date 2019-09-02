using JHSNetProtocol;

namespace LoginServer.Network.Data
{
    public enum GMCOMMAND
    {
        ADD_GOLD,
        ADD_SILVER,
        FINISH_ACTIVES
    }
    public class ConsoleCMD : JHSMessageBase
    {
        public byte Command;
        public uint Value;

        public override void Deserialize(JHSNetworkReader reader)
        {
            Command = reader.ReadByte();
            Value = reader.ReadPackedUInt32();
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.Write(Command);
            writer.WritePackedUInt32(Value);
        }
    }
}
