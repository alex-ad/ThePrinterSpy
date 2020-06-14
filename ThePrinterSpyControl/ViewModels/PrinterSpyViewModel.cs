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

        public static ObservableCollection<UserNodeHead> Users { get; private set; }
        public static ObservableCollection<ComputerNodeHead> Computers { get; private set; }
        public ObservableCollection<DepartmentsNodeHead> Departments { get; }
        public  ObservableCollection<PrinterNodeHead> Printers { get; }
        public ObservableCollection<PrintDataGrid> PrintDatas { get; }
        public static TotalCountStat TotalStat { get; set; }
        public AppConfig AppConfig { get; set; }
        public SelectedPrinter SelectedPrinter { get; set; }
        //public static PrintSpyEntities Context; // ------------------------------------ PRIVATE READONLY ?????????????????

        private readonly DBase _base;
        private RelayCommand<string> _showOptionsWindowCommand = null;
        public RelayCommand<string> ShowOptionsWindowCommand => _showOptionsWindowCommand ?? (_showOptionsWindowCommand = new RelayCommand<string>(ShowOptionsWindows, CanShowOptionsWindows));

        public PrinterSpyViewModel()
        {
            //Context = new PrintSpyEntities();
            _base = new DBase();
            Users = new ObservableCollection<UserNodeHead>();
            Computers = new ObservableCollection<ComputerNodeHead>();
            Departments = new ObservableCollection<DepartmentsNodeHead>();
            Printers = new ObservableCollection<PrinterNodeHead>();
            PrintDatas = new ObservableCollection<PrintDataGrid>();

            TotalStat = new TotalCountStat();
            //AppConfig = new AppConfig(Context);
            SelectedPrinter = new SelectedPrinter();

            GetTotalStat();
            BuildPrintersByUserCollection();
            BuildPrintersByComputerCollection();
            BuildUsersByDepartmentsCollection();
            BuildPrinterCollection();
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

        private void GetTotalStat()
        {
            TotalStat.Users = Context.Users.Count();
            TotalStat.PrintersAll = Context.Printers.Count();
            TotalStat.PrintersEnabled = Context.Printers.Sum(x => x.Enabled ? 1 : 0);
            TotalStat.Computers = Context.Computers.Count();
        }

        public void BuildPrintersByUserCollection()
        {
            foreach (User u in Context.Users)
            {
                var query =
                    from p in Context.Printers
                    where p.UserId == u.Id
                    select p;

                UserNodeHead uNodeHead = new UserNodeHead
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    AccountName = u.AccountName
                };

                if (query.Any())
                {
                    uNodeHead.Printers = new ObservableCollection<PrinterNodeTail>();
                    int enabled = query.Sum(x => x.Enabled ? 1 : 0);
                    foreach (var p in query)
                    {
                        uNodeHead.Printers.Add(new PrinterNodeTail
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Enabled = p.Enabled
                        });
                    }
                    uNodeHead.Comment = $"[{enabled}/{query.Count()}]";
                }
                Users.Add(uNodeHead);
                //Application.Current.Dispatcher.Invoke(new Action(() => Users.Add(uNodeHead)));
            }
        }

        public void BuildPrintersByComputerCollection()
        {
            foreach (Computer c in Context.Computers)
            {
                var query =
                    from p in Context.Printers
                    where p.ComputerId == c.Id
                    select p;

                ComputerNodeHead cNodeHead = new ComputerNodeHead
                {
                    Id = c.Id,
                    NetbiosName = c.Name
                };

                if (query.Any())
                {
                    cNodeHead.Printers = new ObservableCollection<PrinterNodeTail>();
                    int enabled = query.Sum(x => x.Enabled ? 1 : 0);
                    foreach (var p in query)
                    {
                        cNodeHead.Printers.Add(new PrinterNodeTail
                        {
                           Id = p.Id,
                           Name = p.Name,
                           Enabled = p.Enabled
                        });
                    }
                    cNodeHead.Comment = $"[{enabled}/{query.Count()}]";
                }
                Computers.Add(cNodeHead);
               // Application.Current.Dispatcher.Invoke(new Action(() => Computers.Add(cNodeHead)));
            }
        }

        public void BuildUsersByDepartmentsCollection()
        {
            var query =
                from u in Context.Users
                select u;
            
            List<string> depts = query.Select(x => x.Department).Distinct().ToList();
            TotalStat.Departments = depts.Count();
            int id = 0;

            foreach (string d in depts)
            {
                int totalDeptPrintersAll = 0;
                int totalDeptPrintersEnabled = 0;
                int totalDeptUsers = 0;
                id++;

                DepartmentsNodeHead dNodeHead = new DepartmentsNodeHead
                {
                    Id = id,
                    Name = d
                };

                foreach (User u in query.Where(x => x.Department == d))
                {
                    int totalPrintersAll = Context.Printers.Count(x => x.UserId == u.Id);
                    int totalPrintersEnabled = Context.Printers.Count(x => (x.UserId == u.Id) && (x.Enabled));
                    totalDeptPrintersAll += totalPrintersAll;
                    totalDeptPrintersEnabled += totalPrintersEnabled;
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
                //Application.Current.Dispatcher.Invoke(new Action(() => Departments.Add(dNodeHead)));
            }
        }

        public void BuildPrinterCollection()
        {
            var printerNames = Context.Printers.Select(p=>new
            {
                Id = p.Id,
                Name = p.Name,
                Enabled = p.Enabled
            }).Where(p=>p.Enabled).ToList();

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

                /*Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Printers.Add(new PrinterNodeHead
                    {
                        Ids = ids,
                        NameMasked = mask
                    });
                }));*/
                Printers.Add(new PrinterNodeHead
                {
                    Ids = ids,
                    NameMasked = mask
                });
            }
        }

        private void BuildDataByUserId(int id)
        {
            PrintDatas.Clear();

            var query =
                from d in Context.PrintDatas
                join p in Context.Printers on d.PrinterId equals p.Id
                join u in Context.Users on d.UserId equals u.Id
                where d.UserId == id
                      && (
                        (
                            (d.TimeStamp.CompareTo(AppConfig.ReportDate.Start) > -1 || d.TimeStamp.CompareTo(AppConfig.ReportDate.Start) == 0)
                            && (d.TimeStamp.CompareTo(AppConfig.ReportDate.End) < 1 || d.TimeStamp.CompareTo(AppConfig.ReportDate.Start) == 0)
                            && AppConfig.ReportDate.IsEnabled
                        )
                        || (!AppConfig.ReportDate.IsEnabled)
                      )
                select new { d, p, u };

            TotalStat.PagesByNode = 0;
            TotalStat.DocsByNode = 0;
            if (!query.Any()) return;

            int totalPages = 0; int totalDocs = 0;
            foreach (var d in query)
            {
                totalPages += d.d.Pages;
                totalDocs++;
                string userName = (d.u.FullName?.Length > 0) ? d.u.FullName : d.u.AccountName;
                PrintDatas.Add(new PrintDataGrid
                {
                    DocName = d.d.DocName,
                    Pages = d.d.Pages,
                    TimeStamp = d.d.TimeStamp,
                    UserName = userName,
                    PrinterName = d.p.Name
                });
            }

            TotalStat.PagesByNode = totalPages;
            TotalStat.DocsByNode = totalDocs;
            TotalStat.ReportPeriod = AppConfig.ReportDate.ToString();
        }

        private void BuildDataByDepartmentId(string name)
        {
            PrintDatas.Clear();
            var query =
                from d in Context.PrintDatas
                from p in Context.Printers
                from u in Context.Users
                where d.UserId == u.Id
                      && u.Department == name
                      && d.PrinterId == p.Id
                      && (
                          (
                              d.TimeStamp.CompareTo(AppConfig.ReportDate.Start) > -1
                              && d.TimeStamp.CompareTo(AppConfig.ReportDate.End) < 1
                              && AppConfig.ReportDate.IsEnabled
                          )
                          || (!AppConfig.ReportDate.IsEnabled)
                      )
                select new { d, p, u };

            TotalStat.PagesByNode = 0;
            TotalStat.DocsByNode = 0;
            if (!query.Any()) return;

            int totalPages = 0; int totalDocs = 0;
            foreach (var d in query)
            {
                totalPages += d.d.Pages;
                totalDocs++;
                string userName = (d.u.FullName?.Length > 0) ? d.u.FullName : d.u.AccountName;
                PrintDatas.Add(new PrintDataGrid
                {
                    DocName = d.d.DocName,
                    Pages = d.d.Pages,
                    TimeStamp = d.d.TimeStamp,
                    UserName = userName,
                    PrinterName = d.p.Name
                });
            }

            TotalStat.PagesByNode = totalPages;
            TotalStat.DocsByNode = totalDocs;
            TotalStat.ReportPeriod = AppConfig.ReportDate.ToString();
        }

        private void BuildDataByComputerId(int id)
        {
            PrintDatas.Clear();

            var query =
                from d in Context.PrintDatas
                join p in Context.Printers on d.PrinterId equals p.Id
                join u in Context.Users on d.UserId equals u.Id
                where d.ComputerId == id
                      && (
                          (
                              d.TimeStamp.CompareTo(AppConfig.ReportDate.Start) > -1
                              && d.TimeStamp.CompareTo(AppConfig.ReportDate.End) < 1
                              && AppConfig.ReportDate.IsEnabled
                          )
                          || (!AppConfig.ReportDate.IsEnabled)
                      )
                select new { d, p, u };

            TotalStat.PagesByNode = 0;
            TotalStat.DocsByNode = 0;
            if (!query.Any()) return;

            int totalPages = 0; int totalDocs = 0;
            foreach (var d in query)
            {
                totalPages += d.d.Pages;
                totalDocs++;
                string userName = (d.u.FullName?.Length > 0) ? d.u.FullName : d.u.AccountName;
                PrintDatas.Add(new PrintDataGrid
                {
                    DocName = d.d.DocName,
                    Pages = d.d.Pages,
                    TimeStamp = d.d.TimeStamp,
                    UserName = userName,
                    PrinterName = d.p.Name
                });
            }

            TotalStat.PagesByNode = totalPages;
            TotalStat.DocsByNode = totalDocs;
            TotalStat.ReportPeriod = AppConfig.ReportDate.ToString();
        }

        private void BuildDataByPrinterId(int id)
        {
            PrintDatas.Clear();

            var query =
                from d in Context.PrintDatas
                join p in Context.Printers on d.PrinterId equals p.Id
                join u in Context.Users on d.UserId equals u.Id
                where d.PrinterId == id
                      && (
                          (
                              d.TimeStamp.CompareTo(AppConfig.ReportDate.Start) > -1
                              && d.TimeStamp.CompareTo(AppConfig.ReportDate.End) < 1
                              && AppConfig.ReportDate.IsEnabled
                          )
                          || (!AppConfig.ReportDate.IsEnabled)
                      )
                select new { d, p, u };


            TotalStat.PagesByNode = 0;
            TotalStat.DocsByNode = 0;

            if (!query.Any()) return;

            int totalPages = 0; int totalDocs = 0;
            foreach (var d in query)
            {
                totalPages += d.d.Pages;
                totalDocs++;
                string username = (d.u.FullName?.Length > 0) ? d.u.FullName : d.u.AccountName;
                PrintDatas.Add(new PrintDataGrid
                {
                    DocName = d.d.DocName,
                    Pages = d.d.Pages,
                    TimeStamp = d.d.TimeStamp,
                    UserName = username,
                    PrinterName = d.p.Name
                });
            }

            TotalStat.PagesByNode = totalPages;
            TotalStat.DocsByNode = totalDocs;
            TotalStat.ReportPeriod = AppConfig.ReportDate.ToString();
        }

        private void BuildDataByPrintersGroup(List<int> ids)
        {
            PrintDatas.Clear();

            var query =
                from d in Context.PrintDatas
                from i in ids
                join p in Context.Printers on d.PrinterId equals p.Id
                join u in Context.Users on d.UserId equals u.Id
                where d.PrinterId == i
                      && (
                          (
                              d.TimeStamp.CompareTo(AppConfig.ReportDate.Start) > -1
                              && d.TimeStamp.CompareTo(AppConfig.ReportDate.End) < 1
                              && AppConfig.ReportDate.IsEnabled
                          )
                          || (!AppConfig.ReportDate.IsEnabled)
                      )
                select new { d, p, u };

            TotalStat.PagesByNode = 0;
            TotalStat.DocsByNode = 0;
            if (!query.Any()) return;

            int totalPages = 0; int totalDocs = 0;
            foreach (var d in query)
            {
                totalPages += d.d.Pages;
                totalDocs++;
                string username = (d.u.FullName?.Length > 0) ? d.u.FullName : d.u.AccountName;
                PrintDatas.Add(new PrintDataGrid
                {
                    DocName = d.d.DocName,
                    Pages = d.d.Pages,
                    TimeStamp = d.d.TimeStamp,
                    UserName = username,
                    PrinterName = d.p.Name
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
