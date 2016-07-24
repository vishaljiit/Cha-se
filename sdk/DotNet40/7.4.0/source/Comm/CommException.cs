using System;

// Disables warnings for XML doc comments.
#pragma warning disable 1591
#pragma warning disable 1573
#pragma warning disable 1572
#pragma warning disable 1571
#pragma warning disable 1587
#pragma warning disable 1570

namespace Paymentech.Comm
{
	/// <p>Copyright (c) 2008, Chase Paymentech Solutions, LLC. All rights
	/// reserved</p>
	///
	/// <p>Company: Chase Paymentech Solutions</p>
	///
	/// @author Rameshkumar Bhaskharan
	/// @version 1.0
	///
	/// <summary>
	/// Summary description for ConverterException.
	/// </summary>
	[Serializable]
	public class CommException : Exception
	{
		public enum Error
		{
			PTSDK_ERROR_DEFAULT =0,
			/* A protocol error */
			PTSDK_ERROR_PROTOCOL=1,

			/* An error was returned by the gateway */
			PTSDK_ERROR_GATEWAY=2, 

			/* A socket was unexpectedly closed */  
			PTSDK_ERROR_UNEXPECTED_SOCKET_CLOSE=3,

			/* An unknown communication error occured at the socket level */ 
			PTSDK_ERROR_UNKNOWN=4,

			/* A communication error occured while writing a transaction */ 
			PTSDK_ERROR_COMMUNICATION_WRITE_FAILURE=5,

			/* A communication error occured while reading the response of a transaction */ 
			PTSDK_ERROR_COMMUNICATION_READ_FAILURE=51,

			/* The timeout for an outstanding transaction has expired */
			PTSDK_ERROR_TIMEOUT=6,

			/* An integrity check has failed for the payload */
			PTSDK_ERROR_INTEGRITY_CHECK_FAILED=7,

			/* A resource was unavailable */
			PTSDK_ERROR_RESOURCE_UNAVAILABLE=8,

			/* All retry attempts were exhausted */
			PTSDK_ERROR_RETRY_ATTEMPTS_EXHAUSTED=9

		}
		private Error error = Error.PTSDK_ERROR_DEFAULT;
		private string legacyError = null;
		private string gwErrorCode=null;
		private string httpResponseCode=null;
		private string gwErrorReason=null;

		public CommException(string message) : base(message)
		{
		}
		public CommException(Error error, string gwErrorCode, string httpResponseCode, string gwErrorReason, string message) : base(message)
		{
			this.error = error;
			this.gwErrorCode = gwErrorCode;
			this.httpResponseCode = httpResponseCode;
			this.gwErrorReason = gwErrorReason;
		}
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="error"></param>
		public CommException(Error error)
		{
			this.error = error;
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="error"></param>
		/// <param name="message"></param>
		public CommException(Error error, string message) : base(message)
		{
			this.error = error;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="error"></param>
		/// <param name="message"></param>
		public CommException(Error error, string message , string legacyErrorMsg) : base(message)
		{
			this.error = error;
			this.legacyError = legacyErrorMsg;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="error"></param>
		public CommException(string message, Exception ex) : base(message,ex)
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="error"></param>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		public CommException(Error error, string message, Exception exception) : base(message, exception)
		{
			this.error = error;
		}
		/// <summary>
		/// Error code
		/// </summary>
		public Error ErrorCode
		{
			get { return this.error; }
		}

		public string GWErrorCode
		{
			get { return this.gwErrorCode; }
		}

		public string  GWErrorReason
		{
			get { return this.gwErrorReason; }
		}

		public string  HTTPResponseCode
		{
			get { return this.httpResponseCode; }
		}

	}
}
