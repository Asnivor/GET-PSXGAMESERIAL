using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PSX_GetSerial
{
    class Program
    {
        static void Main(string[] args)
        {
            // application accepts path to .cue, .ccd or .toc
            
            if (args.Length != 1)
            {
                QuitApp("ERROR. Problem with command line arguments (make you specify one path and encapsulate with double quotes if there are spaces)");
            }          

            // get the supplied path
            string path = args.First();

            // check whether it exists
            if (!File.Exists(path))
            {
                QuitApp("ERROR. Cannot find the specified file");
            }

            // check it is a suitable type
            if (path.ToLower().EndsWith(".cue") ||
                path.ToLower().EndsWith(".ccd") ||
                path.ToLower().EndsWith(".toc"))
            {
                // try and get the game serial number
                string serial = MedDiscUtils.GetPSXSerial(path);

                Console.WriteLine(serial);

                QuitApp(null);
            }
            else
            {
                QuitApp("ERROR. Incorrect filetype. File must be *.cue, *.ccd or *.toc");
            }
            
        }

        public static void QuitApp(string quitMsg)
        {
            if (quitMsg != null)
            {
                Console.WriteLine(quitMsg);
            }
            //Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
