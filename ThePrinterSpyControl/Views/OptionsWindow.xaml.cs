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
    public partial class OptionsWindow
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
            CheckPrinterNameMaskEnabled.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateSource();
            ComboPrinterNameMaskType.GetBindingExpression(ComboBox.SelectedIndexProperty)?.UpdateSource();
            TextPrinterNameMaskValue.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            DateReportPeriodStart.GetBindingExpression(DatePicker.SelectedDateProperty)?.UpdateSource();
            DateReportPeriodEnd.GetBindingExpression(DatePicker.SelectedDateProperty)?.UpdateSource();
            ComboReportPeriodType.GetBindingExpression(ComboBox.SelectedIndexProperty)?.UpdateSource();
            CheckReportPeriodEnabled.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateSource();
            ComboDbaseConnectionType.GetBindingExpression(ComboBox.SelectedIndexProperty)?.UpdateSource();
            TextDbaseServer.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            TextDbaseName.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            TextDbaseUser.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            CheckAdEnabled.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateSource();
            TextAdServer.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            TextAdUser.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            PswAdPassword.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

            if (ConfigValidator.AnyError) return;

            OptionsViewModel.SaveToLocal();
            OptionsViewModel.SaveToBase();
            Close();
        }

        private async void btnAdSync_Click(object sender, RoutedEventArgs e)
        {
            TextAdServer.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            TextAdUser.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            PswAdPassword.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

            if (ConfigValidator.AnyError) return;

            BtnAdSync.IsEnabled = false;
            await SyncUsersWithAd();
            BtnAdSync.IsEnabled = true;
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
