using System;
using System.IO;
using System.Xml;
using System.Text;
using log4net.Config;
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
	/// The factory class for creating various objects.
	/// </summary>
    public class Factory : IFactory
    {
        private ILog alternateEngineLogger = null;
        private ILog alternateECommerceLogger = null;

        #region IRequestFactory Members

        /// <summary>
        /// Gets the specified environment variable value.
        /// </summary>
        /// <param name="variable">The name of the variable to look up.</param>
        /// <returns>The value of the variable.</returns>
        public string GetEnvironmentVariable(string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }

        /// <summary>
        /// Loads an XML file into a document object.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>An XML document.</returns>
        public XmlDocument LoadXml(string path)
        {
            XmlDocument document = new XmlDocument();
            document.Load(path);
            return document;
        }

        /// <summary>
        /// Configures logging.
        /// </summary>
        /// <remarks>
        /// In service mode, we must loosen up locking of the log files because we'll 
        /// have the DLL and Service writing to them.
        /// </remarks>
        /// <param name="loggingConfig"></param>
        public void ConfigureAndWatchLogger(string loggingConfig)
        {
            if (Config["DotNet.Transaction.EngineType"].ToLower().Trim() == "dll")
            {
                XmlConfigurator.ConfigureAndWatch(new FileInfo(loggingConfig));
                return;
            }

            Filer filer = new Filer();
            string xml = filer.ReadAllText(loggingConfig);
            xml = UncommentLockingModel(xml);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlConfigurator.Configure((XmlElement)doc.GetElementsByTagName("log4net")[0]);
        }

        private string UncommentLockingModel(string orig)
        {
            string xml = orig;
            for (int start = xml.IndexOf("lockingModel"); start != -1; start = xml.IndexOf("lockingModel", start))
            {
                start = xml.LastIndexOf("<!--", start);
                if (start == -1)
                    return orig;
                xml = xml.Substring(0, start) + xml.Substring(start + 4);
                start = xml.IndexOf("-->", start);
                if (start == -1)
                    return orig;
                xml = xml.Substring(0, start) + xml.Substring(start + 3);
            }
            return xml;
        }

        /// <summary>
        /// Gets or sets the logger used for engine logging.
        /// </summary>
        public ILog EngineLogger
        {
            get
            {
                if (alternateEngineLogger != null)
                    return alternateEngineLogger;

                return LogManager.GetLogger("EngineLogger");
            }
            set
            {
                alternateEngineLogger = value;
            }
        }

        /// <summary>
        /// Gets or sets the logger used for transaction logging.
        /// </summary>
        public ILog ECommerceLogger
        {
            get
            {
                if (alternateECommerceLogger != null)
                    return alternateECommerceLogger;

                return LogManager.GetLogger("ECommerceLogger");
            }
            set
            {
                alternateECommerceLogger = value;
            }
        }

        /// <summary>
        /// Creates a new Filer object.
        /// </summary>
        /// <returns></returns>
        public IFiler MakeFiler()
        {
            return new Filer();
        }

        /// <summary>
        /// Gets the singleton Configurator object.
        /// </summary>
        public IConfigurator Config
        {
            get { return Configurator.Instance; }
        }

        /// <summary>
        /// Creates a new CommManager object.
        /// </summary>
        public ICommManager CommMgr
        {
            get { return new CommManager(); }
        }

        public IConfigurator GetCustomInstance(string paymentechHome)
        {
            return Configurator.GetCustomInstance(paymentechHome);
        }

        public IHTTPSConnect MakeHTTPSConnect()
        {
            return new Paymentech.Comm.HTTPSConnect();
        }

        #endregion
    }
}
