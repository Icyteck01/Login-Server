using LoginServer.Engine.Managers;
using LoginServer.MYSQL.Tables;
using LoginServer.Network.Data;

namespace LoginServer.Database
{
    public class ActivesOBJ : IDbEntity
    {
        private Actives Data;

        private ActivesConfig conf;

        public int ActiveId => Data.ActiveId;

        public bool IsNotify = false;

        public int Value { get => Data.value; set => Data.value = value; }
        public bool Collected { get => Data.collected; set => Data.collected = value; }

        public ActivesOBJ(Actives character)
        {
            Data = character;
        }

        public Actives Get()
        {
            return Data;
        }

        public ServerActives GetActive()
        {
            return new ServerActives()
            {
                Id = (uint)ActiveId,
                Value = (uint)Data.value,
                collected = Collected
            };
        }

        public Characters GetCharacter()
        {
            return null;
        }

        public Users GetAccount()
        {
            return null;
        }

        public string GetId()
        {
            return "ActivesOBJ" + ActiveId;
        }

        public object GetEntity()
        {
            return Data;
        }

        public ActiveType activeType
        {
            get
            {
                if (conf == null)
                    conf = ConfigManager.GetActive(Data.ActiveId);

                return conf.Type;
            }
        }

        public bool CanNetSend()
        {
            if(conf == null)
                conf = ConfigManager.GetActive(Data.ActiveId);

            if(conf != null)
            {
                return Data.value != conf.Conditions && !Data.collected;
            }

            return false;
        }

        public bool Compleated
        {
            get
            {
                if (conf == null)
                    conf = ConfigManager.GetActive(Data.ActiveId);

                if (conf != null)
                {
                    return Data.value == conf.Conditions && !Data.collected;
                }

                return false;
            }
        }
    }
}
