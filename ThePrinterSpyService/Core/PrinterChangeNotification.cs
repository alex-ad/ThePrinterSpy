using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace ThePrinterSpyService.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct JobInfo // JOB_INFO_1W
    {
        public uint JobId;
        [MarshalAs(UnmanagedType.LPWStr)] public string pPrinterName;
        [MarshalAs(UnmanagedType.LPWStr)] public string pMachineName;
        [MarshalAs(UnmanagedType.LPWStr)] public string pUserName;
        [MarshalAs(UnmanagedType.LPWStr)] public string pDocument;
        [MarshalAs(UnmanagedType.LPWStr)] public string pDatatype;
        [MarshalAs(UnmanagedType.LPWStr)] public string pStatus;
        public uint Status;
        public uint Priority;
        public uint Position;
        public uint TotalPages;
        public uint PagesPrinted;
        public SystemTime Submitted;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTime
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;
    }

    public class PrinterJobChangeEventArgs : EventArgs
    {
        public int PrinterId { get; }
        public int JobId { get; }
        public string JobName { get; }
        public JOBSTATUS JobStatus { get; }
        public JobInfo JobInfo { get; }
        public PrinterJobChangeEventArgs(int printerId, int jobId, string jobName, JOBSTATUS jobStatus, JobInfo jobInfo)
        {
            PrinterId = printerId;
            JobId = jobId;
            JobName = jobName;
            JobStatus = jobStatus;
            JobInfo = jobInfo;
        }
    }

    public class PrinterNameChangeEventArgs : EventArgs
    {
        public int PrinterId { get; }
        public string PrinterName { get; }

        public PrinterNameChangeEventArgs( int printerId, string printerName)
        {
            PrinterId = printerId;
            PrinterName = printerName;
        }
    }

    public delegate void PrinterJobChanged(object sender, PrinterJobChangeEventArgs e);
    public delegate void PrinterNameChanged(object sender, PrinterNameChangeEventArgs e);

    class PrinterChangeNotification
    {
        private readonly string _printerName;
        private readonly int _printerId;
        private IntPtr _printerHandle = IntPtr.Zero;
        private IntPtr _changeHandle = IntPtr.Zero;
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);
        private readonly PRINTER_NOTIFY_OPTIONS _notifyOptions = new PRINTER_NOTIFY_OPTIONS();
        private readonly Dictionary<int, string> _jobDocNames = new Dictionary<int, string>();
        private static class NativeMethods
        {
            [DllImport("winspool.drv", EntryPoint = "OpenPrinterW", SetLastError = true, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool OpenPrinterW([In][MarshalAs(UnmanagedType.LPWStr)] string pPrinterName, [Out] out IntPtr phPrinter, [In] IntPtr pDefault);

            [DllImport("winspool.drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool ClosePrinterW(Int32 hPrinter);

            [DllImport("winspool.drv", EntryPoint = "GetJobW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetJobW([In] IntPtr hPrinter, [In] Int32 jobId, [In] Int32 level, [Out] byte[] pJob, [In] Int32 cbBuf, ref Int32 lpbSizeNeeded);

            [DllImport("winspool.drv", EntryPoint = "FindFirstPrinterChangeNotification", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr FindFirstPrinterChangeNotification([In] IntPtr hPrinter, [In] Int32 fwFlags, [In] Int32 fwOptions, [In, MarshalAs(UnmanagedType.LPStruct)] PRINTER_NOTIFY_OPTIONS pPrinterNotifyOptions);

            [DllImport("winspool.drv", EntryPoint = "FindNextPrinterChangeNotification", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
            public static extern bool FindNextPrinterChangeNotification
            ([In] IntPtr hChangeObject, [Out] out Int32 pdwChange, [In, MarshalAs(UnmanagedType.LPStruct)] PRINTER_NOTIFY_OPTIONS pPrinterNotifyOptions, [Out] out IntPtr lppPrinterNotifyInfo);
        }

        public event PrinterJobChanged OnPrinterJobChange;
        public event PrinterNameChanged OnPrinterNameChange;

        public PrinterChangeNotification(string pName, int pId)
        {
            _printerName = pName;
            _printerId = pId;
            Start();
        }

        ~PrinterChangeNotification()
        {
            Stop();
        }

        private void Start()
        {
            NativeMethods.OpenPrinterW(_printerName, out _printerHandle, IntPtr.Zero);
            _changeHandle = NativeMethods.FindFirstPrinterChangeNotification(_printerHandle, (int)(PRINTER_CHANGES.PRINTER_CHANGE_JOB + PRINTER_CHANGES.PRINTER_CHANGE_PRINTER), 0, _notifyOptions);
            _resetEvent.Handle = _changeHandle;
            ThreadPool.RegisterWaitForSingleObject(_resetEvent, PrinterNotifyWaitCallback, _resetEvent, -1, true);
        }

        private void Stop()
        {
            if (_printerHandle != IntPtr.Zero)
            {
                NativeMethods.ClosePrinterW((int)_printerHandle);
                _printerHandle = IntPtr.Zero;
            }
        }

        private void PrinterNotifyWaitCallback(object state, bool timeOut)
        {
            if (_printerHandle == IntPtr.Zero) return;
            _notifyOptions.Count = 1;
            IntPtr pNotifyInfo = IntPtr.Zero;
            bool bResult = NativeMethods.FindNextPrinterChangeNotification(_changeHandle, out int pdwChange, _notifyOptions, out pNotifyInfo);
            if ((bResult == false) || (((int)pNotifyInfo) == 0)) return;

            bool relatedChange =
                ((pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_ADD_JOB) == PRINTER_CHANGES.PRINTER_CHANGE_ADD_JOB) ||
                ((pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_SET_JOB) == PRINTER_CHANGES.PRINTER_CHANGE_SET_JOB) ||
                ((pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_DELETE_JOB) == PRINTER_CHANGES.PRINTER_CHANGE_DELETE_JOB) ||
                ((pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_WRITE_JOB) == PRINTER_CHANGES.PRINTER_CHANGE_WRITE_JOB) ||
                ((pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_SET_PRINTER) == PRINTER_CHANGES.PRINTER_CHANGE_SET_PRINTER);
            if (!relatedChange) return;

            Debug.WriteLine("+++"+ pdwChange);

            PRINTER_NOTIFY_INFO info = (PRINTER_NOTIFY_INFO)Marshal.PtrToStructure(pNotifyInfo, typeof(PRINTER_NOTIFY_INFO));
            int pData = (int)pNotifyInfo + Marshal.SizeOf(typeof(PRINTER_NOTIFY_INFO));
            PRINTER_NOTIFY_INFO_DATA[] data = new PRINTER_NOTIFY_INFO_DATA[info.Count];
            for (uint i = 0; i < info.Count; i++)
            {
                data[i] = (PRINTER_NOTIFY_INFO_DATA)Marshal.PtrToStructure((IntPtr)pData, typeof(PRINTER_NOTIFY_INFO_DATA));
                pData += Marshal.SizeOf(typeof(PRINTER_NOTIFY_INFO_DATA));
            }

            for (int i = 0; i < data.Count(); i++)
            {
                if ((data[i].Field == (ushort)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_STATUS) &&
                        ((data[i].Type == (ushort)PRINTERNOTIFICATIONTYPES.JOB_NOTIFY_TYPE)))
                {
                    PrinterJobNotification(data[i]);
                }
                else if ((data[i].Field == (ushort)PRINTERPRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_FIELD_PRINTER_NAME) &&
                         ((data[i].Type == (ushort)PRINTERNOTIFICATIONTYPES.PRINTER_NOTIFY_TYPE)))
                {
                    PrinterNameNotification(data[i]);
                }
            }

            _resetEvent.Reset();
            ThreadPool.RegisterWaitForSingleObject(_resetEvent, PrinterNotifyWaitCallback, _resetEvent, -1, true);
        }

        private JobInfo GetJob(int jobId)
        {
            var bytesWritten = new Int32();
            var ptBuf = new byte[0];

            JobInfo jobInfo = new JobInfo();

            NativeMethods.GetJobW(_printerHandle, jobId, 1, ptBuf, 0, ref bytesWritten);
            if (bytesWritten > 0)
                ptBuf = new byte[bytesWritten];

            if (NativeMethods.GetJobW(_printerHandle, jobId, 1, ptBuf, bytesWritten, ref bytesWritten))
            {
                GCHandle handle = GCHandle.Alloc(ptBuf, GCHandleType.Pinned);
                jobInfo = (JobInfo)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(JobInfo));
                handle.Free();

                return jobInfo;
            }
            return jobInfo;
        }

        private void PrinterJobNotification(PRINTER_NOTIFY_INFO_DATA data)
        {
            JOBSTATUS jStatus = (JOBSTATUS)Enum.Parse(typeof(JOBSTATUS), data.NotifyData.Data.cbBuf.ToString());
            int jobId = (int)data.Id;
            string jobDocName;
            JobInfo jobInfo;

            try
            {
                jobInfo = GetJob(jobId);
                if (jobInfo.PagesPrinted == 0) return;
                if (!_jobDocNames.ContainsKey(jobId))
                    _jobDocNames[jobId] = jobInfo.pDocument;
                jobDocName = jobInfo.pDocument;
            }
            catch
            {
                _jobDocNames.TryGetValue(jobId, out jobDocName);
                return;
            }
            OnPrinterJobChange?.Invoke(this, new PrinterJobChangeEventArgs(_printerId, jobId, jobDocName, jStatus, jobInfo));
        }

        private void PrinterNameNotification(PRINTER_NOTIFY_INFO_DATA data)
        {
            OnPrinterNameChange?.Invoke(this, new PrinterNameChangeEventArgs(_printerId, Marshal.PtrToStringUni(data.NotifyData.Data.pBuf)));
        }
    }
}