using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using ThePrinterSpyService.Models;
using ThePrinterSpyService.Exceptions;

namespace ThePrinterSpyService.Core
{
    public class SpyOnSpool
    {
        public static PrintSpyDbContext PrintSpyContext = new PrintSpyDbContext();

        private readonly Computer _currentComputer;
        private readonly User _currentUser;
        private PrinterChangeNotification _printersMonitor;
        private readonly Dictionary<int, int> _pagesPrinted;
        private List<Printer> _localPrinters;

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
            string userName = (string)collection.Cast<ManagementBaseObject>().First()["UserName"];
            var slash = userName.LastIndexOf('\\');
            if (slash > 0) userName = userName.Substring(slash + 1);

            searcher = new ManagementObjectSearcher($"SELECT SID FROM Win32_UserAccount WHERE name='{userName}'");
            collection = searcher.Get();
            string sid = (string)collection.Cast<ManagementBaseObject>().First()["SID"];

            _currentUser = User.Add(userName, sid);
            _pagesPrinted = new Dictionary<int, int>();
            _localPrinters = new List<Printer>();
        }

        public async Task RunAsync()
        {
            await TaskEx.Run(() =>
            {
                try
                {
                    _printersMonitor = new PrinterChangeNotification(_currentComputer.Name);
                    _localPrinters = Printer.BuildLocalPrintersList(_currentComputer.Id, _currentUser.Id);
                    _printersMonitor.OnPrinterJobChange += OnPrinterJobChange;
                    _printersMonitor.OnPrinterNameChange += OnPrinterNameChange;
                    _printersMonitor.OnPrinterAddedDeleted += _printersMonitor_OnPrinterAddedDeleted;
                }
                catch (Exception ex)
                {
                    ProceedError(ex);
                }
            });
        }

        public void Stop()
        {
            _printersMonitor?.Stop();
            _printersMonitor = null;
        }

        private void AddPrintJob(JobInfo job, int userId, int computerId, int serverId, int printerId)
        {
            var dtUtc = new DateTime(job.Submitted.wYear, job.Submitted.wMonth, job.Submitted.wDay, job.Submitted.wHour,
                job.Submitted.wMinute, job.Submitted.wSecond, DateTimeKind.Utc);
            var dtLocal = TimeZoneInfo.ConvertTimeFromUtc(dtUtc, TimeZoneInfo.Local);

            PrintDataStruct jobInfo = new PrintDataStruct
            {
                PrinterId = printerId,
                UserId = userId,
                ComputerId = computerId,
                ServerId = serverId,
                DocName = job.pDocument,
                Pages = (int)job.PagesPrinted,
                TimeStamp = dtLocal,
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
            if (string.IsNullOrEmpty(e.JobInfo.pPrinterName) || string.IsNullOrEmpty(e.JobInfo.pDocument) ||
                string.IsNullOrEmpty(e.JobInfo.pMachineName) || string.IsNullOrEmpty(e.JobInfo.pUserName)) return;

            var computer = Computer.Get(e.JobInfo.pMachineName.Replace("\\", ""));
            if (computer == null) return;
            var user = User.GetByName(TruncateElemName(e.JobInfo.pUserName));
            if (user == null) return;
            var printer = Printer.Get(TruncateElemName(e.JobInfo.pPrinterName), computer.Id, user.Id);
            if (printer == null || !printer.Enabled) return;

            if (!_pagesPrinted.ContainsKey(e.JobId))
                _pagesPrinted.Add(e.JobId, (int)e.JobInfo.PagesPrinted);
            else if (_pagesPrinted[e.JobId] == (int)e.JobInfo.PagesPrinted)
                return;
            _pagesPrinted[e.JobId] = (int)e.JobInfo.PagesPrinted;

            Debug.WriteLine("2. jobId: " + e.JobId.ToString());
            Debug.WriteLine("2. pPrinterName: " + printer.Name);
            Debug.WriteLine("2. pMachineName: " + computer.Name);
            Debug.WriteLine("2. pUserName: " + user.AccountName);
            Debug.WriteLine("2. pDocument: " + e.JobInfo.pDocument);
            Debug.WriteLine("2. PagesPrinted: " + e.JobInfo.PagesPrinted);

            AddPrintJob(new JobInfo
            {
                PagesPrinted = e.JobInfo.PagesPrinted,
                pDocument = e.JobInfo.pDocument,
                Submitted = e.JobInfo.Submitted,
                JobId = (uint)e.JobId
            }, printer.UserId, printer.ComputerId, printer.ServerId, printer.Id);
        }

        private void OnPrinterNameChange(object sender, PrinterNameChangeEventArgs e)
        {
            Printer.Rename(_currentComputer.Id, _currentUser.Id, e.PrinterName, ref _localPrinters);
        }

        private void _printersMonitor_OnPrinterAddedDeleted(object sender, PrinterNameChangeEventArgs e)
        {
            _localPrinters = Printer.BuildLocalPrintersList(_currentComputer.Id, _currentUser.Id);
        }

        private string TruncateElemName(string name)
        {
            var slash = name.LastIndexOf('\\');
            if (slash > 0) name = name.Substring(slash + 1);
            return name;
        }
    }
}
