using System;
using Paymentech.Core;
using Paymentech.COM;
using Paymentech.Comm;

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
	/// In-process transaction processor
	/// </summary>
	/// <remarks>
	/// Implements ITransactionProcessor and simply forwards
	/// transactions to the LineHandler for processing 
	/// </remarks>
    public class TransactionProcessor : CoreBase, Paymentech.ITransactionProcessor
	{
        public TransactionProcessor(IFactory factory)
		{
            this.factory = factory;
		}

		/// <summary>
		/// Processes a transaction
		/// </summary>
		/// <param name="transaction">The transaction to be processed</param>
		/// <returns>The response</returns>
		public IResponse Process (ITransaction transaction)
		{
            SetDefaultFieldValue(transaction, "OrbitalConnectionUsername");
            SetDefaultFieldValue(transaction, "OrbitalConnectionPassword");
            Logger.DebugFormat("OrbitalConnectionUsername=[{0}]", transaction["OrbitalConnectionUsername"]);
            return new Response(factory.CommMgr.Process(transaction.XML, transaction["MerchantID"], transaction["TraceNumber"]));
		}

        public IIPOSResponse Process(IIPOSTransaction transaction, ref byte[] data)
        {
            try
            {
                CommArgs response = factory.CommMgr.Process(transaction, data);
                return new IPOSResponse(false, 0, "", 0, 0, response.Response);
            }
            catch (CommException ex)
            {
                string reason = ex.GWErrorReason;
                if (reason == null)
                    reason = ex.Message;

                int httpCode = HTTPSConnect.StringToInt(ex.HTTPResponseCode);
                int gwError = HTTPSConnect.StringToInt(ex.GWErrorCode);
                string msg = String.Format("Error caught processing transaction. ErrorCode={0}, ErrorString={1}, HTTPError={2}, GatewayError={3}",
                    ex.ErrorCode, reason, httpCode, gwError);
                Logger.Error(msg, ex);
                return new IPOSResponse(true, (int)ex.ErrorCode, reason, httpCode, gwError, null);
            }
        }
	}
}
