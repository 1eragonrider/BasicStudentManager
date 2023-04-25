using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using MySql.Data.MySqlClient;
using System.Xml;

namespace BasicStudentManager
{
    internal class StartDB
    {
        /// <summary>
        /// Checks if the local database already exists.
        /// </summary>
        /// <param name="dbNamePar">The name of the database used to store the data used in the application.</param>
        /// <returns></returns>
        public bool databaseExists(string dbNamePar)
        {
            // We will try to connect to a database if it exists we return true, else return false
            
            try
            {
                XmlDocument dbDataDoc = new XmlDocument();
                dbDataDoc.Load(this.GetType().Assembly.GetManifestResourceStream("BasicStudentManager.Services.DBConnData.xml"));
                string connectionStr = String.Format("SERVER={0};PORT={1};UID={2};PASSWORD={3};DATABASE={4}",
                    dbDataDoc.DocumentElement.SelectSingleNode("/database/server").InnerText,
                    dbDataDoc.DocumentElement.SelectSingleNode("/database/port").InnerText,
                    dbDataDoc.DocumentElement.SelectSingleNode("/database/security/uid").InnerText,
                    dbDataDoc.DocumentElement.SelectSingleNode("/database/security/pass").InnerText,
                    dbNamePar);
                MySqlConnection myConn = new MySqlConnection(connectionStr);
                myConn.Open();
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
        }
        /// <summary>
        /// Initializes and creates the database to be used in the application to host the student and administrators data.
        /// </summary>
        public string initializeDatabase()
        {
            // Declared links
            XmlDocument dbDataDoc = new XmlDocument();
            dbDataDoc.Load(this.GetType().Assembly.GetManifestResourceStream("BasicStudentManager.Services.DBConnData.xml"));

            // initialize test variables
            string dbName = dbDataDoc.DocumentElement.SelectSingleNode("/database/dbname").InnerText;

            // if the database exists do not initialize the database
            if (databaseExists(dbName))
            {
                return "The database does not need to be initialized. Already exists.";
            }

            // initialize db generation variables
            string databaseGenPrompt = String.Format("CREATE DATABASE IF NOT EXISTS {0}", dbName);

            string connectionStr = String.Format("SERVER={0};PORT={1};UID={2};PASSWORD={3}",
                dbDataDoc.DocumentElement.SelectSingleNode("/database/server").InnerText,
                dbDataDoc.DocumentElement.SelectSingleNode("/database/port").InnerText,
                dbDataDoc.DocumentElement.SelectSingleNode("/database/security/uid").InnerText,
                dbDataDoc.DocumentElement.SelectSingleNode("/database/security/pass").InnerText);

            MySqlConnection myConn = new MySqlConnection(connectionStr);
            MySqlCommand myCommand = new MySqlCommand(databaseGenPrompt, myConn);

            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
                return "Database has been successfully generated.";
            }
            catch (System.Exception exception)
            {
                return "Err. 001 - There was an issue with initializing the database. Details: " + exception.Message;
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }
        }
        /// <summary>
        /// Generates the tables and links them to one another in accordance with the Database model
        /// </summary>
        /// <returns></returns>
        public string generateDatabaseTables()
        {
            //Declare links
            XmlDocument dbDataDoc = new XmlDocument();

            dbDataDoc.Load(this.GetType().Assembly.GetManifestResourceStream("BasicStudentManager.Services.DBConnData.xml"));

            //Declare variables
            string[] tablesToGen = new string[100];

            // Connect to Database
            string connectionStr = String.Format("SERVER={0};PORT={1};UID={2};PASSWORD={3}; DATABASE={4}",
                dbDataDoc.DocumentElement.SelectSingleNode("/database/server").InnerText,
                dbDataDoc.DocumentElement.SelectSingleNode("/database/port").InnerText,
                dbDataDoc.DocumentElement.SelectSingleNode("/database/security/uid").InnerText,
                dbDataDoc.DocumentElement.SelectSingleNode("/database/security/pass").InnerText,
                dbDataDoc.DocumentElement.SelectSingleNode("/database/dbname").InnerText);

            MySqlConnection dbConn = new MySqlConnection(connectionStr);
            dbConn.Open();

            //TGC == Table Generate Command

            // "misc" and "LabAccess" tables
            string miscTGC = "" +
                "CREATE TABLE IF NOT EXISTS misc ( " +
                "idMisc INT NOT NULL UNIQUE AUTO_INCREMENT," +
                "lastMeet DATE," +
                "PRIMARY KEY (idMisc)" +
                ")";
            tablesToGen[0] = miscTGC;

            string labAccessTGC = "" +
                "CREATE TABLE IF NOT EXISTS labaccess ( " +
                "idLabAccess INT NOT NULL UNIQUE AUTO_INCREMENT," +
                "isOrientation ENUM('Yes', 'No')," +
                "isTraining ENUM('Yes', 'No')," +
                "isAccess ENUM('Yes', 'No')," +
                "PRIMARY KEY (idLabAccess)" +
                ")";
            tablesToGen[1] = labAccessTGC;

            // "users" tables
            string usersTGC = "" +
                "CREATE TABLE IF NOT EXISTS users ( " +
                "idUser INT NOT NULL UNIQUE AUTO_INCREMENT," +
                "Misc_idMisc INT NOT NULL," +
                "LabAccess_idLabAccess INT NOT NULL," +
                "firstName VARCHAR(100) NOT NULL," +
                "lastName VARCHAR(100) NOT NULL," +
                "studentID VARCHAR(15) UNIQUE," +
                "gender ENUM('Male', 'Female', 'Indeterminate', 'Unknown') NOT NULL," +
                "dob DATE," +
                "roll ENUM('Student', 'Teacher')," +
                "title ENUM('Mr.', 'Ms.', 'Mrs.', 'Dr.')," +
                "admin ENUM('Yes', 'No')," +
                "PRIMARY KEY (idUser)," +
                "FOREIGN KEY (Misc_idMisc) REFERENCES misc(idMisc)," +
                "FOREIGN KEY (LabAccess_idLabAccess) REFERENCES labaccess(idLabAccess)" +
                ")";
            tablesToGen[2] = usersTGC;

            // "contact" table
            string contactTGC = "" +
                "CREATE TABLE IF NOT EXISTS contact ( " +
                "idContact INT NOT NULL UNIQUE AUTO_INCREMENT," +
                "email NVARCHAR(255) UNIQUE NOT NULL," +
                "phone VARCHAR(30) UNIQUE," +
                "PRIMARY KEY (idContact)" +
                ")";
            tablesToGen[3] = contactTGC;

            // "Attendance" Table
            string attendanceTGC = "" +
                "CREATE TABLE IF NOT EXISTS attendance ( " +
                "idAttendance INT NOT NULL UNIQUE AUTO_INCREMENT," +
                "date DATE," +
                "time TIME(0)," +
                "isLate ENUM('Yes', 'No')," +
                "PRIMARY KEY (idAttendance)" +
                ")";
            tablesToGen[4] = attendanceTGC;

            // "Education" Table
            string educationTGC = "" +
                "CREATE TABLE IF NOT EXISTS education ( " +
                "idEducation INT NOT NULL UNIQUE AUTO_INCREMENT," +
                "level ENUM('Ph.D', 'Graduate', 'Under Graduate', 'Unknown')," +
                "rField VARCHAR(255)," +
                "PRIMARY KEY (idEducation)" +
                ")";
            tablesToGen[5] = educationTGC;

            // Many-to-many link (users and contacts)
            string userHasContactTGC = "" +
                "CREATE TABLE IF NOT EXISTS User_Has_Contact( " +
                "User_idUser INT NOT NULL," +
                "Contact_idContact INT NOT NULL," +
                "FOREIGN KEY (User_idUser) REFERENCES users(idUser)," +
                "FOREIGN KEY (Contact_idContact) REFERENCES contact(idContact)" +
                ")";
            tablesToGen[6] = userHasContactTGC;

            // Many-to-many link (users and attendance)
            string userHasAttendanceTGC = "" +
                "CREATE TABLE IF NOT EXISTS User_Has_Attendance( " +
                "User_idUser INT NOT NULL," +
                "Attendance_idAttendance INT NOT NULL," +
                "FOREIGN KEY (User_idUser) REFERENCES users(idUser)," +
                "FOREIGN KEY (Attendance_idAttendance) REFERENCES attendance(idAttendance)" +
                ")";
            tablesToGen[7] = userHasAttendanceTGC;

            // Many-to-many link (users and education)
            string userHasEducationTGC = "" +
                "CREATE TABLE IF NOT EXISTS User_Has_Education( " +
                "User_idUser INT NOT NULL," +
                "Education_idEducation INT NOT NULL," +
                "FOREIGN KEY (User_idUser) REFERENCES users(idUser)," +
                "FOREIGN KEY (Education_idEducation) REFERENCES education(idEducation)" +
                ")";
            tablesToGen[8] = userHasEducationTGC;

            foreach (var sqltablecmd in tablesToGen)
            {
                if (sqltablecmd != null)
                {
                    try
                    {
                        MySqlCommand command = new MySqlCommand(sqltablecmd, dbConn);
                        command.ExecuteNonQuery();
                    }
                    catch(MySqlException)
                    {
                        return "There was an issue generating the tables in the program database.";
                    }
                    
                }
            }

            return "The Database tables were loaded/generated successfully.";
        }
    }
}
