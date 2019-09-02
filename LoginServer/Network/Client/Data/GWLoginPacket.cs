using JHSNetProtocol;

namespace LoginServer.Network.Data
{
    public class GWLoginPacket : JHSMessageBase
    {
        public string LoginName = "";
        public string Password = "";

        public override void Deserialize(JHSNetworkReader reader)
        {
            LoginName = reader.ReadString();
            Password = reader.ReadString();
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            writer.Write(LoginName);
            writer.Write(Password);
        }
    }
}
