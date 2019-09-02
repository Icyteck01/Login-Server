using JHSNetProtocol;
using JHSRanking;
using log4net.Config;
using LoginServer.Database;
using LoginServer.Engine.Managers;
using LoginServer.MYSQL;
using LoginServer.MYSQL.Tables;
using LoginServer.Utils;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;

namespace LoginServer.Engine.Service
{
    public class DbService
    {
        public const int MaxUpdatePerRun = 1;
        public const int UpdateEvery = 30;

        private static object s_Sync = new object();
        static volatile DbService s_Instance;
        public static DbService Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    lock (s_Sync)
                    {
                        if (s_Instance == null)
                        {
                            s_Instance = new DbService();
                        }
                    }
                }
                return s_Instance;
            }
        }

        public static Dictionary<string, IDbEntity> UPDATE_QUEUE = new Dictionary<string, IDbEntity>();
        private Dictionary<string, IDbEntity> ENTITY = new Dictionary<string, IDbEntity>();
        private Configuration config;
        private ISessionFactory factory;
        private ISession session;

        public void Start()
        {
            float start = JHSTime.Time;
            LOG.Info("Loading database service ...");
            try
            {
                XmlConfigurator.Configure(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs/Net4Log.xml")));
                config = new Configuration().Configure(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs/hibernate.xml"));
                Type[] typelist = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "LoginServer.MYSQL.Tables");
                bool Added = false;
                for (int i = 0; i < typelist.Length; i++)
                {
                    Added = false;
                    string xml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
                    xml += "<hibernate-mapping xmlns=\"urn:nhibernate-mapping-2.2\">";
                    xml += "<class name=\"LoginServer.MYSQL.Tables." + typelist[i].Name + ", LoginServer\" table=\"" + typelist[i].Name.ToLower() + "\">";
                    foreach (PropertyInfo prop in typelist[i].GetProperties())
                    {
                        if (prop.GetCustomAttribute(typeof(IndexAttribute)) != null && !Added)
                        {
                            IndexAttribute att = (IndexAttribute)prop.GetCustomAttribute(typeof(IndexAttribute));
                            if (!att.IsEnum)
                            {
                                xml += "<id name=\"" + prop.Name + "\" column=\"" + prop.Name + "\" type=\"" + prop.PropertyType.Name + "\">";
                                if (att.IsAutoGenerate)
                                    xml += "<generator class=\"native\"></generator>";
                                    xml += "</id>";
                            }
                            else
                            {
                                xml += "<property name=\"" + prop.Name + "\" type =\""+ att.ClassName + "\" />";
                            }
                            Added = true;
                        }
                        else
                        {
                            xml += "<property name=\"" + prop.Name + "\" column=\"" + prop.Name + "\" type =\"" + prop.PropertyType.Name + "\"></property>";
                        }
                    }
                    xml += "</class>";
                    xml += "</hibernate-mapping>";
                    config.AddXmlString(xml);
                }
                factory = config.BuildSessionFactory();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Timer aTimer = new Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = UpdateEvery * 1000;
            aTimer.Enabled = true;
            LOG.Info("Finished loading database service, time-consuming:" + (JHSTime.Time - start) + " sec.");
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                Instance.Update();
            }
            catch { }
        }

        public void Update()
        {
            lock (UPDATE_QUEUE)
            {
                int updates = 0;
                while (updates < MaxUpdatePerRun)
                {
                    if (UPDATE_QUEUE.Count > 0)
                    {
                        IDbEntity entity = UPDATE_QUEUE.Values.First();
                        if (Update(entity))
                        {
                            UPDATE_QUEUE.Remove(entity.GetId());
                            updates++;
                            continue;
                        }
                    }
                    updates++;
                }
            }
        }

        public bool SaveAll()
        {
            lock (UPDATE_QUEUE)
            {
                int updates = 0;
                while (updates < UPDATE_QUEUE.Count)
                {
                    IDbEntity entity = UPDATE_QUEUE.Values.First();
                    if (Instance.Update(entity))
                    {
                        UPDATE_QUEUE.Remove(entity.GetId());
                        updates++;
                        continue;
                    }

                    updates++;
                }
            }
            return true;
        }

        private Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }

        private void CHeck()
        {
            if (factory == null)
            {
                factory = config.BuildSessionFactory();
            }
            if (session == null)
            {
                session = factory.OpenSession();
            }
            if (!session.IsConnected)
            {
                session.Reconnect();
            }
            if (!session.IsOpen)
            {
                session = factory.OpenSession();
            }
        }

        public IList GetList(Type type, int MaxResult = 100)
        {
            CHeck();
            ICriteria sc = session.CreateCriteria(type);
            sc.SetMaxResults(100);
            return sc.List();
        }

        public IList<Rankings> GetRank(LeagueType type)
        {
            CHeck();
            ICriteria dc = session.CreateCriteria<Rankings>();
            dc.SetMaxResults(100);          
            dc.Add(Restrictions.Eq("League", (int)type));
            dc.AddOrder(Order.Desc("LeaguePoints"));
            return dc.List<Rankings>();
        }

        private void Dispose()
        {
            session.Close();
            factory.Close();
        }

        public static uint SaveEntity(object entity)
        {
            return Instance.Save(entity);
        }

        public static bool RemoveEntity(object entity)
        {
            return Instance.Delete(entity);
        }

        public uint Save(object Data)
        {
            CHeck();
            uint ret = 0;
            using (ITransaction transaction = session.BeginTransaction())
            {
                ret = (uint)session.Save(Data);
                transaction.Commit();
            }
            return ret;
        }

        public bool Delete(object Data)
        {
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Delete(Data);
                transaction.Commit();
            }
            return true;
        }

        public bool Update(IDbEntity data)
        {
            lock (UPDATE_QUEUE)
            {
                UPDATE_QUEUE.Remove(data.GetId());
            }
            CHeck();
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Merge(data.GetEntity());
                transaction.Commit();
            }
            
            return true;
        }

        public static void SubmitUpdate2Queue(IDbEntity entity)
        {
            lock (UPDATE_QUEUE)
            {
                UPDATE_QUEUE[entity.GetId()] = entity;
            }
        }

        public static void UpdateEntityIntime(IDbEntity entity)
        {
            Instance.Update(entity);
        }

        public IDbEntity Get(string entityId)
        {
            if (ENTITY.TryGetValue(entityId, out IDbEntity Entity))
                return Entity;

            return null;
        }

        private IDbEntity Get(AccountOBJ entity)
        {
            if (ENTITY.TryGetValue(entity.GetId(), out IDbEntity Entity))
                return Entity;

            return null;
        }

        public RankinngOBJ GetRankinData(RankinngOBJ entity)
        {
            IDbEntity that = Get(entity.GetId());
            if (that != null)
                return (RankinngOBJ)that;

            CHeck();
            Rankings ran = session.QueryOver<Rankings>().Where(x => x.UserId == entity.UserId).List().FirstOrDefault();
            if (ran != null) {
                RankinngOBJ dat = new RankinngOBJ(ran);
                ENTITY.Add(dat.GetId(), dat);
                return dat;
            } else {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    ran = new Rankings()
                    {
                        UserId = entity.UserId,
                        PlayerName = entity.PlayerName,
                        League = (int)LeagueType.NONE,
                        LeaguePoints = 0,
                        GameCount = 0,
                        Kills = 0,
                        Deaths = 0,
                    };
                    session.Save(ran);
                    transaction.Commit();
                }
                return GetRankinData(entity);
            }
        }

        public AccountOBJ GetAccount(AccountOBJ entity, bool AutoCreate = false)
        {
            IDbEntity that = Get(entity);
            if (that != null)
                return (AccountOBJ)that;

            CHeck();
            Users usr = session.QueryOver<Users>().Where(x => x.UserName == entity.Username).List().FirstOrDefault();
            if (usr != null)
            {
                AccountOBJ account = GetAccountFromDB(usr.UserName);
                ENTITY.Add(account.GetId(), account);
                return account;
            }
            else
            {
                if (AutoCreate)
                {
                    AccountOBJ user = new AccountOBJ((Users)entity.GetEntity());
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        user.Id = (uint)session.Save(user.GetEntity());
                        transaction.Commit();
                    }
                    return GetAccount(user);
                }
            }
            return null;
        }

        public AccountOBJ GetAccountFromDB(uint userId)
        {
            CHeck();
            AccountOBJ account = null;
            Users usr = session.QueryOver<Users>().Where(x => x.UserId == userId).List().FirstOrDefault();
            if (usr != null)
            {
                account = new AccountOBJ(usr);

                IList<Characters> chars = session.QueryOver<Characters>().Where(x => x.UserId == account.Id).List();

                account.Skins = new SortedList<uint, CharacterOBJ>();

                for (int c = 0; c < chars.Count; c++)
                {
                    Characters character = chars[c];
                    CharacterOBJ objchar = new CharacterOBJ(character);
                    account.Skins.Add(objchar.PlayerId, objchar);
                }

                if (chars.Count == 0)
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        short skinId = Settings.randomSkins[new Random().Next(Settings.randomSkins.Length)];
                        ConfiModel model = ConfigManager.GetModel(skinId);
                        if (model != null)
                        {
                            Characters ccc = new Characters(account.Id, skinId)
                            {
                                SkinColorId = model.SkinColorId,
                                EyeColorId = model.EyeColorId,
                                HairColorId = model.HairColorId,
                                ShirtColorId = model.ShirtColorId,
                                PantsColorId = model.PantsColorId,
                                BootsColorId = model.BootsColorId,
                                GlovesColorId = model.GlovesColorId
                            };
                            session.Save(ccc);
                            transaction.Commit();
                        }
                    }
                    chars = session.QueryOver<Characters>().Where(x => x.UserId == account.Id).List();
                    for (int c = 0; c < chars.Count; c++)
                    {
                        Characters character = chars[c];
                        CharacterOBJ objchar = new CharacterOBJ(character);

                        account.Skins.Add(objchar.PlayerId, objchar);
                    }
                }

                IList<Actives> actives = session.QueryOver<Actives>().Where(x => x.UserId == account.Id).List();
                ActivesConfig[] allActives = ConfigManager.GetActives();

                if (allActives.Length != actives.Count)
                {
                    for (int c = 0; c < actives.Count; c++)
                    {
                        Actives active = actives[c];
                        ActivesOBJ objactive = new ActivesOBJ(active);
                        account.Actives.Add(objactive.ActiveId, objactive);
                    }

                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        for (int a = 0; a < allActives.Length; a++)
                        {
                            ActivesConfig ac = allActives[a];
                            if (!account.Actives.ContainsKey(ac.ActiveId))
                            {
                                Actives active = new Actives
                                {
                                    UserId = account.Id,
                                    value = 0,
                                    ActiveId = ac.ActiveId,
                                    collected = false
                                };

                                session.Save(active); // <-- this

                                ActivesOBJ objactive = new ActivesOBJ(active);
                                account.Actives.Add(objactive.ActiveId, objactive);
                            }
                        }
                        transaction.Commit();
                    }

                    account.Actives.Clear();
                    actives = session.QueryOver<Actives>().Where(x => x.UserId == account.Id).List();

                    for (int c = 0; c < actives.Count; c++)
                    {
                        Actives active = actives[c];
                        ActivesOBJ objactive = new ActivesOBJ(active);
                        account.Actives.Add(objactive.ActiveId, objactive);
                    }
                }
                else
                {
                    for (int c = 0; c < actives.Count; c++)
                    {
                        Actives active = actives[c];
                        ActivesOBJ objactive = new ActivesOBJ(active);
                        account.Actives.Add(objactive.ActiveId, objactive);
                    }
                }
            }
            return account;
        }

        public AccountOBJ GetAccountFromDB(string username)
        {
            CHeck();
            AccountOBJ account = null;
            Users usr = session.QueryOver<Users>().Where(x => x.UserName == username).List().FirstOrDefault();
            if (usr != null)
            {
                account = new AccountOBJ(usr);

                IList<Characters> chars = session.QueryOver<Characters>().Where(x => x.UserId == account.Id).List();

                account.Skins = new SortedList<uint, CharacterOBJ>();

                for (int c = 0; c < chars.Count; c++)
                {
                    Characters character = chars[c];
                    CharacterOBJ objchar = new CharacterOBJ(character);
                    account.Skins.Add(objchar.PlayerId, objchar);
                }

                if (chars.Count == 0)
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        short skinId = Settings.randomSkins[new Random().Next(Settings.randomSkins.Length)];
                        ConfiModel model = ConfigManager.GetModel(skinId);
                        if (model != null)
                        {
                            Characters ccc = new Characters(account.Id, skinId)
                            {
                                SkinColorId = model.SkinColorId,
                                EyeColorId = model.EyeColorId,
                                HairColorId = model.HairColorId,
                                ShirtColorId = model.ShirtColorId,
                                PantsColorId = model.PantsColorId,
                                BootsColorId = model.BootsColorId,
                                GlovesColorId = model.GlovesColorId
                            };
                            session.Save(ccc);
                            transaction.Commit();
                        }
                    }
                    chars = session.QueryOver<Characters>().Where(x => x.UserId == account.Id).List();
                    for (int c = 0; c < chars.Count; c++)
                    {
                        Characters character = chars[c];
                        CharacterOBJ objchar = new CharacterOBJ(character);

                        account.Skins.Add(objchar.PlayerId, objchar);
                    }
                }

                IList<Actives> actives = session.QueryOver<Actives>().Where(x => x.UserId == account.Id).List();
                ActivesConfig[] allActives = ConfigManager.GetActives();

                if (allActives.Length != actives.Count)
                {
                    for (int c = 0; c < actives.Count; c++)
                    {
                        Actives active = actives[c];
                        ActivesOBJ objactive = new ActivesOBJ(active);
                        account.Actives.Add(objactive.ActiveId, objactive);
                    }

                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        for (int a = 0; a < allActives.Length; a++)
                        {
                            ActivesConfig ac = allActives[a];
                            if (!account.Actives.ContainsKey(ac.ActiveId))
                            {
                                Actives active = new Actives
                                {
                                    UserId = account.Id,
                                    value = 0,
                                    ActiveId = ac.ActiveId,
                                    collected = false
                                };

                                session.Save(active); // <-- this

                                ActivesOBJ objactive = new ActivesOBJ(active);
                                account.Actives.Add(objactive.ActiveId, objactive);
                            }
                        }
                        transaction.Commit();
                    }

                    account.Actives.Clear();
                    actives = session.QueryOver<Actives>().Where(x => x.UserId == account.Id).List();

                    for (int c = 0; c < actives.Count; c++)
                    {
                        Actives active = actives[c];
                        ActivesOBJ objactive = new ActivesOBJ(active);
                        account.Actives.Add(objactive.ActiveId, objactive);
                    }
                }
                else
                {
                    for (int c = 0; c < actives.Count; c++)
                    {
                        Actives active = actives[c];
                        ActivesOBJ objactive = new ActivesOBJ(active);
                        account.Actives.Add(objactive.ActiveId, objactive);
                    }
                }
            }
            return account;
        }


    }
}
