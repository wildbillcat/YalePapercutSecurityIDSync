using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PapercutSecurityIDSync
{
    public class IDManager
    {
        string IDFilePath;
        OracleServer oracleServer;
        PaperCutServer paperCutServer;
        protected Timer tm;
        DateTime LastIDProcess;

        public IDManager()
        {
            LastIDProcess = DateTime.Now.AddDays(-1);
            tm = new Timer(6000);
            tm.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            tm.Enabled = true;
            tm.AutoReset = true;
            //tm.Interval = 6000; // 6 Seconds
            tm.Start();
            GC.KeepAlive(tm);
        }

        public IDManager(string IDFilePath)
        {
            this.IDFilePath = IDFilePath;
        }

        protected void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (DateTime.Now.Day != LastIDProcess.Day)
            {
                tm.Stop();
                tm.Enabled = false;
                readConfiguration();

                if (!System.IO.File.Exists(IDFilePath) || System.IO.File.Exists(IDFilePath) && DateTime.Now.Day != System.IO.File.GetCreationTime(IDFilePath).Day)
                {
                    this.ProcessIDs();
                    LastIDProcess = DateTime.Now;
                }
                tm.Enabled = true;
                tm.Start();
            }
        }

        private void readConfiguration()
        {
            string cwd = AppDomain.CurrentDomain.BaseDirectory;
            string ConfigPath;
            //Parse CWD
            if (!cwd[cwd.Length - 1].Equals('\\'))
            {
                cwd = string.Concat(cwd, @"\");
            }
            //Read Text file that points service at the Billing Directory
            using (System.IO.StreamReader dirPath = new System.IO.StreamReader(string.Concat(cwd, "cfgpath.txt")))
            {
                ConfigPath = dirPath.ReadLine();
            }
            
            //Directory Test
            if (!System.IO.Directory.Exists(ConfigPath)) //The Billing Directory Does not Exist!
            {
                Console.WriteLine("Billing Directory does not exist!");
                return;
            }

            //Config File Test
            if (!System.IO.File.Exists(ConfigPath)) //The Billing Directory Does not Exist!
            {
                Console.WriteLine("Billing Directory does not exist!");
                return;
            }

            string userHR = "";
            string passHR = "";
            string pathHR = "";
            string userSC = "";
            string passSC = "";
            string pathSC = "";
            string paperCutAuthToken = "";
            int paperCutPort = 9191;
            string paperCutServerPath = "";
            


            System.IO.StreamReader file = new System.IO.StreamReader(ConfigPath);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                Console.WriteLine("Line Read: " + line);
                int endofline = line.Length;
                int last = line.IndexOf("=");
                if (last != -1)
                {
                    string setting = line.Substring(0, last - 1).Trim();
                    Console.WriteLine("Setting: " + setting);
                    string value = line.Substring(last + 1, endofline - (last + 1)).Trim();
                    Console.WriteLine("Value: " + value);
                    Console.WriteLine(" ");
                    /////////////////////Import File Path///////////////////////////////////////////////////////
                    if (setting.Equals("IDFilePath"))
                    {
                        IDFilePath = value;
                    }
                    ////////////////////Oracle HR Server/////////////////////////////////////////////////////
                    else if (setting.Equals("userHR"))
                    {
                        userHR = value;
                    }
                    else if (setting.Equals("passHR"))
                    {
                        passHR = value;
                    }
                    else if (setting.Equals("pathHR"))
                    {
                        pathHR = value;
                    }
                    ////////////////////Oracle Security Server/////////////////////////////////////////////////////
                    else if (setting.Equals("userSC"))
                    {
                        userSC = value;
                    }
                    else if (setting.Equals("passSC"))
                    {
                        passSC = value;
                    }
                    else if (setting.Equals("pathSC"))
                    {
                        pathSC = value;
                    }
                    ////////////////////PaperCut Server/////////////////////////////////////////////////////
                    else if (setting.Equals("paperCutAuthToken"))
                    {
                        paperCutAuthToken = value;
                    }
                    else if (setting.Equals("paperCutPort"))
                    {
                        paperCutPort = int.Parse(value);
                    }
                    else if (setting.Equals("paperCutServerPath"))
                    {
                        paperCutServerPath = value;
                    }
                    else
                    {
                        //unrecognized line. Do nothing
                    }
                    ////////
                }
            }

            file.Close();

            oracleServer = new OracleServer(userHR, passHR, pathHR, userSC, passSC, pathSC);
            paperCutServer = new PaperCutServer(paperCutAuthToken, paperCutPort, paperCutServerPath);

        }

        public void ProcessIDs()
        {
            readConfiguration();
            List<string> PaperCutUsers = paperCutServer.GetPaperCutUsers();
            List<UserInfo> UserList = oracleServer.GetUserList(PaperCutUsers);
            WriteIDs(UserList);
            paperCutServer.SetPaperCutUserIDs(IDFilePath);
        }

        public void WriteIDs(List<UserInfo> users)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(IDFilePath)) //Open the handle to the file that will be used to import card numbers
            {
                StringBuilder line = new StringBuilder();
                foreach (UserInfo user in users)
                {
                    line.Append(user.NetID);
                    line.Append("\t");
                    if (user.ProxID == null)
                    {
                        line.Append("-");
                    }
                    else
                    {
                        line.Append(user.ProxID);
                    }
                    line.Append("\t-\t");
                    if (user.MagID == null)
                    {
                        line.Append("-");
                    }
                    else
                    {
                        line.Append(user.MagID);
                    }
                    file.WriteLine(line.ToString());
                    line.Clear();
                }
            }
        }
    }
}
