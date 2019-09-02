namespace LoginServer.Network.Data
{
    public class ServerCharacter
    {
        public uint PlayerId;
        public uint ModelId;
        public bool IsChanged = false;
        public byte SkinColorId = 0;
        public byte HairColorId = 0;
        public byte EyeColorId = 0;
        public byte ShirtColorId = 0;
        public byte PantsColorId = 0;
        public byte BootsColorId = 0;
        public byte GlovesColorId = 0;
    }

    public class ServerActives
    {
        public uint Id = 0;
        public uint Value;
        public bool collected;
    }
}
