using System;
using Paymentech;

/// Please note that this code is for documentation purposes only and is 
/// specifically written in a simplistic way, in order to better represent and 
/// convey specific concepts in the use of the Orbital SDK.  
/// Best practices in error handling and control flow for DotNet coding, 
/// have often been ignored in favor of providing code that clearly represents 
/// a specific concept.  
/// Due to the best practices compromises we have made in our sample code, 
/// we do not recommend using this code in a production environment without 
/// rigorous improvements to error handling and overall architecture.
/// <p>Copyright (c) 2008, Chase Paymentech Solutions, LLC. All rights
/// reserved</p>
///
/// <p>Company: Chase Paymentech Solutions</p>
///
/// @author Rameshkumar Bhaskharan
/// @version 1.0
///
namespace ChasePaymentech.Orbital.Test.Samples
{
	/// <summary>
    /// Summary description for InternationalMaestroSample.
	/// </summary>
	public class InternationalMaestroSample : IOrbitalSample
	{
		// Return the test case name
		public string Name
		{
			get
			{
				return "International Maestro Sample";
			}
		}
		// Run the test case
		public void RunTest(int testCaseNumber)
		{
			try
			{
				// Declare a response
				Paymentech.Response response;
				// Create an authorize transaction
				Transaction transaction =  new Transaction(RequestType.NEW_ORDER_TRANSACTION);

				// Populate the required fields for the given transaction type. You can use’
				// the Paymentech Transaction Appendix to help you populate the transaction’
				transaction["IndustryType"] = "EC";
				transaction["MessageType"] = "A";
				transaction["MerchantID"] = "193979";
				transaction["BIN"] = "000001";
				transaction["OrderID"] = "1234567890123456789012";
				transaction["AccountNum"] = "6759184500000131104";
				transaction["Amount"] = "100";
				transaction["Exp"] = "1214";
                transaction["Comments"] = "International Maestro Authorize Request";
				transaction["ShippingRef"] = "FEDEX WB12345678 Pri 1";
				transaction["DebitCardIssueNum"] = "1";
				transaction["CurrencyCode"] = "826";
				transaction["CardBrand"] = "IM";

				Console.WriteLine("***************************Request XML***************************");
				Console.WriteLine(transaction.XML);
				response = transaction.Process();
				Console.WriteLine("***************************Response XML***************************");
				Console.WriteLine(response.XML);
				Console.WriteLine("ProcStatus ==>" + response["ProcStatus"]);
			}
			catch(Exception e)
			{
				Console.WriteLine("***************************Exception***************************");
				Console.WriteLine(e.ToString());
			}
		}
	}
}
