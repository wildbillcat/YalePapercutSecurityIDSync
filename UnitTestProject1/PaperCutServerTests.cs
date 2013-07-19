using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PapercutSecurityIDSync;
using System.Collections.Generic;


namespace PapercutSecurityIDSyncTests
{
    [TestClass]
    public class PaperCutServerTests
    {
        [TestMethod]
        public void PullPaperCutUsers()
        {
            PaperCutServer paperCutServer = new PaperCutServer(TestConfig.authToken, TestConfig.port, TestConfig.server);
            List<string> Users = paperCutServer.GetPaperCutUsers();
            foreach(string user in Users)
            {
                Console.WriteLine(user);
            }
        }
    }
}
