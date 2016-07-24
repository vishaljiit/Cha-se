<%@ Import Namespace="System" %>
<%@ Import Namespace="ChasePaymentech.Orbital.ShoppingCart.cs" %>
<%@ Page language="c#"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<!-- Please note that this code is for documentation purposes only and is  -->
	<!-- specifically written in a simplistic way, in order to better represent and -->
	<!-- convey specific concepts in the use of the Orbital SDK.  -->
	<!-- Best practices in error handling and control flow for ASPDotNet coding, -->
	<!-- have often been ignored in favor of providing code that clearly represents -->
	<!-- a specific concept.  -->
	<!-- Due to the best practices compromises we have made in our sample code, -->
	<!-- we do not recommend using this code in a production environment without -->
	<!-- rigorous improvements to error handling and overall architecture.-->
	<!-- <p>Copyright (c) 2008, Chase Paymentech Solutions, LLC. All rights -->
	<!-- reserved</p> -->
	<!-- <p>Company: Chase Paymentech Solutions</p> -->
	<!-- @author Rameshkumar Bhaskharan -->
	<!-- @version 1.0 -->
	<script runat="server"> 
protected void Page_Load(Object Source, EventArgs E) 
{
	
    StoreFront store = null;
    ShoppingCart cart = null;
    if (Session["store"] != null )
		store = (StoreFront) Session["store"];
	if ( Session["cart"] != null )
		cart =  ( ShoppingCart) Session["cart"];
	 if (Session["store"] == null ) 	
	 {
		store  = new StoreFront();
		cart =  new ShoppingCart();
		Session["store"] = store;
		Session["cart"] = cart;
	}
	string itemID = cart.GetID(Request);
    if (itemID != null)
	cart.AddItem(itemID);
    ArrayList items = cart.GetCartItems();
    for ( int i = 0 ; i < items.Count ; i++)
    {
	    string item = (string)items[i];
	    double price = Convert.ToDouble(store.getPrice(item));
		TableRow tRow = new TableRow();
		Purchase.Rows.Add(tRow);
		//TableCell tCell2 = new TableCell();
		//HtmlInputCheckBox check = new HtmlInputCheckBox();
		//tCell2.Controls.Add(check);
		//tRow.Cells.Add(tCell2);    
		TableCell tCell = new TableCell();
		tCell.Controls.Add(new LiteralControl(store.getTitle(item)));
		tCell.HorizontalAlign = HorizontalAlign.Left;
		tRow.Cells.Add(tCell);             
		TableCell tCell1 = new TableCell();
		tCell1.Controls.Add(new LiteralControl(price.ToString()));
		tCell1.HorizontalAlign = HorizontalAlign.Right;
		tRow.Cells.Add(tCell1); 
    }
 	double shipCost = Convert.ToDouble(StoreFront.ShippingCost());
	double bookCost = Convert.ToDouble(cart.GetDispTotalPrice(store));
	double totalCost = shipCost + bookCost;
	Books.Text = String.Format("{0:0.00}", bookCost); 
	if ( bookCost > 0 )
	{
		Shipping.Text=String.Format("{0:0.00}", shipCost); 
		Total.Text = String.Format("{0:0.00}", totalCost); 
		CartTotal.Value= (totalCost * 100 ).ToString();
	}
	else
	{
		Shipping.Text="0.00";	
		Total.Text="0.00";
		CartTotal.Value="0";
	}
}
	</script>
	<BODY>
		<FORM name="purchase" action="ThankYou.aspx" method="post">
			<H3>Purchase Items</H3>
			<!-- <INPUT type="submit" value="Remove checked books" name="Remove"> -->
								<!-- <asp:TableCell HorizontalAlign="left" BackColor="#807580" Font-Bold="True"></asp:TableCell> -->
			<asp:table id="Purchase" CELLPADDING="1" CELLSPACING="0" WIDTH="50%" Runat="server">
				<asp:TableRow>
					<asp:TableCell HorizontalAlign="left" BackColor="#807580" Font-Bold="True">Title</asp:TableCell>
					<asp:TableCell BackColor="#807580" HorizontalAlign="Right" Font-Bold="True">Listed Price</asp:TableCell>
				</asp:TableRow>
			</asp:table><asp:table id="Table1" CELLPADDING="1" CELLSPACING="0" WIDTH="50%" Runat="server">
				<asp:TableRow>
					<asp:TableCell HorizontalAlign="RIGHT" />
					<asp:TableCell HorizontalAlign="RIGHT">Books: </asp:TableCell>
					<asp:TableCell id="Books" Runat="server" HorizontalAlign="RIGHT" />
				</asp:TableRow>
				<asp:TableRow>
					<asp:TableCell HorizontalAlign="RIGHT" />
					<asp:TableCell HorizontalAlign="RIGHT">Shipping: </asp:TableCell>
					<asp:TableCell ID="Shipping" Runat="server" HorizontalAlign="RIGHT" />
				</asp:TableRow>
				<asp:TableRow>
					<asp:TableCell HorizontalAlign="RIGHT" />
					<asp:TableCell HorizontalAlign="RIGHT">Total: </asp:TableCell>
					<asp:TableCell ID="Total" Runat="server" HorizontalAlign="RIGHT" />
				</asp:TableRow>
			</asp:table><BR>
			
			<BR>
			<HR>
			<BR>
			<TABLE height="188" cellSpacing="0" cellPadding="1" width="480">
				<H5>Payment information</H5>
				<input runat="server" id="CartTotal" type="hidden" NAME="CartTotal"/>
				<TR>
					<TD align="left">Credit Card number:<BR>
						<INPUT type="text" size="16" name="ccNumber"></TD>
					<TD align="left">Expiration date:<BR>
						<INPUT type="text" size="16" name="ccDate"></TD>
					<TD align="left">Name on card:<BR>
						<INPUT type="text" size="15" name="ccName"></TD>
				</TR>
				<TR>
					<TD align="left">Street Address:<BR>
						<INPUT type="text" size="16" name="address" value="1 Northeastern Blvd"></TD>
					<TD align="left">City :<BR>
						<INPUT type="text" size="16" name="city" value="Salem"></TD>
					<TD align="left">State :<BR>
						<INPUT type="text" size="8" name="state" value="NH"></TD>
					<TD align="left">Zip:<BR>
						<INPUT type="text" size="15" name="zip" value="03079"></TD>
				</TR>
				<TR>
					<TD align="left">Contact Number:<BR>
						<INPUT type="text" size="16" name="contactnumber" value="8903217890"></TD>
				</TR>
			</TABLE>
			<TABLE cellSpacing="0" cellPadding="1" width="60%">
				<H5>Merchant information</H5>
				<TR>
					<TD align="left">Merchant ID:<BR>
						<INPUT type="text" size="16" name="MerchantID"></TD>
					<TD align="left">BIN :<BR>
						<INPUT type="text" size="16" name="BIN"></TD>
					<TD align="left">Order ID :<BR>
						<INPUT type="text" size="8" name="OrderID" value="OrbitalSDK"></TD>
					<TD align="left">Comments :<BR>
						<INPUT type="text" size="15" name="Comments" value="Auth transaction"></TD>
				</TR>
				<TR>
					<TD align="left">Orbital User Name :<BR>
						<INPUT type="text" size="16" name="UserName"></TD>
					<TD align="left">Orbital Password :<BR>
						<INPUT type="text" size="16" name="Password"></TD>
				</TR>
			</TABLE>
			<HR>
			<TABLE cellPadding="30">
				<TR>
					<TD align="center"><BR>
						<A href="StoreFront.aspx"><IMG alt="go back" src="img/back.gif" border="0"></A>
					</TD>
					<TD align="center"><input type="image" src="img/process.gif">
					</TD>
				</TR>
			</TABLE>
		</FORM>
	</BODY>
</HTML>
