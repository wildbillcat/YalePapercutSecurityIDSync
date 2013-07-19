using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapercutSecurityIDSync
{

    public class PaperCutServer
    {

        ServerCommandProxy serverProxy;
        
        public PaperCutServer(string authToken, int port, string server)
        {
            serverProxy = new ServerCommandProxy(server, port, authToken);
        }

        public List<string> GetPaperCutUsers()
        {
            List<string> paperCutUsers = new List<string>();
            int noUsers = this.serverProxy.GetTotalUsers();
            int noUsers2 = noUsers;
            int i = 0;
            while (noUsers2 > 0)
            {
                if ((i + 1) * 1000 < noUsers)
                {
                    paperCutUsers.AddRange(this.serverProxy.ListUserAccounts(i * 1000, (i + 1) * 1000).ToList());
                }
                else
                {
                    paperCutUsers.AddRange(this.serverProxy.ListUserAccounts(i * 1000, noUsers).ToList());
                }
                i++;
                noUsers2 = noUsers2 - 1000;
            }
            paperCutUsers = paperCutUsers.Distinct<string>().ToList<string>();
            return paperCutUsers;
        }

        public void SetPaperCutUserIDs(string file)
        {
            serverProxy.BatchImportUserCardIdNumbers(file, true); //File = path to the file, true = notes to overwrite user pin
        }

    }
}
