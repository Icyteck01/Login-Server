using JHSNetProtocol;

namespace Network.Server.Data
{
    public class Empty : JHSMessageBase
    {
        public override void Deserialize(JHSNetworkReader reader)
        {
            base.Deserialize(reader);
        }

        public override void Serialize(JHSNetworkWriter writer)
        {
            base.Serialize(writer);
        }
    }
}
