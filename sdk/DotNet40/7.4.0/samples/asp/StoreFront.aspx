<%@ Page language="c#"%>
<%@ Import Namespace="System" %>
<%@ Import Namespace="ChasePaymentech.Orbital.ShoppingCart.cs" %>
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
	StoreFront store = new StoreFront();
	string[] bookTitles = store.GetBookTitles();
	string[] bookPrices = store.GetBookProces();
	for ( int ctr=0 ; ctr < bookTitles.Length ; ctr++ )
	{
		TableRow tRow = new TableRow();
		StoreTable.Rows.Add(tRow);
		TableCell tCell1 = new TableCell();
		tRow.Cells.Add(tCell1);
		System.Web.UI.WebControls.HyperLink h = new HyperLink();
		h.Text = bookTitles[ctr] ;
		h.NavigateUrl = "Cart.aspx?itemID=" + ctr ;
		tCell1.Controls.Add(h);
		TableCell tCell = new TableCell();
		tCell.Controls.Add(new LiteralControl(bookPrices[ctr]));
		tCell.HorizontalAlign = HorizontalAlign.Right;
		tRow.Cells.Add(tCell);               
	}
}
	</script>
	<BODY>
		<FORM action="Cart.aspx" method="post">
			<H2 align="center">Select a book to place in your shopping cart</H2>
			<BR>
			<H3><INPUT type="image" src="img/cart.gif" value="View Cart" name="cartView"> Items 
				in Your Cart
				<H3>
					<H3>Available Books</H3>
					<asp:table id="StoreTable" runat="server" CELLPADDING="1" CELLSPACING="0" WIDTH="50%">
						<asp:TableRow>
							<asp:TableCell HorizontalAlign="left" BackColor="#807580" Font-Bold="True">Title</asp:TableCell>
							<asp:TableCell BackColor="#807580" HorizontalAlign="Right" Font-Bold="True">Listed Price</asp:TableCell>
						</asp:TableRow>
					</asp:table>
		</FORM>
		</H3></H3>
	</BODY>
</HTML>
