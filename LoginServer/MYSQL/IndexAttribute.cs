using System;

namespace LoginServer.MYSQL
{
    public class IndexAttribute : Attribute
    {
        public bool IsAutoGenerate = false;
        public string ClassName = "";
        public bool IsEnum = false;

        public IndexAttribute()
        {

        }

        public IndexAttribute(bool AutoGenerate)
        {
            IsAutoGenerate = AutoGenerate;
        }

        public IndexAttribute(bool _IsEnum, string EnumClassName)
        {
            IsEnum = _IsEnum;
            ClassName = EnumClassName;
        }
    }
}