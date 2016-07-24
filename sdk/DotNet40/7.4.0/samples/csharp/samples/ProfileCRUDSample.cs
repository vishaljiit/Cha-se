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
	/// Summary description for ProfileCRUDSample.
	/// </summary>
	public class ProfileCRUDSample : IOrbitalSample
	{
		// Return the test case name
		public string Name
		{
			get
			{
				return "Profile CRUD Sample";
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
				Transaction transaction =  new Transaction(RequestType.PROFILE_TRANSACTION);

				// Populate the required fields for the given transaction type. You can use’
				// the Paymentech Transaction Appendix to help you populate the transaction’
				transaction["CustomerProfileAction"] = "C";
				transaction["CustomerProfileFromOrderInd"] = "A";
				transaction["CustomerProfileOrderOverrideInd"] = "NO";
				transaction["CustomerBin"] = "000001";
                transaction["CustomerMerchantID"] = "041756";
				transaction["CustomerName"] = "December Co";
				transaction["CustomerAddress1"] = "123 My Wa";
				transaction["CustomerCity"] = "Tampa";
				transaction["CustomerState"] = "FL";
				transaction["CustomerZIP"] = "33607";
				transaction["CustomerEmail"] = "someaddress@tampabay.rr.com";
				transaction["CustomerPhone"] = "5554561230";
				transaction["CustomerAccountType"] = "CC";
				transaction["OrderDefaultDescription"] = "Some Stuff";
				transaction["OrderDefaultAmount"] = "150";
				transaction["CCAccountNum"] = "5454545454545454";
				transaction["CCExpireDate"] = "1214";
				transaction["CustomerRefNum"] = "8994";

				Console.WriteLine("***************************Request XML***************************");
				Console.WriteLine(transaction.XML);
				response = transaction.Process();

				Console.WriteLine("***************************Response XML***************************");
				Console.WriteLine(response.XML);
				Console.WriteLine("ProcStatus ==>" + response["ProfileProcStatus"]);
			}
			catch(Exception e)
			{
				Console.WriteLine("***************************Exception***************************");
				Console.WriteLine(e.ToString());
			}
		}
	}
}
