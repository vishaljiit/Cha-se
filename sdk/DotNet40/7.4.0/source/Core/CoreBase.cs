using System;
using System.Text;
using log4net;
using Paymentech.Comm;
using Paymentech.COM;

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
    /// Provides helper methods used by all of the core classes.
    /// </summary>
    [Serializable]
    public class CoreBase
    {
		/// <summary>
		/// The factory object for all classes.
		/// </summary>
        protected IFactory factory = null;
        private ILog logger = null;

        protected int ToInt(string text)
        {
            return HTTPSConnect.StringToInt(text);
        }

        protected IConfigurator Config
        {
            get { return factory.Config; }
        }

		/// <summary>
		/// Gets the engine logger.
		/// </summary>
        protected ILog Logger
        {
            get
            {
                if (logger == null)
                    logger = factory.EngineLogger;
                return logger;
            }
        }

        /// <summary>
        /// Determines if a string is empty. Returns true if the value is null or empty.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>Returns true if the value is null or empty.</returns>
        protected bool IsEmpty(string text)
        {
            return (text == null || text.Trim().Length == 0);
        }

        protected void SetDefaultFieldValue(ITransaction transaction, string name)
        {
            if (!transaction.ItemExists(name))
                return;

            if (IsEmpty(transaction[name]) && !IsEmpty(factory.Config[name]))
            {
                transaction[name] = factory.Config[name];
            }
        }
    }
}
