using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AttiniPoc
{
    public static class ServiceConstants
    {
        public static string AUTHORITY = "https://login.windows.net/common";
        public static Uri RETURNURI = new Uri("https://attinicommsmobile");

        /**
         * Client ID for Rapidcircle1com tenant
         **/
        public static string CLIENTID = "4d812628-ec3a-41f1-92a0-04d773b3ca8c";

        /**
         *  Client ID for zevenseas1 tenant 
         **/
        //public static string CLIENTID = "ad80cceb-7028-4669-92f4-73646137c29e";
        public static string SHAREPOINTURL = null;
        public static string GRAPHURI = "https://graph.microsoft.com";
       
    }
}