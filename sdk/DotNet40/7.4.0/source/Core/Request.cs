using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace Paymentech.Core
{
    /// <summary>
    /// Wraps all of the transaction information that the merchant sends to the server.
    /// </summary>
    public class Request : TemplateBase
    {
        private string transactionType = null;
        private string deprecatedTransactionType = null;
        private Hashtable fields = new Hashtable();
        private ArrayList setFields = new ArrayList();
        private Hashtable complexRoots = new Hashtable();
        private Hashtable recursiveElements = new Hashtable();
        private ArrayList requiredFields = new ArrayList();
        private string templateXML = null;
        private bool isInclude = false;

        private class RecursiveElement
        {
            public string name = null;
            public string countElement = null;
            public int maxElements = 0;
            public string childIndexElement = null;
            public string enforceGreaterThanZero = null;
            public ArrayList elements = new ArrayList();
        }

		/// <summary>
		/// Constructor that takes a transaction type.
		/// </summary>
		/// <param name="transactionType"></param>
        public Request(string transactionType)
            : this(transactionType, false, new Factory())
        {
        }

		/// <summary>
		/// Constructor used mostly for creating additional formats.
		/// </summary>
		/// <param name="transactionType"></param>
		/// <param name="isInclude"></param>
        public Request(string transactionType, bool isInclude)
            : this(transactionType, isInclude, new Factory())
        {
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="transactionType"></param>
		/// <param name="isInclude"></param>
		/// <param name="factory"></param>
        public Request(string transactionType, bool isInclude, IFactory factory)
        {
            this.isInclude = isInclude;
            this.factory = factory;
            this.setFields.Clear();

            // For backward compatibility with pre-PTI40 templates, if an old transaction type
            // is passed to this constructor, get the new transaction type that corresponds to it.
            TransactionMapper pre40Mapper = new TransactionMapper();
            if (pre40Mapper.IsPre40(transactionType))
            {
                deprecatedTransactionType = transactionType;
                this.transactionType = pre40Mapper.GetMappedTransactionType(transactionType);
                Logger.DebugFormat("The transaction type [{0}] is obsolete. Using [{1}] instead.", transactionType, this.transactionType);
            }
            else
                this.transactionType = transactionType;

            string path = GetTemplatePath();

            Logger.DebugFormat("Loading template {0}.", path);

            if (isInclude)
            {
                ParseIncludeTemplate(path);
                return;
            }

            templateXML = factory.MakeFiler().ReadAllText(path);
            XmlDocument template = new XmlDocument();
            template.LoadXml(templateXML);
            ParseTemplate(template, isInclude);

            // For backward compatibility, make sure the default values for certain
            // fields are set, based on a specific mapping scheme defined in TransactionMapper.
            if (pre40Mapper.IsPre40(transactionType))
                pre40Mapper.MapPre40Values(this, transactionType);
        }

        private string GetTemplatePath()
        {
            string path = Config[String.Format("XTTF.Request.{0}", transactionType)];
            
            IFiler filer = factory.MakeFiler();
            if (!IsEmpty(path))
            {
                if (filer.Exists(path))
                    return path;
            }

            // Build the path to the file.
            Logger.Debug("Template path not found in linehandler.properties. Using default path.");
            path = Path.Combine(Config.PaymentechHome, "xml");
            if (isInclude)
                path = CombinePaths(path, "templates", transactionType + ".inc");
            else 
                path = Path.Combine(path, String.Format("{0}.xml", transactionType));

            if (filer.Exists(path))
                return path;

            throw MakeError("The template [{0}] could not be found.", transactionType);
        }

        #region Public Methods

        /// <summary>
        /// Determines if the field has been set.
        /// </summary>
        /// <param name="itemName">The name of the field to test.</param>
        /// <returns>True if the field has been set, false if it has not.</returns>
        public bool IsFieldSet(string itemName)
        {
            return setFields.Contains(itemName);
        }

        /// <summary>
        /// The type of transaction being processed.
        /// </summary>
        public string TransactionType
        {
            get 
            { 
                if (deprecatedTransactionType == null)
                    return transactionType;
                return deprecatedTransactionType;
            }
        }

        /// <summary>
        /// Always gets the actual transaction type to be used, 
        /// and never the deprecated one.
        /// </summary>
        internal string FinalTransactionType
        {
            get
            {
                return transactionType;
            }
        }

        /// <summary>
        /// Determines if the specified field is required. 
        /// </summary>
        /// <remarks>
        /// Some fields must have a value before being sent to the server. 
        /// This method will test if the field is required.
        /// </remarks>
        /// <param name="fieldName">The name of the field.</param>
        /// <returns></returns>
        public bool IsRequired(string fieldName)
        {
            return requiredFields.Contains(fieldName);
        }


        /// <summary>
        /// Gets or sets the value of a field.
        /// </summary>
        /// <param name="itemName">The name of the field to get or set.</param>
        /// <returns>The field's value.</returns>
        public string this[string itemName]
        {
            get
            {
                if (fields.ContainsKey(itemName))
                    return (string) fields[itemName];

                if (!Config.GetBool("skipFieldNotFoundExceptions"))
                    throw MakeError("The field \"{0}\" does not exist.", itemName);

                // If skipFieldNotFoundExceptions is true, just log a warning and return an empty string.
                // This is for backward compatibility with pre-PTI40 requests. 
                Logger.WarnFormat("The field \"{0}\" does not exist.", itemName);
                return "";
            }
            set
            {
                if (fields.ContainsKey(itemName))
                {
                    fields[itemName] = value;
                    if (!setFields.Contains(itemName))
                        setFields.Add(itemName);
                }
                else
                    throw MakeError("The field \"{0}\" does not exist.", itemName);
            }
        }

        /// <summary>
        /// Gets the value of the specified field.
        /// </summary>
        /// <remarks>
        /// This is used by some languages that do not support indexers, such as COM clients.
        /// </remarks>
        /// <param name="itemName">The name of the field to look up.</param>
        /// <returns></returns>
        public string GetField(string itemName)
        {
            return this[itemName];
        }

        /// <summary>
        /// Sets the value of the specified field.
        /// </summary>
        /// <remarks>
        /// This is used by some languages that do not support indexers, such as COM clients.
        /// </remarks>
        /// <param name="itemName">The name of the field to set.</param>
        /// <param name="itemValue">The value of the field.</param>
        public void SetField(string itemName, string itemValue)
        {
            this[itemName] = itemValue;
        }

		/// <summary>
		/// Returns true if the field exists in the template.
		/// </summary>
		/// <param name="itemName"></param>
		/// <returns></returns>
        public bool ItemExists(string itemName)
        {
            return fields.ContainsKey(itemName);
        }

        /// <summary>
        /// Gets a complex format for adding additional data to the request.
        /// </summary>
        /// <returns>The request for entering the format's fields.</returns>
        public Request GetComplexRoot(string name)
        {
            if (complexRoots.ContainsKey(name))
                throw MakeError("The Complex Root \"{0}\" already exists.", name);

            Request root = new Request(name, true, factory);
            RecursiveElement element = new RecursiveElement();
            element.name = name;
            element.enforceGreaterThanZero = Config[String.Format("XTTF.Request.RecursiveElement.{0}.EnforceGreaterThanZero", name)];
            element.elements.Add(root);
            complexRoots.Add(name, element);

            return root;
        }

        /// <summary>
        /// Gets an instance of a list of like formats for adding multiple sets of data in one transaction.
        /// </summary>
        /// <returns>The request for entering the format's fields.</returns>
        public Request GetRecursiveElement(string name)
        {
            Request element = new Request(name, true, factory);
            if (recursiveElements.ContainsKey(name))
            {
                RecursiveElement data = (RecursiveElement)recursiveElements[name];
                if (data.elements.Count == data.maxElements)
                {
                    throw MakeError("Complex root [{0}] can have a maximum of [{1}] recursive elements. [{2}] have been defined.",
                        transactionType, data.maxElements, name);
                }
                if (data.childIndexElement != null)
                    element[data.childIndexElement] = (GetIntField(data.countElement) + 1).ToString();
                if (data.countElement != null)
                    this[data.countElement] = IncrementFieldValue(this[data.countElement]);
                data.elements.Add(element);
            }
            else
            {
                RecursiveElement data = GetRecursiveData(name);
                data.elements.Add(element);
                if (data.countElement != null)
                    this[data.countElement] = "1";
                if (data.childIndexElement != null)
                    element[data.childIndexElement] = "1";
                recursiveElements.Add(name, data);
            }
            return element;
        }

        /// <summary>
        /// Gets the value of the specified field and converts it to an integer.
        /// </summary>
        /// <param name="name">The name of the field to look up.</param>
        /// <returns>An integer version of the field's value.</returns>
        public int GetIntField(string name)
        {
            string val = this[name];
            if (!IsNumeric(val))
                return 0;

            return Convert.ToInt32(val);
        }

        /// <summary>
        /// Returns an XML representation of the Request.
        /// </summary>
        public string XML
        {
            get
            {
                VerifyRequiredFormats();
                string xml = templateXML;
                foreach (string fieldName in fields.Keys)
                {
                    xml = ReplaceTokenWithValue(fieldName, (string)fields[fieldName], xml);
                }

                foreach (string rootName in complexRoots.Keys)
                {
                    RecursiveElement element = (RecursiveElement) complexRoots[rootName];
                    Request root = (Request)element.elements[0];
                    xml = InsertIncludeIntoXml(rootName, root.XML, xml);
                }

                foreach (string itemName in recursiveElements.Keys)
                {
                    RecursiveElement list = (RecursiveElement)recursiveElements[itemName];
                    StringBuilder builder = new StringBuilder();
                    foreach (Request element in list.elements)
                    {
                        builder.Append(element.XML);
                    }
                    xml = InsertIncludeIntoXml(itemName, builder.ToString(), xml);
                }

                xml = RemoveUnusedIncludeTokens(xml);

                return xml;
            }
        }

        /// <summary>
        /// Gets a version of the request's XML with sensitive fields masked.
        /// </summary>
        /// <remarks>
        /// Some fields must always be masked. Others can be specified by the 
        /// merchant by setting the MaskFieldList property in the 
        /// linehandler.properties file.
        /// </remarks>
        public string MaskedXML
        {
            get
            {
                return MaskXML(XML);
            }
        }

        #endregion

        #region Private Methods

        private void VerifyRequiredFormats()
        {
            foreach (RecursiveElement element in complexRoots.Values)
            {
                if (element.enforceGreaterThanZero != null && !((Request)element.elements[0]).recursiveElements.ContainsKey(element.enforceGreaterThanZero))
                    throw MakeError("At least one Recursive Element [{0}] must be added to the Transaction.", element.enforceGreaterThanZero);
                ((Request)element.elements[0]).VerifyRequiredFormats();
            }

            foreach (RecursiveElement element in recursiveElements.Values)
            {
                if (element.enforceGreaterThanZero != null && !recursiveElements.ContainsKey(element.enforceGreaterThanZero))
                    throw MakeError("At least one Recursive Element [{0}] must be added to the Transaction.", element.enforceGreaterThanZero);
                foreach (Request request in element.elements)
                {
                    request.VerifyRequiredFormats();
                }
            }
        }

        /// <summary>
        /// Remove all unused include file tokens from XML. 
        /// </summary>
        /// <remarks>
        /// Include tokens mark where Complex Roots and Recursive Elements are 
        /// inserted into the template. These tokens are replaced with the 
        /// XML from the included file whenever the root or element is 
        /// created. This method removes any that have not been created.
        /// </remarks>
        /// <param name="xml">The XML data containing the tokens.</param>
        /// <returns>Fully-formed XML text without the tokens.</returns>
        private string RemoveUnusedIncludeTokens(string xml)
        {
            while (xml.IndexOf("[#") != -1)
            {
                int start = xml.IndexOf("[#");
                int end = xml.IndexOf("#]");
                if (end == -1)
                    break;
                xml = xml.Substring(0, start) + xml.Substring(end + 2);
            }
            return xml;
        }

        /// <summary>
        /// Inserts the XML of a Complex Root or Recursive Element into
        /// its parent's XML. 
        /// </summary>
        /// <param name="includeName">The name of the root or element to be inserted.</param>
        /// <param name="includeXML">The XML to be inserted into the document.</param>
        /// <param name="xml">The XML source to insert the item's XML into.</param>
        /// <returns>The updated XML source.</returns>
        private string InsertIncludeIntoXml(string includeName, string includeXML, string xml)
        {
            string token = String.Format("[# {0} #]", includeName);
            return xml.Replace(token, includeXML);
        }

        /// <summary>
        /// Replace field tokens with their appropriate values.
        /// </summary>
        /// <remarks>
        /// Each field has a special token, delineated with "[%" and "%]", that specifies
        /// not only the field's name, but whether it is a required field and what default
        /// value, if any, the field should have. 
        /// 
        /// This method takes a field and replaces its token with its appropriate value, 
        /// which is either a string of some kind, an empty string, or a default value defined
        /// in its token.
        /// </remarks>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="fieldValue">The field's value.</param>
        /// <param name="xml">The XML that the field exists in.</param>
        /// <returns>The updated XML source.</returns>
        private string ReplaceTokenWithValue(string fieldName, string fieldValue, string xml)
        {
            string startToken = String.Format("<{0}>", fieldName);
            string endToken = String.Format("</{0}>", fieldName);

            int pos = xml.IndexOf(startToken);
            if (pos == -1)
                return xml;
            pos += startToken.Length;
            int end = xml.IndexOf(endToken, pos);
            if (end == -1)
                return xml;
            string replaceToken = xml.Substring(pos, end - pos);

            string defaultValue = "";
            pos = replaceToken.IndexOf("=");
            bool required = true;
            if (pos != -1)
            {
                required = false;
                end = replaceToken.IndexOf("%]");
                if (end - (pos + 2) > 0)
                    defaultValue = replaceToken.Substring(pos + 1, end - (pos + 2)).Trim();
            }

            if (required && IsEmpty(fieldValue) && IsEmpty(defaultValue))
                throw MakeError(String.Format("The field \"{0}\" must be set to a value.", fieldName));

            if (IsEmpty(fieldValue) && !IsEmpty(defaultValue))
                xml = xml.Replace(replaceToken, defaultValue);
            else
                xml = xml.Replace(replaceToken, fieldValue);

            return xml;
        }

        /// <summary>
        /// Parses a template XML document into its list of fields.
        /// </summary>
        /// <param name="template">An XML document that contains a template.</param>
        /// <param name="isInclude">true if this is an include file, false if it is not.</param>
        private void ParseTemplate(XmlDocument template, bool isInclude)
        {
            // Root request templates always have a Request node whose children we must select.
            // Include templates either have their own root node whose children we want, or
            // the fields are just sitting at the root of the include file.
            XmlNodeList fieldNodes = null;
            if (!isInclude)
                fieldNodes = template.SelectNodes(String.Format("//Request/*/*", transactionType));
            else
            {
                fieldNodes = template.SelectNodes(String.Format("//*/*", transactionType));
                if (fieldNodes.Count == 0)
                {
                    fieldNodes = template.SelectNodes(String.Format("//*", transactionType));
                }
            }

            ParseFieldNodes(fieldNodes);

            // These are two custom fields that don't belong in the template, 
            // but need to be accessible to the merchant. So, they are 
            // added to the hashtable.
            if (!isInclude)
            {
                if (!fields.ContainsKey("TraceNumber"))
                {
                    fields.Add("TraceNumber", "");
                }
                if (!fields.ContainsKey("MerchantID"))
                {
                    fields.Add("MerchantID", "");
                }
            }
        }

        private void ParseFieldNodes(XmlNodeList fieldNodes)
        {
            foreach (XmlNode fieldNode in fieldNodes)
            {
                if (fieldNode.NodeType != XmlNodeType.Element)
                    continue;

                bool isNotField = false;
                foreach (XmlNode node in fieldNode.ChildNodes)
                {
                    if (node.NodeType == XmlNodeType.Element && node.HasChildNodes)
                    {
                        ParseFieldNodes(fieldNode.ChildNodes);
                        isNotField = true;
                        break;
                    }
                }
                if (isNotField)
                    continue;

                string name = fieldNode.Name;
                string val = GetNodeText(fieldNode);
                if (val == null)
                {
                    fields.Add(name, null);
                    continue;
                }
                if (val.Trim().StartsWith("[#"))
                    continue;
                if (val.IndexOf("=") != -1)
                {
                    string[] vals = val.Split('=');
                    if (vals.Length == 2)
                    {
                        string text = vals[1].Replace("%]", "").Trim();
                        if (text != null && text.Trim() == "")
                            text = null;
                        fields.Add(name, text);
                        if (IsEmpty(text))
                            requiredFields.Add(name);
                    }
                    else
                    {
                        fields.Add(name, null);
                        requiredFields.Add(name);
                    }
                }
                else
                    fields.Add(name, null);
            }
        }

        /// <summary>
        /// Parses an template XML document into its list of fields.
        /// </summary>
        /// <remarks>
        /// An include template is really just a fragment of an XML document. It has 
        /// no root element and is intended to be inserted into the root XML that was
        /// loaded by a different Request object. Therefore, the file cannot be loaded
        /// and parsed the same was as a root XML file.
        /// </remarks>
        /// <param name="path">The path to the template file.</param>
        private void ParseIncludeTemplate(string path)
        {
            IFiler filer = factory.MakeFiler();
            string xml = filer.ReadAllText(path);
            templateXML = xml;
            if (!HasRootElement(xml))
                xml = String.Format("<Include>{0}</Include>", xml);
            XmlDocument template = new XmlDocument();
            template.LoadXml(xml);

            ParseTemplate(template, true);
        }

        /// <summary>
        /// Returns true if the XML has a single root node, 
        /// or false if it has more than one.
        /// </summary>
        /// <param name="xml">The XML to check.</param>
        /// <returns>True if it has a single root element, false if not.</returns>
        private bool HasRootElement(string xml)
        {
            int pos = xml.IndexOf(">");
            if (pos == -1)
                return false;
            string node = xml.Substring(0, pos + 1);
            string endNode = String.Format("</{0}", node.Substring(1));
            return xml.Trim().EndsWith(endNode);
        }

        private RecursiveElement GetRecursiveData(string name)
        {
            string root = "XTTF.Request.RecursiveElement";
            RecursiveElement element = new RecursiveElement();
            element.name = name;
            element.countElement = Config[String.Format("{0}.{1}.{2}.CountElement", root, transactionType, name)];
            element.enforceGreaterThanZero = Config[String.Format("{0}.{1}.{2}.EnforceGreaterThanZero", root, transactionType, name)];
            string num = Config[String.Format("{0}.{1}.{2}.MaxCount", root, transactionType, name)];
            if (num != null)
                element.maxElements = Convert.ToInt32(num);
            element.childIndexElement = Config[String.Format("{0}.{1}.{2}.ChildIndexElement", root, transactionType, name)];
            return element;
        }

        private string IncrementFieldValue(string origVal)
        {
            string orig = origVal;
            if (orig == null)
                orig = "0";

            if (!IsNumeric(orig))
                return orig;

            int val = Convert.ToInt32(orig);
            val++;
            return val.ToString();
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

        #endregion
    }
}
