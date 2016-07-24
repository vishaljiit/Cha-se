using System;
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
	/// 
	/// </summary>
    public interface ICommManager
    {
        /// <summary>
        /// Process a transaction with retry.
        /// </summary>
        /// <param name="xml">The XML contents of the request.</param>
        /// <param name="mid">The Merchant ID</param>
        /// <param name="traceNumber">The Trace Number, used for retries.</param>
        /// <returns>The XML of the Gateway's response.</returns>
        string Process(string xml, string mid, string traceNumber);
        /// <summary>
        /// Process a NetConnect request.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data">The message being sent.</param>
        /// <returns>Contains the XML of the response, plus error information.</returns>
        CommArgs Process(IIPOSTransaction transaction, byte[] data);

        CommArgs Process(string transaction, byte[] data);
    }

}
