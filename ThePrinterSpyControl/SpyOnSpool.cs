using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;
using ThePrinterSpyControl.Models;

namespace ThePrinterSpyControl
{
    public class SpyOnSpool
    {
        public static PrintSpyModel PrintSpyContext = new PrintSpyModel();

        public enum Command
        {
            Stop = 0,
            Run = 1
        }
        public Command Status { get; private set; }

        private struct ErrorMessage
        {
            public ErrorMessage(string Message, string StackTrace)
            {
                this.Message = Message;
                this.StackTrace = StackTrace;
            }
            public string Message { get; private set; }
            public string StackTrace { get; private set; }
        }
        private readonly Computer _currentComputer;
        private readonly User _currentUser;

        public SpyOnSpool()
        {
            _currentComputer = Computer.AddComputer(Environment.MachineName);
            _currentUser = User.AddUser(Environment.UserName);
            Status = Command.Stop;
        }

        public async Task RunAsync()
        {
            Status = Command.Run;

            while (Status == Command.Run)
            {
                if (Status == Command.Run)
                    await Task.Run(() =>
                    {
                        try
                        {
                            List<Printer> localPrinters = Printer.GetLocalPrinters(_currentComputer.Id);
                            foreach (Printer p in localPrinters)
                            {
                                if (Status != Command.Run) break;
                                PrinterJobsQuery jobsQuery = new PrinterJobsQuery();
                                PrinterJobsQuery.JobInfo[] jobs = jobsQuery.GetJobs(p.Name);
                                if (jobs?.Length < 1) continue;
                                foreach (PrinterJobsQuery.JobInfo j in jobs)
                                {
                                    if (j.PagesPrinted == 0) continue;
                                    AddPrintJob(j, _currentUser.Id, _currentComputer.Id, p.ServerId, p.Id);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ProceedError(ex);
                        }
                    });
            }
        }

        public void Stop()
        {
            Status = Command.Stop;
        }

        private void AddPrintJob(PrinterJobsQuery.JobInfo job, int userId, int computerId, int serverId, int printerId)
        {
            PrintDataStruct jobInfo = new PrintDataStruct
            {
                PrinterId = printerId,
                UserId = userId,
                ComputerId = computerId,
                ServerId = serverId,
                DocName = job.pDocument,
                Pages = (int)job.PagesPrinted,
                TimeStamp = new DateTime(job.Submitted.wYear, job.Submitted.wMonth, job.Submitted.wDay, job.Submitted.wHour, job.Submitted.wMinute, job.Submitted.wSecond, DateTimeKind.Utc),
                JobId = (int)job.JobId
            };

            PrintData.AddOrUpdate(jobInfo);
            //File.AppendAllText(@"D:\print.txt", job.pDocument);
        }

        private void AddLog(string message)
        {
            File.AppendAllText(@"Logs\exceptions.txt", message);
        }

        private string FormatLogMsg(ErrorMessage errorMessage)
        {
            return $"[ERROR] {DateTime.Now.ToString("HH:mm:ss")}: [Message] {errorMessage.Message} [Stack] {errorMessage.StackTrace}";
        }

        private void ProceedError(Exception exception)
        {
            Status = Command.Stop;
            AddLog(FormatLogMsg(new ErrorMessage(exception.Message, exception.StackTrace)));
        }
    }
}
