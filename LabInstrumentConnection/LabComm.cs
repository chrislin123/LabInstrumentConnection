using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabInstrumentConnection
{
    class LabComm
    {
        EzMySQL SQL = new EzMySQL();

        public void insertlog(string type, string log)
        {
            try
            {
                //去除單引號雙引號
                log = log.Replace("'", "").Replace(@"""", "");

                string time = DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString();
                string ssql = "insert into opd.labinstrumentlog (type,log,time) values('" + type + "','" + log + "','" + time + "')";

                SQL.ExecuteSQL(ssql);
                    
            }
            catch (Exception)
            {   
                
            }
        }
    }
}
