using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
