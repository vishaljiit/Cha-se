using System;

namespace Paymentech
{
	/// <summary>
	/// The RequestType class defines constant strings that can be used to 
	/// create transactions.
	/// </summary>
	public abstract class RequestType
	{
		private RequestType()
		{
		}

		/// <summary>
		/// Used to create a New Order transaction.
		/// </summary>
		public const string NEW_ORDER_TRANSACTION = "NewOrder";
		/// <summary>
		/// Used to create a End of Day transaction.
		/// </summary>
		public const string END_OF_DAY_TRANSACTION = "EOD";
		/// <summary>
		/// Used to create a Flex Cache transaction.
		/// </summary>
		public const string FLEX_CACHE_TRANSACTION = "FlexCache";
		/// <summary>
		/// Used to create a Mark for Capture transaction.
		/// </summary>
		public const string MARK_FOR_CAPTURE_TRANSACTION = "MFC";
		/// <summary>
		/// Used to create a Profile transaction.
		/// </summary>
		public const string PROFILE_TRANSACTION = "Profile";
		/// <summary>
		/// Used to create a Reverse transaction.
		/// </summary>
		public const string REVERSE_TRANSACTION = "Reverse";

		/// <summary>
		/// Used to create a PC3 Core complex root.
		/// </summary>
		public const string PC3_CORE = "PC3Core";
		/// <summary>
		/// Used to create a PC3 Line Items recursive elements.
		/// </summary>
		public const string PC3_LINE_ITEMS = "PC3LineItems";
		/// <summary>
		/// Used to create a Settle reject bin elements.
		/// </summary>
		public const string SETTLE_REJECT_BIN = "SettleRejectBin";
		/// <summary>
		/// Used to create a Prior Authorization ID transaction or element.
		/// </summary>
		public const string PRIOR_AUTH_ID = "PriorAuthID";
		/// <summary>
		/// Used to create a Inquiry transaction
		/// </summary>
		public const string INQUIRY = "Inquiry";
		/// <summary>
		/// Used to create a Account Updater
		/// </summary>
		public const string ACCOUNT_UPDATER = "AccountUpdater";
        /// <summary>
        /// Used to create a Safetech Analysis
        /// </summary>
        public const string SAFETECH_FRAUD_ANALYSIS = "SafetechFraudAnalysis";
        /// <summary>
        /// Used to create a fraud analysis elements.
        /// </summary>
        public const string FRAUD_ANALYSIS = "FraudAnalysis";
		/// <summary>
		/// Used to create a soft merchant descriptors element.
		/// </summary>
		public const string SOFT_MERCHANT_DESCRIPTORS = "SoftMerchantDescriptors";

		#region Deprecated Request Types
		// Common Transactions
		/// <summary>
		/// Used to create a void transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string VOID = "Void";
		/// <summary>
		/// Used to create a batch settlement transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string BATCH_SETTLEMENT = "BatchSettlement";
		/// <summary>
		/// Used to create a partial void transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string PARTIAL_VOID = "PartialVoid";
		// Credit Card Transactions 
		/// <summary>
		/// Used to create an authorize or auth/capture credit card transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string CC_AUTHORIZE = "CC.Authorize";
		/// <summary>
		/// Used to create a mark for capture credit card transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string CC_CAPTURE = "CC.MarkForCapture";
		/// <summary>
		/// Used to create a recurring refund credit card transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string CC_RECURRING_REFUND = "CC.RecurringRefund";
		/// <summary>
		/// Used to create a force credit card transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string FORCE = "CC.Force";
		/// <summary>
		/// Used to create a recurring auth/capture credit card transaction.
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string CC_RECURRING_AUTH_CAPTURE = "CC.RecurringAuthCap";
		/// <summary>
		/// Used to create a credit card refund transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string ECOMMERCE_REFUND = "CC.eCommerceRefund";
		// eFalcon
		/// <summary>
		/// Used to create an eFalcon auth/capture transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string EFALCON_AUTH_CAPTURE = "eFalcon.AuthCap";
		// PC2
		/// <summary>
		/// Used to create a Purchasing Card recurring auth/capture transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string PC2_RECURRING_AUTH_CAPTURE = "PC2.AuthCapRecurring";
		/// <summary>
		/// Used to create a Purchasing Card auth or auth/capture transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string PC2_AUTH = "PC2.Auth";
		// ECP
		/// <summary>
		/// Used to create an auth or auth/capture electronic check transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string ECP_AUTH = "ECP.Authorize";
		/// <summary>
		/// Used to create a capture electronic check transaction.
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string ECP_CAPTURE = "ECP.Capture";
		/// <summary>
		/// Used to create a force deposit electronic check transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string ECP_FORCE_DEPOSIT = "ECP.ForceDeposit";
		/// <summary>
		/// Used to create a refund electronic check transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string ECP_REFUND = "ECP.Refund";
		// Switch Solo
		/// <summary>
		/// Used to create a capture Switch-Solo transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string SWITCH_SOLO_CAPTURE = "SwitchSolo.Capture";
		/// <summary>
		/// Used to create a refund Switch-Solo transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string SWITCH_SOLO_REFUND = "SwitchSolo.Refund";
		/// <summary>
		/// Used to create an auth or auth/capture Switch-Solo transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string SWITCH_SOLO_AUTH = "SwitchSolo.Auth";
		// Profile Management
		/// <summary>
		/// Used to create a profile management transaction.
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string PROFILE_MANAGEMENT = "Profile.Management";
		//FlexCache
		/// <summary>
		/// Used create a stand-alone FlexCache transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string FLEXCACHE = "FlexCache.StandAlone";
		/// <summary>
		/// Used to create a batch FlexCache transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string FLEXCACHE_BATCH = "FlexCache.Batch";
		/// <summary>
		/// Used to create a mark-for-capture FlexCache transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string FLEXCACHE_CAPTURE = "FlexCache.MFC";
		//MOTO
		/// <summary>
		/// Used to create a MOTO transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string MOTO = "MOTO";
		/// <summary>
		/// Used to create a MOTO refund transaction
		/// </summary>
		/// <remarks>THIS IS A DEPRECATED REQUEST TYPE.</remarks>
		public const string MOTO_REFUND = "MOTO.Refund";
		#endregion
	}
}
