using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using log4net;
using log4net.Config;
using log4net.Appender;

namespace Paymentech.Core
{
	/// <summary>
	/// Manages the configuration file and loggers.
	/// </summary>
    public class Configurator : CoreBase, IConfigurator
    {
        private static Configurator config = null;
        private string paymentechHome = null;
        private string configFile = null;
        private string loggingConfigFile = null;
        private Hashtable properties = null;
        private ILog engineLogger = null;

        /// <summary>
        /// Default constructor. Used to construct the singleton object.
        /// </summary>
        public Configurator() : this(new Factory())
        {
        }

        /// <summary>
        /// Used to construct a Configurator object for unit testing.
        /// </summary>
        /// <param name="factory">An instance to a custom factory object. Typically a stub or mock.</param>
        public Configurator(IFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Creates the singleton instance of the Configurator and returns it to the caller.
        /// </summary>
        public static IConfigurator Instance
        {
            get
            {
                if (config == null)
                {
                    config = new Configurator();
                    config.Load();
                }
                return config;
            }
        }

        /// <summary>
        /// Initializes the Configurator with a custom paymentech home directory.
        /// </summary>
        /// <remarks>
        /// It first tries the specified path and will revert to the normal home 
        /// directory algorithm if it is not valid.
        /// </remarks>
        /// <param name="paymentechHome">The custom path to the paymentech home directory.</param>
        /// <returns>The singleton Configurator instance.</returns>
        public static IConfigurator GetCustomInstance(string paymentechHome)
        {
            IFiler filer = new Factory().MakeFiler();

            if (!filer.DirectoryExists(paymentechHome))
                return Configurator.Instance;

            if (!filer.Exists(Path.Combine(paymentechHome, Path.Combine("etc", "linehandler.properties"))))
                return Configurator.Instance;

            config = new Configurator();
            config.PaymentechHome = paymentechHome;
            config.Load();
            return config;
        }

        private void ConfigureLogging()
        {
            loggingConfigFile = factory.GetEnvironmentVariable("PAYMENTECH_LOGCONFIGFILENAME");
            if (loggingConfigFile == null || !File.Exists(loggingConfigFile))
            {
                if (!IsEmpty(paymentechHome))
                {
                    loggingConfigFile = Path.Combine(paymentechHome, "etc\\logging.xml");
                }
                
                if (IsEmpty(loggingConfigFile) || !File.Exists(loggingConfigFile))
                {
                    loggingConfigFile = Path.GetDirectoryName(configFile);
                    loggingConfigFile = Path.Combine(loggingConfigFile, "logging.xml");
                }
            }
            factory.ConfigureAndWatchLogger(loggingConfigFile);

            engineLogger = factory.EngineLogger;
            IFiler filer = factory.MakeFiler();
            string logdir = factory.GetEnvironmentVariable("PAYMENTECH_LOGDIR");
            if (logdir != null)
                return;

            if (logdir == null || logdir.Trim().Length == 0)
            {
                logdir = Path.Combine(PaymentechHome, "logs");
            }
            if (!filer.DirectoryExists(logdir))
            {
                logdir = Path.GetDirectoryName(configFile);
                logdir = Path.Combine(logdir, "..\\logs");
            }
            if (!filer.DirectoryExists(logdir))
            {
                logdir = Path.GetTempPath();
            }

            // Set the logging directory.
            foreach (IAppender app in LogManager.GetRepository().GetAppenders())
            {
                if (app is FileAppender && (app.Name.Equals("Engine") || app.Name.Equals("ECommerce")))
                {
                    FileAppender fileApp = (FileAppender)app;
                    FileInfo file = new FileInfo(fileApp.File);
                    fileApp.File = Path.Combine(logdir, file.Name);
                    fileApp.ActivateOptions();
                }
            }
        }

        private string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        }

        private void LogInitialConfiguration(string configFileContents)
        {
            engineLogger.Info("************ New Configurator created *************");
            engineLogger.InfoFormat("Assembly version = {0}", this["SDKVersion"]);
            engineLogger.InfoFormat("Configurator log config  file = {0}", loggingConfigFile);
            engineLogger.InfoFormat("Configurator configuration file = {0}", configFile);
            engineLogger.Info("Configuration Information");
            engineLogger.Info("-------------------------");
            engineLogger.Info(configFileContents);
            engineLogger.Info("End of Configuration Information");
            engineLogger.Info("-------------------------");

        }

