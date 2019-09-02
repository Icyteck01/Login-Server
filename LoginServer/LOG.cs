using log4net;
using System;
using System.Reflection;

public class LOG
{
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    
    public static void Error(string v)
    {
        log.Error(v);
    }

    public static void Warning(string v)
    {
        log.Warn(v);
    }

    public static void Exception(string v)
    {
        log.Fatal(v);
    }

    public static void Info(string v)
    {
        log.Info(v);
    }

    public static void Debug(string v)
    {
        log.Debug(v);
    }

    public static void Mysql(string v)
    {
        log.Debug(v);
    }

    public static void Network(string v)
    {
        log.Debug(v);
    }
}
