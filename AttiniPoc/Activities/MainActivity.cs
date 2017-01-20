using Android.App;
using Android.Widget;
using Android.OS;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;
using System.Linq;
using Android.Content;
using Android.Runtime;
using System.Net.Http;
using System.Net;
using System.Text;
using static Android.Provider.Settings;
using Android.Telephony;
using Java.Util;
using Java.Lang.Reflect;
using System.Collections.Generic;
using static Android.Media.MediaPlayer;
using Android.Media;
using ModernHttpClient;
using Android.Graphics;
using Java.IO;
using System.IO;
using Android.Webkit;
using Newtonsoft.Json;


namespace AttiniPoc
{
    [Activity(Label = "AttiniPoc", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, IOnErrorListener
    {
        public static AuthenticationContext AuthContext = null, GraphAuthContext = null;
        public static AuthenticationResult AuthResult = null, GraphAuthResult = null;
        public static string UserFullName = "";
        public static bool isAuthenticated;
        private static bool isInitialized;
        private string receiveStream;
        private System.IO.Stream data;

        // videoView.SetVideoPath("http://www.androidbegin.com/tutorial/AndroidCommercial.3gp");
        //string vidLink = "https://zevenseas1.sharepoint.com/sites/AttiniDev/AllNews/Lists/Photos/Discovering%20Robeco%20-%20corporate%20movie.mp4";
        //string vidLinkReq = "https://zevenseas1.sharepoint.com/sites/AttiniDev/AllNews/_api/web/getfilebyserverrelativeurl('/sites/AttiniDev/AllNews/Lists/Photos/Discovering Robeco - corporate movie.mp4')/$value";
        //string siteUrl = "https://zevenseas1.sharepoint.com";

        string vidLinkReq = "https://rapidcircle1com.sharepoint.com/sites/iimdev/_api/web/getfilebyserverrelativeurl('/sites/IIMDev/Photos/SampleVideo_1280x720_1mb.mp4')/$value";
        string siteUrl = "https://rapidcircle1com.sharepoint.com";
        private DeviceModel deviceDetailObj;

        //test2@rahulpatilzevenseas.onmicrosoft.com
        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
             SetContentView (Resource.Layout.Main);
            var videoView = FindViewById<VideoView>(Resource.Id.videoView1);
            var imgView = FindViewById<ImageView>(Resource.Id.imageView1);

            Initialize(ServiceConstants.AUTHORITY, ServiceConstants.RETURNURI, ServiceConstants.CLIENTID, siteUrl);

            AuthContainer authResult = await GetAccessToken(siteUrl, new PlatformParameters(this));


            if (!authResult.IsException)
            {
                var devId = GetDeviceId();
                var newsResponse = await RegisterDevice(authResult.AuthResult.UserInfo.DisplayableId, devId , "Motog");

                deviceDetailObj = JsonConvert.DeserializeObject<DeviceModel>(newsResponse);

                var authDevice = await AuthenticateDevice(deviceDetailObj.EncodedAccountName, devId, deviceDetailObj.HostUrl);

                var mStream = await GetFileBytes(authResult.AuthResult.AccessToken);
                //var profileImage = BitmapFactory.DecodeStream(mStream);
                //imgView.SetImageBitmap(profileImage);


                if (mStream != null)
                {
                    var arry = ReadFully(mStream);

                    Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
                    Java.IO.File dir = new Java.IO.File(sdCard.AbsolutePath + "/MyFolder");
                    dir.Mkdirs();
                    Java.IO.File file = new Java.IO.File(dir, RandomFilenameGenrator() + ".mp4");
                    if (!file.Exists())
                    {
                        file.CreateNewFile();
                        file.Mkdir();
                        System.IO.File.WriteAllBytes(file.Path, arry);


                    }


                    MediaController mController = new Android.Widget.MediaController(this);
                    mController.SetAnchorView(videoView);

                    var headersCookie = new Dictionary<string, string>();
                    headersCookie.Add("Authorization", "Bearer " + authResult.AuthResult.AccessToken);
                    videoView.SetVideoPath(file.Path);

                    videoView.SetOnErrorListener(this);
                    videoView.Start();
                }

                //MediaController mController = new Android.Widget.MediaController(this);
                //mController.SetAnchorView(videoView);
                //var mUri = Android.Net.Uri.Parse(vidLinkReq);

                //var headersCookie = new Dictionary<string, string>();
                //headersCookie.Add("Authorization", "Bearer " + authResult.AuthResult.AccessToken);
                //videoView.SetVideoURI(mUri);
                //videoView.SetMediaController(mController);
                //videoView.SetOnErrorListener(this);
                //videoView.Start();
            }



        }

        public async Task<string> AuthenticateDevice(string encodedAccountName, string deviceId, string hostUrl)
        {
            string url = "https://www.attinicomms2.com/api/AuthenticateDevice" + "?encodedAccountName=" + encodedAccountName + "&deviceId=" + deviceId + "&hostUrl=" + hostUrl;

            HttpClientHandler handler = new HttpClientHandler();
            HttpClient httpClient = new HttpClient(handler);
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        receiveStream = await response.Content.ReadAsStringAsync();
                        break;
                    case HttpStatusCode.Forbidden:
                        receiveStream = "UserInvalid";
                        break;
                    default:
                        receiveStream = "UserInvalid";
                        break;
                }


            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message.ToString());
            }
            return receiveStream;
        }


        private string RandomFilenameGenrator()
        {
            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "");
            GuidString = GuidString.Replace("+", "");
            return GuidString;
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
        public  async Task<AuthContainer> GetAccessToken(string siteUrl, PlatformParameters param)
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
                    Logout();
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
        public  void Logout()
        {
            try
            {

                if (AuthContext != null)
                {
                    AuthContext.TokenCache.Clear();
                    AuthContext = null;
                    AuthResult = null;
                }
                if (GraphAuthContext != null)
                {
                    GraphAuthContext.TokenCache.Clear();
                    GraphAuthContext = null;
                    GraphAuthResult = null;
                }

                //AppDelegate.NewToken = true;
                if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.LollipopMr1)
                {
                    CookieManager.Instance.RemoveAllCookies(null);
                    
                    CookieManager.Instance.Flush();
                }
                
                
            }
            catch (Exception ex)
            {

               
            }
        }
        public static async Task<AuthenticationResult> GetGraphAccessToken(PlatformParameters param)
        {
            try
            {
                if (!isInitialized)
                    return null;

                string resourceId = ServiceConstants.GRAPHURI;
                GraphAuthContext = new AuthenticationContext(ServiceConstants.AUTHORITY);
                if (GraphAuthContext.TokenCache.ReadItems().Any())
                    GraphAuthContext = new AuthenticationContext(GraphAuthContext.TokenCache.ReadItems().First().Authority);
                GraphAuthResult = await GraphAuthContext.AcquireTokenAsync(resourceId, ServiceConstants.CLIENTID, ServiceConstants.RETURNURI, param);

                UserFullName = AuthResult.UserInfo.GivenName + " " + AuthResult.UserInfo.FamilyName;

                isAuthenticated = true;

                return GraphAuthResult;
            }
            catch (Exception ex)
            {

            }

            return null;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode, data);

        }
        public async Task<System.IO.Stream> GetFileBytes(string accesstoken)
        {
            
            HttpClient httpClient = new HttpClient(new NativeMessageHandler());
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accesstoken );
            try
            {


                var res = await httpClient.GetAsync(vidLinkReq).ConfigureAwait(false);
                if (res.IsSuccessStatusCode)
                {
                    data = await res.Content.ReadAsStreamAsync().ConfigureAwait(false);

                }
                else
                {
                    Logout();
                }



                return data;

            }
            catch (Exception ex)
            {
                Logout();
                //throw;
            }

            return null;
        }
        public static byte[] ReadFully(System.IO.Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


        public async Task<string> RegisterDevice(string userEmail, string deviceId, string deviceName)
        {
            string url = "https://www.attinicomms2.com/api/RegisterDevice";

            //string paramString = JsonConvert.SerializeObject(parameters);
            string paramString = "DeviceId=" + deviceId + "&Name=" + deviceName + "&encodedAccountName=" + userEmail;
            var content = new StringContent(paramString, System.Text.Encoding.UTF8, "application/json");
            content.Headers.Clear();
            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            HttpClientHandler handler = new HttpClientHandler();
            HttpClient httpClient = new HttpClient(handler);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(url, content);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        receiveStream = await response.Content.ReadAsStringAsync();
                        break;
                    case HttpStatusCode.Forbidden:
                        receiveStream = "UserInvalid";
                        break;
                    default:
                        receiveStream = null;
                        break;
                }

            }
            catch (Exception e)
            {
                receiveStream = null;
            }
            return receiveStream;
        }

        private string GetDeviceId()
        {
            // TODO Auto-generated method stub
            string androidId = Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId);

            string telephoneId = "";
            string teleSim = "";
            TelephonyManager telMan = (TelephonyManager)this.GetSystemService(Context.TelephonyService);
            if (telMan != null)
            {
                if (telMan.DeviceId != null)
                {
                    telephoneId = telMan.DeviceId;
                }
                if (telMan.SimSerialNumber != null)
                {
                    teleSim = telMan.SimSerialNumber;
                }
            }
            UUID deviceUuid = new UUID(androidId.GetHashCode(), ((long)telephoneId.GetHashCode() << 32 | teleSim.GetHashCode()));

            return deviceUuid.ToString();
        }

        public bool OnError(MediaPlayer mp, [GeneratedEnum] MediaError what, int extra)
        {
            var m = what.ToString();
            return false;
        }
    }
}

