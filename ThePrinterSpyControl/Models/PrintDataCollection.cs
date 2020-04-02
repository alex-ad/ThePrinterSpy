using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThePrinterSpyControl.Models
{
    class PrintDataCollection
    {
        private static int _id = 0;
        private ObservableCollection<PrintData> PrintDatas { get; }

        public PrintDataCollection()
        {
            PrintDatas = new ObservableCollection<PrintData>();
            PrintDatas.CollectionChanged += PrintDatas_CollectionChanged;
        }

        public PrintData AddOrUpdate(PrintDataStruct jobInfo) => IsExists(jobInfo) ? Update(jobInfo) : Add(jobInfo);

        public PrintData Add(PrintDataStruct jobInfo)
        {
            _id++;
            PrintData printData = new PrintData(_id, jobInfo);
            PrintDatas.Add(printData);
            return printData;
        }

        public PrintData Update(PrintDataStruct jobInfo)
        {
            PrintData printDataOld = GetPrintData(jobInfo);
            PrintData prinDataNew = new PrintData(printDataOld.Id, jobInfo);
            PrintDatas[PrintDatas.IndexOf(printDataOld)] = prinDataNew;
            return prinDataNew;
        }

        private PrintData GetPrintData(PrintDataStruct jobInfo) => PrintDatas.FirstOrDefault(d => (d.JobId == jobInfo.JobId) && (DateTime.Compare(d.TimeStamp, jobInfo.TimeStamp) == 0));

        private bool IsExists(PrintDataStruct jobInfo) => PrintDatas.FirstOrDefault(d => (d.JobId == jobInfo.JobId) && (DateTime.Compare(d.TimeStamp, jobInfo.TimeStamp) == 0)) != null;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (PrintData d in PrintDatas)
            {
                sb.AppendLine($@"Id: {d.Id}/{PrintDatas.Count}; PrinterId: {d.PrinterId}; UserId: {d.UserId}; ServerId: {d.ServerId}; ComputerId: {d.ComputerId}; DocName: {d.DocName}; Pages: {d.Pages}; TimeStamp: {d.TimeStamp}; JobId: {d.JobId}");
            }
            return sb.ToString();
        }

        private void PrintDatas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            return;
        }
    }
}
