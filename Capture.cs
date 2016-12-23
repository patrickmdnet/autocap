/*
 * Code to capture screenshot and write to disk
 */

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using NLog;
using AutoCap.Properties;

namespace AutoCap
{
    class Capture : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private String capturePath;
        private String captureFile;

        public Capture()
        {
            capturePath = Settings.Default.CapturePath;
            captureFile = Settings.Default.CaptureFile;
            logger.Debug("capturePath={0}; captureFile={1};", capturePath, captureFile);
        }

        public void Tick(object sender, EventArgs e)
        {
            captureScreenToFile();
        }
        
        public void captureScreenToFile()
        {
            String fileName = Path.Combine(capturePath, captureFile);

            try
            {
                Rectangle s_rect = Screen.PrimaryScreen.Bounds;
                using (Bitmap bmp = new Bitmap(s_rect.Width, s_rect.Height))
                {
                    using (Graphics gScreen = Graphics.FromImage(bmp))
                        gScreen.CopyFromScreen(s_rect.Location, Point.Empty, s_rect.Size);
                    bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error writing to {0}.", fileName);
            }
        }

        public void Dispose()
		{
        }
    }
}
