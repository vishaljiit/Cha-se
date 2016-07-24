using System;
using System.Text;
using Paymentech.COM;
using System.EnterpriseServices;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Reflection;
using Paymentech.Core;
using log4net;


namespace Paymentech
{
	/// <summary>
	/// A Paymentech Orbital Transasction Request
	/// </summary>
	/// <remarks>
	/// <para>
	/// The Transaction class encapsulates the inner workings of the SSL 
	/// communication engine in order to provide an easy-to-use interface 
	/// for creating, sending, and evaluating the results of a secure 
	/// electronic payment transaction. 
	/// </para>
	/// </remarks>
	[Guid("B7C639DC-ECB4-41e7-AD68-E411343D15E7")]
	[ClassInterface(ClassInterfaceType.None)]
	[ProgId("Paymentech.Transaction")]
    public class Transaction : CoreBase, Paymentech.COM.ITransaction, IDisposable
	{
		/// <summary>
		/// The request for the transaction.
		/// </summary>
        protected Request request = null;
       
		/// <summary>
		/// Default constructor. Any class derived from Transaction will use this
		/// constructor.
		/// </summary>
        /// <exclude />
        public Transaction()
		{
            factory = new Factory();
		}

		/// <summary>
		/// Constructs a Transaction object
		/// </summary>
		/// <remarks>
		/// The transaction constructor requires a valid transaction type.  Valid 
		/// transaction types are defined by the <see cref="RequestType"/> class.
		/// </remarks>
		/// <param name="transType">The transaction type.</param>
		public Transaction(string transType)
		{
            try
            {
                factory = new Factory();
                request = new Request(transType, false, factory);
                IConfigurator config = Configurator.Instance;
            }
            catch (Exception ex)
            {
                throw MakeError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Constructs a Transaction object
        /// </summary>
        /// <remarks>
        /// The transaction constructor requires a valid transaction type.  Valid 
        /// transaction types are defined by the <see cref="RequestType"/> class.
        /// </remarks>
        /// <param name="transType"><para>The transaction type.</para></param>
        /// <param name="paymentechHome">
        /// <para>
        /// The path to the install directory. This optional parameter can be used to 
        /// specify the location of the SDK when you cannot set the PAYMENTECH_HOME
        /// environment variable. 
        /// </para>
        /// </param>
        public Transaction(string paymentechHome, string transType)
            : this(paymentechHome, transType, new Factory())
        {
        }

        /// <summary>
        /// Constructs a Transaction object
        /// </summary>
        /// <remarks>
        /// NOTE: This constructor should never be used. It is for testing purposes only.
        /// </remarks>
        /// <exclude />
        /// <param name="transType">The transaction type.</param>
        /// <param name="paymentechHome">The path to the install directory.</param>
        /// <param name="factory">The </param>
        public Transaction(string paymentechHome, string transType, IFactory factory)
        {
            try
            {
                this.factory = factory;
                IConfigurator config = factory.GetCustomInstance(paymentechHome);
                request = new Request(transType, false, factory);
            }
            catch (Exception ex)
            {
                throw MakeError(ex, ex.Message);
            }
        }

        /// <summary>
        /// This is only used to maintain consistency of behavior
        /// between this version of the SDK and previous versions.
        /// This will ensure that merchants upgrading from previous
        /// versions will not require code changes during upgrade.
        /// </summary>
        ~Transaction()
        {
            request = null;
        }

        /// <summary>
        /// Gets or sets the directory path to where the SDK was installed.
        /// </summary>
        /// <remarks>
        /// The best way to change the location of the SDK is by passing the 
        /// path to it in Transaction's constructor.
        /// </remarks>
        public string PaymentechHome
        {
            get 
            { 
                try 
                {
                    return Configurator.Instance.PaymentechHome;
                }
                catch (Exception ex)
                {
                    throw MakeError(ex, ex.Message);
                }
            }
            set
            {
                try 
                {
                    IConfigurator config = factory.GetCustomInstance(value);
                }
                catch (Exception ex)
                {
                    throw MakeError(ex, ex.Message);
                }
            }
        }

        /// <summary>
        /// Gets the contents of the request in XML format. This is the data that is sent to the Gateway.
        /// </summary>
        /// <remarks>
        /// <para>
        /// XML returns the XML of the request as it is to be sent to the Gateway. No formatting is 
        /// included, so printing it out will not have newlines. All values that you set in your code
        /// will be included in the XML, as will any default values.
        /// </para>
        /// <para>
        /// This property is read-only.
        /// </para>
        /// </remarks>
        /// <exception cref="System.Exception">
        /// An exception will be thrown if any required fields have not yet been set. You must be sure
        /// to set all necessary fields before calling using this property.
        /// </exception>
        /// <returns>A string containing the request in XML format.</returns>
        public string XML 
		{
			get
			{
                try
                {
                    return request.XML;
                }
                catch (Exception ex)
                {
                    throw MakeError(ex, ex.Message);
                }
            }
		}

		/// <summary>
		/// Returns true if the field exists.
		/// </summary>
		/// <param name="itemName">The name of the field to be tested.</param>
		/// <returns>true if the field exists, false if it does not.</returns>
        public bool ItemExists(string itemName)
        {
            return request.ItemExists(itemName);
        }

		/// <summary>
		/// Gets the contents of the request in XML format. This is the data that is sent to the Gateway.
		/// </summary>
        /// <remarks>
        /// asXML returns the XML of the request as it is to be sent to the Gateway. No formatting is 
        /// included, so printing it out will not have newlines. All values that you set in your code
        /// will be included in the XML, as will any default values.
        /// </remarks>
        /// <exception cref="System.Exception">
        /// An exception will be thrown if any required fields have not yet been set. You must be sure
        /// to set all necessary fields before calling this method.
        /// </exception>
		/// <returns>A string containing the request in XML format.</returns>
		public string asXML()
		{
            return XML;
		}


		/// <summary>
		/// Determines whether the transaction is valid.
		/// </summary>
		/// <returns>
		/// Returns true if the transaction was properly constructed and is valid,
		/// false otherwise.
		/// </returns>
		public bool IsValid 
		{
            get { return (request != null);  }
		}
		
		/// <summary>
		/// The transaction type of the transaction
		/// </summary>
		public string TransactionType 
		{
            get 
            {
                try
                {
                    if (request == null)
                        return null;
                    return request.TransactionType;
                }
                catch (Exception ex)
                {
                    throw MakeError(ex, ex.Message);
                }
            }
		}

        /// <summary>
        /// Gets the transaction type of the transaction.
        /// </summary>
        /// <returns>The name of the transaction type used for this transaction.</returns>
		public string Type
		{
            get 
            {
                return TransactionType;
            }
			set 
			{
                try
                {
                    IConfigurator config = Configurator.Instance;
                    request = new Request(value);
                }
                catch (Exception ex)
                {
                    throw MakeError(ex, ex.Message);
                }
            }
		}

        /// <summary>
        /// Sets a field in the transaction's request.
        /// </summary>
        /// <remarks>
        /// Throws an exception if itemName does not refer to an element 
        /// in the template.
        /// </remarks>
        /// <param name="itemName">The name of the field to be set.</param>
        /// <param name="itemValue">The value to set the field to.</param>
        public void setField(string itemName, string itemValue)
		{
			this[itemName] = itemValue;
		}

		/// <summary>
		/// Gets or sets the value of the specified field.
		/// </summary>
		/// <remarks>
		/// Throws an exception if itemName does not refer to an element 
		/// in the template.
		/// </remarks>
        /// <param name="itemName">The name of the field being referenced.</param>
        /// <returns>The value of the field.</returns>
		public string this [string itemName]
		{
			get 
			{
				if (request == null)
                    throw MakeError("Transaction Object is invalid");
                try
                {
                    return request[itemName];
                }
                catch (Exception ex)
                {
                    throw MakeError(ex, ex.Message);
                }
            }
			set
			{
				if (request == null)
                    throw MakeError("Transaction Object is invalid");

                try
                {
                    request[itemName] = value;
                }
                catch (Exception ex)
                {
                    throw MakeError(ex, ex.Message);
                }
            }
		}

		/// <summary>
		/// Processes a transaction
		/// </summary>
		/// <remarks>
		/// <para>Process () submits a transaction to the Orbital Gateway and 
		/// retrieves a <see cref="Paymentech.Response"/> object.
        /// </para>
        /// <para>
        /// NOTE: You should never call this method from a TransactionElement object. This is for use only by Transaction.
        /// </para>
		/// </remarks>
        /// <returns>A <see cref="Paymentech.Response"/> object that contains the reply from the Orbital Gateway.</returns>
		public Response Process ()
		{
            try
            {
                // Get the transaction processor from the factory
                ITransactionProcessor processor = TransactionProcessorFactory.GetProcessor(factory);
                factory.EngineLogger.InfoFormat("Processing the transaction \"{0}\"...", TransactionType);

                ILog logger = factory.ECommerceLogger;

                if (logger.IsDebugEnabled)
                    logger.DebugFormat("Request {0} ==> {1}", TransactionType, request.MaskedXML);

                IResponse response = processor.Process(this);

                if (logger.IsDebugEnabled)
                    logger.DebugFormat("Response {0} ==> {1}", TransactionType, response.MaskedXML);

                return (Response)response;
            }
            catch (Exception ex)
            {
                throw MakeError(ex, ex.Message);
            }
        }


        /// <summary>
        /// Gets a complex format for adding additional data to the request.
        /// </summary>
        /// <param name="name">The name of the complex root's template.</param>
        /// <returns>The transaction element for entering the format's fields.</returns>
        public TransactionElement GetComplexRoot(string name)
		{
            try
            {
                Logger.DebugFormat("Getting Complex Root [{0}].", name);
                Request root = request.GetComplexRoot(name);
                TransactionElement transElement = new TransactionElement(root);
                return transElement;
            }
            catch (Exception ex)
            {
                throw MakeError(ex, ex.Message);
            }
        }

		/// <summary>
		/// Releases all allocated resources
		/// </summary>
		/// <remarks>
		/// The Dispose () method allows the developer to release resources 
		/// allocated by the Transaction object without waiting for the 
		/// garbage collector.  After calling Dispose () the Transaction object 
		/// becomes invalid and can not be used.
		/// </remarks>
		public  void Dispose ()
		{
            Logger.Info("Transaction.Dispose is deprecated. You no longer need to call it.");
            request = null;
		}

        internal Request Request
        {
            get { return request; }
        }

        /// <summary>
        /// Log and throw an exception.
        /// </summary>
        /// <param name="format">The format string for the error message.</param>
        /// <param name="args">Parameters to go into the message.</param>
        /// <returns>The exception to be thrown.</returns>
        private Exception MakeError(string format, params object[] args)
        {
            string message = String.Format(format, args);
            Logger.Error(message);
            return new Exception(message);
        }

        /// <summary>
        /// Log and throw an exception.
        /// </summary>
        /// <param name="ex">The original exception.</param>
        /// <param name="format">The format string for the error message.</param>
        /// <param name="args">Parameters to go into the message.</param>
        /// <returns>The exception to be thrown.</returns>
        private Exception MakeError(Exception ex, string format, params object[] args)
        {
            string message = String.Format(format, args);
            Logger.Error(message, ex);
            return new Exception(message, ex);
        }
    }
}
