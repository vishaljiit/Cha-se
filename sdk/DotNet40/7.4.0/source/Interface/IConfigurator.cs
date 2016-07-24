using System;
using System.Text;
using log4net;

namespace Paymentech.Core
{
	/// <summary>
	/// 
	/// </summary>
    public interface IConfigurator
    {
        /// <summary>
        /// Determines the path to paymentech home and loads the linehandler.properties file.
        /// </summary>
        void Load();
        /// <summary>
        /// Gets a specified property value.
        /// </summary>
        /// <param name="index">The property whose value is to be returned.</param>
        /// <returns>The property's value, or null if no property exists.</returns>
        string this[string index] { get; }
        /// <summary>
        /// Gets or sets the path to directory where the SDK is installed.
        /// </summary>
        string PaymentechHome { get; set; }
        /// <summary>
        /// Gets or sets the logger used for engine logging.
        /// </summary>
        ILog EngineLogger { get; set; }
        /// <summary>
        /// Gets or sets the logger used for transaction logging.
        /// </summary>
        ILog ECommerceLogger { get; set; }
        /// <summary>
        /// Gets a list of all the properties in the properties file.
        /// </summary>
        /// <param name="filter">A wildcard-based filter to retrieve a subset of properties.</param>
        /// <returns>A list of properties.</returns>
        string[] GetPropertyNames(string filter);

        /// <summary>
        /// Gets a boolean property value.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        bool GetBool(string property);

        /// <summary>
        /// Gets an integer property value.
        /// </summary>
        /// <param name="index">The property whose value to get.</param>
        /// <returns></returns>
        int GetInt(string index);
    }
}
