using System;
using System.Linq;
using ThePrinterSpyService.Core;

namespace ThePrinterSpyService.Models
{
    public struct PrintDataStruct
    {
        public int PrinterId { get; set; }
        public int UserId { get; set; }
        public int ComputerId { get; set; }
        public int ServerId { get; set; }
        public string DocName { get; set; }
        public int Pages { get; set; }
        public DateTime TimeStamp { get; set; }
        public int JobId { get; set; }
    }

    public class PrintData
    {
        public int Id { get; set; }
        public int PrinterId { get; set; }
        public int UserId { get; set; }
        public int ComputerId { get; set; }
        public int ServerId { get; set; }
        public string DocName { get; set; }
        public int Pages { get; set; }
        public DateTime TimeStamp { get; set; }
        public int JobId { get; set; }

        public static PrintData AddOrUpdate(PrintDataStruct jobInfo) => IsExists(jobInfo) ? Update(jobInfo) : Add(jobInfo);

        public static PrintData Add(PrintDataStruct jobInfo)
        {
            PrintData printData = new PrintData
            {
                PrinterId = jobInfo.PrinterId,
                UserId = jobInfo.UserId,
                ComputerId = jobInfo.ComputerId,
                ServerId = jobInfo.ServerId,
                DocName = jobInfo.DocName,
                Pages = jobInfo.Pages,
                TimeStamp = jobInfo.TimeStamp,
                JobId = jobInfo.JobId
            };
            SpyOnSpool.PrintSpyContext.PrintDatas.Add(printData);
            SpyOnSpool.PrintSpyContext.SaveChanges();
            return printData;
        }

        public static PrintData Update(PrintDataStruct jobInfo)
        {
            PrintData printData = GetPrintData(jobInfo);
            if (printData.Pages == jobInfo.Pages)
                return printData;
            printData.Pages = jobInfo.Pages;
            SpyOnSpool.PrintSpyContext.Entry(printData).Property(d => d.Pages).IsModified = true;
            SpyOnSpool.PrintSpyContext.SaveChanges();
            return printData;
        }

        public static PrintData GetPrintData(PrintDataStruct jobInfo) => SpyOnSpool.PrintSpyContext.PrintDatas.FirstOrDefault(
            d => (d.JobId == jobInfo.JobId)
                 && (DateTime.Compare(d.TimeStamp, jobInfo.TimeStamp) == 0)
                 && (d.PrinterId == jobInfo.PrinterId)
                 && (d.UserId == jobInfo.UserId)
                 && (d.ComputerId == jobInfo.ComputerId)
                 && (d.ServerId == jobInfo.ServerId)
                 && (d.DocName == jobInfo.DocName));

        public static bool IsExists(PrintDataStruct jobInfo) => SpyOnSpool.PrintSpyContext.PrintDatas.FirstOrDefault(
            d => (d.JobId == jobInfo.JobId)
                 && (DateTime.Compare(d.TimeStamp, jobInfo.TimeStamp) == 0)
                 && (d.PrinterId == jobInfo.PrinterId)
                 && (d.UserId == jobInfo.UserId)
                 && (d.ComputerId == jobInfo.ComputerId)
                 && (d.ServerId == jobInfo.ServerId)
                 && (d.DocName == jobInfo.DocName)) != null;
    }
}
