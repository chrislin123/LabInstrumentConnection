using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Configuration;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Data;

namespace LabMachiLib
{
    public class EzMySQL
    {
        //string Connection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        string Connection = string.Empty;
        
        ArrayList arrayName = new ArrayList();
        ArrayList arrayValue = new ArrayList();
        ArrayList arrayType = new ArrayList();

        public EzMySQL(string sConnString)
        {
            Connection = sConnString;
        }

        /// <summary>
        /// 設定SQL操作SqlParameter時的 變數名稱、數值、數值型態 ex:Select * From Employ Where ID = @ID : Var(ID,"1")
        /// </summary>
        /// <param name="Name">變數名稱</param>
        /// <param name="Value">數值</param>
        /// <param name="Type">數值型態</param>
        public void Var(string Name, object Value, SqlDbType Type = SqlDbType.NVarChar)
        {
            arrayName.Add(Name);
            arrayValue.Add(Value);
            arrayType.Add(Type);
        }
        /// <summary>
        /// 根據Sql Statement 回傳 單一數值.查詢不到一律回傳空字串
        /// </summary>
        /// <param name="SQLstr">Sql Statement</param>
        /// <param name="ConnString">連線字串</param>
        /// <returns></returns>
        public string Get_Scalar(string SQLstr, string ConnString = "")
        {
            if (string.IsNullOrEmpty(ConnString)) ConnString = Connection;

            using (MySqlConnection conn = new MySqlConnection(ConnString))
            {
                using (MySqlCommand Cmd = new MySqlCommand(SQLstr, conn))
                {
                    if (arrayName.Count > 0)
                    {
                        for (int i = 0; i <= arrayName.Count - 1; i++)
                        {
                            Cmd.Parameters.AddWithValue("@" + arrayName[i], arrayType[i]).Value = arrayValue[i];
                        }
                    }

                    Cmd.CommandTimeout = 0;

                    conn.Open();
                    
                    var Result = Cmd.ExecuteScalar();

                    conn.Close();

                    if (Result != null)
                    {
                        return Result.ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }           
        }
        /// <summary>
        /// 執行無回傳值Sql Statement
        /// </summary>
        /// <param name="SQLstr">Sql Statement</param>
        /// <param name="ConnString">連線字串</param>
        public void ExecuteSQL(string SQLstr, string ConnString = "")
        {

            if (string.IsNullOrEmpty(ConnString)) ConnString = Connection;

            using (MySqlConnection conn = new MySqlConnection(ConnString))
            {
                using (MySqlCommand Cmd = new MySqlCommand(SQLstr, conn))
	            {
                    if (arrayName.Count > 0)
                    {
                        for (int i = 0; i <= arrayName.Count - 1; i++)
                        {
                            Cmd.Parameters.AddWithValue("@" + arrayName[i], arrayType[i]).Value = arrayValue[i];
                        }
                    }

                    conn.Open();                   

                    Cmd.ExecuteNonQuery();

                    conn.Close();    
	            }
            }            
        }
        /// <summary>
        /// 執行須具有Transection的SQL指令
        /// </summary>
        /// <param name="SQLstr">SQL Statement</param>
        /// <param name="ConnString">連線字串</param>
        public void ExecuteTransectionSQL(string SQLstr, string ConnString = "")
        {
            if (string.IsNullOrEmpty(ConnString)) ConnString = Connection;

            MySqlConnection conn = new MySqlConnection(ConnString);
            MySqlCommand Cmd = new MySqlCommand(SQLstr, conn);
            conn.Open();
            MySqlTransaction trans = conn.BeginTransaction();

            try
            {
                if (arrayName.Count > 0)
                {
                    for (int i = 0; i <= arrayName.Count - 1; i++)
                    {
                        Cmd.Parameters.AddWithValue("@" + arrayName[i], arrayType[i]).Value = arrayValue[i];
                    }
                }
                
                Cmd.Transaction = trans;

                Cmd.ExecuteNonQuery();
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                Cmd.Dispose();
                trans.Dispose();
            }



        }
        /// <summary>
        /// 執行查詢，回傳Datatable
        /// </summary>
        /// <param name="SQLstr">Sql Statement</param>
        /// <param name="ConnString">連線字串</param>
        /// <returns></returns>
        public DataTable Get_DataTable(string SQLstr, string ConnString = "")
        {
            if (string.IsNullOrEmpty(ConnString)) ConnString = Connection;

            DataTable dt = new DataTable();

            using (MySqlConnection conn = new MySqlConnection(ConnString))
            {
                using (MySqlCommand Cmd = new MySqlCommand(SQLstr, conn))
                {
                    Cmd.CommandTimeout = 0;

                    if (arrayName.Count > 0)
                    {
                        for (int i = 0; i <= arrayName.Count - 1; i++)
                        {
                            Cmd.Parameters.AddWithValue("@" + arrayName[i], arrayType[i]).Value = arrayValue[i];
                        }
                    }                    

                    MySqlDataAdapter da = new MySqlDataAdapter();
                    
                    da = new MySqlDataAdapter(Cmd);
                    da.Fill(dt);

                    conn.Close();
                }
            }

            return dt;
        }
        /// <summary>
        /// 執行查詢，回傳DataSet
        /// </summary>
        /// <param name="SQLstr">Sql Statement</param>
        /// <param name="ConnString">連線字串</param>
        /// <returns></returns>
        public DataSet Get_Dataset(string SQLstr, string ConnString = "")
        {
            if (string.IsNullOrEmpty(ConnString)) ConnString = Connection;

            DataSet ds = new DataSet();

            using (MySqlConnection conn = new MySqlConnection(ConnString))
            {
                using (MySqlCommand Cmd = new MySqlCommand(SQLstr, conn))
                {
                    Cmd.CommandTimeout = 0;

                    if (arrayName.Count > 0)
                    {
                        for (int i = 0; i <= arrayName.Count - 1; i++)
                        {
                            Cmd.Parameters.AddWithValue("@" + arrayName[i], arrayType[i]).Value = arrayValue[i];
                        }
                    }

                    MySqlDataAdapter da = new MySqlDataAdapter();

                    da = new MySqlDataAdapter(Cmd);
                    da.Fill(ds, "Table");

                    conn.Close();
                }
            }

            return ds;
        }
        /// <summary>
        /// 將查詢結果轉為ArrayList，需指定欄位名稱
        /// </summary>
        /// <param name="SQLstr">Sql Statement</param>
        /// <param name="Col">欄位名稱</param>
        /// <param name="ConnString">連線字串</param>
        /// <returns></returns>
        public ArrayList Get_Arraylist(string SQLstr, string Col, string ConnString = "")
        {

            DataTable dt = new DataTable();
            ArrayList array = new ArrayList();

            dt = Get_DataTable(SQLstr, ConnString);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    array.Add(dt.Rows[i][Col]);
                }
            }
            dt.Dispose();

            return array;


        }
        /// <summary>
        /// 清空變數
        /// </summary>
        public void Clear()
        {
            arrayName.Clear();
            arrayValue.Clear();
            arrayType.Clear();
        }

    }
}
