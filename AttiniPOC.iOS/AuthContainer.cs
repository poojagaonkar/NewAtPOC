using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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