using System;
using System.Collections;

// Disables warnings for XML doc comments.
#pragma warning disable 1591
#pragma warning disable 1573
#pragma warning disable 1572
#pragma warning disable 1571
#pragma warning disable 1587
#pragma warning disable 1570

namespace Paymentech.Comm
{

	//
	//
	//
	// Title: Arguments to Communications Layer
	//
	// Description: Arguements to be passed to comm modules
	//
	// Copyright (c) 2008, Chase Paymentech Solutions, LLC. All rights
	// reserved
	//
	// Company: Chase Paymentech
	//
	// @author Rameshkumar Bhaskharan
	// @version 1.0
	// 

	/// <summary>
	/// This class contains data that is passed to the CommManager which then
	/// passes it on to a comm module
	/// </summary>
	public class CommArgs
	{
		// Sttaus of the transaction;
		private bool isItGood=false;
		// Message Payload
		private string request = null;
		// Message Payload
		private byte[] requestAsByte = null;
		// Response string back from the Host
		private string response = null;
		// First in batch indicator for NC
		private bool firstInBatch = false;
		// Terminal ID for NC
		private string terminalID = null;
		// Merchant ID for NC
		private string merchantID = null;
		// Trace number for GW
		private string traceNumber = null;
		// Retry count response from GW
		private string retryCount=null;
		// Last retry attempt from GW
		private string lastRetryAttempt=null;
		// User Name for NC
		private string ncUsername=null;
		// User Password NC
		private string ncPassword=null;
		// Message format for NC like UTF/ISO/SLM
		private string msgFormat=null;
		// Capture mode for NC like HCS/TCS/SLM
		private string captureMode=null;
		// Cookie for statefule transaction
		private ArrayList cookies=null;
		// Host type GW for Gateway and NC for NetConnect
		private CommType commType;
        // Use cookies to maintain the state.
        private bool isStateful = false;
		
		/// <summary>
		/// Transaction status
		/// </summary>
		public bool IsItGood
		{
			get
			{
				return isItGood;
			}
			set
			{
				isItGood = value;
			}
		}

		/// <summary>
		/// Cookie
		/// </summary>
		public ArrayList Cookie
		{
			get
			{
				return cookies;
			}
			set
			{
				cookies = value;
			}
		}
		/// <summary>
		/// Terminal ID for NC
		/// </summary>
		public string TerminalID
		{
			get
			{
				return terminalID;
			}
			set
			{
				terminalID = value;
			}
		}
		/// <summary>
		/// Message format for NC like  UTF/ISO/SALEM
		/// </summary>
		public string MsgFormat
		{
			get
			{
				return msgFormat;
			}
			set
			{
				msgFormat = value;
			}
		}
		/// <summary>
		/// Capture Mode for NC like HCS/TCS/SLM 
		/// </summary>
		public string CaptureMode
		{
			get
			{
				return captureMode;
			}
			set
			{
				captureMode = value;
			}
		}
		/// <summary>
		/// User Name for NC
		/// </summary>

		public string NCUserName
		{
			get
			{
				return ncUsername;
			}
			set
			{
				ncUsername = value;
			}
		}
		/// <summary>
		/// User password for NC
		/// </summary>
		public string NCPassword
		{
			get
			{
				return ncPassword;
			}
			set
			{
				ncPassword = value;
			}
		}

		/// <summary>
		/// User password for NC
		/// </summary>
		public byte[] RequestAsByte
		{
			get
			{
				return requestAsByte;
			}
			set
			{
				requestAsByte = value;
			}
		}

		/// <summary>
		/// Retry count from GW
		/// </summary>
		public string RetryCount
		{
			get
			{
				return retryCount;
			}
			set
			{
				retryCount = value;
			}
		}
		/// <summary>
		/// Last Retry Attempt from GW
		/// </summary>
		public string LastRetryAttempt
		{
			get
			{
				return lastRetryAttempt;
			}
			set
			{
				lastRetryAttempt = value;
			}
		}

		/// <summary>
		///  First In Batch for NC
		/// </summary>
		public bool FirstInBatch
		{
			get
			{
				return firstInBatch;
			}
			set
			{
				firstInBatch = value;
			}
		}
		
		/// <summary>
		/// Merchnat ID for NC & GW
		/// </summary>
		public string MerchantID
		{
			get
			{
				return merchantID;
			}
			set
			{
				merchantID = value;
			}
		}
		
		/// <summary>
		/// Message Payload for  GW & NC
		/// </summary>
		public string Request
		{
			get
			{
				return request;
			}
			set
			{
				request = value;
			}
		}

		/// <summary>
		/// Transaction response from GW & NC
		/// </summary>
		public string Response
		{
			get
			{
				return response;
			}
			set
			{
				response = value;
			}
		}

		/// <summary>
		///  Trace number for GW
		/// </summary>
		public string TraceNumber
		{
			get
			{
				return traceNumber;
			}
			set
			{
				traceNumber = value;
			}
		}

        /// <summary>
        /// Use cookies to maintain the state.
        /// </summary>
        public bool IsStateful
        {
            get
            {
                return isStateful;
            }
            set
            {
                isStateful = value;
            }
        }
		
		/// <summary>
		/// Communication type GW for gateway and NC for NetConnect
		/// </summary>
		public CommType COMMType
		{
			get
			{
				return commType;
			}
			set
			{
				commType = value ;
			}
		}

		/// <summary>
		/// COMM Types
		/// </summary>
		public enum CommType
		{
			/// <summary>
			/// GW for Gateway
			/// </summary>
			GW=1, 
			/// <summary>
			/// NC for Netconnect
			/// </summary>
			NC=2};

	
	}
}