        private string FindPaymentechHome()
        {
            if (paymentechHome != null && DirectoryHasSDK(paymentechHome))
                return paymentechHome;

            string home = factory.GetEnvironmentVariable("PAYMENTECH_HOME");
            if (DirectoryHasSDK(home))
                return home;

            string pathVar = factory.GetEnvironmentVariable("PATH");
            if (pathVar == null)
            {
                if (DirectoryHasSDK("."))
                    return ".";
                return null;
            }

            string[] paths = pathVar.Split(';');

            foreach (string path in paths)
            {
                if (DirectoryHasSDK(path))
                    return path;
            }

            string location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Console.WriteLine("Location = " + location);
            if (DirectoryHasSDK(location))
                return location;

            return null;
        }

        private bool DirectoryHasSDK(string directory)
        {
            if (directory == null)
                return false;
            configFile = Path.Combine(directory, "etc\\linehandler.properties");
            IFiler filer = factory.MakeFiler();
            bool found = filer.Exists(configFile);
            if (found)
                return found;

            configFile = null;
            return filer.Exists(Path.Combine(directory, "xml\\NewOrder.xml"));
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

        #region IConfigurator Members

        /// <summary>
        /// Determines the path to paymentech home and loads the linehandler.properties file.
        /// </summary>
        public void Load()
        {
            properties = new Hashtable();
            paymentechHome = FindPaymentechHome();

            IFiler filer = factory.MakeFiler();
            string configPath = factory.GetEnvironmentVariable("PAYMENTECH_CONFIGFILENAME");
            if (configPath != null && File.Exists(configPath))
                configFile = configPath;

            if (paymentechHome == null)
                throw MakeError("Failed to find the location of the SDK. Ensure that PAYMENTECH_HOME environment variable is set.");
            if (configFile == null)
                throw MakeError("Failed to find linehandler.properties. Ensure that PAYMENTECH_HOME environment variable is set.");

            string text = filer.ReadAllText(configFile);

            string[] lines = text.Split('\n');
            foreach (string theLine in lines)
            {
                string line = theLine.Trim();
                if (line.StartsWith("#") || line.Length == 0)
                    continue;

                string[] pair = line.Split('=');
                if (pair.Length == 1)
                    properties.Add(pair[0].Trim(), "");
                else
                {
                    string val = pair[1].Trim().Replace("%PAYMENTECH_HOME%", paymentechHome);
                    properties.Add(pair[0].Trim(), val);
                }
            }

            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            properties.Add("SDKVersion", version.ToString());

            ConfigureLogging();

            LogInitialConfiguration(text);
        }

        /// <summary>
        /// Gets a specified property value.
        /// </summary>
        /// <param name="index">The property whose value is to be returned.</param>
        /// <returns>The property's value, or null if no property exists.</returns>
        public string this[string index]
        {
            get 
            {
                if (properties == null || !properties.ContainsKey(index))
                    return null;
                return (string)properties[index];
            }
        }

        /// <summary>
        /// Gets an integer version of a property value
        /// </summary>
        /// <param name="index">The name of the property to get.</param>
        /// <returns></returns>
        public int GetInt(string index)
        {
            if (properties == null || !properties.ContainsKey(index))
                return 0;

            try
            {
                return Convert.ToInt32((string)properties[index]);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the path to directory where the SDK is installed.
        /// </summary>
        public string PaymentechHome
        {
            get { return paymentechHome;  }
            set { paymentechHome = value; }
        }

		/// <summary>
		/// Gets a boolean property value from the properties file. 
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
        public bool GetBool(string property)
        {
            string val = this[property];
            return (val != null) ? (val.Trim().ToLower() == "true") : false;
        }

        /// <summary>
        /// Gets or sets the logger used for engine logging.
        /// </summary>
        public ILog EngineLogger 
        {
            get { return factory.EngineLogger; }
            set { factory.EngineLogger = value; }
        }

        /// <summary>
        /// Gets or sets the logger used for transaction logging.
        /// </summary>
        public ILog ECommerceLogger 
        {
            get { return factory.ECommerceLogger; }
            set { factory.ECommerceLogger = value; }
        }

        /// <summary>
        /// Gets a list of all the properties in the properties file.
        /// </summary>
        /// <param name="filter">A wildcard-based filter to retrieve a subset of properties.</param>
        /// <returns>A list of properties.</returns>
        public string[] GetPropertyNames(string filter)
        {
            string pattern = WildcardToRegex(filter);
            Regex regex = new Regex(pattern);

            ArrayList list = new ArrayList();
            foreach (string property in properties.Keys)
            {
                if (regex.IsMatch(property))
                    list.Add(property);
            }

            return (string[])list.ToArray(typeof(string));
        }
        #endregion
    }
}
