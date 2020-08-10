using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace test_sms
{
    public partial class WebForm1 : System.Web.UI.Page
    {



        protected void Page_Load(object sender, EventArgs e)
        {



            //----------- temel bilgiler -- start---- //
            string kullaniciAdi = "api kullanıcı adı";
            string sifre = "api şifreniz.";


            string apiKEY = stringToMD5(kullaniciAdi + sifre);



            string smsMesaj = "test mesaj";
            string numaralar = "5445359675,5444444444";
            string baslik = "TEST";
            //----------- temel bilgiler -- end ---- //



            string url = "https://organikapi.com/v2/" + apiKEY + "/";
            bool isPost = false;



            // GET ile işlem yapılacak ise... - en basit yöntem.
            string URL_parameters = "header=" + baslik + "&gsms=" + numaralar + "&message=" + Base64Encode(smsMesaj);


            // POST ile işlem yapılacak ise... - daha gelişmiş seçenekler sunar.
            string xml_parameters = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
 + "<document>"
 + "<data>"
 + "   <deliveries>"
 + "      <options>"
 + "         <header>" + baslik + "</header>"
 + "      </options>"
 + "      <recipients>";

            for (var i=0;i< numaralar.Split(',').Length;i++)
            {
                xml_parameters += "<gsms>"+ numaralar.Split(',')[i]+ "</gsms>";
            }

            xml_parameters +="</recipients>"
+"      <message>"+ Base64Encode(smsMesaj) + "</message>"
+"   </deliveries>"
+"</data>"
+"</document>";




            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                wc.Encoding = Encoding.UTF8;

                string xmlResult = "";
                if (isPost)
                {
                    url += "sendsms/xml";
                    xmlResult = wc.UploadString(url, xml_parameters);

                }
                else
                {
                    url += "smsviaget/xml";
                    xmlResult = wc.UploadString(url + "?" + URL_parameters, "");

                }


                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlResult);
                XmlNodeList result = doc.SelectNodes("/response/result");
                string sonuc = result[0].InnerText;

                if (sonuc == "0")
                {
                    Response.Write("islem basarisiz.<hr/>");

                    XmlNodeList error = doc.SelectNodes("/response/error/message");
                    string error_message = error[0].InnerText;
                    Response.Write("Hata Mesaj: " + error_message);

                }
                else
                {
                    Response.Write("islem basarili.");
                }

            }/* WebClient */




        }/* Page_Load */




        public string stringToMD5(string input)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }//stringToMD5

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }//Base64Encode





    }//System.Web.UI
}//namespace test_sms