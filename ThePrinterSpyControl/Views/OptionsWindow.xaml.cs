using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.DirectoryServices;
using ThePrinterSpyControl.ModelBuilders;
using ThePrinterSpyControl.Models;
using ThePrinterSpyControl.Modules;
using ThePrinterSpyControl.Validators;
using TextBox = System.Windows.Controls.TextBox;

namespace ThePrinterSpyControl.Views
{
    /// <summary>
    /// Логика взаимодействия для OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        private static OptionsWindow _obj;
        private readonly UsersCollection _users = new UsersCollection();
        private readonly DepartmentsCollection _departments = new DepartmentsCollection();

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
            checkAdEnabled.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();
            textAdServer.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            textAdUser.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            pswAdPassword.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            if (ConfigValidator.AnyError) return;

            OptionsViewModel.SaveToLocal();
            OptionsViewModel.SaveToBase();
            Close();
        }

        private async void btnAdSync_Click(object sender, RoutedEventArgs e)
        {
            textAdServer.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            textAdUser.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            pswAdPassword.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            if (ConfigValidator.AnyError) return;

            btnAdSync.IsEnabled = false;
            await SyncUsersWithAd();
            btnAdSync.IsEnabled = true;
        }

        private async Task SyncUsersWithAd()
        {
            await Task.Run(() =>
            {
                ActiveDirectory ad = new ActiveDirectory(new AdIdentity(AppConfig.ActiveDirectory.Server, AppConfig.ActiveDirectory.User, AppConfig.ActiveDirectory.Password));
                foreach (var u in ad)
                {
                    if (string.IsNullOrEmpty(u.Sid.ToString()) || string.IsNullOrEmpty(u.SamAccountName) || string.IsNullOrEmpty(u.DisplayName)) continue;
                    var de = u.GetUnderlyingObject() as DirectoryEntry;
                    u.Description = Convert.ToString(de?.Properties["department"].Value);
                    _users.UpdateUser(u);
                }
            });
            _departments.GetAll();
        }
    }
}
