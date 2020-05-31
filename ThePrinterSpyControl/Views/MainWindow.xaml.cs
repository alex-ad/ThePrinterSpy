using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ThePrinterSpyControl.Commands;
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
        public PrinterSpyViewModel MainViewModel { get; set; } = new PrinterSpyViewModel();

        public MainWindow()
        {
            InitializeComponent();
            treeUsers.SelectedItemChanged += TreeUsers_SelectedItemChanged;
            treeComputers.SelectedItemChanged += TreeComputers_SelectedItemChanged;
            treeDepartments.SelectedItemChanged += TreeDepartments_SelectedItemChanged;
            treePrinters.SelectionChanged += TreePrinters_SelectionChanged;
        }

        private void TreePrinters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IList list = e.AddedItems;
            if (list?.Count < 1) return;
            PrinterNodeHead element = list[0] as PrinterNodeHead;
            if (element == null) return;
            MainViewModel.BuildPrintDataCollection(element.Ids, PrinterSpyViewModel.PrintDataGroup.PrintersGroup);
        }

        private void TreeDepartments_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is DepartmentsNodeHead)
                MainViewModel.BuildPrintDataCollection(((DepartmentsNodeHead)e.NewValue).Name, PrinterSpyViewModel.PrintDataGroup.Department);
            else
                MainViewModel.BuildPrintDataCollection(((UserNodeTail)e.NewValue).Id, PrinterSpyViewModel.PrintDataGroup.User);
        }

        private void TreeComputers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is ComputerNodeHead)
                MainViewModel.BuildPrintDataCollection(((ComputerNodeHead)e.NewValue).Id, PrinterSpyViewModel.PrintDataGroup.Computer);
            else
                MainViewModel.BuildPrintDataCollection(((PrinterNodeTail)e.NewValue).Id, PrinterSpyViewModel.PrintDataGroup.Printer);
        }

        private void TreeUsers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is UserNodeHead)
                MainViewModel.BuildPrintDataCollection(((UserNodeHead)e.NewValue).Id, PrinterSpyViewModel.PrintDataGroup.User);
            else
                MainViewModel.BuildPrintDataCollection(((PrinterNodeTail)e.NewValue).Id, PrinterSpyViewModel.PrintDataGroup.Printer);
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
    }
}
