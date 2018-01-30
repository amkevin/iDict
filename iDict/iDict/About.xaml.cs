using System;
using System.Windows;

namespace iDict
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.Manual;

            double dX = SystemParameters.WorkArea.Width;
            double dY = SystemParameters.WorkArea.Height;

            this.Top = dY - this.Height;
            this.Left = dX - this.Width * 1.5;
        }

        private void mAbout_Deactivated(object sender, EventArgs e)
        {
            // close when lost focus
            this.Close();
        }
    }
}
