using System;
using System.Text;
using Paymentech.Core;

namespace Paymentech
{
	/// <summary>
	/// A factory for ITransactionProcessor's
	/// </summary>
#if DOTNET40
    [System.Security.SecuritySafeCritical]
#endif
    internal class TransactionProcessorFactory
	{
        internal TransactionProcessorFactory()
        {
		}

		/// <summary>
		/// Returns a Transaction Processor
		/// </summary>
		/// <remarks>
		/// Returns either a dll-based or a service-based transaction
		/// processor depending upon the DotNet.Transaction.EngineType
		/// config value.
		/// </remarks>
		/// <returns>A newly created transaction processor</returns>
        internal static ITransactionProcessor GetProcessor(IFactory factory)
		{
			ITransactionProcessor ret = null;
            string type = factory.Config["DotNet.Transaction.EngineType"];
			switch (type.ToUpper())
			{
				case "SERVICE":
                    factory.EngineLogger.Debug("Creating a TransactionProcessorProxy...");
					ret = new TransactionProcessorProxy (factory);
					break;
				case "DLL":
                    factory.EngineLogger.Debug("Creating a TransactionProcessor...");
                    ret = new TransactionProcessor(factory);
					break;
				default:
					string err = String.Format("Invalid config value \"{0}\" for DotNet.Transaction.EngineType", type);
                    factory.EngineLogger.Error(err);
					throw new Exception (err);
			}
			return ret;
		}
	}
}
