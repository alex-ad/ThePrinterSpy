using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ThePrinterSpyService.Models;
using System.Security.Principal;

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

            var identity = WindowsIdentity.GetCurrent();
            var userName = identity.Name;
            var slash = userName.LastIndexOf('\\');
            if (slash > 0) userName = userName.Substring(slash + 1);
            var sid = identity.Owner?.Value ?? "";

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
                    _printersMonitor = new PrinterChangeNotification(_currentUser.Id, _currentComputer.Id, _currentComputer.Name);
                    _localPrinters = Printer.BuildLocalPrintersList(_currentComputer.Id, _currentUser.Id);
                    _printersMonitor.OnPrinterJobChange += OnPrinterJobChange;
                    _printersMonitor.OnPrinterNameChange += OnPrinterNameChange;
                    _printersMonitor.OnPrinterAddedDeleted += _printersMonitor_OnPrinterAddedDeleted;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("error: "+ex.Message);
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
        }

        private void OnPrinterNameChange(object sender, PrinterNameChangeEventArgs e)
        {
            Printer.Rename(_currentComputer.Id, _currentUser.Id, e.PrinterName, ref _localPrinters);
        }

        private void _printersMonitor_OnPrinterAddedDeleted(object sender, PrinterNameChangeEventArgs e)
        {
            _localPrinters = Printer.BuildLocalPrintersList(_currentComputer.Id, _currentUser.Id);
        }
    }
}
