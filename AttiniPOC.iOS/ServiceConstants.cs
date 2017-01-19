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
        public const string ConnectionString   = "sb://attinipocnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=7Vowv9aut7NfVu1hQF3ahfbCYs2VktCOPfDKmd5rtfs=";
        public const string NotificationHubPath = "sb://attinipocnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=r6AoCozdY819RqQVhJqV66M+N0RCE9wMgKdxBYbK8AU=";

    }
}