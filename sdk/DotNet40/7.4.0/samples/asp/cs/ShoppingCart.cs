using System;
using System.Text;
using System.Collections;
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
	/// Summary description for ShoppingCart.
	/// </summary>
	public class ShoppingCart
	{
		private ArrayList items = new ArrayList();
		public ShoppingCart()
		{
		}
		public  ArrayList GetItems()
		{
			return items;
		}
		public ArrayList GetCartItems()
		{
			return items;
		}
		public string GetID(HttpRequest request)
		{
				String id;
			if ( request.HttpMethod.Equals("POST"))
			{
				id = null;
				string remove = request["Remove"];
				int size = items.Count;
				if (remove != null) 
				{
					for (int i = size; i > 0; i--)
					{
						string removeBook = request["_ctl" + (i - 1).ToString()];
						if (removeBook != null) 
						{
							items.RemoveAt(i-1);
						}
					}
				}
			}
			else 
			{
				
				id = request["itemID"];
			}
			return id;
		}

		public void AddItem(string id) 
		{
			items.Add(id);
		}

		public int GetSize() 
		{
			int size = 0;
			if (items != null)
				return size = items.Count;
			return size;
		}



		public string GetDispTotalPrice(StoreFront list)
		{
			double price = 0.00;
			for (int i = 0; i < items.Count; i++)
			{
				string item =  (string)items[i];
				price += Convert.ToDouble(list.getPrice(item));
			}
			return String.Format("{0:0.00}", price); 
		}

		public double GetTotalPrice(StoreFront list)
		{
			double price = 0;
			for (int i = 0; i < items.Count; i++)
			{
				string item = (string)items[i];
				price += Convert.ToDouble(list.getPrice(item));
			}
			return price;
		}

	}
}




