using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Paymentech.COM
{
	/// <summary>
	/// A Paymentech Orbital TransasctionElement Request
	/// </summary>
	/// <remarks> 
	/// <para>
	/// The TransactionElement class allows you to create a request object 
	/// type based on the input parameter and return the object back to 
	/// the calling program to set the field values(s).  
	/// </para>
	/// </remarks>
	[Guid("4A338D80-0B7D-42d4-A0D4-0108C65AA36F")]
	[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface ITransactionElement 
	{
		/// <summary>
		/// Gets the transaction type of the transaction
		/// </summary>
		[DispId(1)]
		string Type { get; set; }

        /// <summary>
        /// Gets or sets the value of the specified field.
        /// </summary>
        /// <remarks>
        /// Throws an exception if itemName does not refer to an element 
        /// in the template.
        /// </remarks>
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
        /// Gets a complex format for adding additional data to the request.
        /// </summary>
        /// <param name="name">The name of the complex root's template.</param>
        /// <returns>The transaction element for entering the format's fields.</returns>
        [DispId(5)]
		TransactionElement GetComplexRoot(string name);

        /// <summary>
        /// Gets the contents of the complex root in XML format. 
        /// </summary>
        /// <remarks>
        /// asXML returns the XML of the complex root as it is to be sent to the Gateway. No formatting is 
        /// included, so printing it out will not have newlines. All values that you set in your code
        /// will be included in the XML, as will any default values.
        /// </remarks>
        /// <exception cref="System.Exception">
        /// An exception will be thrown if any required fields have not yet been set. You must be sure
        /// to set all necessary fields before calling this method.
        /// </exception>
        /// <returns>A string containing the request in XML format.</returns>
        [DispId(6)]
		string asXML();

        /// <summary>
        /// Releases all modules and allocated memory.
        /// </summary>
        /// <remarks>
        /// NOTE: This method is no longer required. 
        /// It remains only for backward compatibility, but has no function.
        /// </remarks>
        [DispId(7)]
		void Dispose ();
		
		/// <summary>
		/// Create a request object type based on the input parameter
		/// and return the object back to the calling program to set the field values(s).
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="name">The template name.</param>
		/// <returns>A new TransactionElement based on the input template name.</returns>
		[DispId(8)]
		TransactionElement GetRecursiveElement(string name);
	}

}
