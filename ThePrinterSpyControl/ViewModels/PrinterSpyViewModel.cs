using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using ThePrinterSpyControl.Commands;
using ThePrinterSpyControl.Models;
using ThePrinterSpyControl.Modules;
using ThePrinterSpyControl.Views;

namespace ThePrinterSpyControl.ViewModels
{
    public class PrinterSpyViewModel
    {
        //http://blog.gandjustas.ru/2009/09/29/mvvm-treeview/
        //https://stackru.com/questions/20816956/poluchit-vyibrannyij-element-wpf-treeview
        //https://issue.life/questions/39147491
        //https://iconbird.com/search/?q=iconset:Beautiful%20Flat%20Icons
        //https://iconbird.com/search/?q=iconset:Blueberry
        public enum PrintDataGroup : byte
        {
            User = 1,
            Printer = 2,
            PrintersGroup = 3,
            Computer = 4,
            Department = 5
        }

        public static UsersCollection UsersCollection { get; set; }
        public static PrintersCollection PrintersCollection { get; set; }
        public static ComputersCollection ComputersCollection { get; set; }



        //public static ObservableCollection<ComputerNodeHead> Computers { get; private set; }
        public ObservableCollection<DepartmentsNodeHead> Departments { get; }
        public  ObservableCollection<PrinterNodeHead> Printers { get; }
        public ObservableCollection<PrintDataGrid> PrintDatas { get; }



        public static TotalCountStat TotalStat { get; set; }
        public AppConfig AppConfig { get; set; }
        public static SelectedPrinter SelectedPrinter { get; set; }

        private readonly DBase _base;
        private RelayCommand<string> _showOptionsWindowCommand = null;
        public RelayCommand<string> ShowOptionsWindowCommand => _showOptionsWindowCommand ?? (_showOptionsWindowCommand = new RelayCommand<string>(ShowOptionsWindows, CanShowOptionsWindows));

        public PrinterSpyViewModel()
        {
            _base = new DBase();
            PrintersCollection = new PrintersCollection();
            UsersCollection = new UsersCollection();
            ComputersCollection = new ComputersCollection();

            //Computers = new ObservableCollection<ComputerNodeHead>();
            Departments = new ObservableCollection<DepartmentsNodeHead>();
            Printers = new ObservableCollection<PrinterNodeHead>();
            PrintDatas = new ObservableCollection<PrintDataGrid>();

            TotalStat = new TotalCountStat();
            AppConfig = new AppConfig();
            SelectedPrinter = new SelectedPrinter();

            PrintersCollection.GetAll();
            UsersCollection.GetAll();
            ComputersCollection.GetAll();
            

            //GetTotalStat();
            //BuildPrintersByUserCollection();
            //BuildPrintersByComputerCollection();
            //BuildUsersByDepartmentsCollection();
            //BuildPrinterCollection();
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

        /*public async Task BuildPrintersByUserCollectionAsync()
        {
            await Task.Run(BuildPrintersByUserCollection);
        }

        public async Task BuildPrintersByComputerCollectionAsync()
        {
            await Task.Run(BuildPrintersByComputerCollection);
        }

        public async Task BuildUsersByDepartmentsCollectionAsync()
        {
            await Task.Run(BuildUsersByDepartmentsCollection);
        }

        public async Task BuildPrinterCollectionAsync()
        {
            await Task.Run(BuildPrinterCollection);
        }

        private async Task BuildDataByComputerIdAsync(int id)
        {
            await Task.Run(() => BuildDataByComputerId(id));
        }

        private async Task BuildDataByPrinterIdAsync(int id)
        {
            await Task.Run(() => BuildDataByPrinterId(id));
        }

        private async Task BuildDataByDepartmentIdAsync(string name)
        {
            await Task.Run(() => BuildDataByDepartmentId(name));
        }

        private async Task BuildDataByPrintersGroupAsync(List<int> id)
        {
            await Task.Run(() => BuildDataByPrintersGroup(id));
        }

        private async Task BuildDataByUserIdAsync(int id)
        {
            await Task.Run(() => BuildDataByUserId(id));
        }*/

        /*private  void GetTotalStat()
        {
            TotalStat.Users = _base.GetUsersCount().Result;
            TotalStat.PrintersAll = _base.GetPrintersCount().Result;
            TotalStat.PrintersEnabled = _base.GetEnabledPrintersCount().Result;
            TotalStat.Computers = _base.GetComputersCount().Result;
        }*/

        /*public async void BuildPrintersByUserCollection()
        {
            foreach (User u in await _base.GetUsersList())
            {
                if (await _base.GetPrintersByUserCount(u) < 1) continue;
                var printers = await _base.GetPrintersByUser(u);

                UserNodeHead uNodeHead = new UserNodeHead
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    AccountName = u.AccountName
                };
                
                uNodeHead.Printers = new ObservableCollection<PrinterNodeTail>();
                int enabled = printers.Count(p => p.Enabled);
                uNodeHead.Comment = $"[{enabled}/{printers.Count()}]";

                UsersPrinters.AddUser(uNodeHead);

                foreach (var p in printers)
                {
                    UsersPrinters.AddPrinter(new PrinterNodeTail
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Enabled = p.Enabled
                    }, uNodeHead);
                }
            }
        }*/

        /*public async void BuildPrintersByComputerCollection()
        {
            foreach (Computer c in await _base.GetComputersList())
            {
                if (await _base.GetPrintersByComputerCount(c) < 1) continue;
                var printers = await _base.GetPrintersByComputer(c);

                ComputerNodeHead cNodeHead = new ComputerNodeHead
                {
                    Id = c.Id,
                    NetbiosName = c.Name
                };

                cNodeHead.Printers = new ObservableCollection<PrinterNodeTail>();
                int enabled = printers.Count(x => x.Enabled);
                foreach (var p in printers)
                {
                    cNodeHead.Printers.Add(new PrinterNodeTail
                    {
                       Id = p.Id,
                       Name = p.Name,
                       Enabled = p.Enabled
                    });
                }
                cNodeHead.Comment = $"[{enabled}/{printers.Count()}]";
                Computers.Add(cNodeHead);
                Application.Current.Dispatcher.Invoke(new Action(() => Computers.Add(cNodeHead)));
            }
        }*/

        /*public async void BuildUsersByDepartmentsCollection()
        {
            var users = await _base.GetUsersList();
            
            List<string> depts = users.Select(x => x.Department).Distinct().ToList();
            TotalStat.Departments = depts.Count();
            int id = 0;

            foreach (string d in depts)
            {
                int totalDeptUsers = 0;
                id++;

                DepartmentsNodeHead dNodeHead = new DepartmentsNodeHead
                {
                    Id = id,
                    Name = d
                };

                foreach (User u in users.Where(x => x.Department == d))
                {
                    int totalPrintersAll = await _base.GetPrintersCount();
                    int totalPrintersEnabled = await _base.GetEnabledPrintersByUserCount(u);
                    totalDeptUsers++;
                    UserNodeTail uNodeTail = new UserNodeTail
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        AccountName = u.AccountName,
                        Comment = $"{totalPrintersEnabled}/{totalPrintersAll}"
                    };
                    dNodeHead.Users.Add(uNodeTail);
                }
                dNodeHead.Comment = $"{totalDeptUsers}";
                Departments.Add(dNodeHead);
            }
        }*/

        /*public async void BuildPrinterCollection()
        {
            if (await _base.GetEnabledPrintersCount() < 1) return;
            var printerNames = await _base.GetEnabledPrintersList();

            while (printerNames.Any())
            {
                string name = printerNames[0].Name;
                string mask;
                List<int> ids = new List<int>();
                if (AppConfig.PrinterNameMask.IsEnabled)
                {
                    mask = AppConfig.GetMaskedPrinterName(name);
                    ids = printerNames.Where(p => p.Name.Contains(mask)).Select(i => i.Id).ToList();
                }
                else
                {
                    mask = name;
                    ids.Add(printerNames[0].Id);
                }
                printerNames.RemoveAll(x => ids.Contains(x.Id));

                Printers.Add(new PrinterNodeHead
                {
                    Ids = ids,
                    NameMasked = mask
                });
            }
        }*/

        private async void BuildDataByUserId(int id)
        {
            PrintDatas.Clear();

            var data = await _base.GetDataByUserId(id, AppConfig.ReportDate.Start, AppConfig.ReportDate.End,
                AppConfig.ReportDate.IsEnabled);
            if (!data.Any()) return;

            TotalStat.PagesByNode = 0;
            TotalStat.DocsByNode = 0;

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
                    PrinterName = d.Printer.Name
                });
            }

