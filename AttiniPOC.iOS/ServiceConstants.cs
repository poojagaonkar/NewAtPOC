using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AttiniPoc
{
    public static class ServiceConstants
    {
        public static string AUTHORITY = "https://login.windows.net/common";
        //public static string AUTHORITY = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize?";
        public static Uri RETURNURI = new Uri("https://attinicommsmobile");
        public static string CLIENTID = "4d812628-ec3a-41f1-92a0-04d773b3ca8c";
        public static string GRAPHURI = "https://graph.microsoft.com";
        public static string SHAREPOINTURL = null;


    }
}