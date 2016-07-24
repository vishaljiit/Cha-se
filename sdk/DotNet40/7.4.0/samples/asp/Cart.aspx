<%@ Import Namespace="ChasePaymentech.Orbital.ShoppingCart.cs" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System" %>
<%@ Page language="c#" %>
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
		Order.Rows.Add(tRow);
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
	TableRow tRow1 = new TableRow();
	Order.Rows.Add(tRow1);
	TableRow tRow2 = new TableRow();
	Order.Rows.Add(tRow2);
	//TableCell tCell31 = new TableCell();
	//tRow2.Cells.Add(tCell31);           
	TableCell tCell32 = new TableCell();
	tRow2.Cells.Add(tCell32);           
	TableCell tCell3 = new TableCell();
	tCell3.Controls.Add(new LiteralControl("Total =>" + cart.GetDispTotalPrice(store)));
	tCell3.HorizontalAlign = HorizontalAlign.Right;  
	tRow2.Cells.Add(tCell3);         
}
	</script>
	<BODY>
		<FORM method="post">
			<H3>Shopping Cart Contents</H3>
			<input id="hidden1" type="hidden" name="pageCount" runat="server"> 
			<!-- <INPUT type="submit" value="Remove checked books" name="Remove"> -->
			<!-- <asp:TableCell HorizontalAlign="left" BackColor="#807580" Font-Bold="True"></asp:TableCell> -->
			<asp:table id="Order" CELLPADDING="1" CELLSPACING="0" WIDTH="50%" Runat="server">
				<asp:TableRow>
					<asp:TableCell HorizontalAlign="left" BackColor="#807580" Font-Bold="True">Title</asp:TableCell>
					<asp:TableCell BackColor="#807580" HorizontalAlign="Right" Font-Bold="True">Listed Price</asp:TableCell>
				</asp:TableRow>
			</asp:table>
			<TABLE cellPadding="30">
				<TR>
					<TD align="center"><BR>
						<A href="StoreFront.aspx"><IMG alt="go back" src="img/back.gif" border="0"></A>
					</TD>
					<TD align="center"><BR>
						<A href="Purchase.aspx"><IMG alt="proceed" src="img/checkout.gif" border="0"></A>
					</TD>
				</TR>
			</TABLE>
		</FORM>
	</BODY>
</HTML>
