using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Cache;

namespace ConsoleApp.SampleScreenScraping
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
                throw new Exception("Args length not 2, you must not have included username and password");

            string userName = args[0];
            string password = args[1];

            // WebClient navigate to Platts website
            //CookieWebClient wc = new CookieWebClient();
            string uri = "https://success.peco.com";// "/downloadsettlements.asp?f=SetABacctDec-2011.TXT.part.95755";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.Credentials = new NetworkCredential(userName, password);
            //webRequest.KeepAlive = true;
            webRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic U3BhcmtFZ3k6U1BBODRFR1k=");
            webRequest.Accept = "*/*";
            webRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            webRequest.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
            webRequest.UserAgent = "User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            webRequest.CookieContainer = new CookieContainer();

            //webRequest.Method = WebRequestMethods.Http.Post;
            //webRequest.ContentType = "application/x-www-form-urlencoded";
            //string postData =
            //    "uid=SparkEgy&for=SparkEgy&p_date_default=DEC-2011&reconDatePicker=DEC-2011&reconDatePicker2=12%2F08%2F2011&selSuppliers=8184&radSettlementType=B&radAggLevel=1";
            //byte[] requestBytes = Encoding.ASCII.GetBytes(postData);

            //webRequest.ContentLength = requestBytes.Length;
            //using (Stream sr = webRequest.GetRequestStream())
            //{
            //    sr.Write(requestBytes, 0, requestBytes.Length);
            //}


            //SetABacctDec-2011.TXT.part.95755

            //            string cookieString = "SUCCESS=sessact=3%2F8%2F2012+9%3A35%3A43+PM&LOGON%5FSUPPLIER%5FINDEX=8184&LOGON%5FSUPPLIER=SparkEgy&db%5Faction=OK&login%5Fid=97124; ASPSESSIONIDASSQADCR=ABHHMMCAGIOBMJBIIBICFMMO";
            //            wc.Headers.Add(HttpRequestHeader.Cookie, cookieString);
            //wc.Credentials = new NetworkCredential(userName, password);

            string responseString = string.Empty;
            HttpWebResponse response = null;

            try
            {
                //responseString = wc.DownloadString(uri);
                response = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (Exception ex)
            {
                throw;
            }

            byte[] responseBytes = null;
            using (StreamReader reseponseStreamReader = new StreamReader(response.GetResponseStream()))
            {
                responseString = reseponseStreamReader.ReadToEnd();
            }

            int i = 0;

            // Have a response with 2 cookies. Time to use those cookies in the next request
            //----------------------------------------
            // WebClient navigate to Platts website
            //CookieWebClient wc = new CookieWebClient();
            string uriPost = "https://success.peco.com/downloadsettlements.asp";// "?f=SetABacctDec-2011.TXT.part.95755";
            HttpWebRequest webRequestPost = (HttpWebRequest)WebRequest.Create(uriPost);
            webRequestPost.Credentials = new NetworkCredential(userName, password);
            webRequestPost.ServicePoint.Expect100Continue = false;

            webRequestPost.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            webRequestPost.KeepAlive = true;
            webRequestPost.Referer = "https://success.peco.com/downloadsettlements.asp";

            webRequestPost.AllowAutoRedirect = false;
            //webRequestPost.MaximumAutomaticRedirections = 100;

            webRequestPost.Headers.Add(HttpRequestHeader.Authorization, "Basic U3BhcmtFZ3k6U1BBODRFR1k=");
            webRequestPost.Accept = "text/html, application/xhtml+xml, */*";
            webRequestPost.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            webRequestPost.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
            webRequestPost.UserAgent = "User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            webRequestPost.CookieContainer = new CookieContainer();

            webRequestPost.CookieContainer.Add(response.Cookies);   // use the cookies from the response

            webRequestPost.Method = WebRequestMethods.Http.Post;
            webRequestPost.ContentType = "application/x-www-form-urlencoded";
            string postData =
                "uid=SparkEgy&for=SparkEgy&p_date_default=DEC-2011&reconDatePicker=DEC-2011&reconDatePicker2=12%2F08%2F2011&selSuppliers=8184&radSettlementType=B&radAggLevel=1";
            byte[] requestBytes = Encoding.ASCII.GetBytes(postData);

            string responseStringPost = string.Empty;
            HttpWebResponse responsePost = null;

            webRequestPost.ContentLength = requestBytes.Length;
            using (Stream sr = webRequestPost.GetRequestStream())
            {
                sr.Write(requestBytes, 0, requestBytes.Length);
            }

            try
            {
                responsePost = (HttpWebResponse)webRequestPost.GetResponse();
            }
            catch (Exception ex)
            {
                throw;
            }

            int j = 0;


            byte[] responseBytesPost = null;
            using (StreamReader reseponseStreamReader = new StreamReader(responsePost.GetResponseStream()))
            {
                responseStringPost = reseponseStreamReader.ReadToEnd();
            }

            int k = 0;

            // Now that I have the response, "carve" out the remaining URL:
            // "<head><title>Object moved</title></head>\n<body><h1>Object Moved</h1>This object may be found <a HREF=\"downloadsettlement.asp?f=SetABacctDec-2011.TXT.part.50001\">here</a>.</body>\n".Substring("<head><title>Object moved</title></head>\n<body><h1>Object Moved</h1>This object may be found <a HREF=\"downloadsettlement.asp?f=SetABacctDec-2011.TXT.part.50001\">here</a>.</body>\n".IndexOf("downloadsettlement.asp")).Replace("\">here</a>.</body>", "")
            string urlSuffix = responseStringPost.Substring(responseStringPost.IndexOf("downloadsettlement.asp")).Replace("\">here</a>.</body>", "");
            int z = 0;

            // One final request to get the file
            string uriFinal = "https://success.peco.com/" + urlSuffix;
            HttpWebRequest webRequestFinal = (HttpWebRequest)WebRequest.Create(uriFinal);
            webRequestFinal.Credentials = new NetworkCredential(userName, password);
            //webRequest.KeepAlive = true;
            webRequestFinal.Headers.Add(HttpRequestHeader.Authorization, "Basic U3BhcmtFZ3k6U1BBODRFR1k=");
            webRequestFinal.Accept = "*/*";
            webRequestFinal.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            webRequestFinal.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
            webRequestFinal.UserAgent = "User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            webRequestFinal.CookieContainer = new CookieContainer();
            webRequestFinal.CookieContainer.Add(response.Cookies);


            string responseStringFinal = string.Empty;
            HttpWebResponse responseFinal = null;

            try
            {
                responseFinal = (HttpWebResponse)webRequestFinal.GetResponse();
            }
            catch (Exception ex)
            {
                throw;
            }

            byte[] responseBytesFinal = null;
            using (StreamReader reseponseStreamReader = new StreamReader(responseFinal.GetResponseStream()))
            {
                responseStringFinal = reseponseStreamReader.ReadToEnd();
            }

            // save string into Text file
            using (StreamWriter sw = new StreamWriter(@"c:\data\PECOFiles\Test.txt", false))
            {
                sw.Write(responseStringFinal);
            }


            Console.WriteLine("Done");
            Console.Read();
        }
    }
}
