using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ThePrinterSpyControl.Commands;
using ThePrinterSpyControl.DataBase;
using ThePrinterSpyControl.ModelBuilders;
using ThePrinterSpyControl.Models;
using ThePrinterSpyControl.Modules;
using ThePrinterSpyControl.Views;

namespace ThePrinterSpyControl.ViewModels
{
    public class PrinterSpyViewModel
    {
        public enum PrintDataGroup : byte
        {
            User = 1,
            Printer = 2,
            PrintersGroup = 3,
            Computer = 4,
            Department = 5
        }

        public static UsersCollection UsersCollection { get; private set; }
        public static PrintersCollection PrintersCollection { get; private set; }
        public static ComputersCollection ComputersCollection { get; private set; }
        public static DepartmentsCollection DepartmentsCollection { get; private set; }
        private static ServersCollection ServersCollection { get; set; }
        public static PrinterMaskedNameCollection PrinterNamesCollection { get; private set; }
        public ObservableCollection<PrintDataGrid> PrintDatas { get; }
        public static TotalCountStat TotalStat { get; private set; }
        public static SelectedPrinter SelectedPrinter { get; private set; }
        public AppConfig AppConfig { get; }

        private readonly DBase _base;
        private RelayCommand<string> _showOptionsWindowCommand;
        private RelayCommand<string> _showAboutWindowCommand;
        public RelayCommand<string> ShowOptionsWindowCommand => _showOptionsWindowCommand ?? (_showOptionsWindowCommand = new RelayCommand<string>(ShowOptionsWindows, CanShowOptionsWindows));
        public RelayCommand<string> ShowAboutWindowCommand => _showAboutWindowCommand ?? (_showAboutWindowCommand = new RelayCommand<string>(ShowAboutWindows, CanShowAboutWindows));

        public PrinterSpyViewModel()
        {
            _base = new DBase();
            TotalStat = new TotalCountStat();
            PrintersCollection = new PrintersCollection();
            UsersCollection = new UsersCollection();
            ComputersCollection = new ComputersCollection();
            DepartmentsCollection = new DepartmentsCollection();
            PrinterNamesCollection = new PrinterMaskedNameCollection();
            ServersCollection = new ServersCollection();
            PrintDatas = new ObservableCollection<PrintDataGrid>();
            AppConfig = new AppConfig();
            SelectedPrinter = new SelectedPrinter();

            GetAll();
        }

        public void BuildPrintDataCollection(object id, PrintDataGroup group)
        {
            if (group == PrintDataGroup.Computer)
                BuildDataByComputerId((int)id);
            else if (group == PrintDataGroup.Printer)
                BuildDataByPrinterId((int)id);
            else if (group == PrintDataGroup.Department)
                BuildDataByDepartmentId((string)id);
            else if (group == PrintDataGroup.PrintersGroup)
                BuildDataByPrintersGroup((List<int>)id);
            else BuildDataByUserId((int)id);
        }

        private void BuildDataByUserId(int id)
        {
            PrintDatas.Clear();
            TotalStat.PagesByNode = 0;
            TotalStat.DocsByNode = 0;

            var data = _base.GetDataByUserId(id, AppConfig.ReportDate.Start, AppConfig.ReportDate.End,
                AppConfig.ReportDate.IsEnabled).Result;
            if (!data.Any()) return;

            int totalPages = 0; int totalDocs = 0;
            foreach (var d in data)
            {
                totalPages += d.Data.Pages;
                totalDocs++;
                string userName = (d.User.FullName?.Length > 0) ? d.User.FullName : d.User.AccountName;
                PrintDatas.Add(new PrintDataGrid
                {
                    DocName = d.Data.DocName,
                    Pages = d.Data.Pages,
                    TimeStamp = d.Data.TimeStamp,
                    UserName = userName,
                    PrinterName = d.Printer.Name,
                    ComputerName = d.Computer.Name
                });
            }

            TotalStat.PagesByNode = totalPages;
            TotalStat.DocsByNode = totalDocs;
            TotalStat.ReportPeriod = AppConfig.ReportDate.ToString();
        }

        private void BuildDataByDepartmentId(string name)
        {
            PrintDatas.Clear();
            TotalStat.PagesByNode = 0;
            TotalStat.DocsByNode = 0;

            var data = _base.GetDataByDepartmentName(name, AppConfig.ReportDate.Start, AppConfig.ReportDate.End,
                AppConfig.ReportDate.IsEnabled).Result;
            if (!data.Any()) return;

            int totalPages = 0; int totalDocs = 0;
            foreach (var d in data)
            {
                totalPages += d.Data.Pages;
                totalDocs++;
                string userName = (d.User.FullName?.Length > 0) ? d.User.FullName : d.User.AccountName;
                PrintDatas.Add(new PrintDataGrid
                {
                    DocName = d.Data.DocName,
                    Pages = d.Data.Pages,
                    TimeStamp = d.Data.TimeStamp,
                    UserName = userName,
                    PrinterName = d.Printer.Name,
                    ComputerName = d.Computer.Name
                });
            }

            TotalStat.PagesByNode = totalPages;
            TotalStat.DocsByNode = totalDocs;
            TotalStat.ReportPeriod = AppConfig.ReportDate.ToString();
        }

