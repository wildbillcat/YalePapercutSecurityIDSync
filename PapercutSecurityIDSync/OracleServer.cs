using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using System.Data;
using System.Data.Common;

namespace PapercutSecurityIDSync
{
    public class OracleServer
    {
        private string constrHR;
        private string constrSC;
        private string ProviderName = "Oracle.DataAccess.Client";
        private DbProviderFactory factory;

        public OracleServer(string userHR, string passHR, string pathHR, string userSC, string passSC, string pathSC)
        {
            constrHR = "User Id=" + userHR + "; Password=" + passHR + "; Data Source=" + pathHR + ";";
            constrSC = "User Id=" + userSC + "; Password=" + passSC + "; Data Source=" + pathSC + ";";
            factory = DbProviderFactories.GetFactory(ProviderName);
        }

        /// <summary>
        /// Retrieves the list of users badge Numbers
        /// </summary>
        /// <param name="NetIDs">List of Users' NetIDs</param>
        public List<UserInfo> GetUserList(List<string> NetIDs)
        {
            using (DbConnection humanResources = factory.CreateConnection())
            {
                using (DbConnection securityResources = factory.CreateConnection())
                {
                    try
                    {
                        List<UserInfo> usersList = new List<UserInfo>();

                        humanResources.ConnectionString = constrHR;
                        humanResources.Open();

                        securityResources.ConnectionString = constrSC;
                        securityResources.Open();

                        OracleCommand cmdHR = (OracleCommand)factory.CreateCommand();
                        cmdHR.Connection = (OracleConnection)humanResources;

                        OracleCommand cmdSEC = (OracleCommand)factory.CreateCommand();
                        cmdSEC.Connection = (OracleConnection)securityResources;

                        string sqlQueryHR = "SELECT upi FROM yuapps.yuhr_current_active_people WHERE (net_id = :NetID)";
                        string sqlQuerySEC = "SELECT bid, bid_format_id FROM pic_prod.badge_validation_v WHERE (logical_status = 'ACTIVE') AND (upi = :upi)";

                        cmdHR.CommandText = sqlQueryHR;
                        cmdSEC.CommandText = sqlQuerySEC;

                        foreach (string NetID in NetIDs)
                        {
                            try
                            {
                                cmdHR.Parameters.Add(":NetID", NetID.ToUpper().ToCharArray());
                                string upiNumber = (string)cmdHR.ExecuteScalar();
                                cmdSEC.Parameters.Add(":upi", upiNumber.ToCharArray());
                                OracleDataReader securityReader = cmdSEC.ExecuteReader();
                                string MagID = null;
                                string ProxID = null;
                                while (securityReader.Read())
                                {
                                    string BID = (string)securityReader.GetValue(0);
                                    int BIDType = securityReader.GetInt32(1);
                                    if (BIDType == UserInfo.Magstrip)
                                    {
                                        MagID = BID;
                                    }
                                    else if (BIDType == UserInfo.Prox)
                                    {
                                        ProxID = BID;
                                    }
                                }
                                usersList.Add(new UserInfo(NetID, MagID, ProxID));  
                            }
                            catch (Exception ex)
                            {
                                //Error specific to the user, skip user.
                                Console.WriteLine(NetID);
                                Console.WriteLine(ex.Message);
                            }
                            cmdHR.Parameters.Clear();
                            cmdSEC.Parameters.Clear();
                        }
                        return usersList;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                        return null;
                    }
                }
            }
        }

    }

    public struct UserInfo
    {
        public string NetID;
        public string MagID;
        public string ProxID;

        public static int Magstrip = 1;
        public static int Prox = 2;

        public UserInfo(string NetID, string MagID, string ProxID)
        {
            this.NetID = NetID;
            this.MagID = MagID;
            this.ProxID = ProxID;
        }
    }
}