            TotalStat.PagesByNode = totalPages;
            TotalStat.DocsByNode = totalDocs;
            TotalStat.ReportPeriod = AppConfig.ReportDate.ToString();
        }

        private async void BuildDataByDepartmentId(string name)
        {
            PrintDatas.Clear();
            var data = await _base.GetDataByDepartmentName(name, AppConfig.ReportDate.Start, AppConfig.ReportDate.End,
                AppConfig.ReportDate.IsEnabled);
            if (!data.Any()) return;

            TotalStat.PagesByNode = 0;
            TotalStat.DocsByNode = 0;

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
                    PrinterName = d.Printer.Name
                });
            }

            TotalStat.PagesByNode = totalPages;
            TotalStat.DocsByNode = totalDocs;
            TotalStat.ReportPeriod = AppConfig.ReportDate.ToString();
        }

        private async void BuildDataByComputerId(int id)
        {
            PrintDatas.Clear();

            var data = await _base.GetDataByComputerId(id, AppConfig.ReportDate.Start, AppConfig.ReportDate.End,
                AppConfig.ReportDate.IsEnabled);
            if (!data.Any()) return;

            TotalStat.PagesByNode = 0;
            TotalStat.DocsByNode = 0;

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
                    PrinterName = d.Printer.Name
                });
            }

            TotalStat.PagesByNode = totalPages;
            TotalStat.DocsByNode = totalDocs;
            TotalStat.ReportPeriod = AppConfig.ReportDate.ToString();
        }

        private async void BuildDataByPrinterId(int id)
        {
            PrintDatas.Clear();

            var data = await _base.GetDataByPrinterId(id, AppConfig.ReportDate.Start, AppConfig.ReportDate.End,
                AppConfig.ReportDate.IsEnabled);
            if (!data.Any()) return;

            TotalStat.PagesByNode = 0;
            TotalStat.DocsByNode = 0;

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
                    PrinterName = d.Printer.Name
                });
            }

            TotalStat.PagesByNode = totalPages;
            TotalStat.DocsByNode = totalDocs;
            TotalStat.ReportPeriod = AppConfig.ReportDate.ToString();
        }

        private async void BuildDataByPrintersGroup(List<int> ids)
        {
            PrintDatas.Clear();

            var data = await _base.GetDataByPrintersGroup(ids, AppConfig.ReportDate.Start, AppConfig.ReportDate.End,
                AppConfig.ReportDate.IsEnabled);
            if (!data.Any()) return;

            TotalStat.PagesByNode = 0;
            TotalStat.DocsByNode = 0;

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
                    PrinterName = d.Printer.Name
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
    }
}
