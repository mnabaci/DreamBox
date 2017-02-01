using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;

namespace ImageProcessing
{
    public class Server
    {        
        #region SESSION
        private string sessionFileName;
        public string session = null;
        public string SESSION{
            get
            {         
                if ((session == null) && File.Exists(sessionFileName))
                    session = File.ReadAllText(sessionFileName).Trim();
                return session;
            }
            set
            {
                this.session = value;
                File.WriteAllText(sessionFileName, SESSION);
            }
        }
        #endregion
        #region Instance
        private static Server instance;
        public static Server Instance
        {
            get
            {
                if (instance == null)
                    instance = new Server();
                return instance;
            }
        }
        #endregion
        private Server()
        {
            sessionFileName = "session.txt";
        }
        public string URL
        {
            get
            {
                return Settings.Instance.PHPServerURL;
            }
        }
        public string Username
        {
            get
            {
                return Settings.Instance.PHPServerUsername;
            }
        }
        public string Password
        {
            get
            {
                return Settings.Instance.PHPServerPassword;
            }
        }
        private bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public bool Login()
        {
            HttpWebResponse response2 = null;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(URL + "/dogrulama/dogrula");
                request.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
                var postData = "rumuz=" + Username;
                postData += "&sifre=" + Password;
                postData += "&format=json";
                request.Headers["X-Requested-With"] = "XMLHttpRequest";
                var data = System.Text.Encoding.ASCII.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.Timeout = 3000;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                SESSION = response.Headers.Get("set-cookie");
                return true;
            }
            catch (WebException hata)
            {
                response2 = (HttpWebResponse)hata.Response;
                if (response2 == null) return false;
                var responseString2 = new StreamReader(response2.GetResponseStream()).ReadToEnd();
                JObject obje = JObject.Parse(responseString2.ToString());
                string array = (string)obje["exceptionMessage"];
                int a = (int)response2.StatusCode;
                //throw new Exception(array);
                return false;
            }
        }
        private string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
        private static readonly Dictionary<Guid, string> _knownImageFormats =
            (from p in typeof(ImageFormat).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
             where p.PropertyType == typeof(ImageFormat)
             let value = (ImageFormat)p.GetValue(null, null)
             select new { Guid = value.Guid, Name = value.ToString() })
            .ToDictionary(p => p.Guid, p => p.Name);

        private static string GetImageFormatName(ImageFormat format)
        {
            string name;
            if (_knownImageFormats.TryGetValue(format.Guid, out name))
                return name;
            return null;
        }
        public bool SaveImage(Image image,ImageFormat format,string filename)
        {
            HttpWebResponse response2 = null;

            try
            {
                string sFormat = GetImageFormatName(format);
                var postData = "format=json";
                postData += "&ad=" + filename;
                postData += "&uzanti=" + sFormat.ToLower();
                postData += "&base64=data:image/"+sFormat.ToLower()+";base64," + ImageToBase64(image,format);

                var request = (HttpWebRequest)WebRequest.Create(URL + "/gbox/image/pcden-ekle");
                request.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
                request.Headers["X-Requested-With"] = "XMLHttpRequest";
                request.Headers["cookie"] = SESSION;
                var data = System.Text.Encoding.ASCII.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                string cookie_value = SESSION;
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return true;
            }
            catch (WebException hata)
            {
                response2 = (HttpWebResponse)hata.Response;
                if (response2 == null) return false;
                var responseString2 = new StreamReader(response2.GetResponseStream()).ReadToEnd();
                int a = (int)response2.StatusCode;
                if (a == 401)
                {
                    throw new UnAutExeption();
                }
                else
                {
                    if (responseString2 != "")
                    {
                        JObject obje = JObject.Parse(responseString2.ToString());
                        string array = (string)obje["exceptionMessage"];
                        return false;
                    }
                    else return false;

                }
            }
        }
    }
}
