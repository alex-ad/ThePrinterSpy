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
using ThePrinterSpyControl.Modules;
using ThePrinterSpyControl.ViewModels;
using Props = ThePrinterSpyControl.Properties.Settings;
using TextBox = System.Windows.Controls.TextBox;

namespace ThePrinterSpyControl.Views
{
    /// <summary>
    /// Логика взаимодействия для OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        private static OptionsWindow _obj;
        public static OptionsWindow Instance
        {
            get
            {
                _obj = _obj ?? new OptionsWindow();
                return _obj;
            }
        }

        public AppConfig OptionsViewModel { get; set; } = new AppConfig();

        public OptionsWindow()
        {
            InitializeComponent();
        }

        private void wndOptions_Closed(object sender, EventArgs e)
        {
            _obj = null;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            checkPrinterNameMaskEnabled.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();
            comboPrinterNameMaskType.GetBindingExpression(ComboBox.SelectedIndexProperty).UpdateSource();
            textPrinterNameMaskValue.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            dateReportPeriodStart.GetBindingExpression(DatePicker.TextProperty).UpdateSource();
            dateReportPeriodEnd.GetBindingExpression(DatePicker.TextProperty).UpdateSource();
            comboReportPeriodType.GetBindingExpression(ComboBox.SelectedIndexProperty).UpdateSource();
            checkReportPeriodEnabled.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();
            comboDbaseConnectionType.GetBindingExpression(ComboBox.SelectedIndexProperty).UpdateSource();
            textDbaseServer.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            textDbaseName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            textDbaseUser.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            //passPassword.GetBindingExpression(PasswordBox.Tex)
            checkAdEnabled.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();
            textAdServer.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            textAdUser.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            OptionsViewModel.SaveToLocal();
            OptionsViewModel.SaveToDbase();
            Close();
        }
    }
}
