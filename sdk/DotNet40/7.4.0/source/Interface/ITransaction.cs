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
	[Guid("C9A5CC5A-83F0-419e-BA24-5446C061B68B")]
	[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface ITransaction 
	{
        /// <summary>
        /// Gets the transaction type of the transaction.
        /// </summary>
        /// <returns>The name of the transaction type used for this transaction.</returns>
        [DispId(1)]
		string Type { get; set; }

        /// <summary>
        /// Gets or sets the value of the specified field.
        /// </summary>
        /// <remarks>
        /// Throws an exception if itemName does not refer to an element 
        /// in the template.
        /// </remarks>
        /// <param name="index">The name of the field being referenced.</param>
        /// <returns>The value of the field.</returns>
        [DispId(2)]
		[IndexerName("Field")]
		string this[string index] { get; set; }

        /// <summary>
        /// Sets a field in the transaction's request.
        /// </summary>
        /// <remarks>
        /// Throws an exception if itemName does not refer to an element 
        /// in the template.
        /// </remarks>
        /// <param name="itemName">The name of the field to be set.</param>
        /// <param name="itemValue">The value to set the field to.</param>
        [DispId(3)]
		void setField(string itemName, string itemValue);

		/// <summary>
		/// Processes a transaction
		/// </summary>
		/// <remarks>
		/// Process () submits a transaction to the Orbital Gateway and 
		/// retrieves a <see cref="Paymentech.Response"/> object.
		/// </remarks>
		/// <returns>The transaction response</returns>
		[DispId(4)]
		Response Process ();

        /// <summary>
        /// Gets a complex format for adding additional data to the request.
        /// </summary>
        /// <param name="name">The name of the complex root's template.</param>
        /// <returns>The transaction element for entering the format's fields.</returns>
        [DispId(5)]
		TransactionElement GetComplexRoot(string name);

        /// <summary>
        /// Releases all modules and allocated memory.
        /// </summary>
        /// <remarks>
        /// NOTE: This method is no longer required. 
        /// It remains only for backward compatibility, but has no function.
        /// </remarks>
        /// <returns>A string containing the request in XML format.</returns>
        [DispId(6)]
		string asXML();

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
		[DispId(7)]
		void Dispose ();

        /// <summary>
        /// Gets the contents of the request in XML format. This is the data that is sent to the Gateway.
        /// </summary>
        /// <remarks>
        /// <para>
        /// XML returns the XML of the request as it is to be sent to the Gateway. No formatting is 
        /// included, so printing it out will not have newlines. All values that you set in your code
        /// will be included in the XML, as will any default values.
        /// </para>
        /// <para>
        /// This property is read-only.
        /// </para>
        /// </remarks>
        /// <exception cref="System.Exception">
        /// An exception will be thrown if any required fields have not yet been set. You must be sure
        /// to set all necessary fields before calling using this property.
        /// </exception>
        /// <returns>A string containing the request in XML format.</returns>
        [DispId(8)]
        string XML { get; }

        /// <summary>
        /// Returns true if the field exists.
        /// </summary>
        /// <param name="itemName">The name of the field to be tested.</param>
        /// <returns>true if the field exists, false if it does not.</returns>
        [DispId(9)]
        bool ItemExists(string itemName);

        /// <summary>
        /// Sets the path to the SDK.
        /// </summary>
        /// <remarks>
        /// This only needs to be set if the %PAYMENTECH_HOME% environment 
        /// variable is not set, or if you want to override it.
        /// </remarks>
        [DispId(10)]
        string PaymentechHome { get; set; }
	}
}
