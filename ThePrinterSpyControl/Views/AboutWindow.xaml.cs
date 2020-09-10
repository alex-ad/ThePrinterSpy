using System;
using System.Windows;
using System.Windows.Input;

namespace ThePrinterSpyControl.Views
{
    /// <summary>
    /// Логика взаимодействия для AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        private static AboutWindow _obj;

        public static AboutWindow Instance
        {
            get
            {
                _obj = _obj ?? new AboutWindow();
                return _obj;
            }
        }

        public AboutWindow()
        {
            InitializeComponent();
        }

        private void CloseWindowCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void WndAbout_Closed(object sender, EventArgs e)
        {
            _obj = null;
        }
    }
}
