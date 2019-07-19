using DataTables.AspNetCore.Mvc.Binder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace IsoBase.Data
{
    public class PageData
    {
        public string paternAngka { get; } = @"[^0-9 ,*]";
        public string paternHuruf { get; } = @"[^a-zA-Z ,*]";
        public string paternAngkaHuruf { get; } = @"[^0-9a-zA-Z ,*]";
        //public string paternAngka = @"[^0-9]";
        //public string paternHurufLike = @"[^a-zA-Z,%]";
        //public string paternHuruf = @"[^a-zA-Z]";

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

        public PageData(DataTablesRequest dataRequest)
        {
            this.dataReq = dataRequest;
            RowStart = dataReq.Start;
            RowLength = dataReq.Length;
            FilterWhere = new List<string>();
            GenerateOrderColumn();
            FilterString();
            CountAllData();
            CountFilterAllData();
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

        /*
         * Ubah Filter dalam bentuk list ke bentuk string untuk query
         */
        public void FilterString()
        {
            string filter = "";
            foreach (var fl in this.FilterWhere) filter = string.Concat(filter, fl);
            FilterWhereString = filter;
        }

        /*
         * Hanya untuk menggabungkan string dalam bentuk query
         */
        public string GenerateQueryData()
        {
            string Offset = string.Format(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", this.RowStart, this.RowLength);
            string query = string.Concat(this.select, this.Tabel, FilterWhereString, this.OrderKolom, Offset);
            return query;
        }

        void CountAllData()
        {
            string queryString = string.Concat("SELECT COUNT(1) ", this.Tabel);
            this.recordsTotal = 0;
        }

        void CountFilterAllData()
        {
            string queryString = string.Concat("SELECT COUNT(1) ", this.Tabel, FilterWhereString);
            this.recordsFilterd = 0;
        }
    }
}
