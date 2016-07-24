using System;
using System.Xml;
using System.Text;
using log4net;
using Paymentech.Comm;

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
	/// 
	/// </summary>
    public interface IFactory
    {
        /// <summary>
        /// Gets the specified environment variable value.
        /// </summary>
        /// <param name="variable">The name of the variable to look up.</param>
        /// <returns>The value of the variable.</returns>
        string GetEnvironmentVariable(string variable);
        /// <summary>
        /// Loads an XML file into a document object.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>An XML document.</returns>
        XmlDocument LoadXml(string path);
        /// <summary>
        /// Creates a new Filer object.
        /// </summary>
        /// <returns></returns>
        IFiler MakeFiler();
        /// <summary>
        /// Gets the singleton Configurator object.
        /// </summary>
        IConfigurator Config { get; }
        /// <summary>
        /// Creates a new CommManager object.
        /// </summary>
        ICommManager CommMgr { get; }
        /// <summary>
        /// Configures logging.
        /// </summary>
        /// <param name="loggingConfig"></param>
        void ConfigureAndWatchLogger(string loggingConfig);
        /// <summary>
        /// Gets or sets the logger used for engine logging.
        /// </summary>
        ILog EngineLogger { get; set; }
        /// <summary>
        /// Gets or sets the logger used for transaction logging.
        /// </summary>
        ILog ECommerceLogger { get; set; }

        IConfigurator GetCustomInstance(string paymentechHome);
        IHTTPSConnect MakeHTTPSConnect();
    }
}
