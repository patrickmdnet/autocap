/*
 * Based on https://www.codeproject.com/articles/290013/formless-system-tray-application
 */

using System;
using System.Windows.Forms;
using System.Threading;
using NLog;
using AutoCap.Properties;

namespace AutoCap
{
	static class Program
	{
        private static Mutex mutex = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [STAThread]
        static void Main()
		{
            bool createdNew;

            // prevent second instance from starting
            mutex = new Mutex(true, "AutoCap", out createdNew);

            if (!createdNew)
            {
                logger.Debug("Existing AutoCap already running, exiting");
                return;
            }
            logger.Info("Starting");

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

            // use System.Windows.Forms.Timer; executes on the UI thread
            using (System.Windows.Forms.Timer AutoCapTimer = new System.Windows.Forms.Timer())
            {
                int minInterval = Settings.Default.CaptureMinuteInterval;
                if (minInterval < 1 || minInterval > 60)
                {
                    minInterval = 1;
                }
                logger.Debug("minInterval={0}", minInterval);

                AutoCapTimer.Interval = (minInterval * 60 * 1000); // minInterval minutes

                using (Capture AutoCapCapture = new Capture()) {
                    AutoCapCapture.captureScreenToFile();

                    AutoCapTimer.Tick += new EventHandler(AutoCapCapture.Tick);
                    AutoCapTimer.Start();

			        // Show the system tray icon.					
			        using (ProcessIcon pi = new ProcessIcon())
			        {
				        pi.Display();
				        Application.Run();
			        }                    
                }
            }

            logger.Info("Exiting");
		}
	}
}
