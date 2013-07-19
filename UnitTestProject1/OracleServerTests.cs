using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PapercutSecurityIDSync;
using PapercutSecurityIDSyncTests;
using System.Collections.Generic;


namespace PapercutSecurityIDSyncTests
{
    [TestClass]
    public class OracleServerTests
    {
        [TestMethod]
        public void PullUserIDs()
        {
            OracleServer oracleServer = new OracleServer(TestConfig.userHR, TestConfig.passHR, TestConfig.pathHR, TestConfig.userSC, TestConfig.passSC, TestConfig.pathSC);
            List<string> users = new List<string>();
            users.Add("pem4");
            users.Add("tdw3");
            users.Add("aa679");
            users.Add("agm38");
            List<UserInfo> userlist = oracleServer.GetUserList(users);
            foreach (UserInfo person in userlist)
            {
                Console.WriteLine(string.Concat("NetID: ", person.NetID));
                Console.WriteLine(string.Concat("ProxID: ", person.ProxID));
                Console.WriteLine(string.Concat("MagID: ", person.MagID));
                Console.WriteLine(" ");
            }
        }
    }
}
