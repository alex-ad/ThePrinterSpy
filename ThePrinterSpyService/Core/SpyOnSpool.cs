using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using ThePrinterSpyService.Exceptions;
using ThePrinterSpyService.Models;

namespace ThePrinterSpyService.Core
{
    public class SpyOnSpool
    {
        public static PrintSpyDbContext PrintSpyContext = new PrintSpyDbContext();

        private readonly Computer _currentComputer;
        private readonly User _currentUser;
        //private readonly Dictionary<int, PrinterChangeNotification> _printersMonitor;
        private PrinterChangeNotification _printersMonitor;
        private readonly Dictionary<int, int> _pagesPrinted;

        public SpyOnSpool()
        {
            _currentComputer = Computer.Add(Environment.MachineName);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem");
            if (searcher == null)
            {
                Log.AddTextLine("ManagementObjectSearcher: WMI is unavailable");
                throw new ThePrinterSpyException("WMI is unavailable", "ManagementObjectSearcher");
            }
            
            ManagementObjectCollection collection = searcher.Get();
            string username = (string)collection.Cast<ManagementBaseObject>().First()["UserName"];
            var slash = username.LastIndexOf('\\');
            if (slash > 0) username = username.Substring(slash+1);

            searcher = new ManagementObjectSearcher($"SELECT SID FROM Win32_UserAccount WHERE name='{username}'");
            collection = searcher.Get();
            string sid = (string)collection.Cast<ManagementBaseObject>().First()["SID"];

            _currentUser = User.Add(username, sid);
            //_printersMonitor = new Dictionary<int, PrinterChangeNotification>();
            _printersMonitor = new PrinterChangeNotification();
            _pagesPrinted = new Dictionary<int, int>();
        }

        public async Task RunAsync()
        {
            await TaskEx.Run(() =>
            {
                try
                {
                    List<Printer> localPrinters = Printer.GetLocalPrinters(_currentComputer.Id, _currentUser.Id);
                    /*foreach (Printer p in localPrinters)
                    {
                        if (!p.Enabled) continue;
                        _printersMonitor.Add(p.Id, new PrinterChangeNotification(p.Name, p.Id));
                        _printersMonitor[p.Id].OnPrinterJobChange += OnPrinterJobChange;
                        _printersMonitor[p.Id].OnPrinterNameChange += OnPrinterNameChange;
                    }*/
                    _printersMonitor.OnPrinterJobChange += OnPrinterJobChange;
                    _printersMonitor.OnPrinterNameChange += OnPrinterNameChange;
                }
                catch (Exception ex)
                {
                    ProceedError(ex);
                }
            });
        }

        public void Stop()
        {
        }

        private void AddPrintJob(JobInfo job, int userId, int computerId, int serverId, int printerId)
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
        }

        private void ProceedError(Exception exception)
        {
            Log.AddException(exception);
            Stop();
        }

        private void OnPrinterJobChange(object sender, PrinterJobChangeEventArgs e)
        {
            if (!_pagesPrinted.ContainsKey(e.JobId))
                _pagesPrinted.Add(e.JobId, (int)e.JobInfo.PagesPrinted);
            else if (_pagesPrinted[e.JobId] == (int)e.JobInfo.PagesPrinted)
                return;
            _pagesPrinted[e.JobId] = (int)e.JobInfo.PagesPrinted;

            AddPrintJob(new JobInfo
            {
                PagesPrinted = e.JobInfo.PagesPrinted,
                pDocument = e.JobName,
                Submitted = e.JobInfo.Submitted,
                JobId = (uint)e.JobId
            }, _currentUser.Id, _currentComputer.Id, _currentComputer.Id, e.PrinterId);

            Debug.WriteLine($"INFO ::: {e.PrinterId} - {e.JobId} - {e.JobName} - {e.JobStatus} - {e.JobInfo.PagesPrinted}/{e.JobInfo.TotalPages}");
        }

        private void OnPrinterNameChange(object sender, PrinterNameChangeEventArgs e)
        {
            /*_printersMonitor[e.PrinterId].OnPrinterJobChange -= OnPrinterJobChange;
            _printersMonitor[e.PrinterId] = null;
            _printersMonitor[e.PrinterId] = new PrinterChangeNotification(e.PrinterName, e.PrinterId);
            _printersMonitor[e.PrinterId].OnPrinterJobChange += OnPrinterJobChange;*/
            Printer.Rename(e.PrinterId, e.PrinterName);
        }
    }
}
