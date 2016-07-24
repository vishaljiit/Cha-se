set Trans = CreateObject("Paymentech.Transaction")
rem Set the path to where the SDK was installed if %PAYMENTECH_HOME% is not set.
rem Trans.PaymentechHome = "C:\Paymentech\sdk"
Trans.Type = "NewOrder"

rem Set all Mandatory elements in NewOrder
Trans.Field("BIN") = "000001"
Trans.Field("IndustryType") = "EC"
Trans.Field("MerchantID") = "041756"
Trans.Field("MessageType") = "A"
Trans.Field("AccountNum") = "4444444444444448"
Trans.Field("CardSecValInd") = "1"
Trans.Field("AccountNum") = "123"

Trans.Field("CurrencyCode") = "840"
Trans.Field("CurrencyExponent") = "2"
Trans.setField"CustomerProfileFromOrderInd","A"
Trans.setField"OrderID","ABC def123-,$@"
Trans.setField "Amount","98765"

Wscript.Echo "***************** Request *****************" & vbCrLf
Wscript.Echo Trans.asXML & vbCrLf 

set Resp = Trans.Process

Wscript.Echo "***************** Response *****************" & vbCrLf
Wscript.Echo Resp.asXML & vbCrLf 

