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
	/// Summary description for VerifiedByVisaSample.
	/// </summary>
	public class VerifiedByVisaSample : IOrbitalSample
	{
		// Return the test case name
		public string Name
		{
			get
			{
				return "Verified By Visa Sample";
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

				// Populate the required fields for the given transaction type. You can use�
				// the Paymentech Transaction Appendix to help you populate the transaction�
				transaction["IndustryType"] = "EC";
				transaction["MessageType"] = "A";
				transaction["MerchantID"] = "041756";
				transaction["BIN"] = "000001";
				transaction["AccountNum"] = "4055011111111111";
				transaction["OrderID"] = "122003SAVBV";
				transaction["Amount"] = "100";
				transaction["Exp"] = "1214";
				transaction["AVSname"] = "Jon Smith";
				transaction["AVSaddress1"] = "4200 W Cypress St";
				transaction["AVScity"] = "Tampa";
				transaction["AVSstate"] = "FL";
				transaction["AVSzip"] = "33607";
				transaction["Comments"] = "Test VBV Request";
				transaction["ShippingRef"] = "FEDEX WB12345678 Pri 1";
				transaction["XID"] = "123456789312345678801234567";
				transaction["CAVV"] = "123456789312345678801234567";

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
