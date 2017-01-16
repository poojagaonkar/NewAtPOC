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
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AttiniPoc
{
   public class AuthContainer
    {
        public AuthenticationResult AuthResult { get; set; }
        public bool IsException { get; set; }
        public string Message { get; set; }
    }
}