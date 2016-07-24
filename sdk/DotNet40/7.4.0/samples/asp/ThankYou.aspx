<%@ Import Namespace="ChasePaymentech.Orbital.ShoppingCart.cs" %>
<%@ Import Namespace="System" %>
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
		SimpleAuthInternet sdk = new SimpleAuthInternet();
		string  resp = sdk.ProcessTransaction(Request);
		Result.Text=resp;
}
	</script>
	<BODY>
		<H3 ALIGN="center">Thank you for shopping at Salem Book Depot!!!</H3>
		<TR>
			<td align="center">
				<asp:Label ID="Result" Runat="server" />
				<H4 ALIGN="center"></H4>
			</td>
		</TR>
		<BR>
		<BR>
		<FORM>
			<TABLE WIDTH="100%">
				<TR>
					<TD ALIGN="center">
						<IMG SRC="img/books.jpg">
					</TD>
				</TR>
				<TR align="center">
					<td>
						<A HREF="Purchase.aspx"><IMG SRC="img/back.gif" ALT="go back" BORDER="0"></A>
					</td>
				</TR>
			</TABLE>
		</FORM>
	</BODY>
</HTML>
