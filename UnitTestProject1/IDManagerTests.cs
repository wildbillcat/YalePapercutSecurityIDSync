using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PapercutSecurityIDSync;
using System.Collections.Generic;

namespace PapercutSecurityIDSyncTests
{
    [TestClass]
    public class IDManagerTests
    {
        [TestMethod]
        public void ProcessIDs()
        {
            PaperCutServer paperCutServer = new PaperCutServer(TestConfig.authToken, TestConfig.port, TestConfig.server);
            OracleServer oracleServer = new OracleServer(TestConfig.userHR, TestConfig.passHR, TestConfig.pathHR, TestConfig.userSC, TestConfig.passSC, TestConfig.pathSC);


            List<string> PaperCutUsers = paperCutServer.GetPaperCutUsers();
            List<UserInfo> UserList = oracleServer.GetUserList(PaperCutUsers);
            IDManager IDMan = new IDManager(TestConfig.IDFilePath);
            IDMan.WriteIDs(UserList);
            //paperCutServer.SetPaperCutUserIDs(IDFilePath);
        }

        [TestMethod]
        public void ProcessIDsUsingIDManager()
        {
            IDManager IDMan = new IDManager(TestConfig.IDFilePath);
            IDMan.OnTimedEvent(null, null);
            //paperCutServer.SetPaperCutUserIDs(IDFilePath);
        }

    }
}
