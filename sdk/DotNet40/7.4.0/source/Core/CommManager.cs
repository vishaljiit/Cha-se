using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using Paymentech.COM;
using Paymentech.Comm;

// Disables warnings for XML doc comments.
#pragma warning disable 1591
#pragma warning disable 1573
#pragma warning disable 1572
#pragma warning disable 1571
#pragma warning disable 1587
#pragma warning disable 1570

namespace Paymentech.Core
{
    /// <summary>
    /// API for handling all communication operations.
    /// </summary>
    public class CommManager : CoreBase, ICommManager
    {
        private Hashtable cookies = new Hashtable();

        #region ICommManager Members

        public CommManager() : this(new Factory())
        {
        }

        public CommManager(IFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Process transaction with retry.
        /// </summary>
        /// <param name="xml">The XML of the request.</param>
        /// <param name="mid">The MerchantID</param>
        /// <param name="traceNumber">The Trace Number for use in retry.</param>
        /// <returns></returns>
        public string Process(string xml, string mid, string traceNumber)
        {
            IHTTPSConnect con = factory.MakeHTTPSConnect();
            CommArgs args = new CommArgs();
            args.COMMType = CommArgs.CommType.GW;
            args.Request = xml;
            if (!IsEmpty(traceNumber))
            {
                args.MerchantID = mid;
                args.TraceNumber = traceNumber;
            }
            if (!IsEmpty(mid))
                args.MerchantID = mid;
            con.CompleteTransaction(args);
            return args.Response;
        }

        /// <summary>
        /// Process transaction with retry.
        /// </summary>
        /// <param name="msg">The XML of the message to be sent.</param>
        /// <param name="data">The binary data to be sent.</param>
        /// <returns></returns>
        public CommArgs Process(IIPOSTransaction transaction, byte[] data)
        {
            IHTTPSConnect con = factory.MakeHTTPSConnect();
            CommArgs args = new CommArgs();
            args.COMMType = CommArgs.CommType.NC;
            args.FirstInBatch = transaction.FirstInBatch;
            if (args.FirstInBatch)
            {
                Logger.Debug("Clearing the cookie.");
                cookies.Remove(transaction.MerchantID);
            }
            switch ((NetConnectMethod)transaction.method)
            {
                case NetConnectMethod.HCSMethod:
                    args.CaptureMode = "HCS";
                    break;
                case NetConnectMethod.SLMMethod:
                    args.CaptureMode = "SLM";
                    break;
                case NetConnectMethod.TCSMethod:
                    args.CaptureMode = "TCS";
                    break;
            }
            if (!IsEmpty(transaction.MerchantID))
                args.MerchantID = transaction.MerchantID;
            if (!IsEmpty(transaction.MessageType))
                args.MsgFormat = transaction.MessageType;
            if (!IsEmpty(transaction.Username))
                args.NCUserName = transaction.Username;
            if (!IsEmpty(transaction.Password))
                args.NCPassword = transaction.Password;
            if (!IsEmpty(transaction.TerminalID))
                args.TerminalID = transaction.TerminalID;
            args.RequestAsByte = data;
            args.IsStateful = (transaction.TransactionType == IPOSTransactionType.TransTypeStateful);

            if (cookies.ContainsKey(args.MerchantID))
            {
                Logger.Debug("Sending the cookie.");
                args.Cookie = (ArrayList)cookies[args.MerchantID];
            }
            CommArgs response = con.CompleteTransaction(args);

            if (args.FirstInBatch)
            {
                Logger.Debug("Setting the cookie");
                cookies.Add(args.MerchantID, response.Cookie);
            }
            return response;
        }

        /// <summary>
        /// Process transaction with retry.
        /// </summary>
        /// <param name="msg">The XML of the message to be sent.</param>
        /// <param name="data">The binary data to be sent.</param>
        /// <returns></returns>
        public CommArgs Process(string transaction, byte[] data)
        {
            Hashtable properties = ParseTransactionString(transaction);
            IHTTPSConnect con = factory.MakeHTTPSConnect();
            CommArgs args = new CommArgs();
            args.COMMType = CommArgs.CommType.NC;
            string mid = (string)properties["MerchantID"];
            if (properties.ContainsKey("FirstInBatch"))
                args.FirstInBatch = ((string)properties["FirstInBatch"] == "true");
            if (args.FirstInBatch)
            {
                Logger.Debug("Clearing the cookie.");
                cookies.Remove(mid);
            }
            if (properties.ContainsKey("method"))
                args.CaptureMode = (string)properties["method"];
            if (properties.ContainsKey("MerchantID"))
                args.MerchantID = mid;
            if (properties.ContainsKey("MessageType"))
                args.MsgFormat = (string)properties["MessageType"];
            if (properties.ContainsKey("TerminalID"))
                args.TerminalID = (string)properties["TerminalID"];
            if (properties.ContainsKey("Username"))
                args.NCUserName = (string)properties["Username"];
            if (properties.ContainsKey("Password"))
                args.NCPassword = (string)properties["Password"];
            if (cookies.ContainsKey(mid))
            {
                Logger.Debug("Sending the cookie.");
                args.Cookie = (ArrayList)cookies[mid];
            }
            args.RequestAsByte = data;

            CommArgs response = con.CompleteTransaction(args);

            if (args.FirstInBatch)
            {
                Logger.Debug("Setting the cookie");
                cookies.Add(args.MerchantID, response.Cookie);
            }
            return response;
        }
        
        #endregion

        private Hashtable ParseTransactionString(string transaction)
        {
            Hashtable properties = new Hashtable();
            string[] props = transaction.Split(',');

            foreach (string prop in props)
            {
                string[] pair = prop.Split('=');
                if (pair.Length != 2)
                    continue;
                properties.Add(pair[0], pair[1]);
            }

            return properties;
        }
    }
}
