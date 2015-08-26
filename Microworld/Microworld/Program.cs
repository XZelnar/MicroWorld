using System;

namespace MicroWorld
{
#if WINDOWS
    static class Program
    {
        public enum Framework
        {
            MONO = 0,
            XNA = 1
        }
        private static Framework _framework;
        internal static Framework ExecutingFramework
        {
            get { return _framework; }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            _framework = Framework.XNA;
            System.Windows.Forms.Application.EnableVisualStyles();
            if (args != null && args.Length != 0)
            {
                if (args[0].ToLower() == "enced")
                {
                    Settings.startAsEncEdit = true;
                }
            }
            using (Main game = new Main())
            {
                //TODO reenable
                //try
                //{
                    game.Run();
                /*
                }
                catch (Exception e)
                {
                    MicroWorld.Main.game.IsMouseVisible = true;
                    if (!System.IO.Directory.Exists("debug")) 
                        System.IO.Directory.CreateDirectory("debug");
                    String fn = "debug/error" + DateTime.Now.Ticks.ToString() + ".log";
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(fn);
                    sw.WriteLine(e.Message);
                    sw.WriteLine();
                    sw.WriteLine(e.Source);
                    sw.WriteLine();
                    sw.WriteLine(e.StackTrace);
                    sw.WriteLine();
                    sw.Close();

                    System.Windows.Forms.MessageBox.Show("Oops! Something was where it was not supposed to be.\r\nCrash log has been saved to debug/error.log.\r\nPlease, consider submitting it.");
                }//*/
                /*
                }
                catch (Exception e)
                {
                    MicroWorld.Main.game.IsMouseVisible = true;
                    if (!System.IO.Directory.Exists("debug")) System.IO.Directory.CreateDirectory("debug");
                    String fn = "debug/error" + DateTime.Now.Ticks.ToString() + ".log";
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(fn);
                    sw.WriteLine(e.Message);
                    sw.WriteLine();
                    sw.WriteLine(e.Source);
                    sw.WriteLine();
                    sw.WriteLine(e.StackTrace);
                    sw.WriteLine();
                    sw.Close();

                    //System.Windows.Forms.MessageBox.Show("Oops! Something was where it was not supposed to be.\r\nCrash log has been saved");
                    Debug.ReportSender rs = new Debug.ReportSender();
                    rs.ErrorFileName = fn;
                    rs.ShowDialog();
                }//*/
            }
        }
    }
#endif
}

