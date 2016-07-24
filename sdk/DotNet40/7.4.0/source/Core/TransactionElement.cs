using System;
using Paymentech.COM;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Paymentech.Core;

namespace Paymentech
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
	[Guid("225C3F6F-3A43-4a43-9426-E28C3771CF2D")]
	[ClassInterface(ClassInterfaceType.None)]
	[ProgId("Paymentech.TransactionElement")]
    public class TransactionElement : Transaction, ITransactionElement
	{
		/// <summary>
		/// The default class constructor. Required by Visual Basic programs.
		/// </summary>
		public TransactionElement()
		{
		}

		internal TransactionElement(Request request)
		{
			this.request = request;
		}

		/// <summary>
		/// Create a request object type based on the input parameter
		/// and return the object back to the calling program to set the field values(s).
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="name">The template name.</param>
		/// <returns>A new TransactionElement based on the input template name.</returns>
        public TransactionElement GetRecursiveElement(string name)
		{
            try
            {
                Logger.DebugFormat("Getting Recursive Element [{0}].", name);
                return new TransactionElement(request.GetRecursiveElement(name));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw new Exception(ex.Message, ex);
            }
        }
	}

}
