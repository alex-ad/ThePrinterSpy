using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ThePrinterSpyControl.ModelBuilders;
using ThePrinterSpyControl.Models;
using ThePrinterSpyControl.Modules;
using ThePrinterSpyControl.ViewModels;
using Props = ThePrinterSpyControl.Properties.Settings;

namespace ThePrinterSpyControl.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public PrinterSpyViewModel MainViewModel { get; } = new PrinterSpyViewModel();
        
        private readonly PrintersCollection _printers = new PrintersCollection();
        private readonly ComputersCollection _computers = new ComputersCollection();
        private readonly UsersCollection _users = new UsersCollection();

        public MainWindow()
        {
            InitializeComponent();
            treeUsers.SelectedItemChanged += TreeUsers_SelectedItemChanged;
            treeUsers.GotFocus += TreeUsers_GotFocus;
            treeComputers.SelectedItemChanged += TreeComputers_SelectedItemChanged;
            treeComputers.GotFocus += TreeComputers_GotFocus;
            treeDepartments.SelectedItemChanged += TreeDepartments_SelectedItemChanged;
            treeDepartments.GotFocus += TreeDepartments_GotFocus;
            treePrinters.SelectionChanged += TreePrinters_SelectionChanged;
            treePrinters.GotFocus += TreePrinters_GotFocus;
            textNewPrinterName.TextChanged += TextNewPrinterName_TextChanged;
            checkPrinterEnabled.Click += CheckPrinterEnabled_Click;
            btnPrinterRename.Click += BtnPrinterRename_Click;
            btnPrinterDeleteFromDb.Click += BtnPrinterDeleteFromDb_Click;
            TabControl.SelectionChanged += TabControl_SelectionChanged;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PrinterSpyViewModel.SelectedPrinter.Reset();
        }

        private void BtnPrinterDeleteFromDb_Click(object sender, RoutedEventArgs e)
        {
            var id = PrinterSpyViewModel.SelectedPrinter.Id;
            PrinterManagement.DeleteFromDb(PrinterSpyViewModel.SelectedPrinter);
            _computers.PropertyPrinterListChanged(id);
            _users.PropertyPrinterListChanged(id);
            PrinterSpyViewModel.SelectedPrinter.Id = 0;
        }

        private void BtnPrinterRename_Click(object sender, RoutedEventArgs e)
        {
            var id = PrinterSpyViewModel.SelectedPrinter.Id;
            if (id < 1) return;
            string computerName = _computers.GetNameByPrinterId(id);
            if (string.IsNullOrEmpty(computerName) || string.IsNullOrEmpty(PrinterSpyViewModel.SelectedPrinter.NewName) || string.Equals(PrinterSpyViewModel.SelectedPrinter.NewName, PrinterSpyViewModel.SelectedPrinter.OldName, StringComparison.InvariantCulture)) return;
            PrinterManagement.Rename(PrinterSpyViewModel.SelectedPrinter, computerName);
            PrinterSpyViewModel.SelectedPrinter.OldName = PrinterSpyViewModel.SelectedPrinter.NewName;
            _computers.PropertyPrinterIdsChanged(id);
            _users.PropertyPrinterIdsChanged(id);
        }

        private void CheckPrinterEnabled_Click(object sender, RoutedEventArgs e)
        {
            if (PrinterSpyViewModel.SelectedPrinter.Id < 1) return;
            var c = sender as CheckBox;
            if (c == null) return;
            PrinterSpyViewModel.SelectedPrinter.IsNewModel = false;
            PrinterSpyViewModel.SelectedPrinter.Enabled = c.IsChecked ?? false;
            var id = PrinterSpyViewModel.SelectedPrinter.Id;
            _computers.PropertyPrinterIdsChanged(id);
            _users.PropertyPrinterIdsChanged(id);
        }

        private void TextNewPrinterName_TextChanged(object sender, TextChangedEventArgs e)
        {
            var t = sender as TextBox;
            if (t == null || t.Text.Length < 2) return;
            PrinterSpyViewModel.SelectedPrinter.NewName = t.Text;
        }

        private void TreePrinters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IList list = e.AddedItems;
            if (list == null || list.Count < 1) return;
            PrinterMaskedNameNode element = list[0] as PrinterMaskedNameNode;
            if (element == null) return;
            MainViewModel.BuildPrintDataCollection(element.Ids, PrinterSpyViewModel.PrintDataGroup.PrintersGroup);
        }

        private void TreePrinters_GotFocus(object sender, RoutedEventArgs e)
        {
            var elem = e.Source as ListView;
            if (elem?.SelectedItem == null) return;

            MainViewModel.BuildPrintDataCollection(((PrinterMaskedNameNode)elem.SelectedItem).Ids, PrinterSpyViewModel.PrintDataGroup.PrintersGroup);
        }

        private void TreeDepartments_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is DepartmentNode)
                MainViewModel.BuildPrintDataCollection(((DepartmentNode)e.NewValue).Name, PrinterSpyViewModel.PrintDataGroup.Department);
            else
                MainViewModel.BuildPrintDataCollection(e.NewValue, PrinterSpyViewModel.PrintDataGroup.User);
        }

        private void TreeDepartments_GotFocus(object sender, RoutedEventArgs e)
        {
            var elem = e.Source as TreeView;
            if (elem?.SelectedItem == null) return;

            if (elem.SelectedItem is DepartmentNode)
                MainViewModel.BuildPrintDataCollection(((DepartmentNode)elem.SelectedItem).Name, PrinterSpyViewModel.PrintDataGroup.Department);
            else
                MainViewModel.BuildPrintDataCollection(elem.SelectedItem, PrinterSpyViewModel.PrintDataGroup.User);
        }

        private void TreeComputers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is ComputerNode)
            {
                PrinterSpyViewModel.SelectedPrinter.Reset();
                MainViewModel.BuildPrintDataCollection(((ComputerNode)e.NewValue).Id, PrinterSpyViewModel.PrintDataGroup.Computer);
            }
            else
            {
                SelectActivePrinter((int)e.NewValue);
                MainViewModel.BuildPrintDataCollection(e.NewValue, PrinterSpyViewModel.PrintDataGroup.Printer);
            }
        }

        private void TreeComputers_GotFocus(object sender, RoutedEventArgs e)
        {
            var elem = e.Source as TreeView;
            if (elem?.SelectedItem == null) return;

            if (elem.SelectedItem is ComputerNode)
            {
                PrinterSpyViewModel.SelectedPrinter.Reset();
                MainViewModel.BuildPrintDataCollection(((ComputerNode)elem.SelectedItem).Id, PrinterSpyViewModel.PrintDataGroup.Computer);
            }
            else
            {
                SelectActivePrinter((int)elem.SelectedItem);
                MainViewModel.BuildPrintDataCollection(elem.SelectedItem, PrinterSpyViewModel.PrintDataGroup.Printer);
            }
        }

        private void TreeUsers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is UserNode)
            {
                PrinterSpyViewModel.SelectedPrinter.Reset();
                MainViewModel.BuildPrintDataCollection(((UserNode)e.NewValue).Id, PrinterSpyViewModel.PrintDataGroup.User);
            }
            else
            {
                SelectActivePrinter((int)e.NewValue);
                MainViewModel.BuildPrintDataCollection(e.NewValue, PrinterSpyViewModel.PrintDataGroup.Printer);
            }
        }

        private void TreeUsers_GotFocus(object sender, RoutedEventArgs e)
        {
            var elem = e.Source as TreeView;
            if (elem?.SelectedItem == null) return;

            if (elem.SelectedItem is UserNode)
            {
                PrinterSpyViewModel.SelectedPrinter.Reset();
                MainViewModel.BuildPrintDataCollection(((UserNode)elem.SelectedItem).Id, PrinterSpyViewModel.PrintDataGroup.User);
            }
            else
            {
                SelectActivePrinter((int)elem.SelectedItem);
                MainViewModel.BuildPrintDataCollection(elem.SelectedItem, PrinterSpyViewModel.PrintDataGroup.Printer);
            }
        }

        private void SaveAsCmdExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var saveDlg = new SaveFileDialog{ Filter = "Excel|*.xlsx"};
            if (true == saveDlg.ShowDialog())
            {
                ExportToFile ExportTo = new ExportToFile();
                ExportTo.Excel(saveDlg.FileName, MainViewModel.PrintDatas);
            }
        }

        private void SaveAsCmdCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainViewModel.PrintDatas.Count > 0;
        }

        private void RefreshCmdExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            MainViewModel.GetAll();
        }

        private void RefreshCmdCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Props.Default.WinY = (int)Application.Current.MainWindow.Top;
            Props.Default.WinX = (int)Application.Current.MainWindow.Left;
            Props.Default.WinWidth = (int)Application.Current.MainWindow.Width;
            Props.Default.WinHeight = (int)Application.Current.MainWindow.Height;
            Props.Default.TabDepartments = menuTabDepartments.IsChecked;
            Props.Default.TabComputers = menuTabComputers.IsChecked;
            Props.Default.TabPrinters = menuTabPrinters.IsChecked;
            Props.Default.TabUsers = menuTabUsers.IsChecked;
            Props.Default.Save();
            OptionsWindow.Instance.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Top = Props.Default.WinY;
            Application.Current.MainWindow.Left = Props.Default.WinX;
            Application.Current.MainWindow.Width = Props.Default.WinWidth;
            Application.Current.MainWindow.Height = Props.Default.WinHeight;
            menuTabDepartments.IsChecked = Props.Default.TabDepartments;
            menuTabComputers.IsChecked = Props.Default.TabComputers;
            menuTabPrinters.IsChecked = Props.Default.TabPrinters;
            menuTabUsers.IsChecked = Props.Default.TabUsers;
        }

        private void tabUsers_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //if ((bool) e.NewValue) MainViewModel.BuildPrintersByUserCollection();
        }

        private void tabComputers_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //if ((bool)e.NewValue) MainViewModel.BuildPrintersByComputerCollection();
        }

        private void tabDepartments_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //if ((bool)e.NewValue) MainViewModel.BuildUsersByDepartmentsCollection();
        }

        private void tabPrinters_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //if ((bool)e.NewValue) MainViewModel.BuildPrinterCollection();
        }

        private void SelectActivePrinter(int printerId)
        {
            var p = _printers.GetPrinter(printerId);
            if (p == null) return;
            PrinterSpyViewModel.SelectedPrinter.Id = p.Id;
            PrinterSpyViewModel.SelectedPrinter.OldName = p.Name;
            PrinterSpyViewModel.SelectedPrinter.NewName = p.Name;
            PrinterSpyViewModel.SelectedPrinter.IsNewModel = true;
            PrinterSpyViewModel.SelectedPrinter.Enabled = p.Enabled;
        }
    }
}
