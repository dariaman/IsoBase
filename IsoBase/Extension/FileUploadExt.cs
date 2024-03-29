﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IsoBase.Extension
{
    public class FileUploadExt
    {
        private static Random random = new Random();
        private IConfigurationRoot Configuration { get; set; }

        public string DirPlanExcel { get; private set; }

        public FileUploadExt()
        {
            var builder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            DirPlanExcel = Configuration.GetValue<string>("FileSetting:FilePlanExcel");
        }

        public async Task<string> BackupFile(int StorageType, IFormFile Fileupload)
        {
            /*
             * 1. direktori File Upload Plan Excel
             */

            string pathDir = "";
            switch (StorageType)
            {
                case 1:
                    pathDir = this.DirPlanExcel;
                    break;
            }
            string FullPath;
            FileStream stream;
            try
            {
                FullPath = Path.Combine(pathDir, Fileupload.FileName + RandomString(12));
                stream = new FileStream(FullPath, FileMode.Create);
                await Fileupload.CopyToAsync(stream);
                stream.Close();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return FullPath;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
