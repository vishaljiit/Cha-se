using System;
using System.IO;
using System.Collections;
using System.Xml;
using Paymentech;
using ChasePaymentech.Orbital.Test.Samples;


using System.Text.RegularExpressions;
/// <p>Copyright (c) 2008, Chase Paymentech Solutions, LLC. All rights
/// reserved</p>
///
/// <p>Company: Chase Paymentech Solutions</p>
///
/// @author Rameshkumar Bhaskharan
/// @version 1.0
///
namespace ChasePaymentech.Orbital.Test
{
	/// <summary>
	/// Summary description for SampleDriver
	/// </summary>
	class SampleDriver
	{
		public static Hashtable internetSample = new Hashtable();
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			//Print application version
			PrintVersion();

            // Prepare all sample test case
			PrepareTest();

            if (args.Length > 0 && args[0].ToLower() == "-all")
            {
                for (int i = 1; i <= internetSample.Count; i++) 
                    ValidateAndRun(i.ToString(), false);
                return;
            }
			
            // Check and run the command line 
            if (args.Length > 0)
            {
                ValidateAndRun(args[0], false);
                return;
            }

			while(true)
			{
				PrintUsage();
				Console.Write("\nEnter your selection ==>");
				string input = Console.ReadLine();
				// Run the test case
				ValidateAndRun(input, true);
			}
		}
		/// <summary>
		/// Validate the input and run the test case
		/// </summary>
		/// <param name="sampleIndex"></param>
		static void ValidateAndRun(string sampleIndex, bool prompt)
		{
			try
			{
				RunTest(Convert.ToInt32(sampleIndex), prompt);
			}
			catch(Exception)
			{
				Console.WriteLine("Invalid sample selection - Press any key to continue");
				Console.ReadLine();
			}
		}
		/// <summary>
		/// Run the specific test
		/// </summary>
		/// <param name="sampleIndex"></param>
		static void RunTest(int sampleIndex, bool prompt)
		{
			if ( sampleIndex == 0 )
				Environment.Exit(0);
			if ( internetSample.ContainsKey(sampleIndex))
			{
				((IOrbitalSample) internetSample[sampleIndex]).RunTest(sampleIndex);
			}
			else
			{
				Console.Write("Invalid sample selection - ");
			}

            if (prompt)
            {
                Console.Write("Press any key to continue\n");
                Console.ReadLine();
            }
		}
		/// <summary>
		/// Prepare the sample test case
		/// </summary>
		static void PrepareTest()
		{
			internetSample.Add(1,new AuthSample());
			internetSample.Add(2,new AuthWithRetrySample());
			internetSample.Add(3,new AuthCaptureSample());
			internetSample.Add(4,new CaptureSample());
			internetSample.Add(5,new FullVoidSample());
			internetSample.Add(6,new PartialVoidSample());
			internetSample.Add(7,new RecurringSample());
			internetSample.Add(8,new RefundSample());
			internetSample.Add(9,new RecurringRefundSample());
			internetSample.Add(10,new ForcedAuthSample());
			internetSample.Add(11,new ECPAuthorizeSample());
			internetSample.Add(12,new InternationalMaestroSample());
			internetSample.Add(13,new PC2AuthCapSample());
			internetSample.Add(14,new ProfileCRUDSample());
			internetSample.Add(15,new FlexCacheSample());
			internetSample.Add(16,new FlexCacheBatchSample());
			internetSample.Add(17,new MOTOSample());
			internetSample.Add(18,new MOTORefundSample());
			internetSample.Add(19,new PC3Sample());
			internetSample.Add(20,new PriorAuthSample());
			internetSample.Add(21,new MCSecureCodeSample());
			internetSample.Add(22,new VerifiedByVisaSample());
			internetSample.Add(23,new BMLSample());
			internetSample.Add(24,new PinLessSample());
			internetSample.Add(25,new RecurringManageBillingSample());
			internetSample.Add(26,new InquiryBillingSample());
			internetSample.Add(27,new BatchSample());
			internetSample.Add(28,new AccountUpdaterSample());
		}
		/// <summary>
		/// Print the application version
		/// </summary>
		static void PrintVersion()
		{
			try
			{				
				System.Reflection.Assembly asm = System.Reflection.Assembly.GetAssembly(typeof(Paymentech.Transaction));
				Console.WriteLine("Orbital DotNet SDK Version " + asm.GetName().Version);
			}
			catch
			{
				Console.WriteLine("Unable to get Orbital DotNet SDK Version");
			}
		}

        /// <summary>
		/// Print the sample application usage
		/// </summary>
		static void PrintUsage()
		{
			Console.WriteLine("\n" + String.Format("{0,-15}{1,-50}","Test Number","Test Description"));   
			Console.WriteLine("----------------------------------------------------");   
			Console.WriteLine(String.Format("{0,-15}{1,-50}","0","Quit Sample Application"));   
			int[] testCase= new int[internetSample.Keys.Count];
			internetSample.Keys.CopyTo(testCase,0);
			Array.Sort(testCase);
			foreach(int ctr in testCase)
			{
				Console.WriteLine(String.Format("{0,-15}{1,-50}",ctr,((IOrbitalSample) internetSample[ctr]).Name));
			}
		}
	}
}

