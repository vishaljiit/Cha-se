using System;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Paymentech.COM
{
	/// <summary>
	/// A Paymentech Orbital Transasction Request
	/// </summary>
	/// <remarks>
	/// <para>
	/// The Transaction class encapsulates the inner workings of the SSL 
	/// communication engine in order to provide an easy-to-use interface 
	/// for creating, sending, and evaluating the results of a secure 
	/// electronic payment transaction. 
	/// </para>
	/// </remarks>
    [Guid("FE3DADB0-98F2-4304-A6FE-5CA6A27E6522")]
	[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IIPOSTransaction 
	{
		/// <summary>
		/// Gets the transaction type of the transaction
		/// </summary>
		[DispId(1)]
		string TerminalID { get; set; }

		/// <summary>
		/// Gets a transaction element
		/// </summary>
		[DispId(2)]
        string MerchantID { get; set; }

        /// <summary>
        /// Sets a transaction element
        /// </summary>
        [DispId(4)]
        string Username { get; set; }

        /// <summary>
        /// Sets a transaction element
        /// </summary>
        [DispId(5)]
        string Password { get; set; }

        /// <summary>
		/// Processes a transaction
		/// </summary>
		/// <remarks>
		/// Process () submits a transaction to the Orbital Gateway and 
		/// retrieves a <see cref="Paymentech.Response"/> object.
		/// </remarks>
		/// <returns>The transaction response</returns>
		[DispId(6)]
		IIPOSResponse Process (ref byte[] data);

		/// <summary>
		/// Processes a transaction
		/// </summary>
        [DispId(7)]
        bool FirstInBatch { get; set; }

		/// <summary>
		/// Gets the transaction data in XML format
		/// </summary>
        [DispId(8)]
        int method { get; set; }

        /// <summary>
        /// Gets the transaction type
        /// </summary>
        [DispId(9)]
        IPOSTransactionType TransactionType { get; set; }

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
		[DispId(10)]
		void Dispose ();

        /// <summary>
        /// Processes a transaction
        /// </summary>
        [DispId(11)]
        string XML { get; }

        /// <summary>
        /// Creates a printable text description of the transaction data.
        /// </summary>
        /// <returns></returns>
        [DispId(12)]
        string ToString();

        /// <summary>
        /// Gets a transaction element
        /// </summary>
        [DispId(13)]
        string MessageType { get; set; }
    }
}
