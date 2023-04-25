using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicStudentManager
{
    internal class Files_Logs
    {
        private static string appDataDirectory = "C:\\Users\\Andre\\AppData\\Roaming\\";
        private static string logFilePath = appDataDirectory + "studentManager\\Logs\\Log.txt";

        public string getAppDataDirectory()
        {
            return appDataDirectory;
        }

        public string getLogFilePath()
        {
            return logFilePath;
        }

        /// <summary>
        /// Creates the folders to store the generated files.
        /// </summary>
        public void generateFileStructure()
        {
            
            if (!Directory.Exists(appDataDirectory + "StudentManager"))
            {
                // Create the main directory
                System.IO.Directory.CreateDirectory(appDataDirectory + "StudentManager");
            }

            if (!Directory.Exists(appDataDirectory + "StudentManager\\Data"))
            {
                // create the Data directory
                System.IO.Directory.CreateDirectory(appDataDirectory + "StudentManager\\Data");
            }

            if (!Directory.Exists(appDataDirectory + "StudentManager\\Logs"))
            {
                // create the Logs directory
                System.IO.Directory.CreateDirectory(appDataDirectory + "StudentManager\\Logs");
            }          
        }

        /// <summary>
        /// Logs the date, time, and result/exception into a log file.
        /// </summary>
        /// <param name="fsPar">Filestream of log file to write to.</param>
        /// <param name="logInputPar">The error/exception/log to write to the log file.</param>
        public void log(StreamWriter fsPar, string logInputPar)
        {
            bool logged = false; // flips if write command executes with no problem

            // Date, Time, --- Log input
            string logInput = DateTime.Now.ToString() + " --- " + logInputPar + "\n";
            char[] toLog = logInput.ToCharArray();
            int attempts = 0; // keeps track of attempts to log


            while (!logged)
            {
                attempts++;

                try
                {
                    fsPar.Write(toLog, 0, toLog.Length);
                    logged = true;
                }
                catch (Exception)
                {
                    logged = false;
                }

                if (attempts > 10)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Create a new log file upon startup of the program.
        /// </summary>
        public void createLogFiles()
        {

            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            // Create a new file     
            FileStream fs = File.Create(logFilePath);
            fs.Close();

            // log file creation
            StreamWriter sw = File.AppendText(logFilePath);
            log(sw, "Log File was created.");
            sw.Close();
            
        }
    }
}
