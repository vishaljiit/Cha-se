using System;
using System.Net;

using System.IO;
using System.Net.Sockets;
using System.Collections;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

using log4net.Config;
using log4net;
using log4net.Core;
using log4net.Appender;
using log4net.Repository.Hierarchy;

using Paymentech.Core;


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
	// Title: HTTPS Connect Communication Module
	//
	// Description: This communication module implements the paymentech
	// "HTTPSConnect" application protocol
	//
	// Copyright (c) 2008, Chase Paymentech Solutions, LLC. All rights
	// reserved
	//
	// Company: Chase Paymentech Solutions
	//
	/// @author Rameshkumar Bhaskharan
	// @version 1.0
	//

	/// <summary>
	/// This communication module implements the paymentech
	/// "HTTPSConnect" application protocol
	/// </summary>
	public class HTTPSConnect : IHTTPSConnect
	{
		private static string Host = null;
		private static int Port = 0;
		private static int MaxConnections=0;
		private static int ReadTimeOutSecs = 0;
        private static int ConnectLoopWaitMSecs = 0;
        private static int ConnectFailRetries = 0;
        private static int ConnectTimeOutSecs = 0;
        private static int WriteTimeOutSecs = 0;
		private static bool VerifyHost=false;
		private static string ProxyUserName = null;
		private static string ProxyPassword = null;
		private static string DTDVersion=null;
		private static string requestName=null;
		private static string responseName=null;
		private static string controlRequestName=null;
		private static string controlResponseName=null;
		private const string configFileName = "linehandler.properties";
		private static bool retryEnable = false;
		private static int retryAttempt=1;
		private static string merchantID=null;
		private static string terminalID=null;
		private static string ncUsername=null;
		private static string ncPassword=null;
		private static bool integrityCheck=false;
		private static string msgFormat=null;
		private static string captureMode=null;
		private static string session= null;
		private static string version=null;
        private static string serverCN = null;
        private static string failoverServerCN = null;
        // host used if connect to primary host fails
		private static string failOverHost = null;
		// port that goes with host above
		private static int failOverPort = 0;	
		// component of the mime header
		private static string contentType = null;
		// URI (path) on the servers site
		private static string authorizationURI = null;
		// firewall proxy (on client side)
		private static string proxyHost = null;	
		// firewall port 
		private static int proxyPort = 0;
		// secs before switching back to primary	
		private static long failOverDurationSecs = 0;	
		// size of our read() input buffer
		private static long inBufSize = 1024;		
		private const string MimeVersionHeader = "MIME-Version";
		private const string MimeVersionDefault = "1.1";
		private const int SocketTimeoutError = 10060;
		//
		// The following is the list of exception names that, if received while 
		// attempting to connect, will cause a retry of the connect attempt
		// This feature is only active if the <ConnectFailRetryReasons> element
		// in the config.xml file is present and has exception and status names
		private static string connectFailRetryReasons = null; 
		// split what is in connectFailRetryReasons, one exception per string
		private static string [] connectFailRetryReasonsArray = null; 
		//
		// Indicates whether there was a failure with the last transaction
		//
		private static bool lastConnectFailed = false;
		private static long failOverStart = 0;	// millisec failover started
		// The last host we used
		private static string lastHost = null;
		private static int lastPort = 0;
		private static bool initialized = false;
		private static ILog engineLogger = null;
		/// <summary>
		/// Convert a string into integer
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static int StringToInt(string data)
		{
			try
			{
				return Convert.ToInt32(data);
			}
			catch(Exception)
			{
				return 0;
			}
		}

		/// <summary>
		/// Initilaize all teh static variables
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		private static void Init()
		{
			if ( ! initialized )
			{
				IConfigurator config = Configurator.Instance;
				Host = config["LineHandlerFactory.LineHandler.hostname"];
				if ( Host == null )
					throw new CommException ( "Missing Primary Host name in " + configFileName);
				Port = config.GetInt("LineHandlerFactory.LineHandler.port");
				if ( Port == 0 )
					throw new CommException ( "Missing Primary port in " + configFileName);
				failOverHost = config["LineHandlerFactory.LineHandler.failover.hostname"];
				failOverPort = config.GetInt("LineHandlerFactory.LineHandler.failover.port");
				contentType = config["NetConnect.message_format"];
				authorizationURI = config["LineHandlerFactory.LineHandler.authorization_uri"];
				if ( authorizationURI == null )
					throw new CommException ( "Missing Authorization URI in " + configFileName);
				proxyHost = config["LineHandlerFactory.LineHandler.proxyHostname"];
				proxyPort = config.GetInt("LineHandlerFactory.LineHandler.proxyPort");
				failOverDurationSecs = config.GetInt("LineHandlerFactory.LineHandler.failover.timerSeconds");
				MaxConnections = config.GetInt("Resource.maxConcurrentRequests");
                string text = null;
                VerifyHost = config.GetBool("LineHandlerFactory.LineHandler.ValidateCertificate");
                serverCN = config["LineHandlerFactory.LineHandler.ServerCN"];
                failoverServerCN = config["LineHandlerFactory.LineHandler.Failover.ServerCN"];
                retryEnable = config.GetBool("LineHandlerFactory.LineHandler.auto_retry_enable");
                retryAttempt = config.GetInt("LineHandlerFactory.LineHandler.auto_retry_attempts") + 1;
                ConnectFailRetries = config.GetInt("LineHandlerFactory.LineHandler.restart_attempts");
                ConnectTimeOutSecs = config.GetInt("LineHandlerFactory.LineHandler.connect_timeout_seconds");
                ConnectLoopWaitMSecs = config.GetInt("LineHandlerFactory.LineHandler.connect_loop_wait_mseconds");
                ProxyUserName = config["LineHandlerFactory.LineHandler.proxyUserName"];
				ProxyPassword = config["LineHandlerFactory.LineHandler.proxyPassword"];
				ReadTimeOutSecs = config.GetInt("LineHandlerFactory.LineHandler.timeout_seconds");
                WriteTimeOutSecs = config.GetInt("LineHandlerFactory.LineHandler.write_timeout_seconds");
                DTDVersion = config["LineHandlerFactory.LineHandler.mimeheader.DTDVersion"];
				version = config["SDKVersion"];
				if ( DTDVersion == null )
					throw new CommException ( "Missing DTD Version in " + configFileName);
				requestName = config["LineHandlerFactory.LineHandler.mimeheader.Request_name"];
				if ( requestName == null )
					throw new CommException ( "Missing Request Name in " + configFileName);
				responseName = config["LineHandlerFactory.LineHandler.mimeheader.Response_name"];
				if ( responseName == null )
					throw new CommException ( "Missing Response Name in " + configFileName);
				controlRequestName = config["LineHandlerFactory.LineHandler.mimeheader.Control_Request_name"];
				if ( controlRequestName == null )
					throw new CommException ( "Missing Control Response Name in " + configFileName);
				controlResponseName = config["LineHandlerFactory.LineHandler.mimeheader.Control_Response_name"];
				if ( controlResponseName == null )
					throw new CommException ( "Missing COntrol Response Name in " + configFileName);
				merchantID = config["NetConnect.merchant_id"];
				terminalID = config["NetConnect.terminal_id"];
				ncUsername = config["NetConnect.username"];
				ncPassword = config["NetConnect.password"];
                text = config["NetConnect.message_integrity_check"];
                integrityCheck = (text != null && text.ToUpper() == "ON");
			    //integrityCheck = ( config["NetConnect.message_integrity_check"] == null ) ? false : ( config["NetConnect.message_integrity_check"].ToUpper().Equals("ON")) ? true : false ; 
				msgFormat = config["NetConnect.message_format"];
				captureMode = config["NetConnect.terminal_capture_mode"];
				session = config["NetConnect.session_persistence"];
				//VerifyHost = false;
				inBufSize = 1024;		
				connectFailRetryReasons = null; 
				connectFailRetryReasonsArray = null; 
				lastConnectFailed = false;
				failOverStart = 0;	// millisec failover started
				lastHost = null;
				lastPort = 0;
				if ( ( authorizationURI == null )|| ( authorizationURI.Length == 0 ) )
				{
					throw new CommException( "AuthorizationURI in config.xml must  be set" );
				}
				if ( MaxConnections == 0 )
				{
					throw new CommException( "MaxConnections must be greater than 0" );
				}
				// host names need to be lower case
				if ( failOverHost != null )
				{
					failOverHost = failOverHost.ToLower();
				}
				if ( proxyHost != null )
				{
					proxyHost = proxyHost.ToLower();
				}
				// This option allows the host name in the server certificate 
				// tobe ignored. Should only be set to "false" in a test 
				// environment
                if (VerifyHost == false)
                {
                    ServicePointManager.ServerCertificateValidationCallback = null;
                    ServicePointManager.CertificatePolicy = new AcceptServerNameMismatch();
                }
                else if (serverCN != null)
                {
                    ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(CertValidationCallback);
                }

                initialized = true;
				InitConnectFailRetryReasons( connectFailRetryReasons );
				engineLogger = config.EngineLogger;
			}
		
		}

        public static bool CertValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            string checkCN = (lastConnectFailed) ? failoverServerCN : serverCN;
            string name = certificate.GetName();
            if (name == null)
                return false;
            string[] items = name.Split(',');
            string serverCommonName = null;
            foreach (string item in items)
            {
                if (item.Trim().StartsWith("CN="))
                    serverCommonName = item.Trim().Substring(3);
            }
            if (serverCommonName != null && serverCommonName.Equals(checkCN, StringComparison.CurrentCultureIgnoreCase))
                return true;
            return false;
        }

		/// <summary>
		/// Default constructor
		/// </summary>
		public HTTPSConnect() 
		{
		}
		/// <summary>
		/// dummy method for interface
		/// </summary>
		public void Close()
		{
			// not needed for this protocol because all interface objects 
			// are one time use
		}
		/// <summary>
		/// parse the whitespace separated names of exceptions and statuses
		/// </summary>
		/// <param name="rawString">raw string data from XML element</param>
		private static void InitConnectFailRetryReasons( string rawString )
		{
			string [] splitStrings = null;
	
			if ( rawString != null )
			{
				Regex reg = new Regex("[ \t\r\n]+");
				// split on white space
				splitStrings = reg.Split( rawString ); 
				// if either rawString or result of split is null 
				// then make empty array
				if ( splitStrings == null )
				{
					splitStrings = new string[ 0 ];
				}
				// see if there was leading white space (normally 
				// there is)
				if ( ( splitStrings.Length > 0 ) 
					&& ( splitStrings[ 0 ].Length == 0 ) ) 
				{
					// trailing white
					if ( splitStrings[ 
						splitStrings.Length - 1 ].Length == 0 ) 
					{
						// leading and trailing white space 
						// so shorten by 2
						connectFailRetryReasonsArray = 
							new string[ 
							splitStrings.Length - 2 ];
					}
					else
					{
						// just leading white space so 
						// shorten by 1
						connectFailRetryReasonsArray = 
							new string[ 
							splitStrings.Length - 1 ];
					}
					for ( int i=0; i < 
						connectFailRetryReasonsArray.Length; 
						i++ )
					{
						// the +1 moves everything past the 
						// leading white space
						connectFailRetryReasonsArray[ i ] = 
							splitStrings[ i + 1 ];
					}
				}
				else if ( ( splitStrings.Length > 0 ) 
					// this happens when trailing white space
					&& ( splitStrings[ 
					splitStrings.Length - 1 ].Length == 0 ) ) 
				{
					connectFailRetryReasonsArray = 
						new string[ splitStrings.Length - 1 ];

					for ( int i=0; 
						i < connectFailRetryReasonsArray.Length; i++ )
					{
						connectFailRetryReasonsArray[ i ] = 
							splitStrings[ i ];
					}
				}
				else // otherwise the normal split is good for use
				{
					connectFailRetryReasonsArray = splitStrings;
				}
			}
		}

		/// <summary>
		/// Whenever a communication succeeds (from a connectivity standpoint)
		/// then call this method
		/// </summary>
		/// <param name="httpRequest">last used request object</param>
		private void ClientSuccess( HttpWebRequest httpRequest )
		{
			lastConnectFailed = false;
	
			SetLastHost( httpRequest );
		}

		/// <summary>
		/// Same as above but when failure
		/// </summary>
		/// <param name="httpRequest">last used request object</param>
		private void ClientFailure( HttpWebRequest httpRequest )
		{
			lastConnectFailed = true;

			SetLastHost( httpRequest );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="httpRequest"></param>
		private void SetLastHost( HttpWebRequest httpRequest )
		{
			if ( ( httpRequest != null ) && ( httpRequest.Address 
				!= null ) )
			{
				lastHost = httpRequest.Address.Host;
				lastPort = httpRequest.Address.Port;
			}
		}

		/// <summary>
		/// Determines the correct host to use 
		/// </summary>
		/// <param name="proxyUser">user</param>
		/// <param name="proxyPassword">password</param>
		/// <returns>The appropriate http interface object to use</returns>
		private HttpWebRequest DetermineClient( string proxyUser, 
			string proxyPassword, CommArgs args, int retryCount)
		{
			string primaryHost = Host;
			int primaryPort = Port;
            string chosenHost = Host;
            int chosenPort = Port;
            HttpWebRequest retval = null;
			long currTime = ( DateTime.Now.Ticks / 10000 );
			
			// see if we even have a failover option
			if ( ( failOverHost != null ) && ( failOverPort > 0 ) )
			{
				if ( lastConnectFailed )
				{	
					if ( ( lastHost == primaryHost ) && 
						( lastPort == primaryPort ) )
					{
                        chosenHost = failOverHost;
                        chosenPort = failOverPort;
                        failOverStart = currTime;
					}
					else
					{
						failOverStart = 0;
					}
				}	
				else	// last connect was a success
				{	
					// check to see if it is time to return to 
					// primary
					if ( ( failOverStart > 0 ) &&
						( ( ( failOverDurationSecs * 1000 ) 
						+ failOverStart ) > currTime ) )
					{
                        chosenHost = failOverHost;
                        chosenPort = failOverPort;
                    }
				}
			}

            engineLogger.Info(String.Format("Attempt {0} to connect to {1}:{2}", retryCount + 1, chosenHost, chosenPort));

            retval = GetHttpClient(chosenHost,
                chosenPort, proxyHost, proxyPort,
                proxyUser, proxyPassword, args);

			return retval;
		}
		/// <summary>
		/// This method checks the exception to see if we should try to 
		/// connect again
		/// </summary>
		/// <param name="exception">exception that we got back when we tried to 
		/// connect</param>
		/// <returns>true if we should try again, false otherwise</returns>
		private bool RetryConnect( Exception exception )
		{
			bool retval = true;  // if nothing is specified, we always retry
			Exception baseEx = exception.GetBaseException();

			// When the <ConnectFailRetryReasons> element in the 
			// config.xml file is set, we retry only for specific 
			// exceptions and WebException statuses
			if ( ( connectFailRetryReasonsArray != null ) 
				&& ( connectFailRetryReasonsArray.Length > 0 ) &&
				( baseEx != null ) )
			{
				Type baseExType = baseEx.GetType();
				string status = "";

				if ( baseEx is System.Net.WebException )
				{
					WebException ex = ( WebException ) exception;

					status = ex.Status.ToString();
				}

				string exceptionFullName = baseExType.FullName;
				string exceptionName = baseExType.Name;
	
				engineLogger.Debug( "Retry handler got exception: " 
					+ exceptionName );
		
				// Look to see if it is one of the exceptions or 
				// statuses that should cause us to retry
				for ( int i=0; i < connectFailRetryReasonsArray.Length;
					i++ )
				{
					if ( ( connectFailRetryReasonsArray[ i ] == 
						exceptionName ) ||
						( connectFailRetryReasonsArray[ i ] == 
						exceptionFullName ) ||
						( connectFailRetryReasonsArray[ i ] == 
						status ) )
					{	
					
						retval = true;
						break;
					}
				}
			}

			return retval;
		}

		/// <summary>
		/// Connect and send request
		/// </summary>
		/// <param name="newStream">stream created in the 
		/// DoConnect() method</param>
		/// <param name="request">data and options for request</param>
		/// <returns>object we used to send the request</returns>
		private HttpWebRequest DoRequest( out Stream newStream,  CommArgs args )
		{

			string postData = null;
			byte[]  requestBytes = null;
			int dataLength = 0;
			// used to get bytes out of a string
			ASCIIEncoding encoding = new ASCIIEncoding();
			if ( args.COMMType == CommArgs.CommType.GW )
			{
				postData = args.Request;
				dataLength = postData.Length;
				requestBytes = encoding.GetBytes( postData );
			}
			if ( args.COMMType == CommArgs.CommType.NC )
			{
				requestBytes = args.RequestAsByte;
				dataLength = requestBytes.Length;
			}
			
			
			// this will be the return value
			HttpWebRequest httpRequest = null;
			
			
			// convert the string to an array of bytes
			
			newStream = null;
			if ( args.COMMType == CommArgs.CommType.NC &&  integrityCheck && args.MsgFormat.ToUpper().Equals("UTF197") )
			{
				if ( ! this.checkDataIntegerity(requestBytes))
					return null;
				else
				{
					byte[] temp  = new byte[requestBytes.Length -1 ];
					Array.Copy(temp, requestBytes, requestBytes.Length -1 );
					requestBytes = temp;
				}
			}


			try
			{
				httpRequest = DoConnect( out newStream,  
					dataLength , args );
			}
			catch( Exception ex )
			{
				if ( engineLogger.IsDebugEnabled )
				{
					engineLogger.Debug ( ex.Message );
				}

				string reason = " Got Exception: ";
				WebException wex = null;
				int externalErr = 0;
	
				reason += ex.GetType().FullName;

				// see if we got the special web exception that 
				// has an informative status in it
				if ( ex is WebException )
				{
					wex = ( WebException ) ex;
					externalErr = ( int ) wex.Status;
					reason += " with status: " + 
						wex.Status.ToString();

                    if (wex.Status == WebExceptionStatus.TrustFailure)
                        throw ex;
				}
			
				throw ex;
			}

			// if connect succeeded
			if ( newStream != null )
			{
				int timeOut = WriteTimeOutSecs * 1000;

				// setting the timeout to zero means "never time out"
				if ( timeOut == 0 )
				{
					timeOut = -1;
				}

				// separate timeout for writing
				httpRequest.Timeout = timeOut;

				try
				{
					// write to the socket
					newStream.Write( requestBytes, 0, 
						requestBytes.Length );
				}
				catch ( Exception ex )
				{
					if ( engineLogger.IsDebugEnabled )
					{
						//CommUtils.PrintException( ex );
					}

					HttpWebResponse httpResponse = null;

					// strange but true, an exception while 
					// writting can have a response in it.
					if ( ex is WebException )
					{
						WebException wex = (WebException) ex;
						httpResponse = ( HttpWebResponse ) 
							wex.Response;

						engineLogger.Debug("WebException status is: " + wex.Status.ToString() +
							" with value: " + ( int ) wex.Status );
					}
					if ( ex is SocketException )
						throw new CommException(CommException.Error.PTSDK_ERROR_COMMUNICATION_WRITE_FAILURE,ex.Message);
					throw ex;
				}
			}

			return httpRequest;
		}

		public void PopulateMimeHeader( HttpWebRequest httpRequest, 
			string contentType, 
			int contentLength, CommArgs args )
		{

			httpRequest.Method = "POST";
			httpRequest.Headers.Set( MimeVersionHeader, MimeVersionDefault );
			httpRequest.ContentLength = contentLength;
			httpRequest.Headers.Set( "Interface-Version", version );
			if ( args.COMMType == CommArgs.CommType.GW )
			{
				httpRequest.Headers.Set( "Content-transfer-encoding", "text/xml" );
				httpRequest.Headers.Set( "Document-Type", "Request" );
				httpRequest.Headers.Set( "DTDVersion", DTDVersion );
				httpRequest.Headers.Set( "Request_name", requestName );
				httpRequest.Headers.Set( "Response_name", responseName );
				httpRequest.Headers.Set( "Request-number", "1" );
				httpRequest.Headers.Set( "Control_Request_name", controlRequestName );
				httpRequest.Headers.Set( "Control_Response_name", controlResponseName );
				httpRequest.ContentType = "application/" + DTDVersion ;
				if ( args.MerchantID != null )
					httpRequest.Headers.Set("Merchant-id",args.MerchantID);
				if ( args.TraceNumber != null )
					httpRequest.Headers.Set("Trace-number",args.TraceNumber);
			}
			if ( args.COMMType == CommArgs.CommType.NC )
			{
				if ( args.Cookie != null )
				{
					foreach( string cookie in args.Cookie )
						httpRequest.Headers["Cookie"] = cookie;
				}
				if ( args.MerchantID != null )
					httpRequest.Headers.Set("Merchant-id",args.MerchantID);
				else
					httpRequest.Headers.Set("Merchant-id",HTTPSConnect.merchantID);
				if ( args.NCUserName != null )
						httpRequest.Headers.Set("Auth-User",args.NCUserName);
				else
						httpRequest.Headers.Set("Auth-User",HTTPSConnect.ncUsername);
				if ( args.NCPassword != null )
					httpRequest.Headers.Set("Auth-Password",args.NCPassword);
				else
					httpRequest.Headers.Set("Auth-Password",HTTPSConnect.ncPassword);
				if ( args.FirstInBatch )
					httpRequest.Headers.Set("Header-Record","true");
				string tempContentType=null;
				if ( args.MsgFormat != null )
					tempContentType = args.MsgFormat;
				else
					tempContentType = HTTPSConnect.msgFormat;
				if ( args.CaptureMode != null )
					tempContentType = tempContentType + "/" + args.CaptureMode;
				else
					tempContentType = tempContentType + "/" + HTTPSConnect.captureMode;
				httpRequest.ContentType = tempContentType;
				httpRequest.Headers.Set("Stateless-Transaction", (args.IsStateful) ? "false" : "true");

                string trantype = httpRequest.Headers.Get("Stateless-Transaction");
				
				if ( args.MerchantID != null )
					httpRequest.Headers.Set("Auth-MID",args.MerchantID);
				else
					httpRequest.Headers.Set("Auth-MID",HTTPSConnect.merchantID);

				if ( args.TerminalID != null )
					httpRequest.Headers.Set("Auth-TID",args.TerminalID);
				else
					httpRequest.Headers.Set("Auth-TID",HTTPSConnect.terminalID);
			}
			ValidateMimeHeaders(httpRequest);
			logHttpRequest(httpRequest);
		}


		/// <summary>
		/// Connect to the remote host
		/// </summary>
		/// <param name="newStream">Stream to write to</param>
		/// <param name="sman">security info</param>
		/// <param name="dataLength">length of the data we need to send</param>
		/// <returns>new connection stream</returns>
		private HttpWebRequest DoConnect( out Stream newStream, 
			int dataLength, CommArgs args )
		{
			HttpWebRequest httpRequest = null;
			long startTime = (( long ) DateTime.Now.Ticks ) / ( ( long ) 10000 );
			int timeOut = ConnectTimeOutSecs * 1000;
			int remainingTime = timeOut;
		
			newStream = null;

			for ( int i = 0; ( i <= ConnectFailRetries ) && ( newStream == null ); i++ )
			{
				try
				{
					// see if we need to talk to the primary 
					// server or the failover
					httpRequest = DetermineClient( ProxyUserName, ProxyPassword ,  args, i);
				}
				catch ( Exception ex )
				{
					string message = "Got exception: " + 
						ex.GetType().Name +
						" with message: " + ex.Message;
					engineLogger.Warn( message );
		
					throw new CommException(message +  "\nFailed to create httpclient", ex );
				}

				// populate the mime header
				PopulateMimeHeader( httpRequest,
					contentType, dataLength , args );

				try
				{
					// setting the timeout to zero means "never time out"
					if ( timeOut == 0 )
					{
						timeOut = -1;
					}

					// set the overall time out
					httpRequest.Timeout = ( int ) timeOut;
					newStream = httpRequest.GetRequestStream();
					ClientSuccess( httpRequest );
				}
				catch ( Exception ex )
				{
					//ClientFailure( httpRequest );
					if ( ex is WebException )
					{
						WebException wex = ( WebException ) ex;

						engineLogger.Debug("WebException status is: " + wex.Status.ToString() +
							" with value: " + ( int ) wex.Status );

                        if (wex.Status == WebExceptionStatus.TrustFailure)
                        {
                            throw ex;
                        }
					}

					// test to see if this is an exception
					// for which we should retry, if not,
					// throw the exception
					if ( RetryConnect( ex ) )
					{
						// make sure we haven't reached our 
						// retry limit already
						if ( i < ConnectFailRetries )
						{
							engineLogger.Debug( 
								"Retrying HTTPS request . . ." );

							// if we are supposed to wait 
							// between retries, do it here
							if ( ConnectLoopWaitMSecs > 0 )
							{
								Thread.Sleep( 
									ConnectLoopWaitMSecs );
							}
						}
						else
						{
							ClientFailure( httpRequest );
							throw ex;
						}
					}
						// we have been told not to retry this exception
					else
					{
						ClientFailure( httpRequest );
						throw ex;
					}
				}

				if ( timeOut > 0 )
				{
					// check to see if we timed out
					int elapsedTime = ( int ) ( ( ( long ) DateTime.Now.Ticks 
						/ ( long ) 10000 ) - startTime );

					remainingTime = timeOut - elapsedTime;

					if ( remainingTime < 0 )
					{
						ClientFailure( httpRequest );
						throw new CommException( "Timeout occurred while trying to connect" );
					}
				}

				if ( ( newStream == null ) && ( ConnectLoopWaitMSecs > 0 ) )
				{
					Thread.Sleep( ConnectLoopWaitMSecs );
				}
			}

			return httpRequest;
		}

		public void logHttpRequest(HttpWebRequest req )
		{
			if ( engineLogger.IsDebugEnabled )
			{
				engineLogger.Debug("*****Request Headers*****");
				foreach ( string  key in req.Headers.AllKeys )
                    engineLogger.DebugFormat("{0}[{1}]", key, req.Headers[key]);
                engineLogger.DebugFormat("Content-Length[{0}]", req.ContentLength);
			}

		}

		public void logHttpResponse(HttpWebResponse res )
		{
			
			if ( engineLogger.IsDebugEnabled )
			{
				engineLogger.Debug("*****Response Headers*****");
				foreach ( string key in res.Headers.AllKeys )
                    engineLogger.DebugFormat("{0}[{1}]", key, res.Headers[key]);
			}

		}
		public void logCommArgs(CommArgs args)
		{
			if ( engineLogger.IsDebugEnabled )
			{
				engineLogger.Debug("*****Comm Args*****");
				engineLogger.Debug("FirstInBatch[" + args.FirstInBatch + "]" );
				engineLogger.Debug("Terminal ID[" + args.TerminalID + "]" );
				engineLogger.Debug("Merchant ID[" + args.MerchantID + "]" );
				engineLogger.Debug("Trace Number[" + args.TraceNumber + "]" );
				engineLogger.Debug("NC User Name[" + args.NCUserName + "]" );
				engineLogger.Debug("Message Format[" + args.MsgFormat + "]" );
				engineLogger.Debug("Capture Mode[" + args.CaptureMode + "]" );
				engineLogger.Debug("Comm Type[" + args.COMMType + "]" );

				if ( args.Cookie != null && args.Cookie.Count > 0 )
				{
					for( int ctr=0 ; ctr < args.Cookie.Count ; ctr++ )
						engineLogger.Debug("Cookies[" + ctr + "][" + args.Cookie[ctr] + "]" );
				}
			}

		}

		public CommArgs CompleteTransaction( CommArgs args  ) 
		{

			// Initialize all the static 
			if ( ! initialized )
				HTTPSConnect.Init();
		
			logCommArgs(args);

			bool firstTry = true;	// set to false on second attempt
			Stream newStream = null;	

			HttpWebRequest httpRequest = null;

            int numRetries = (retryEnable && ReadTimeOutSecs >= 90 && args.TraceNumber != null) ? retryAttempt : 1;

            for (int retry = 0; retry < numRetries; retry++)
            {
                // This loop facilitates failover by allowing a switch to 
                // the failover host and port		
                for (int i = 0; i < 2; i++)
                {
                    // First connect and send the request
                    try
                    {
                        httpRequest = DoRequest(out newStream, args);

                        if (httpRequest == null && args.COMMType == CommArgs.CommType.NC)
                            args.Response = ("\x15");
                    }
                    catch (Exception ex)
                    {
                        if (ex is WebException)
                        {
                            WebException wex = (WebException)ex;
                            if (wex.Status == WebExceptionStatus.TrustFailure)
                                throw ex;
                        }

                        ClientFailure(httpRequest);

                        // make sure we don't lose handles
                        if (newStream != null)
                        {
                            // ignore any exceptions
                            try
                            {
                                newStream.Close();
                            }
                            catch { }

                            // make sure it is null so the finally
                            // near the end of this method does not
                            // try to close twice
                            newStream = null;
                        }

                        // if we only tried once, try again
                        if ((firstTry) && (failOverHost != null)
                            && (failOverPort != 0))
                        {
                            if (lastHost != null)
                            {
                                engineLogger.Warn(
                                    "Failed using host: "
                                    + lastHost);
                            }

                            engineLogger.Warn(
                                "Retrying using alternate host");

                            firstTry = false;
                            continue;
                        }
                        else
                        {
                            throw new CommException(ex.Message, ex);
                        }
                    }

                    // if we got here we succeeded in sending the request
                    break;
                }

                // Now get the response
                // we don't failover because failing over a "read" could cause
                // a dup
                try
                {
                    args = DoResponse(httpRequest, args);
                    if (args.Response != null)
                    {
                        engineLogger.Debug("Received " +
                            args.Response.Length + " bytes");
                    }
                    else
                    {
                        engineLogger.Debug("DoResponse returned null");
                    }
                    break;
                }
                // if other exception then wrap it in a CommException
                catch (Exception ex)
                {
                    if (retry < numRetries - 1)
                        continue;
                    throw new CommException(ex.Message, ex);
                }
                finally
                {
                    // We're done looping.
                    if (newStream != null)
                    {
                        try
                        {
                            newStream.Close();
                        }
                        catch { }
                    }
                }
            }
		
			ClientSuccess( httpRequest );
			args.IsItGood = true;
			return args;
		}




		/// <summary>
		/// Look at response that came back 
		/// </summary>
		/// <param name="httpRequest">http request object</param>
		/// <returns>response data</returns>
		private CommArgs DoResponse( HttpWebRequest httpRequest , CommArgs args )
		{
			string respStr = null;  // return value
			HttpWebResponse httpResponse = null;

			try
			{
				ByteBuffer buf = null;

				int timeOut = ReadTimeOutSecs * 1000;

				// setting the timeout to zero means "never time out"
				if ( timeOut == 0 )
				{
					timeOut = -1;
				}

				// this will throw an exception if the
				// stream was auto closed which happens (for
				// example) when a proxy authentication fails
				httpRequest.Timeout = timeOut;
				httpResponse = ( HttpWebResponse ) 
					httpRequest.GetResponse();
				
				if ( httpResponse == null )
				{
					throw new CommException( CommException.Error.PTSDK_ERROR_INTEGRITY_CHECK_FAILED,"Data integrity check failed");
				}

				buf = GetStreamMessage( httpResponse );
				if ( buf == null || buf.Bytes.Length == 0 )
				{
					throw new CommException( CommException.Error.PTSDK_ERROR_PROTOCOL ,"Null returned from GetStreamMessage()" , "Received unexpected blank response line!" );
				}

				if ( httpResponse.Headers.Keys.Count == 0 )
				{
					throw new CommException( CommException.Error.PTSDK_ERROR_PROTOCOL ,"No mime header" , "No mime-headers were read in the response!" );
				}

                // Sometimes the content-length has two numbers separated by a comma. 
                // It shouldn't, but sometimes does. In these cases, we take the first 
                // of these numbers.
				int contentLength = 0;
                string[] lengths = httpResponse.Headers["content-length"].Split(',');
				contentLength = StringToInt(lengths[0]);
				if ( contentLength != buf.Bytes.Length )
					throw new CommException( CommException.Error.PTSDK_ERROR_PROTOCOL , "Expected [" + contentLength + "] bytes, but received [" + buf.Bytes.Length + "] bytes." );

				this.logHttpResponse(httpResponse);

				if ( args.COMMType == CommArgs.CommType.GW )
				{
					args.RetryCount = httpResponse.Headers["retry-count"];
					args.LastRetryAttempt = httpResponse.Headers["last-retry-attempt"];
				}
				if ( args.COMMType == CommArgs.CommType.NC )
				{
					ArrayList temp = new ArrayList();
					for(int i=0; i < httpResponse.Headers.Count; ++i) 
					 if (httpResponse.Headers.Keys[i].ToUpper().Equals("SET-COOKIE"))
						 temp.Add(httpResponse.Headers[i]);
					string errorCode = httpResponse.Headers["Error-Code"];
					engineLogger.Debug("Error-Code[" + errorCode + "]");
					string httpResponseCode = ((int)httpResponse.StatusCode).ToString();
					engineLogger.Debug("HTTP Response code[" + httpResponseCode + "]");
					string errorDesc = null;
					if ( errorCode != null )
					{
						errorDesc = httpResponse.Headers["Error-Reason"];
						throw new CommException( CommException.Error.PTSDK_ERROR_GATEWAY,errorCode,httpResponseCode,errorDesc,"Exception while processing the request");
					}
					if ( httpResponseCode.Length != 0 && ! httpResponseCode.Equals("200") && ! httpResponseCode.Equals("412"))
						throw new CommException( CommException.Error.PTSDK_ERROR_GATEWAY,"",httpResponseCode,"HTTP error response received.","Exception while processing the request");


				}

				// change the bytes we read into a string
				respStr = ByteArrayToString( buf.Array() );
			}
			catch ( Exception ex )
			{

				if ( ex is WebException )
				{
					WebException wex = ( WebException ) ex;
					httpResponse = ( HttpWebResponse ) wex.Response;
					engineLogger.Debug("WebException status is: " + wex.Status.ToString() +
						" with value: " + ( int ) wex.Status );
					if ( args.COMMType == CommArgs.CommType.GW )
					{
						ByteBuffer quickResponse = null;
						if ( httpResponse == null )
						{
							throw new CommException( CommException.Error.PTSDK_ERROR_GATEWAY,"Null response returned");
						}

						quickResponse = GetStreamMessage( httpResponse );
						if ( quickResponse == null || quickResponse.Bytes.Length == 0 )
						{
							throw new CommException( CommException.Error.PTSDK_ERROR_PROTOCOL ,"Null returned from GetStreamMessage()" , "Received unexpected blank response line!" );
						}

						if ( httpResponse.Headers.Keys.Count == 0 )
						{
							throw new CommException( CommException.Error.PTSDK_ERROR_PROTOCOL ,"No mime header" , "No mime-headers were read in the response!" );
						}

						int contentLength = 0;
						contentLength = StringToInt(httpResponse.Headers["content-length"]);
						if ( contentLength != quickResponse.Bytes.Length )
							throw new CommException( CommException.Error.PTSDK_ERROR_PROTOCOL , "Expected [" + contentLength + "] bytes, but received [" +quickResponse.Bytes.Length + "] bytes." );
						args.Response = ByteArrayToString( quickResponse.Array() );
						return args;
					}

				}
				if ( ex is SocketException )
					throw new CommException ( CommException.Error.PTSDK_ERROR_COMMUNICATION_READ_FAILURE ,"Exception while reading the response");

				throw ex;
			}
			args.Response = respStr;
			return args;
		}

		public static string ByteArrayToString(byte[] bytes)
		{
			System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
			return enc.GetString(bytes);
		}

		/// <summary>
		/// Get the external error out of the base WebException
		/// </summary>
		/// <param name="ex">original exception</param>
		/// <returns>spectrum error equivalent</returns>
		private WebExceptionStatus GetWebExceptionStatus( Exception ex )
		{
			WebExceptionStatus retval = WebExceptionStatus.Success;

			Exception innerEx = ex.InnerException;

			if ( innerEx == null )
			{
				innerEx = ex;
			}

			if ( innerEx is WebException )
			{
				WebException wex = ( WebException ) innerEx;

				retval = wex.Status;
			}
			else if ( ex is WebException )
			{
				WebException wex = ( WebException ) ex;

				retval = wex.Status;
			}
		
			return retval;
		}

		/// <summary>
		/// Create a HttpWebRequest object and initialize it
		/// </summary>
		/// <param name="hostname">host to connect to</param>
		/// <param name="portnum">port to connect to</param>
		/// <param name="proxyHost">proxy host (if there is one)</param>
		/// <param name="proxyPort">proxy port (if there is one)</param>
		/// <param name="proxyUser">user to authenticate to proxy</param>
		/// <param name="proxyPassword">password for authenticating to proxy</param>
		/// <returns>new HttpWebRequest object</returns>
		private HttpWebRequest GetHttpClient( string hostname, int portnum, 
			string proxyHost, int proxyPort, string proxyUser, 
			string proxyPassword , CommArgs args) 
		{
			Uri myUri = new Uri("https://" + hostname + ":" + portnum + 
				authorizationURI );

			engineLogger.Debug("URI is: " + myUri.ToString() );

			HttpWebRequest retval = (HttpWebRequest) WebRequest.Create(
				myUri );
	
			if ( proxyHost != null )
			{
                String proxyURI = String.Format("http://{0}:{1}", proxyHost, proxyPort);
                engineLogger.DebugFormat("Using proxy {0}", proxyURI);
				WebProxy proxy = new WebProxy( proxyURI, true );

				retval.Proxy = proxy;

				// Create a NetworkCredential object and associate it 
				// with the Proxy property of request object.
//				proxy.Credentials = new NetworkCredential( proxyUser, 
//					proxyPassword );
			}
				// We can't just leave the "Proxy" unset, if we don't set it to the
				// empty one, windows will use the Internet Explorer's database to
				// find one.
			else
			{
				retval.Proxy = GlobalProxySelection.GetEmptyWebProxy();
			}

			// The following property is obsolete
			// ReadWriteTimeout = ReadTimeOutSecs * 1000;

			retval.ServicePoint.Expect100Continue = false;

			if ( args.FirstInBatch )
				retval.KeepAlive = true ;
			else
				retval.KeepAlive = false;

			retval.AllowAutoRedirect = true;
			retval.ServicePoint.ConnectionLimit = MaxConnections;

			return retval;        
		}

		/// <summary>
		/// Combine partial reads to a single ByteBuffer
		/// </summary>
		/// <param name="httpResponse">response object</param>
		/// <returns>message returned from paymentech</returns>
		private ByteBuffer GetStreamMessage( HttpWebResponse httpResponse )
		{
			Queue blockList = new Queue();
			ByteBuffer retval = null;
			Stream istream = httpResponse.GetResponseStream();
			ulong total = 0;
			int numRead;

			do
			{
				// create a new buffer for each bunch of data
				byte [] inbuf = new byte[ inBufSize ];
			
				// try to read a full buffer full of data
				numRead = istream.Read( inbuf, 0, ( int ) inBufSize );

				// if we got some data, save it
				if ( numRead > 0 )
				{
					// if we did a partial read, then make a new 
					// (smaller) buffer for it
					if ( numRead < inBufSize )
					{
						// make new buffer smaller to fit less 
						// data
						byte [] newbuf = new byte[ numRead ];

						// copy bytes from larger buffer to new
						// smaller one
						for ( int i=0; i < numRead; i++ )
						{
							newbuf[ i ] = inbuf[ i ];
						}

						// put this new smaller buffer on the 
						// queue
						blockList.Enqueue( newbuf );
					}
					else // normal full buffer of data
					{
						blockList.Enqueue( inbuf );
					}

					// keep a running total
					total += ( ulong ) numRead;
				}
			} 
			while ( numRead > 0 ); // quit if read partial buffer

			// create a byte buffer that will hold all the data
			if ( total > 0 )
			{
				retval = new ByteBuffer( total );
		

				// make an iterator to go through the input blocks
				IEnumerator myEnumerator = blockList.GetEnumerator();

				// loop through the list and add each buffer to the ByteBuffer
				while ( myEnumerator.MoveNext() )
				{
					byte [] buf = ( byte[] ) myEnumerator.Current;
					retval.Put( buf, 0, buf.Length );
				}
			}
			if ( httpResponse.Headers["Connection"] != null )
				if ( httpResponse.Headers["Connection"].ToUpper().Equals("CLOSE"))
					istream.Close();
			return retval;
		}



		bool checkDataIntegerity (byte[] data)
		{
			bool checkPassed = false;
			if ( data != null )
			{
				int length = data.Length;
				short lrc=0;
				short lrcCompare = data[length -1];
				if ( length > 1 ) 
				{
					for( int idx = 0 ; idx < length ; idx++ )
						lrc ^= data[idx];
					checkPassed = ( lrc == lrcCompare );
				}
			}
			return checkPassed;
		}


		/// <summary>
		/// Remove any MIME headers with an empty value.
		/// </summary>
		/// <param name="httpRequest">Request containing the MIME headers</param>
		private void ValidateMimeHeaders(HttpWebRequest httpRequest)
		{
			ArrayList remove = new ArrayList();  // list of valueless headers

			foreach (String header in httpRequest.Headers)
			{
				String value = httpRequest.Headers[header];
				if (value == null || value.Trim().Length == 0)
					remove.Add(header);
			}

			foreach (String header in remove)
				httpRequest.Headers.Remove(header);
		}
		
		
		
		/// <summary>
		/// Put all the required fields in the mime header
		/// </summary>
		/// <param name="httpRequest">object that will send the request</param>
		/// <param name="sman">security info</param>
		/// <param name="contentType">special header field</param>
		/// <param name="contentLength">length of data</param>
		
		/// <summary>
		/// Special class that allows <VerifyHost>false</VerifyHost> in config.xml 
		/// which disables the checking
		/// of the host name in the server's certificate
		/// </summary>
		public class AcceptServerNameMismatch : ICertificatePolicy
		{
			// HACK: This is a workaround.  The .NET Framwork 1.1 should expose these, 
			// but they don't.
			//		public enum CertificateProblem : long
			//		{
			//			CertEXPIRED                   = 2148204801L,
			//			CertVALIDITYPERIODNESTING     = 2148204802L,
			//			CertROLE                      = 2148204803L,
			//			CertPATHLENCONST              = 2148204804,
			//			CertCRITICAL                  = 2148204805,
			//			CertPURPOSE                   = 2148204806,
			//			CertISSUERCHAINING            = 2148204807,
			//			CertMALFORMED                 = 2148204808,
			//			CertUNTRUSTEDROOT             = 2148204809,
			//			CertCHAINING                  = 2148204810,
			//			CertREVOKED                   = 2148204812,
			//			CertUNTRUSTEDTESTROOT         = 2148204813,
			//			CertREVOCATION_FAILURE        = 2148204814,
			//			CertCN_NO_MATCH               = 2148204815,
			//			CertWRONG_USAGE               = 2148204816,
			//			CertUNTRUSTEDCA               = 2148204818
			//		}

			/// <summary>
			/// Implement CheckValidationResult to ignore problems that we 
			/// are willing to accept.
			/// </summary>
			public bool CheckValidationResult(ServicePoint sp, X509Certificate cert,
				WebRequest request, int problem)
			{  
				const int CertificateNameDoesntMatch = unchecked( 
						  ( int ) 2148204815 );
				// only accept server name failed match
				if ( problem == CertificateNameDoesntMatch ) 
					return true;

				// The 1.1 framework calls this method with a problem of 0, 
				// even if nothing is wrong
				return (problem == 0);         
			} 

		}
	}

}  // end of namespace


