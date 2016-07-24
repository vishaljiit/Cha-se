using System;
using System.Net;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
/// Please note that this code is for documentation purposes only and is 
/// specifically written in a simplistic way, in order to better represent and 
/// convey specific concepts in the use of the Orbital SDK.  
/// Best practices in error handling and control flow for DotNet coding, 
/// have often been ignored in favor of providing code that clearly represents 
/// a specific concept.  
/// Due to the best practices compromises we have made in our sample code, 
/// we do not recommend using this code in a production environment without 
/// rigorous improvements to error handling and overall architecture.
/// /// <p>Copyright (c) 2008, Chase Paymentech Solutions, LLC. All rights
/// reserved</p>
///
/// <p>Company: Chase Paymentech Solutions</p>
///
/// @author Rameshkumar Bhaskharan
/// @version 1.0
///
namespace ChasePaymentech.Orbital.ShoppingCart.cs
{
	/// <summary>
	/// Summary description for Simple Auth.
	/// </summary>
	public class SimpleAuthInternet
	{
		public SimpleAuthInternet()
		{
		}
		public string ProcessTransaction(HttpRequest webRequest)
		{
			// Create the transaction related variables
			Paymentech.Response response;
			Paymentech.Transaction transaction;
			string returnValue=null;
			try
			{
				//Create a new online transaction request object
				// Set the required name value pairs for online auth transaction
				transaction = new Paymentech.Transaction(Paymentech.RequestType.NEW_ORDER_TRANSACTION);
				transaction["OrbitalConnectionUsername"] = webRequest["UserName"];
				transaction["OrbitalConnectionPassword"] = webRequest["Password"];
				transaction["IndustryType"] = "EC";
				transaction["MessageType"] = "A";
				transaction["MerchantID"] = webRequest["MerchantID"];
				transaction["BIN"] = webRequest["BIN"];
				transaction["AccountNum"] = webRequest["ccNumber"];
				transaction["OrderID"] = webRequest["OrderID"];
				transaction["Amount"] = webRequest["CartTotal"];
				transaction["Exp"] = webRequest["ccDate"];
				transaction["AVSname"] = webRequest["ccName"];
				transaction["AVSaddress1"] = webRequest["address"];
				transaction["AVScity"] = webRequest["city"];
				transaction["AVSstate"] = webRequest["state"];
				transaction["AVSzip"] = webRequest["zip"];
				transaction["ShippingRef"] = "FEDEX WB12345678 Pri 1";
				transaction["Comments"] = "This is a test auth w/ avs information";
				// send the transaction for processing
				response = transaction.Process();
				// Check for the status of teh transaction
				String status = response["ProcStatus"];
				if ( status == null )
					return  "Exception while processing the request";
				if (Convert.ToInt32(status) > 0 )
				{
					returnValue="Error processing the request - Check the data try again";
					return returnValue;
				}
					returnValue =  "Your Transaction Reference Number is  [" + response.TxRefNum + "]";
				return returnValue;
			}
			catch (Exception e)
			{
				returnValue = "Exception while processing the request";
				return returnValue + e.ToString();
			}
		}
	}
}
