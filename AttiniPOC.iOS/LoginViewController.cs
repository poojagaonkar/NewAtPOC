using AttiniPoc;
using Foundation;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace AttiniPOC.iOS
{
    public partial class LoginViewController : UIViewController
    {

        public static AuthenticationContext AuthContext = null, GraphAuthContext = null;
        public static AuthenticationResult AuthResult = null, GraphAuthResult = null;
        public static string UserFullName = "";
        public static bool isAuthenticated;
        private static bool isInitialized;
        private string receiveStream;
        private System.IO.Stream data;
        string vidLinkReq = "https://rapidcircle1com.sharepoint.com/sites/iimdev/_api/web/getfilebyserverrelativeurl('/sites/IIMDev/Photos/SampleVideo_1280x720_1mb.mp4')/$value";
        string siteUrl = "https://rapidcircle1com.sharepoint.com";


        public LoginViewController (IntPtr handle) : base (handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            btnLogin.TouchUpInside += BtnLogin_TouchUpInside;
        }

        private async void BtnLogin_TouchUpInside(object sender, EventArgs e)
        {
            Initialize(ServiceConstants.AUTHORITY, ServiceConstants.RETURNURI, ServiceConstants.CLIENTID, siteUrl);

            AuthContainer authResult = await GetAccessToken(siteUrl, new PlatformParameters(this));

        }
        public static void Initialize(string authority, Uri returnUri, string clientId, string siteUrl)
        {
            try
            {
                ServiceConstants.AUTHORITY = authority;
                ServiceConstants.RETURNURI = returnUri;
                ServiceConstants.CLIENTID = clientId;
                ServiceConstants.SHAREPOINTURL = siteUrl;

                isInitialized = true;
            }
            catch (Exception ex)
            {
                isInitialized = false;
            }
        }
        public async Task<AuthContainer> GetAccessToken(string siteUrl, PlatformParameters param)
        {
            AuthContainer verbAuthResult = null;
            try
            {
                if (!isInitialized)
                    return null;

                //Uri SPUri = new Uri(siteUrl);
                //ServiceConstants.SHAREPOINTURL = siteUrl;
                //string resourceId = SPUri.Scheme + "://" + SPUri.Host;
                //string resourceId = siteUrl;
                AuthContext = new AuthenticationContext(ServiceConstants.AUTHORITY);
                if (AuthContext.TokenCache.ReadItems().Any())
                    AuthContext = new AuthenticationContext(AuthContext.TokenCache.ReadItems().First().Authority);


                try
                {
                    AuthResult = await AuthContext.AcquireTokenAsync(siteUrl, ServiceConstants.CLIENTID, ServiceConstants.RETURNURI, param);
                }
                catch (Exception ex)
                {
                    //Logout();
                }











                UserFullName = AuthResult.UserInfo.GivenName + " " + AuthResult.UserInfo.FamilyName;

                isAuthenticated = true;

                verbAuthResult = new AuthContainer()
                {
                    AuthResult = AuthResult,
                    IsException = false,
                    Message = ""
                };

                return verbAuthResult;
            }
            catch (AdalServiceException adalEx)
            {
                verbAuthResult = new AuthContainer()
                {
                    AuthResult = null,
                    IsException = true,
                    Message = "User cancelled sign in."
                };
            }
            catch (Exception ex)
            {
                verbAuthResult = new AuthContainer()
                {
                    AuthResult = null,
                    IsException = true,
                    Message = ex.Message
                };
            }

            return verbAuthResult;
        }

    }
}