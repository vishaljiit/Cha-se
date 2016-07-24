using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using System.Text;
using Paymentech.COM;
using Paymentech.Comm;
using Paymentech.Core;
using log4net;

namespace Paymentech
{
	/// <summary>
	/// Implements ITransactionProcessor, and forwards a transaction request
	/// to the Transaction processing service.
	/// </summary>
    internal class TransactionProcessorProxy : CoreBase, Paymentech.ITransactionProcessor
	{

        // Mutex to lock the channel
        private static Mutex s_channelMutex = new Mutex();
        // The static TcpChannel
        private static TcpChannel s_channel = null;

        public TransactionProcessorProxy(IFactory factory)
		{
            this.factory = factory;
		}

		/// <summary>
		/// Processes a transaction
		/// </summary>
		/// <remarks >
		/// Passes the transaction to a remote transaction processor for 
		/// processing.
		/// </remarks>
		/// <param name="transaction">The transaction to be processed</param>
		/// <returns>The gateway response</returns>
		public IResponse Process (ITransaction transaction)
		{
			// Register the channel
			// Double-check lock on the mutex
			if (s_channel == null)
			{
				s_channelMutex.WaitOne ();
				try
				{
					if (s_channel == null)
					{
                        factory.EngineLogger.Debug("Creating a TcpChannel for .Net Remoting.");
						s_channel = new TcpChannel ();
						ChannelServices.RegisterChannel (s_channel);
					}
				}
				finally
				{
					s_channelMutex.ReleaseMutex ();
				}
			}

			// Create the RemoteTransactionProcessor

            SetDefaultFieldValue(transaction, "OrbitalConnectionUsername");
            SetDefaultFieldValue(transaction, "OrbitalConnectionPassword");
            factory.EngineLogger.DebugFormat("OrbitalConnectionUsername=[{0}]", transaction["OrbitalConnectionUsername"]);
            
            IRemoteTransactionProcessor proc = 
				(IRemoteTransactionProcessor)
				Activator.GetObject (typeof (IRemoteTransactionProcessor), 
				URL);

			// Convert the transaction to XML
			string xml = transaction.XML;
            Request request = ((Transaction)transaction).Request;

			// Process the transaction
            string responseXML = proc.ProcessTransaction(xml, request["MerchantID"], request["TraceNumber"]);
            if (IsEmpty(responseXML))
            {
                Logger.Debug("No response was received from remote processor.");
                return null;
            }

            if (responseXML.Trim().StartsWith("["))
            {
                responseXML = responseXML.Trim().Replace("[", "").Replace("]", "");
                string[] codes = responseXML.Split(',');
                throw MakeError(codes[1]);
            }

            return new Response(responseXML);
		}

        /// <summary>
        /// Processes a transaction
        /// </summary>
        /// <remarks >
        /// Passes a NetConnect transaction to a remote transaction processor for 
        /// processing.
        /// </remarks>
        /// <param name="transaction">The transaction to be processed</param>
        /// <param name="data"></param>
        /// <returns>The gateway response</returns>
        public IIPOSResponse Process(IIPOSTransaction transaction, ref byte[] data)
        {
            // Register the channel
            // Double-check lock on the mutex
            if (s_channel == null)
            {
                s_channelMutex.WaitOne();
                try
                {
                    if (s_channel == null)
                    {
                        s_channel = new TcpChannel();
                        ChannelServices.RegisterChannel(s_channel);
                    }
                }
                finally
                {
                    s_channelMutex.ReleaseMutex();
                }
            }

            // Create the RemoteTransactionProcessor

            IRemoteTransactionProcessor proc = (IRemoteTransactionProcessor)
                Activator.GetObject(typeof(IRemoteTransactionProcessor), URL);

            // Process the transaction
            //string tran = transaction.XML;
            string response = proc.ProcessTransaction(transaction.XML, data);
            if (response.StartsWith("["))
            {
                response = response.Replace("[", "").Replace("]", "");
                string[] codes = response.Split(',');
                int errorCode = ToInt(codes[0]);
                int httpError = ToInt(codes[2]);
                int gwError = ToInt(codes[3]);
                return new IPOSResponse(true, errorCode, codes[1], httpError, gwError, null);
            }

            return new IPOSResponse(false, 0, "", 0, 0, response);
            
        }

		/// <summary>
		/// Gets the URL for the Remote transaction processor.  The URL is built
		/// from the DotNet.Transaction.Service.ServerName, 
		/// DotNet.Transaction.Service.Port, and 
		/// DotNet.Transaction.Service.URI config settings.
		/// </summary>
		private string URL
		{
			get
			{
				string protocol = "tcp";

				//"tcp://localhost:9999/TransactionProcessor"
				StringBuilder sb = new StringBuilder (protocol);
				sb.Append ("://");
				sb.Append (ServerName);
				sb.Append (":");
				sb.Append (ServicePort.ToString());
				sb.Append ("/");
				sb.Append (ServiceURI);
					
				return sb.ToString ();
			}
		}

		/// <summary>
		///  Gets the DotNet.Transaction.Service.Port config value
		/// </summary>
		internal int ServicePort
		{
			get
			{
				int ret = 0;
				string configName = "DotNet.Transaction.Service.Port";
                string port = factory.Config[configName];
				if (port == "")
					throw MakeError("Missing config value " + configName);
				try
				{
					ret = Convert.ToInt32 (port);
				}
				catch (Exception)
				{
					throw MakeError("Invalid config value [" + port + "] specified for " + configName);
				}
				return ret;
			}
		}

		/// <summary>
		/// Gets the DotNet.Transaction.Service.URI config value
		/// </summary>
		internal string ServiceURI
		{
			get
			{
				string ret = "";
				string configName = "DotNet.Transaction.Service.URI";
                ret = factory.Config[configName];

				if (ret == "")
					throw MakeError("Missing config value " + configName);

				return ret;
			}
		}

		/// <summary>
		/// Gets the DotNet.Transaction.Service.ServerName config value
		/// </summary>
		internal string ServerName
		{
			get
			{
				string ret = "";
				string configName = "DotNet.Transaction.Service.ServerName";
                ret = factory.Config[configName];

                if (ret == "")
                {
                    throw MakeError("Missing config value {0}", configName);
                }

				return ret;
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
    }
}
