using JHSNetProtocol;
using log4net;
using System;
using System.Reflection;

namespace LoginServer.Utils
{
    public class JHSLoggingOverite : IJHSLogger
    {
        private static readonly ILog log = LogManager.GetLogger("JHSProtocol");

        public void Log(object v)
        {
            log.Info(v);
        }

        public void LogError(string v)
        {
            log.Error(v);
        }

        public void LogError(object v)
        {
            log.Error(v);
        }

        public void LogWarning(string v)
        {
            log.Warn(v);
        }
    }
}
