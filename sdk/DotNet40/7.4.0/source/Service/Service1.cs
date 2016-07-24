using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using Paymentech;
using Paymentech.Core;
using log4net;

namespace Paymentech
{
	internal class Service1 : System.ServiceProcess.ServiceBase
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private static ILog logger = null;
        private static IFactory factory = null;

		public Service1()
		{
			// This call is required by the Windows.Forms Component Designer.
			InitializeComponent();

            factory = new Factory();
            logger = factory.EngineLogger;
		}

		// The main entry point for the process
		static void Main(string [] args)
		{
			if (args.Length >= 1)
			{
				// We may have to install or uninstall
				string arg = args[0].Replace ("/", "-");
				if (arg.ToLower () == "-install" || 
					arg.ToLower () == "-i")
				{
					logger.Debug("Installing Paymentech .Net Service");
					Install ();
                    return;
				}
				else if (arg.ToLower () == "-uninstall" ||
					arg.ToLower () == "-u")
				{
					logger.Debug("Uninstalling Paymentech .Net Service");
					Uninstall ();
                    return;
				}
                else if (arg.ToLower() == "-home" && args.Length >= 2)
                {
                    Configurator.Instance.PaymentechHome = args[1];
                }
                else
                {
                    logger.ErrorFormat(".NET Service started with invalid parameter [{0}]", args[0]);
                    logger.Debug(".NET service terminating abnormally");
                    return;
                }
			}

            
            System.ServiceProcess.ServiceBase[] ServicesToRun;

			// More than one user Service may run within the same process. To add
			// another service to this process, change the following line to
			// create a second service object. For example,
			//
			//   ServicesToRun = New System.ServiceProcess.ServiceBase[] {new Service1(), new MySecondUserService()};
			//
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new Service1() };

			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		/// <summary>
		/// Installs the service by calling InstallUtil.exe
		/// </summary>
		private static void Install ()
		{
			string [] args = new string [1];
			args[0] = (typeof(Service1).Assembly.Location);
			AppDomain dom = AppDomain.CreateDomain("execDom");
			Type type = typeof(System.Object);
			String path = type.Assembly.Location;
			StringBuilder sb = new StringBuilder(path.Substring(0, path.LastIndexOf("\\")));
			sb.Append("\\InstallUtil.exe");

			dom.ExecuteAssembly(sb.ToString(), null, args);
		}
		/// <summary>
		/// Uninstalls the service
		/// </summary>
		private static void Uninstall ()
		{
			//UnInstall this Windows Service using InstallUtil.exe
			String [] args = new String[2];
			args[0] = "/u";
			args[1] = (typeof(Service1).Assembly.Location);
			AppDomain dom = AppDomain.CreateDomain("execDom");
			Type type = typeof(System.Object);
			String path = type.Assembly.Location;
			StringBuilder sb = new StringBuilder(path.Substring(0, path.LastIndexOf("\\")));
			sb.Append("\\InstallUtil.exe");

			dom.ExecuteAssembly(sb.ToString(), null, args);
		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			this.ServiceName = "PT.NETService";
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		/// <remarks>
		/// Registers the service for .NET Remoting.
		/// </remarks>
		protected override void OnStart(string[] args)
		{
			try
			{
                IFactory factory = new Factory();
				string objectType = "Paymentech.RemoteTransactionProcessor";
                int port = Convert.ToInt32(factory.Config["DotNet.Transaction.Service.Port"]);
                string uri = factory.Config["DotNet.Transaction.Service.URI"];

                logger.DebugFormat(".Net service starting... Registering type {0} on port {1}.", objectType, port);
				TcpChannel channel = new TcpChannel (port);
				ChannelServices.RegisterChannel (channel);
				// RemotingConfiguration.ApplicationName="PTDotNetService";
				RemotingConfiguration.RegisterWellKnownServiceType (
					typeof (RemoteTransactionProcessor), uri, 
					WellKnownObjectMode.Singleton);
				logger.Debug(".NET service started.");
			}
			catch (Exception e)
			{
			    logger.Error(e.Message);
			    logger.Debug(".NET service terminating abnormally");
				throw;
			}
			
		}
 
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			// TODO: Add code here to perform any tear-down necessary to stop your service.
		}
	}
}