        private void BuildDataByComputerId(int id)
        {
            PrintDatas.Clear();
            TotalStat.PagesByNode = 0;
            TotalStat.DocsByNode = 0;

            var data = _base.GetDataByComputerId(id, AppConfig.ReportDate.Start, AppConfig.ReportDate.End,
                AppConfig.ReportDate.IsEnabled).Result;
            if (!data.Any()) return;

            int totalPages = 0; int totalDocs = 0;
            foreach (var d in data)
            {
                totalPages += d.Data.Pages;
                totalDocs++;
                string userName = (d.User.FullName?.Length > 0) ? d.User.FullName : d.User.AccountName;
                PrintDatas.Add(new PrintDataGrid
                {
                    DocName = d.Data.DocName,
                    Pages = d.Data.Pages,
                    TimeStamp = d.Data.TimeStamp,
                    UserName = userName,
                    PrinterName = d.Printer.Name,
                    ComputerName = d.Computer.Name
                });
            }

            TotalStat.PagesByNode = totalPages;
            TotalStat.DocsByNode = totalDocs;
            TotalStat.ReportPeriod = AppConfig.ReportDate.ToString();
        }

        private void BuildDataByPrinterId(int id)
        {
            PrintDatas.Clear();
            TotalStat.PagesByNode = 0;
            TotalStat.DocsByNode = 0;

            var data = _base.GetDataByPrinterId(id, AppConfig.ReportDate.Start, AppConfig.ReportDate.End,
                AppConfig.ReportDate.IsEnabled).Result;
            if (!data.Any()) return;

            int totalPages = 0; int totalDocs = 0;
            foreach (var d in data)
            {
                totalPages += d.Data.Pages;
                totalDocs++;
                string username = (d.User.FullName?.Length > 0) ? d.User.FullName : d.User.AccountName;

                PrintDatas.Add(new PrintDataGrid
                {
                    DocName = d.Data.DocName,
                    Pages = d.Data.Pages,
                    TimeStamp = d.Data.TimeStamp,
                    UserName = username,
                    PrinterName = d.Printer.Name,
                    ComputerName = d.Computer.Name
                });
            }

            TotalStat.PagesByNode = totalPages;
            TotalStat.DocsByNode = totalDocs;
            TotalStat.ReportPeriod = AppConfig.ReportDate.ToString();
        }

        private void BuildDataByPrintersGroup(List<int> ids)
        {
            PrintDatas.Clear();
            TotalStat.PagesByNode = 0;
            TotalStat.DocsByNode = 0;

            var data = _base.GetDataByPrintersGroup(ids, AppConfig.ReportDate.Start, AppConfig.ReportDate.End,
                AppConfig.ReportDate.IsEnabled).Result;
            if (!data.Any()) return;

            int totalPages = 0; int totalDocs = 0;
            foreach (var d in data)
            {
                totalPages += d.Data.Pages;
                totalDocs++;
                string username = (d.User.FullName?.Length > 0) ? d.User.FullName : d.User.AccountName;
                PrintDatas.Add(new PrintDataGrid
                {
                    DocName = d.Data.DocName,
                    Pages = d.Data.Pages,
                    TimeStamp = d.Data.TimeStamp,
                    UserName = username,
                    PrinterName = d.Printer.Name,
                    ComputerName = d.Computer.Name
                });
            }

            TotalStat.PagesByNode = totalPages;
            TotalStat.DocsByNode = totalDocs;
            TotalStat.ReportPeriod = AppConfig.ReportDate.ToString();
        }

        private bool CanShowOptionsWindows(string arg)
        {
            return !OptionsWindow.Instance.IsVisible;
        }

        private void ShowOptionsWindows(string obj)
        {
            OptionsWindow.Instance.Show();
        }
        
        private bool CanShowAboutWindows(string arg)
        {
            return !AboutWindow.Instance.IsVisible;
        }

        private void ShowAboutWindows(string obj)
        {
            AboutWindow.Instance.Show();
        }

        public void GetAll()
        {
            PrintersCollection.GetAll();
            UsersCollection.GetAll();
            ComputersCollection.GetAll();
            DepartmentsCollection.GetAll();
            PrinterNamesCollection.GetAll();
            ServersCollection.GetAll();
        }
    }
}
