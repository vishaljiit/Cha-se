using System;
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
	/// Interface for a TransactionProcessor
	/// </summary>
	public interface ITransactionProcessor
	{
        IResponse Process(ITransaction transaction);
		IIPOSResponse Process(IIPOSTransaction transaction, ref byte[] data);
	}
}
