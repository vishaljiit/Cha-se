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
	/// Summary description for BMLSample.
	/// </summary>
	public class BMLSample : IOrbitalSample
	{
		// Return the test case name
		public string Name
		{
			get
			{
				return "BML Sample";
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
				transaction["IndustryType"] = "MO";
				transaction["MessageType"] = "A";
				transaction["BIN"] = "000001";
				transaction["MerchantID"] = "639013";
				transaction["TerminalID"] = "001";
				transaction["AccountNum"] = "5049900000000000";
				transaction["CurrencyCode"] = "840";
				transaction["CurrencyExponent"] = "2";
				transaction["AVSzip"] = "21093";
                transaction["AVSaddress1"] = "9690 DEERECO RD";
				transaction["AVSaddress2"] = "SUITE 705";
				transaction["AVScity"] = "TIMONIUM";
				transaction["AVSstate"] = "MD";
				transaction["AVSphoneNum"] = "5559211900";
				transaction["AVSname"] = "ORBITALC MERCHANTC";
				transaction["AVScountryCode"] = "US";
				transaction["AVSDestzip"] = "894139700";
				transaction["AVSDestaddress1"] = "1245 TAMARACK DR UNIT 15";
				transaction["AVSDestcity"] = "LINCOLN PARK";
                transaction["AVSDeststate"] = "NV";
				transaction["AVSDestphoneNum"] = "5559211900";
				transaction["AVSDestname"] = "JUSTIN MERCHANTC";
				transaction["AVSDestcountryCode"] = "US";
				transaction["OrderID"] = "1234567890123456789012";
				transaction["Amount"] = "100";
				transaction["Comments"] = "BML Auth Test";
				transaction["BMLShippingCost"] = "6";
				transaction["BMLTNCVersion"] = "12103";
				transaction["BMLCustomerRegistrationDate"] = "20060922";
				transaction["BMLCustomerTypeFlag"] = "N";
				transaction["BMLItemCategory"] = "5500";
				transaction["BMLCustomerBirthDate"] = "19601117";
				transaction["BMLCustomerSSN"] = "4016";
				transaction["BMLCustomerAnnualIncome"] = "25000";
				transaction["BMLCustomerResidenceStatus"] = "R";
				transaction["BMLCustomerCheckingAccount"] = "Y";
				transaction["BMLCustomerSavingsAccount"] = "Y";
				transaction["BMLProductDeliveryType"] = "PHY";

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
