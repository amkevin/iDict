using Microsoft.Win32;
using System.Windows;

namespace iDict
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : Window
    {
        #region Setting Init
        /// <summary>
        /// setting init
        /// </summary>
        public Setting()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.Manual;

            double dX = SystemParameters.WorkArea.Width;
            double dY = SystemParameters.WorkArea.Height;

            this.Top = dY - this.Height;
            this.Left = dX - this.Width*1.5;

            if (IsExistKey("iDict"))
            {
                this.cbxAutoStart.IsChecked = true;
            }
            else
            {
                this.cbxAutoStart.IsChecked = false;
            }
        }
        #endregion


        #region close window when lost fucos
        /// <summary>
        /// close window when lost fucos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mSetting_Deactivated(object sender, System.EventArgs e)
        {
            this.Close();
        }
        

        /// <summary>
        /// checkbox checked click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxAutoStart_Click(object sender, RoutedEventArgs e)
        {
            if (IsExistKey("iDict"))
            {
                if (SetRegistryKey("iDict", false))
                {
                    this.cbxAutoStart.IsChecked = false;
                }
                else
                {
                    this.cbxAutoStart.IsChecked = true;
                }
            }
            else
            {
                if (SetRegistryKey("iDict", true))
                {
                    this.cbxAutoStart.IsChecked = true;
                }
                else
                {
                    this.cbxAutoStart.IsChecked = false;
                }
            }
        }
        #endregion


        #region check if registerred in windows system.
        /// <summary>
        /// check if registerred in windows system.
        /// </summary>
        /// <param name="strKeyName">register key name</param>
        /// <returns>yes/no</returns>
        private bool IsExistKey(string strKeyName)
        {
            bool isExist = false;
            try
            {
                RegistryKey hUser = Registry.CurrentUser;
                RegistryKey hURun = hUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false);
                if (hURun == null)
                {
                    return isExist;
                }
                else
                {
                    string[] strRunKeys = hURun.GetValueNames();
                    foreach (string strRunkey in strRunKeys)
                    {
                        //MessageBox.Show(strRunkey);
                        if (strRunkey.ToUpper().Equals(strKeyName.ToUpper()))
                        {
                            hURun.Close();
                            isExist = true;
                        }
                        
                    }
                }
                return isExist;
            }
            catch
            {
                return isExist;
            }
        }
        #endregion


        #region Set/Cancel auto start with windows system
        /// <summary>
        /// Set/Cancel auto start with windows system
        /// </summary>
        /// <param name="strKeyName">Register Key Name</param>
        /// <param name="isAutoStart">Yes/No</param>
        /// <returns>yes/no</returns>
        private bool SetRegistryKey(string strKeyName, bool isAutoStart)
        {
            bool SetSuccess = false;
            try
            {
                RegistryKey hUser = Registry.CurrentUser;
                RegistryKey hURun = hUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                if (hURun == null)
                {
                    hUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                }

                if (isAutoStart)
                {
                    hURun.SetValue(strKeyName, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                    hURun.Close();
                    SetSuccess = true;
                }
                else
                {
                    string[] strRunKeys = hURun.GetValueNames();
                    foreach (string strRunkey in strRunKeys)
                    {
                        //MessageBox.Show(strRunkey);
                        if (strRunkey.ToUpper() == strKeyName.ToUpper())
                        {
                            hURun.DeleteValue(strKeyName);
                            hURun.Close();
                            SetSuccess = true;
                        }

                    }
                }
                return SetSuccess;
            }
            catch
            {
                return SetSuccess;
            }
        }
        #endregion
    }
}