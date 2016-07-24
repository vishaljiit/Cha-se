using System;
//using System.Collections.Generic;
using System.Text;
using Paymentech.COM;
using System.Runtime.InteropServices;

namespace Paymentech
{
    /// <summary>
    /// 
    /// </summary>
    [Guid("f82b52b9-a043-4137-a98a-72977ee4ab6a")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Paymentech.IPOSResponse")]
#if DOTNET40
    [System.Security.SecuritySafeCritical]
#endif
    public class IPOSResponse : IIPOSResponse, IDisposable 
    {
        private byte[] data = null;
        private bool isError = false;
        private int errorCode = 0;
        private int httpError = 0;
        private int gatewayError = 0;
        private string errorString = null;
        private string strData = null;

        /// <summary>
        /// 
        /// </summary>
        public IPOSResponse()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="errorCode"></param>
        /// <param name="errorString"></param>
        /// <param name="httpError"></param>
        /// <param name="gatewayError"></param>
        /// <param name="data"></param>
        public IPOSResponse(bool error, int errorCode, string errorString, int httpError, int gatewayError, string data)
        {
            this.isError = error;
            this.errorCode = errorCode;
            this.errorString = errorString;
            this.httpError = httpError;
            this.gatewayError = gatewayError;
            this.strData = data;

            if (data == null)
                return;

            this.data = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
                this.data[i] = (byte)data[i];
        }
        /// <summary>
        /// 
        /// </summary>
        public int ErrorCode 
        {
            get { return errorCode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ErrorString 
        {
            get { return errorString; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int HTTPError 
        {
            get { return httpError; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int GatewayError 
        {
            get { return gatewayError; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Error 
        {
            get { return isError; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public byte[] Data 
        {
            get { return data; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public string XML
        {
            get
            {
                StringBuilder xml = new StringBuilder("<xml><is_error>");
                xml.Append((isError) ? "true" : "false");
                xml.Append("</is_error><error_code>");
                xml.Append(errorCode);
                xml.Append("</error_code><http_error>");
                xml.Append(httpError);
                xml.Append("</http_error><gateway_error>");
                xml.Append(gatewayError);
                xml.Append("</gateway_error><error_string>");
                xml.Append(errorString);
                xml.Append("</error_string><data>");
                xml.Append(strData);
                xml.Append("</data></xml>");
                return xml.ToString();
            }
        }

    }
}
