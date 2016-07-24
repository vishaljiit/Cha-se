using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using Paymentech;

namespace Paymentech.COM
{
	/// <summary>
	/// Encapsulates an Orbital Gateway response.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The Response class encapsulates the Orbital Gatway response and 
	/// can be used to evaluate the results of a transaction.
	/// </para>
	/// <para>
	/// Using the Response object typically involves checking the <see cref="Error"/> property to
	/// determine whether there were errors in the transaction, and then determining the
	/// outcome of the transaction by checking the <see cref="Approved"/> or <see cref="Declined"/> property.
	/// </para>
	/// </remarks>
	[Guid("796F1129-280E-432d-B9E2-4DD58DBF18DD")]
	[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IResponse 
	{
		/// <summary>
		/// Determines if the object is valid
		/// </summary>
		[DispId(1)]
		bool Good { get; }

		/// <summary>
		/// Gets the transaction status
		/// </summary>
		[DispId(2)]
		int Status { get; }

		/// <summary>
		/// Indicates whether the transaction was approved.
		/// </summary>
		/// <remarks><seealso cref="get_Declined"/></remarks>
		[DispId(3)]
		bool Approved { get; }

		/// <summary>
		/// Determines whether a transaction was declined
		/// </summary>
		[DispId(4)]
		bool Declined { get; }

		/// <summary>
		/// Gets the transaction reference number
		/// </summary>
		[DispId(5)]
		string TxRefNum { get; }

		/// <summary>
		///  Gets the authorization code
		/// </summary>
		[DispId(6)]
		string AuthCode { get; }

		/// <summary>
		///  Gets the gateway response message
		/// </summary>
		[DispId(7)]
		string Message { get; }

		/// <summary>
		/// Gets the CVV2 Response Code
		/// </summary>
		[DispId(8)]
		string CVV2RespCode { get; }

		/// <summary>
		/// Gets the AVS Response Code
		/// </summary>
		[DispId(9)]
		string AVSRespCode { get; }

		/// <summary>
		/// Gets the value of a field.
		/// </summary>
		/// <returns></returns>
		[DispId(10)]
		[IndexerName("Value")]
		string this[string index] { get;  }

		/// <summary>
		/// Indicates an error occurred within the gateway.  
		/// </summary>
		/// <remarks>
		/// <para>
		/// Low level communcation errors result in exceptions being thrown from within 
		/// <see cref="Transaction.Process()">Transaction.Process()</see>, but errors that occur within 
		/// the gateway will result in an Error value of True.  If Error is set
		/// to true, the error message will be contained in the <see cref="get_Message"/>
		/// property.
		/// </para>
		/// </remarks>
		[DispId(11)]
		bool Error { get; }

		/// <summary>
		/// Gets the response code
		/// </summary>
		[DispId(12)]
		string ResponseCode { get; }

        /// <summary>
        /// Gets the contents of the response in XML format. This is the data that is returned from the Gateway.
        /// </summary>
        /// <returns>A string containing the response in XML format.</returns>
        [DispId(13)]
		string asXML();

        /// <summary>
        /// Releases all modules and allocated memory.
        /// </summary>
        /// <remarks>
        /// NOTE: This method is no longer required. 
        /// It remains only for backward compatibility, but has no function.
        /// </remarks>
        [DispId(14)]
		void Dispose();

        /// <summary>
        /// Returns the Response object's XML.
        /// </summary>
        /// <returns></returns>
        [DispId(15)]
        string XML { get; }

        /// <summary>
        /// Gets the response code
        /// </summary>
        [DispId(16)]
        string MaskedXML { get; }

    }
}
