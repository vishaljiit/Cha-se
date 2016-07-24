using System;
using System.Text;
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
	/// Summary description for StoreFront.
	/// </summary>
	public class StoreFront
	{
		public static string STRINGID = "Store";
		public static string shipingCost="10.00";
		string[] bookTitles = {"Orbital SDK - Java for Dummies","Orbital SDK - C++ for Dummies","Spectrum SDK - Java for Dummies","Spectrum SDK - C++ for Dummies","Spectrum SDK - Perl for Dummies","Spectrum SDK - COM for Dummies","Spectrum SDK - .Net for Dummies"};
		string[] bookPrices = {"5.80","4.80","7.99","12.95","9.99","15.50","5.95"}  ;
		public StoreFront()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public string[] GetBookTitles()
		{
			return bookTitles;
		}
		public static string ShippingCost()
		{
			return String.Format("{0:0.00}", shipingCost);         
		}
		public string[] GetBookProces()
		{
			return bookPrices;
		}

		public string getTitle(string id) 
		{
			int i=0;
			try 
			{
				i = Convert.ToInt32(id);
			}
			catch (Exception) 
			{
				return null;
			}
			return bookTitles[i];
		}
		public string getPrice(string id) 
		{
			int i=0;
			try 
			{
				i = Convert.ToInt32(id);
			}
			catch (Exception) { }
			return bookPrices[i];
		}
    	private double string2Double(string s) 
		{
			double d;
			try 
			{
				d = Convert.ToDouble(s);
			}
			catch (Exception) 
			{
				d = 0.0;
			}
			return d;
		}
	}
}







