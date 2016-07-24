using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Xml;
using Paymentech.COM;
using Paymentech.Core;

namespace Paymentech
{
	/// <summary>
	/// Encapsulates the response from the Gateway.
	/// </summary>
    [Serializable]
    public class Response : TemplateBase, IResponse
    {
        private XmlDocument document = null;
        private string xml = null;
        private bool error = false;
        private int procStatus = -1;
        private bool quickResponse = false;
        private bool isProfileResponse = false;
        private bool isAccountUpdaterResponse = false;
        private string approvalStatus = null;
        private bool approved = false;
        private bool declined = false;
        private string cvv2 = null;
        private string txRefNum = null;
        private string message = null;
        private string authCode = null;
        private string responseCode = null;
        private string avs = null;

        private bool isError = false;
        private int errorCode = 0;
        private int httpError = 0;
        private int gatewayError = 0;
        private string errorString = null;

        // This only exists to keep the Dispose/IsValid functionality 
        // consistent for merchants who coded to the old SDK.
        private bool isValid = true;

        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="xml"></param>
        public Response(string xml) : this(xml, new Factory())
        {
        }

        /// <summary>
        /// Constructor used for unit testing, taking a Factory object.
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="factory"></param>
        public Response(string xml, IFactory factory) : this(false, 0, "", 0, 0, xml, factory)
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
        /// <param name="xml"></param>
        public Response(bool error, int errorCode, string errorString, int httpError, int gatewayError, string xml) 
            : this(error, errorCode, errorString, httpError, gatewayError, xml, new Factory())
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
		/// <param name="xml"></param>
		/// <param name="factory"></param>
        public Response(bool error, int errorCode, string errorString, int httpError, int gatewayError, string xml, IFactory factory)
        {
            this.isValid = true;
            this.isError = error;
            this.errorCode = errorCode;
            this.errorString = errorString;
            this.httpError = httpError;
            this.gatewayError = gatewayError;

            this.factory = factory;
            this.xml = xml;

            if (IsEmpty(xml))
                return;

            try
            {
                document = new XmlDocument();
                // Don't resolve the DTD.
                document.XmlResolver = null;
                document.LoadXml(xml);

                FillResponseProperties();
            }
            catch (Exception ex)
            {
              String msg = "Unable to parse response XML.  Response data follows...\n" + xml + "\nEnd of response data.";
              Console.WriteLine(msg);
              Logger.Error(msg);
              throw MakeError(ex, ex.Message);
            }
        }

        /// <summary>
        /// This is only used to maintain consistency of behavior
        /// between this version of the SDK and previous versions.
        /// This will ensure that merchants upgrading from previous
        /// versions will not require code changes during upgrade.
        /// </summary>
        ~Response()
        {
            isValid = false;
        }


        #region Public Methods

        /// <summary>
        /// Determines if the object is valid
        /// </summary>
        /// <remarks>
        /// Deprecated. This is no longer necessary.
        /// </remarks>
        public bool IsValid
        {
            get { return isValid; }
        }

        /// <summary>
        /// Gets a response value
        /// </summary>
        [IndexerName("Value")]
        public string this[string itemName]
        {
            get 
            {
                return GetField(itemName, true);
            }
        }

        /// <summary>
        /// Indicates whether there was an error processing the 
        /// transaction through the gateway.
        /// </summary>
        /// <remarks>
        /// The Good property has the opposite value of the <see cref="Error"/> property.
        /// <seealso cref="Error"/>
        /// </remarks>
        public bool Good
        {
            get
            {
                return !error;
            }
        }

        /// <summary>
        /// Gets the transaction status
        /// </summary>
        public int Status
        {
            get
            {
                return procStatus;
            }
        }

        /// <summary>
        /// Indicates whether the transaction was approved.
        /// </summary>
        /// <remarks><seealso cref="Declined"/></remarks>
        public bool Approved
        {
            get
            {
                return approved;
            }
        }

        /// <summary>
        /// Determines whether a transaction was declined
        /// </summary>
        public bool Declined
        {
            get
            {
                return declined;
            }
        }

        /// <summary>
        /// Gets the transaction reference number
        /// </summary>
        public string TxRefNum
        {
            get
            {
                return txRefNum;
            }
        }

        /// <summary>
        ///  Gets the authorization code
        /// </summary>
        public string AuthCode
        {
            get
            {
                return authCode;
            }
        }

        /// <summary>
        ///  Gets the gateway response message
        /// </summary>
        /// <remarks><seealso cref="Error"/>
        /// </remarks>
        public string Message
        {
            get
            {
                return message;
            }
        }

        /// <summary>
        /// Gets the CVV2 Response Code
        /// </summary>
        public string CVV2ResponseCode
        {
            get
            {
                return cvv2;
            }
        }

        /// <summary>
        /// Gets the CVV2 Response Code
        /// </summary>
        public string CVV2RespCode
        {
            get
            {
                return cvv2;
            }
        }

        /// <summary>
        /// Gets the AVS Response Code
        /// </summary>
        public string AVSResponseCode
        {
            get
            {
                return avs;
            }
        }

        /// <summary>
        /// Gets the AVS Response Code
        /// </summary>
        public string AVSRespCode
        {
            get
            {
                return avs;
            }
        }

