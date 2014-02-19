using System.IO;
using System.Net;
using System.Text;

namespace vkinviter
{
    public class HttpWebResponseEx
    {
        public HttpWebResponse HttpWebResponse { get; set; }

        private string _ResponseText;

        public string ResponseText
        {
            get
            {
                if (_ResponseText == null)
                {
                    using (StreamReader sReader = new StreamReader(HttpWebResponse.GetResponseStream(), 
                        Encoding.GetEncoding(HttpWebResponse.CharacterSet)))
                    {
                        _ResponseText = sReader.ReadToEnd();
                    }
                }
                return _ResponseText;
            }
        }       

        public override string ToString()
        {
            return ResponseText;
        }
    }
}
