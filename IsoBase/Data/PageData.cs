using DataTables.AspNetCore.Mvc.Binder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace IsoBase.Data
{
    public class PageData
    {
        public string paternAngkaLike { get; } = @"[^0-9 ,*]";
        public string paternHurufLike { get; } = @"[^a-zA-Z ,*]";
        public string paternAngkaHurufLike { get; } = @"[^0-9a-zA-Z ,*]";
        public string paternAngka = @"[^0-9]";
        //public string paternHurufLike = @"[^a-zA-Z,%]";
        public string paternHuruf = @"[^a-zA-Z]";

        public int RowStart { get; }
        public int RowLength { get; }
        public int recordsTotal { get; set; } = 0;
        public int recordsFilterd { get; set; } = 0;

        public String select { get; set; }
        public String Tabel { get; set; }
        public List<String> FilterWhere;
        String FilterWhereString = "";
        String OrderKolom = "";

        public DataTablesRequest dataReq;
        private readonly ApplicationDbContext _context;

        public PageData(DataTablesRequest dataRequest, ApplicationDbContext DBcontext)
        {
            _context = DBcontext;
            this.dataReq = dataRequest;
            RowStart = dataReq.Start;
            RowLength = dataReq.Length;
            FilterWhere = new List<string>();
            GenerateOrderColumn();
        }

        public void CountData()
        {
            try
            {
                /*
                 * Urutan gak boleh terbalik, generate filter dulu kemudian hitung data
                 */
                GenerateFilterString();
                CountAllData();
                CountFilterAllData();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void GenerateOrderColumn()
        {
            foreach (var req in dataReq.Orders) OrderKolom = " ORDER BY " + string.Format(" {0} {1} ", (req.Column + 1).ToString(), req.Dir.ToUpper());
        }

        /*
         * Cleansing teks yang akan di search sebelum dieksekusi dalam query dengan pola seperti static patern variable diatas
         * selalu diawali dengan 'AND' contoh hasil nya
         * AND cm.Name LIKE 'reli%' 
         * AND cm.ClientID LIKE '5'
         * 
         * Kemudian dikumpulkan dulu dalam bentuk List String
         */
        public void AddWhereRegex(string PaternRegex, string SearchValue, string KolomTabel)
        {
            var tmp = " AND " + KolomTabel + " LIKE '" + Regex.Replace(SearchValue, PaternRegex, "").Replace("*", "%") + "'";
            this.FilterWhere.Add(tmp);
        }

        //public static DataTable ListData(SqlConnection conn, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        //{
        //    DataTable dt = new DataTable();
        //    SqlDataAdapter da = new SqlDataAdapter(cmdText, conn);
        //    da.Fill(dt);
        //    return dt;
        //}

        public DataTable ListData()
        {
            var conn = (SqlConnection)_context.Database.GetDbConnection().CreateCommand().Connection;
            string queryString = this.GenerateQueryData();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(queryString, conn);
            da.Fill(dt);
            return dt;
        }

        //public DataTable ListData()
        //{
        //    string queryString = this.GenerateQueryData();
        //    var cmd = (SqlCommand)_context.Database.GetDbConnection().CreateCommand();
        //    cmd.CommandType = CommandType.Text;
        //    cmd.CommandText = queryString;
        //    DataTable dt = new DataTable();
        //    SqlDataAdapter adp = new SqlDataAdapter();
        //    try
        //    {
        //        adp.SelectCommand = cmd;
        //        if (cmd.Connection.State == ConnectionState.Closed) cmd.Connection.Open();


        //    }
        //    catch (Exception ex) { throw new Exception(ex.Message); }
        //    finally { if (cmd.Connection.State == ConnectionState.Open) cmd.Connection.Close(); }

        //    return dt;
        //}


        /*
         * Ubah Filter dalam bentuk list ke bentuk string untuk query
         */
        public void GenerateFilterString()
        {
            string filter = "";
            foreach (var fl in this.FilterWhere) filter = string.Concat(filter, fl);
            FilterWhereString = filter;
        }

        /*
         * Hanya untuk menggabungkan string dalam bentuk query
         */
        string GenerateQueryData()
        {
            string Offset = string.Format(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", this.RowStart, this.RowLength);
            string query = string.Concat(this.select, this.Tabel, FilterWhereString, this.OrderKolom, Offset);
            return query;
        }

        // Hitung jumlah semua data tanpa filter
        void CountAllData()
        {
            string queryString = string.Format("SELECT COUNT(1) {0} ", this.Tabel);
            var jlh = GetScalarValue(queryString);
            this.recordsTotal = int.Parse(jlh);
        }

        // Hitung jumlah data dengan filter
        void CountFilterAllData()
        {
            string queryString = string.Format("SELECT COUNT(1) {0} {1} ", this.Tabel, FilterWhereString);
            var jlh = GetScalarValue(queryString);
            this.recordsFilterd = int.Parse(jlh);
        }

        string GetScalarValue(string QueryForScalar)
        {
            string result = "0";
            var cmd = _context.Database.GetDbConnection().CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = QueryForScalar;
            try
            {
                if (cmd.Connection.State == ConnectionState.Closed) cmd.Connection.Open();
                result = cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (cmd.Connection.State == ConnectionState.Open) cmd.Connection.Close();
            }
            return result;
        }
    }
}