        /// <summary>
        /// Indicates an error occurred within the gateway.  
        /// </summary>
        /// <remarks>
        /// <para>
        /// Low level communcation errors result in exceptions being thrown from within 
        /// <see cref="Transaction.Process()">Transaction.Process()</see>, but errors that occur within 
        /// the gateway will result in an Error value of True.  If Error is set
        /// to true, the error message will be contained in the <see cref="Message"/>
        /// property.
        /// </para>
        /// </remarks>
        public bool Error
        {
            get
            {
                return error;
            }
        }

        /// <summary>
        /// Gets the response code
        /// </summary>
        public string ResponseCode
        {
            get
            {
                return responseCode;
            }
        }

        /// <summary>
        /// Gets the contents of the response in XML format. This is the data that is returned from the Gateway.
        /// </summary>
        /// <returns>A string containing the response in XML format.</returns>
        public string XML
        {
            get
            {
                return xml;
            }
        }

        /// <summary>
        /// Gets the contents of the response in XML format. This is the data that is returned from the Gateway.
        /// </summary>
        /// <returns>A string containing the response in XML format.</returns>
        public string asXML()
        {
            return XML;
        }

		/// <summary>
		/// Gets the XML of the response with sensitive fields masked.
		/// </summary>
        public string MaskedXML
        {
            get
            {
                return MaskXML(XML);
            }
        }

        /// <summary>
        /// Releases all modules and allocated memory.
        /// </summary>
        /// <remarks>
        /// NOTE: This method is no longer required. 
        /// It remains only for backward compatibility, but has no function.
        /// </remarks>
        public void Dispose()
        {
            isValid = false;

            error = false;
            procStatus = -1;
            quickResponse = false;
            isProfileResponse = false;
            isAccountUpdaterResponse = false;
            approvalStatus = null;
            approved = false;
            declined = false;
            cvv2 = null;
            txRefNum = null;
            message = null;
            authCode = null;
            responseCode = null;
            avs = null;

            isError = false;
            errorCode = 0;
            httpError = 0;
            gatewayError = 0;
        }
        #endregion

        #region Private Methods

        private void FillResponseProperties()
        {
            quickResponse = (document.GetElementsByTagName("QuickResp").Count > 0);
            isProfileResponse = (document.GetElementsByTagName("ProfileResp").Count > 0);
            isAccountUpdaterResponse = (document.GetElementsByTagName("AccountUpdaterResp").Count > 0);
            approvalStatus = GetField("ApprovalStatus", false);

            string status = GetField("ProfileProcStatus", false);
            if (IsEmpty(status))
                status = GetField("ProcStatus", false);
            if (IsNumeric(status))
            {
                procStatus = Convert.ToInt32(status);
            } 

            if (IsNumeric(approvalStatus))
            {
                int approvalInt = Convert.ToInt32(approvalStatus);
                if (approvalInt == 1)
                    approved = true;
                else if (approvalInt == 0)
                    declined = true;
                else if (approvalInt > 1)
                    error = true;
            }
            else 
                error = (procStatus != 0);


            cvv2 = GetField("CVV2RespCode", false);
            txRefNum = GetField("TxRefNum", false);
            authCode = GetField("AuthCode", false);

            if (isProfileResponse || isAccountUpdaterResponse)
                message = GetField("CustomerProfileMessage", false);
            else
                message = GetField("StatusMsg", false);

            string responseType = GetResponseType();
            string responseTag = "";
            string avsTag = "";
            if (responseType != null && responseType.ToLower() == "host")
            {
                responseTag = "HostRespCode";
                avsTag = "HostAVSRespCode";
            }
            else
            {
                responseTag = "RespCode";
                avsTag = "AVSRespCode";
            }
            responseCode = GetField(responseTag, false);
            avs = GetField(avsTag, false);
        }

        private string GetResponseType()
        {
            if (quickResponse)
                return "gateway";

            string responseType = factory.Config["Response.response_type"];
            if (IsEmpty(responseType) || (responseType.ToLower() != "gateway" && responseType.ToLower() != "host"))
                throw MakeError("The config property \"Response.response_type={0}\" is invalid.", responseType);
            return responseType;
        }

        private string GetField(string itemName, bool throwOnError)
        {
            try
            {
                if (!isValid)
                    return null;
                if (itemName == "IsValid")
                    return null;
                XmlNodeList nodes = document.GetElementsByTagName(itemName);
                if (nodes.Count == 0)
                    return "";

                return GetNodeText(nodes[0]);
            }
            catch (Exception ex)
            {
                factory.EngineLogger.Error(ex.Message, ex);
                throw MakeError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Log and throw an exception.
        /// </summary>
        /// <param name="format">The format string for the error message.</param>
        /// <param name="args">Parameters to go into the message.</param>
        /// <returns>The exception to be thrown.</returns>
        private Exception MakeError(string format, params object[] args)
        {
            string message = String.Format(format, args);
            Logger.Error(message);
            return new Exception(message);
        }

        /// <summary>
        /// Log and throw an exception.
        /// </summary>
        /// <param name="ex">The original exception.</param>
        /// <param name="format">The format string for the error message.</param>
        /// <param name="args">Parameters to go into the message.</param>
        /// <returns>The exception to be thrown.</returns>
        private Exception MakeError(Exception ex, string format, params object[] args)
        {
            string message = String.Format(format, args);
            Logger.Error(message, ex);
            return new Exception(message, ex);
        }

        #endregion
    }
}
