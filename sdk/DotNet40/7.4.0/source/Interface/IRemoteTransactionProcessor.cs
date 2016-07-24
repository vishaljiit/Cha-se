using System;
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
	/// 
	/// </summary>
	public interface IRemoteTransactionProcessor
	{
        string ProcessTransaction(string xml, string mid, string traceNumber);
        string ProcessTransaction(string transaction, byte[] data);
	}
}
