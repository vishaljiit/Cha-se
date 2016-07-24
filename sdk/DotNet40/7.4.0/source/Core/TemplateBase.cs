using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;

namespace Paymentech.Core
{
	/// <summary>
	/// Provides common methods for many template-handling classes.
	/// </summary>
    [Serializable]
    public class TemplateBase : CoreBase
    {
        /// <summary>
        /// Mask sensitive fields in the supplied XML. 
        /// </summary>
        /// <remarks>
        /// There are certain hard-coded fields that are always to be masked. 
        /// The merchant can specify any other fields to be masked by setting 
        /// the MaskFieldList property in the linehandler.properties file.
        /// </remarks>
        /// <param name="inputXML"></param>
        /// <returns></returns>
        protected string MaskXML(string inputXML)
        {
            string xml = MaskField( inputXML, "AccountNum" );
            xml = MaskField( xml, "CardSecVal" );
            xml = MaskField( xml, "CheckDDA" );
            xml = MaskField( xml, "BMLCustomerSSN" );
            xml = MaskField( xml, "CCAccountNum" );
            xml = MaskField( xml, "ECPAccountDDA" );
            xml = MaskField( xml, "CAVV" );
            xml = MaskField( xml, "OrbitalConnectionPassword" );
            xml = MaskField( xml, "AAV" );
            xml = MaskField( xml, "StartAccountNum" );
            xml = MaskField( xml, "EUDDIBAN" );

            string customMaskFields = null;
            try
            {
                customMaskFields = factory.Config["MaskFieldList"];
            }
            catch (Exception) { }

            if ( customMaskFields == null )
            {
                return xml;
            }

            string[] masks = customMaskFields.Split( ',' );
            foreach ( string mask in masks )
            {
                xml = MaskField( xml, mask );
            }

            return xml;
        }

        /// <summary>
        /// Mask the specified field (tag) with X's. Some special tags have special formatting for their masks.
        /// </summary>
        /// <param name="xml">The body of the XML document in which the tag must be masked.</param>
        /// <param name="element">The name of the element to mask.</param>
        /// <returns>The masked XML.</returns>
        protected string MaskField(string xml, string element)
        {
            string tag = element.Trim();
            Regex exp = new Regex(String.Format("<{0}>(.*?)</{1}>", tag, tag));
            Match match = exp.Match(xml);
            if (!match.Success)
                return xml;

            string val = match.Groups[1].Value;
            if (IsEmpty(val))
                return xml;

            string maskedVal = "";
            if (tag.ToLower().Trim() == "accountnum")
            {
                if ( val.Length > 4 )
                {
                    maskedVal = String.Format( "############-{0}", val.Substring( val.Length - 4 ) );
                }
                else
                {
                    maskedVal = "############";
                }
            }
            else if (tag.ToLower().Trim() == "cardsecval")
            {
                maskedVal = "###";
            }
            else if ( tag.ToLower().Contains( "password" ) )
            {
                maskedVal = "########";
            }
            else
            {
                maskedVal = new string( '#', val.Length );
            }

            return exp.Replace(xml, String.Format("<{0}>{1}</{2}>", tag, maskedVal, tag));
        }

        /// <summary>
        /// Determines if the specified string represents a number.
        /// </summary>
        /// <param name="numberString">The string to be tested.</param>
        /// <returns>Returns true if the string is numeric, false if it is not.</returns>
        protected bool IsNumeric(object numberString)
        {
            char[] ca = numberString.ToString().ToCharArray();
            for (int i = 0; i < ca.Length; i++)
            {
                if (!char.IsNumber(ca[i]))
                    if (ca[i] != '.')
                        return false;
            }
            if (numberString.ToString().Trim() == "")
                return false;
            return true;
        } 

        /// <summary>
        /// Gets the text node of the specified node. 
        /// </summary>
        /// <remarks>
        /// If the node does not 
        /// have a immediate child that is a text node, it will not dig
        /// deeply, but will instead just return null.
        /// </remarks>
        /// <param name="node">The node whose text element to return.</param>
        /// <returns>An XmlNode referencing the text node, or null if none was found.</returns>
        protected string GetNodeText(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType.Equals(XmlNodeType.Text))
                    return child.Value;
            }
            return null;
        }

        /// <summary>
        /// Helper method to combine multiple file path segments.
        /// </summary>
        /// <param name="paths">The segments of the file path to be combined.</param>
        /// <returns>A fully-qualified path to a file or directory.</returns>
        protected string CombinePaths(params string[] paths)
        {
            string fullPath = "";
            foreach (string path in paths)
            {
                fullPath = Path.Combine(fullPath, path);
            }
            return fullPath;
        }
    }
}
