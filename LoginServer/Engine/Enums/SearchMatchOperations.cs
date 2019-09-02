using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer.Engine.Enums
{
    public enum SearchMatchOperations
    {
        Search,
        Cancel,
        ERR_CHARATER_NOT_THERE,
        ERR_ACCOUNT_NOT_FOUND,
        NO_ERROR_SEARCHING,
        SEARCHING_INIT,
        CHECK_START,
        START
    }
}
