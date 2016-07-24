using System;
//using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Xml;
using Paymentech.Core;
using Paymentech.COM;

namespace Paymentech
{

    /// <summary>
    /// 
    /// </summary>
    [ComVisible(true)]
#if !DOTNET11
    [System.Runtime.CompilerServices.ScopelessEnum]
#endif
    [Guid("1F1B61CE-BB46-4dfe-B88B-CC64AA05A4AB")]
    public enum IPOSTransactionType
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(0)]
        [ComAliasName("TransTypeUnset")]
        TransTypeUnset = 0,
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        [ComAliasName("TransTypeStateless")]
        TransTypeStateless = 1,
        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        [ComAliasName("TransTypeStateful")]
        TransTypeStateful = 2
    }

    /// <summary>
    /// 
    /// </summary>
    [ComVisible(true)]
#if !DOTNET11
    [System.Runtime.CompilerServices.ScopelessEnum]
#endif
    [Guid("319401B2-7969-4dfb-858E-AF07947501B1")]
    public enum NetConnectMethod
    {
        /// <summary>
        /// 
        /// </summary>
        [DispId(0)]
        [ComAliasName("HCSMethod")]
        HCSMethod = 0,
        /// <summary>
        /// 
        /// </summary>
        [DispId(1)]
        [ComAliasName("TCSMethod")]
        TCSMethod = 1,
        /// <summary>
        /// 
        /// </summary>
        [DispId(2)]
        [ComAliasName("SLMMethod")]
        SLMMethod = 2,
        /// <summary>
        /// 
        /// </summary>
        [DispId(3)]
        [ComAliasName("DefaultMethod")]
        DefaultMethod = 99
    }

    /// <summary>
    /// 
    /// </summary>
    [Guid("5BB3DB0D-F5A0-4ac3-A30B-C88DC791F047")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Paymentech.IPOSTransaction")]
    public class IPOSTransaction : Paymentech.COM.IIPOSTransaction, IDisposable
    {
        private IFactory factory = null;
        private string terminalID = null;
        private string username = null;
        private string password = null;
        private string merchantID = null;
        private string messageType = null;
        private bool firstInBatch = false;
        private int netConnectMethod = (int)NetConnectMethod.DefaultMethod;
        private IPOSTransactionType transactionType = IPOSTransactionType.TransTypeUnset;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="VariableName"></param>
        /// <param name="VariableValue"></param>
        /// <returns></returns>
        [DllImport("KERNEL32.DLL")]
        public static extern bool SetEnvironmentVariable(string VariableName, string VariableValue);

        /// <summary>
        /// Default constructor.
        /// </summary>
        public IPOSTransaction() : this(new Factory())
        {
        }

		/// <summary>
		/// Default constructor. Any class derived from Transaction will use this
		/// constructor.
		/// </summary>
		public IPOSTransaction(IFactory factory)
		{
            this.factory = factory;
            IConfigurator config = Configurator.Instance;
        }

        /// <summary>
        /// Default constructor. Any class derived from Transaction will use this
        /// constructor.
        /// </summary>
        public IPOSTransaction(string xml)
        {
            string result = GetProperty("first_in_batch", xml);
            if (result != null)
                FirstInBatch = (result.ToLower() == "true");
            result = GetProperty("merchant_id", xml);
            if (result != null)
                MerchantID = result;
            result = GetProperty("method", xml);
            if (result != null)
                method = StringToInt(result);
            result = GetProperty("password", xml);
            if (result != null)
                Password = result;
            result = GetProperty("terminal_id", xml);
            if (result != null)
                TerminalID = result;
            result = GetProperty("message_type", xml);
            if (result != null)
                MessageType = result;
            result = GetProperty("transaction_type", xml);
            if (result.ToLower() == "transtypestateful")
                TransactionType = IPOSTransactionType.TransTypeStateful;
            else
                TransactionType = IPOSTransactionType.TransTypeStateless; result = GetProperty("username", xml);
            if (result != null)
                Username = result;
        }

        private int StringToInt(string val)
        {
			try 
			{
				return Int32.Parse(val);
			}
			catch
			{
				return 0;
			}
		}

        private string GetProperty(string name, string xml)
        {
            int start = xml.IndexOf(String.Format("<{0}>", name));
            if (start == -1)
                return null;
            start += (name.Length + 2);
            int end = xml.IndexOf(String.Format("</{0}>", name), start);
            if (end == -1)
                return null;
            return xml.Substring(start, end - start);
        }

        /// <summary>
        /// 
        /// </summary>
        public string TerminalID 
        {
            get { return terminalID; }
            set
            {
                SetSafeString(value, ref terminalID);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string Username
        {
            get { return username; }
            set
            {
                SetSafeString(value, ref username);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string Password
        {
            get { return password; }
            set
            {
                if (value != null)
                    password = value;
                else
                    password = "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IPOSTransactionType TransactionType
        {
            get { return transactionType; }
            set
            {
                transactionType = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string MerchantID
        {
            get { return merchantID; }
            set
            {
                SetSafeString(value, ref merchantID);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string MessageType
        {
            get { return messageType; }
            set
            {
                SetSafeString(value, ref messageType);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IIPOSResponse Process(ref byte[] data)
        {

            // Get the transaction processor from the factory
            ITransactionProcessor processor = TransactionProcessorFactory.GetProcessor(factory);
            return processor.Process(this, ref data);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool FirstInBatch 
        {
            get { return firstInBatch; }
            set { firstInBatch = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int method
        {
            get
            {
                return netConnectMethod; 
            }
            set
            {
                netConnectMethod = value;
            }
        }

        /// <summary>
        /// Creates a printable text description of the transaction data.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder text = new StringBuilder();
            text.AppendFormat("FirstInBatch={0}", (firstInBatch) ? "true" : "false");
            if (merchantID != null)
                text.AppendFormat(",MerchantID={0}", merchantID);
            text.AppendFormat(",method={0}", method);
            if (username != null)
                text.AppendFormat(",Username={0}", username);
            if (password != null)
                text.AppendFormat(",Password={0}", password);
            if (terminalID != null)
                text.AppendFormat(",TerminalID={0}", terminalID);
            text.AppendFormat(",TransactionType={0}", transactionType);

            return text.ToString();
        }

        /// <summary>
        /// Gets the transaction data in XML format
        /// </summary>
        public string XML
        {
            get
            {
                StringBuilder xml = new StringBuilder("<xml>");
                xml.AppendFormat("<first_in_batch>{0}</first_in_batch>", FirstInBatch);
                if (MerchantID != null)
                {
                    xml.AppendFormat("<merchant_id>{0}</merchant_id>", MerchantID);
                }
                xml.AppendFormat("<method>{0}</method>", method);
                if (Password != null)
                {
                    xml.AppendFormat("<password>{0}</password>", Password);
                }
                if (TerminalID != null)
                {
                    xml.AppendFormat("<terminal_id>{0}</terminal_id>", TerminalID);
                }
                if (MessageType != null)
                    xml.AppendFormat("<message_type>{0}</message_type>", MessageType); 
                xml.AppendFormat("<transaction_type>{0}</transaction_type>", TransactionType);

                if (Username != null)
                {
                    xml.AppendFormat("<username>{0}</username>", Username);
                }
                xml.Append("</xml>");
                return xml.ToString();
            }
        }

		// IDisposable
		/// <summary>
		/// Releases all allocated resources
		/// </summary>
		/// <remarks>
		/// The Dispose () method allows the developer to release resources 
		/// allocated by the Transaction object without waiting for the 
		/// garbage collector.  After calling Dispose () the Transaction object 
		/// becomes invalid and can not be used.
		/// </remarks>
		public  void Dispose ()
		{
		}

        /// <summary>
        /// The strings that are set in this class should not be null. 
        /// This method ensures that they are set properly.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="data"></param>
        private void SetSafeString(string val, ref string data)
        {
            if (val != null)
                data = val;
            else
                data = "";
        }
    }
}
