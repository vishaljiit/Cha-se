using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Runtime.CompilerServices;

namespace ChasePaymentech.Orbital.Test.Samples
{
	/// <summary>
	/// Summary description for XMLUtility.
	/// </summary>
	public class XMLUtility
	{
		private static XmlDocument xmlDoc = null ;
		private static XMLUtility xml = null;
		private XMLUtility()
		{
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static XMLUtility GetInstance()
		{
			if ( xml != null )
				return xml;
			xml = new XMLUtility();
			xmlDoc = new XmlDocument();
			string sampleConfigName = "SampleData.xml";
			string sampleConfigLoc  = "samples/bin";
			string fileName = null;
			if (!File.Exists(Environment.CurrentDirectory + "/" + sampleConfigName))
			{
				if (!File.Exists(Environment.GetEnvironmentVariable("PAYMENTECH_HOME") + "/" + sampleConfigLoc + "/" + sampleConfigName))
				{
					Console.WriteLine("Unable to locate SampleData.xml file - Press any key to continue");
					Console.ReadLine();
					Environment.Exit(0);
				}
				else
					fileName = Environment.GetEnvironmentVariable("PAYMENTECH_HOME") + "/" + sampleConfigLoc + "/" + sampleConfigName  ;
			}
			else
				fileName = Environment.CurrentDirectory + "/" + sampleConfigName;
			try
			{
				xmlDoc.Load(fileName);
				FileInfo file = new FileInfo(fileName);
				Console.WriteLine("Using sample data from ==> " + fileName );
				return xml;
			}
			catch(Exception e )
			{
				Console.WriteLine("Error while loading SampleData.xml file");
				Console.WriteLine(e.ToString());
				Console.WriteLine("Exiting the application - Press any key to continue");
				Console.ReadLine();
				Environment.Exit(0);
			}
			return xml;
		}
		public String RetNodeValue(String xPath)
		{
			XPathNavigator nav= xmlDoc.CreateNavigator();
			XPathNodeIterator iter = nav.Select(xPath);
			if ( iter.Count == 0 )
				return null;
			else
			{
				iter.MoveNext();
				return iter.Current.Value;
			}
		}
	}
}
