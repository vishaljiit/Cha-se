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
	/// Summary description for PC3Sample.
	/// </summary>
	public class PC3Sample : IOrbitalSample
	{
		// Return the test case name
		public string Name
		{
			get
			{
				return "PC3 Sample";
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
				transaction["MerchantID"] = "041756";
				transaction["BIN"] = "000001";
				transaction["OrderID"] = "122003SA";
				transaction["AccountNum"] = "5191409037560100";
				transaction["Amount"] = "25000";
				transaction["Exp"] = "1209";
				transaction["AVSname"] = "Jon Smith";
				transaction["AVSaddress1"] = "4200 W Cypress St";
				transaction["AVScity"] = "Tampa";
				transaction["AVSstate"] = "FL";
				transaction["AVSzip"] = "33607";
				transaction["Comments"] = "This is a PC3 Sample";
				transaction["ShippingRef"] = "FEDEX WB12345678 Pri 1";
				transaction["TaxInd"] = "1";
				transaction["Tax"] = "100";
				transaction["PCOrderNum"] = "PO8347465";
				transaction["PCDestZip"] = "33607";
				transaction["PCDestName"] = "Terry";
				transaction["PCDestAddress1"] = "88301 Teak Street";
				transaction["PCDestAddress2"] = "Apt 5";
				transaction["PCDestCity"] = "Hudson";
				transaction["PCDestState"] = "FL";

				// Create the PC3Core transaction element using transaction object
				TransactionElement pc3core = transaction.GetComplexRoot(Paymentech.RequestType.PC3_CORE);

				// Populate the required fields for the given transaction type. You can use’
				// the Paymentech Transaction Appendix to help you populate the transaction’
				pc3core["PC3FreightAmt"] = "10";
				pc3core["PC3DutyAmt"] = "11";
				pc3core["PC3DestCountryCd"] = "USA";
				pc3core["PC3ShipFromZip"] = "34667";
				pc3core["PC3DiscAmt"] = "5";
				pc3core["PC3VATtaxAmt"] = "6";
				pc3core["PC3VATtaxRate"] = "2";
				pc3core["PC3AltTaxInd"] = "1";
				pc3core["PC3AltTaxAmt"] = "5";
				pc3core["PC3LineItemCount"] = "5";

				// Create the PC3LineItems transaction element using pc3core object
				TransactionElement pc3lineitem = pc3core.GetRecursiveElement(Paymentech.RequestType.PC3_LINE_ITEMS);

				// Populate the required fields for the given transaction type. You can use’
				// the Paymentech Transaction Appendix to help you populate the transaction’
				pc3lineitem["PC3DtlDesc"] = "1234567890123456789";
				pc3lineitem["PC3DtlProdCd"] = "123456789";
				pc3lineitem["PC3DtlQty"] = "1";
				pc3lineitem["PC3DtlUOM"] = "LBR";
				pc3lineitem["PC3DtlTaxAmt"] = "0";
				pc3lineitem["PC3DtlTaxRate"] = "0";
				pc3lineitem["PC3Dtllinetot"] = "50";
				pc3lineitem["PC3DtlDisc"] = "0";
				pc3lineitem["PC3DtlCommCd"] = "3";
				pc3lineitem["PC3DtlUnitCost"] = "5";
				pc3lineitem["PC3DtlGrossNet"] = "Y";
				pc3lineitem["PC3DtlTaxType"] = "Y";
				pc3lineitem["PC3DtlDiscInd"] = "Y";
				pc3lineitem["PC3DtlDebitInd"] = "D";

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
