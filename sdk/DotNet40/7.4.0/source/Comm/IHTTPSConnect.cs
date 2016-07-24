using System;

// Disables warnings for XML doc comments.
#pragma warning disable 1591
#pragma warning disable 1573
#pragma warning disable 1572
#pragma warning disable 1571
#pragma warning disable 1587
#pragma warning disable 1570

namespace Paymentech.Comm
{
	/// <summary>
	/// Summary description for HTTPSConnect.
	/// </summary>
	public interface IHTTPSConnect
	{
		CommArgs CompleteTransaction( CommArgs args ) ;
	}
}
