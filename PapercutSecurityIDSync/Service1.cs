using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PapercutSecurityIDSync
{
    public partial class Service1 : ServiceBase
    {

        IDManager idmgr;


        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            idmgr = new IDManager();
            GC.KeepAlive(idmgr);
        }

        protected override void OnStop()
        {
            
        }
    }
}
