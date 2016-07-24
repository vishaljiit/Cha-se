using System;
using System.Collections;
using System.Text;

// Disables warnings for XML doc comments.
#pragma warning disable 1591
#pragma warning disable 1573
#pragma warning disable 1572
#pragma warning disable 1571
#pragma warning disable 1587
#pragma warning disable 1570

namespace Paymentech.Core
{
    /// <summary>
    /// This class provides an easy mechanism for converting old, pre-PTI40 transactions and fields to 
    /// their corresponding PTI40 transactions and fields. 
    /// </summary>
    /// <remarks>
    /// The Orbital SDK provides backward compatibility with merchant applications that have been written
    /// for older Orbital specs. This means that a merchant application that was written a long time ago, 
    /// before Orbital's switch to PTI40, will be using transaction types that no longer exist. To make it
    /// tricker, PTI40 introduced new required fields that the merchant's app will not be coding to.
    /// Luckily, they can be easily defaulted based on the transaction type the merchant used. This 
    /// class does the necessary mapping and setting of Request values on behalf of the merchant.
    /// </remarks>
    public class TransactionMapper
    {
        /// <summary>
        /// Contains a mapping of new transaction types to old transaction types. 
        /// </summary>
        private Hashtable pre40Transactions = new Hashtable();
 
        public TransactionMapper()
        {
            pre40Transactions[RequestType.CC_AUTHORIZE] = RequestType.NEW_ORDER_TRANSACTION;
            pre40Transactions[RequestType.CC_RECURRING_REFUND] = RequestType.NEW_ORDER_TRANSACTION;
            pre40Transactions[RequestType.ECOMMERCE_REFUND] = RequestType.NEW_ORDER_TRANSACTION;
            pre40Transactions[RequestType.CC_RECURRING_AUTH_CAPTURE] = RequestType.NEW_ORDER_TRANSACTION;
            pre40Transactions[RequestType.ECP_AUTH] = RequestType.NEW_ORDER_TRANSACTION;
            pre40Transactions[RequestType.ECP_FORCE_DEPOSIT] = RequestType.NEW_ORDER_TRANSACTION;
            pre40Transactions[RequestType.ECP_REFUND] = RequestType.NEW_ORDER_TRANSACTION;
            pre40Transactions[RequestType.FORCE] = RequestType.NEW_ORDER_TRANSACTION;
            pre40Transactions[RequestType.MOTO] = RequestType.NEW_ORDER_TRANSACTION;
            pre40Transactions[RequestType.MOTO_REFUND] = RequestType.NEW_ORDER_TRANSACTION;
            pre40Transactions[RequestType.PC2_RECURRING_AUTH_CAPTURE] = RequestType.NEW_ORDER_TRANSACTION;
            pre40Transactions[RequestType.PC2_AUTH] = RequestType.NEW_ORDER_TRANSACTION;
            pre40Transactions[RequestType.SWITCH_SOLO_REFUND] = RequestType.NEW_ORDER_TRANSACTION;
            pre40Transactions[RequestType.SWITCH_SOLO_AUTH] = RequestType.NEW_ORDER_TRANSACTION;
            pre40Transactions[RequestType.VOID] = RequestType.REVERSE_TRANSACTION;
            pre40Transactions[RequestType.PARTIAL_VOID] = RequestType.REVERSE_TRANSACTION;
            pre40Transactions[RequestType.FLEXCACHE] = RequestType.FLEX_CACHE_TRANSACTION;
            pre40Transactions[RequestType.FLEXCACHE_BATCH] = RequestType.FLEX_CACHE_TRANSACTION;
            pre40Transactions[RequestType.FLEXCACHE_CAPTURE] = RequestType.FLEX_CACHE_TRANSACTION;
            pre40Transactions[RequestType.BATCH_SETTLEMENT] = RequestType.END_OF_DAY_TRANSACTION;
            pre40Transactions[RequestType.CC_CAPTURE] = RequestType.MARK_FOR_CAPTURE_TRANSACTION;
            pre40Transactions[RequestType.ECP_CAPTURE] = RequestType.MARK_FOR_CAPTURE_TRANSACTION;
            pre40Transactions[RequestType.SWITCH_SOLO_CAPTURE] = RequestType.MARK_FOR_CAPTURE_TRANSACTION;
            pre40Transactions[RequestType.PROFILE_MANAGEMENT] = RequestType.PROFILE_TRANSACTION;
        }

        /// <summary>
        /// Checks if the specified transaction type is pre-PTI40. 
        /// </summary>
        /// <param name="transactionName">The transaction type that the merchant specified.</param>
        /// <returns>True if the transaction type is pre-PTI40, false if it is not.</returns>
        public bool IsPre40(string transactionName)
        {
            return pre40Transactions.ContainsKey(transactionName);
        }

