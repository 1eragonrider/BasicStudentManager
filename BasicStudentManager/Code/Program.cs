using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing.Text;
using System.Xml.Linq;
using System.Data;
using System.IO;
using System.Text;

namespace BasicStudentManager
{
    internal static class Program
    {

        static void Main()
        {
            // Initialize files and logs
            Files_Logs fl = new Files_Logs();
            fl.generateFileStructure();
            fl.createLogFiles();
            StreamWriter logfilestream = File.AppendText(fl.getLogFilePath());

            // Initialize Database
            StartDB dbInit = new StartDB();
            fl.log(logfilestream, dbInit.initializeDatabase());
            fl.log(logfilestream, dbInit.generateDatabaseTables());

            // Application / Forms start
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            // Clean Up
            logfilestream.Flush();
            logfilestream.Close();
        }
    }
}
