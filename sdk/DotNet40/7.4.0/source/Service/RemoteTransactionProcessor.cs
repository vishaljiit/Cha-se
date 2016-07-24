using System;
using System.Text;
using Paymentech.Core;
using Paymentech.Comm;
using Paymentech.COM;

// Disables warnings for XML doc comments.
#pragma warning disable 1591
#pragma warning disable 1573
#pragma warning disable 1572
#pragma warning disable 1571
#pragma warning disable 1587
#pragma warning disable 1570

namespace Paymentech
{
	/// <summary>
	/// Used by the Orbital Service.  Implements IRemoteTransactionProcessor
	/// </summary>
    public class RemoteTransactionProcessor : MarshalByRefObject, Paymentech.IRemoteTransactionProcessor
	{
        private IFactory factory = null;

        public RemoteTransactionProcessor()
        {
            this.factory = new Factory();
            // Force logging to be configured.
            IConfigurator config = factory.Config;
        }
        
        public RemoteTransactionProcessor(IFactory factory)
		{
            this.factory = factory;
		}

		/// <summary>
		/// Processes a transaction with retry logic
		/// </summary>
		/// <param name="xml">An Orbital transaction in XML format</param>
		/// <param name="mid">The Merchant ID</param>
		/// <param name="traceNumber">The Trace Number</param>
		/// <returns>An Orbital Response in XML format</returns>
        public string ProcessTransaction(string xml, string mid, string traceNumber)
		{
            try
            {
                string response = factory.CommMgr.Process(xml, mid, traceNumber);
                if (response == null || response.Trim().Length == 0)
                    return null;
                return response;
            }
            catch (CommException ex)
            {
                string errorMessage = ex.Message;
                if (ex.GWErrorReason != null && ex.GWErrorReason.Trim().Length > 0)
                    errorMessage = ex.GWErrorReason;
                string msg = String.Format("Error caught processing transaction. ErrorCode={0}, ErrorString={1}, HTTPError={2}, GatewayError={3}",
                    ex.ErrorCode, errorMessage, ex.HTTPResponseCode, ex.GWErrorCode);
                factory.EngineLogger.Error(msg, ex);
                return String.Format("[{0},{1},{2},{3}]", (int)ex.ErrorCode, errorMessage, ex.HTTPResponseCode, ex.GWErrorCode);
            }
		}

        /// <summary>
        /// Processes a transaction
        /// </summary>
        /// <param name="trans">An NetConnect transaction.</param>
        /// <param name="data">The data to transmit to the server.</param>
        /// <returns>A NetConnect Response in XML format</returns>
        public string ProcessTransaction(string transaction, byte[] data)
        {
            try
            {
                IIPOSTransaction trans = new IPOSTransaction(transaction);
                CommArgs response = factory.CommMgr.Process(trans, data);
                return response.Response;
            }
            catch (CommException ex)
            {
                string reason = ex.GWErrorReason;
                if (reason == null)
                    reason = ex.Message;
                string msg = String.Format("Error caught processing transaction. ErrorCode={0}, ErrorString={1}, HTTPError={2}, GatewayError={3}",
                    ex.ErrorCode, reason, ex.HTTPResponseCode, ex.GWErrorCode);
                factory.EngineLogger.Error(msg, ex);
                return String.Format("[{0},{1},{2},{3}]", (int)ex.ErrorCode, reason, ex.HTTPResponseCode, ex.GWErrorCode);
            }
        }

	}
}