        /// <summary>
        /// This sets default values for some new fields that did not exist in the merchant-specified transaction type.
        /// </summary>
        /// <param name="request">The Request object to be updated.</param>
        /// <param name="pre40TransactionName">The transaction type that the merchant supplied.</param>
        public void MapPre40Values(Request request, string pre40TransactionName)
        {
            MapIndustryType(request, pre40TransactionName);
            MapMessageType(request, pre40TransactionName);

            if (pre40TransactionName == RequestType.PC2_AUTH)
                request["CardSecVal"] = "705";
            if (pre40TransactionName == RequestType.ECP_AUTH || pre40TransactionName == RequestType.ECP_FORCE_DEPOSIT || pre40TransactionName == RequestType.ECP_REFUND)
            {
                request["CardBrand"] = "EC";
                request["BankPmtDelv"] = "B";
                request["BankAccountType"] = "C";
            }
            if (pre40TransactionName == RequestType.SWITCH_SOLO_AUTH || pre40TransactionName == RequestType.SWITCH_SOLO_REFUND)
            {
                request["CardBrand"] = "SW";
                if (pre40TransactionName == RequestType.SWITCH_SOLO_AUTH)
                {
                    request["CurrencyCode"] = "826";
                    request["DebitCardStartDate"] = "1200";
                    request["DebitCardIssueNum"] = "1";
                }
            } 
            else if (request.FinalTransactionType == RequestType.FLEX_CACHE_TRANSACTION && pre40TransactionName == RequestType.FLEXCACHE_CAPTURE)
            {
                request["FlexAction"] = "RedemptionCompletion";
            }
        }

        /// <summary>
        /// Returns the name of the new transaction type that should be used in place of the specified one.
        /// </summary>
        /// <param name="pre40TransactionType">The pre-PTI40 transaction type specified by the merchant.</param>
        /// <returns>The correct PTI40 transaction type to be used.</returns>
        public string GetMappedTransactionType(string pre40TransactionType)
        {
            return (string)pre40Transactions[pre40TransactionType];
        }

        /// <summary>
        /// Determines the default value for the IndustryType field and sets it.
        /// </summary>
        /// <param name="request">The Request object to be updated.</param>
        /// <param name="pre40TransactionName">The transaction type that the merchant supplied.</param>
        private void MapIndustryType(Request request, string pre40TransactionName)
        {
            if (!request.ItemExists("IndustryType") || request.IsFieldSet("IndustryType"))
                return;
            switch (pre40TransactionName)
            {
                case RequestType.CC_RECURRING_AUTH_CAPTURE:
                    request["IndustryType"] = "RC";
                    break;
                case RequestType.CC_RECURRING_REFUND:
                    request["IndustryType"] = "RC";
                    break;
                case RequestType.PC2_RECURRING_AUTH_CAPTURE:
                    request["IndustryType"] = "RC";
                    break;
                case RequestType.MOTO:
                    request["IndustryType"] = "MO";
                    break;
                case RequestType.MOTO_REFUND:
                    request["IndustryType"] = "MO";
                    break;
                default:
                    request["IndustryType"] = "EC";
                    break;
            }
        }

        /// <summary>
        /// Determines the default value for the MessageType field and sets it.
        /// </summary>
        /// <param name="request">The Request object to be updated.</param>
        /// <param name="pre40TransactionName">The transaction type that the merchant supplied.</param>
        private void MapMessageType(Request request, string pre40TransactionName)
        {
            
            if (!request.ItemExists("MessageType") || (request["MessageType"] != null && request["MessageType"].Trim().Length > 0))
                return;
            switch (pre40TransactionName)
            {
                case RequestType.CC_AUTHORIZE:
                    request["MessageType"] = "A";
                    break;
                case RequestType.ECOMMERCE_REFUND:
                    request["MessageType"] = "FR";
                    break;
                case RequestType.ECP_AUTH:
                    request["MessageType"] = "AC";
                    break;
                case RequestType.ECP_CAPTURE:
                    request["MessageType"] = "C";
                    break;
                case RequestType.ECP_FORCE_DEPOSIT:
                    request["MessageType"] = "AC";
                    break;
                case RequestType.ECP_REFUND:
                    request["MessageType"] = "FR";
                    break;
                case RequestType.FORCE:
                    request["MessageType"] = "AC";
                    break;
                case RequestType.PC2_AUTH:
                    request["MessageType"] = "A";
                    break;
                case RequestType.SWITCH_SOLO_AUTH:
                    request["MessageType"] = "AC";
                    break;
                case RequestType.SWITCH_SOLO_REFUND:
                    request["MessageType"] = "FR";
                    break;
                case RequestType.SWITCH_SOLO_CAPTURE:
                    request["MessageType"] = "C";
                    break;
                case RequestType.CC_RECURRING_AUTH_CAPTURE:
                    request["MessageType"] = "AC";
                    break;
                case RequestType.CC_RECURRING_REFUND:
                    request["MessageType"] = "FR";
                    break;
                case RequestType.PC2_RECURRING_AUTH_CAPTURE:
                    request["MessageType"] = "AC";
                    break;
                case RequestType.MOTO:
                    request["MessageType"] = "A";
                    break;
                case RequestType.MOTO_REFUND:
                    request["MessageType"] = "FR";
                    break;
                default:
                    request["MessageType"] = "AC";
                    break;
            }
        }
    }
}
