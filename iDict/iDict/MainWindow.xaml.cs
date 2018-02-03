using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using System.IO;
using MenuItem = System.Windows.Forms.MenuItem;
using ContextMenu = System.Windows.Forms.ContextMenu;

namespace iDict
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // variables define
        List<string> listDict = new List<string>();
        private NotifyIcon notifyIcon = null;
        private const Int32 WORDMAXNUM = 100;


        #region main window
        /// <summary>
        /// initialize main window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitialTray();
        }


        /// <summary>
        /// search box content change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxWord_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strWord = textBoxWord.Text.Trim();
            Int32 intCount = listBox.Items.Count;
            Int32 intFind = 0;

            if (strWord.Equals(""))
            {
                if (intCount > 0)
                {
                    this.listBox.Items.Clear();
                    listDict.Clear();
                    this.textBoxDetail.Text = "";
                }
                
            }
            else
            {
                if (intCount.Equals(0))
                {
                    ReadDictData(strWord);
                    if (listDict.Count > 0)
                    {
                        for (int i = 0; i < listDict.Count; i++)
                        {
                            this.listBox.Items.Add(listDict[i].Split('\t')[0].Trim());
                        }
                        this.listBox.SelectedIndex = 0;
                        this.textBoxDetail.Text = listDict[0].Split('\t')[1].Trim().Replace("~^~", "\n");
                    }
                    
                }
                for (int i = 0; i < listDict.Count; i++)
                {
                    if (listDict[i].Trim().StartsWith(strWord))
                    {
                        this.listBox.SelectedIndex = i;
                        if ((i + 8) < listDict.Count)
                        {
                            this.listBox.ScrollIntoView(this.listBox.Items[i+8]);
                        }
                        else
                        {
                            this.listBox.ScrollIntoView(this.listBox.Items[i]);
                        }
                        this.textBoxDetail.Text = listDict[i].Split('\t')[1].Trim().Replace("~^~", "\n");
                        intFind += 1;
                        break;
                    }
                }

                if (intFind.Equals(0))
                {
                    this.listBox.Items.Clear();
                    listDict.Clear();
                    this.textBoxDetail.Text = "";
                    ReadDictData(strWord);
                    if (listDict.Count > 0)
                    {
                        for (int i = 0; i < listDict.Count; i++)
                        {
                            this.listBox.Items.Add(listDict[i].Split('\t')[0].Trim());
                        }
                        this.listBox.SelectedIndex = 0;
                        this.textBoxDetail.Text = listDict[0].Split('\t')[1].Trim().Replace("~^~", "\n");
                    }
                }
            }
        }


        

        private void listBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("mouse left button up" + this.listBox.SelectedIndex.ToString());
            Int32 intIndex = this.listBox.SelectedIndex;
            if (intIndex > -1)
            {
                this.textBoxWord.Text = listDict[intIndex].Split('\t')[0].Trim();
                this.textBoxDetail.Text = listDict[intIndex].Split('\t')[1].Trim().Replace("~^~", "\n");
            }
        }
        

        /// <summary>
        /// windows closeing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = WindowState.Minimized;
            this.ShowInTaskbar = false;
        }
        #endregion


        #region read dictionary file
        /// <summary>
        /// read dictionary file
        /// </summary>
        /// <param name="strWord">word start key</param>
        private void ReadDictData(string strWord)
        {
            Int32 intWordNum = 0;
            string strDictFile = System.Windows.Forms.Application.StartupPath + @"\iDict.bin";

            if (!File.Exists(strDictFile))
            {
                return;
            }
            FileStream FS = new FileStream(strDictFile, FileMode.Open, FileAccess.Read);
            StreamReader SR = new StreamReader(FS, Encoding.Default);
            //read file by StreamReader
            SR.BaseStream.Seek(0, SeekOrigin.Begin);
            //read from dict file until last line or exceed max num records
            string strLine = SR.ReadLine();
            while (strLine != null && intWordNum <= WORDMAXNUM)
            {
                if (strLine.Trim().StartsWith(strWord))
                {
                    listDict.Add(strLine.Trim());
                    intWordNum += 1;
                }
                strLine = SR.ReadLine();
            }
            //close StreamReader object
            SR.Close();
            FS.Close();
        }
        #endregion


        #region system tray icon
        /// <summary>
        /// initialize system tray icon
        /// </summary>
        private void InitialTray()
        {
            //initialize notify icon  
            notifyIcon = new NotifyIcon();
            //notifyIcon.BalloonTipText = "iDict started...";
            notifyIcon.Text = "iDict";
            //notifyIcon.Icon = new Icon("iDict.ico");
            notifyIcon.Icon = new Icon(System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/iDict.ico", UriKind.RelativeOrAbsolute)).Stream);
            notifyIcon.Visible = true;
            //notifyIcon.ShowBalloonTip(1000);
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseClick);
            //mouse over event
            notifyIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseMove);

            //Open/Hide menu
            MenuItem open = new MenuItem("Hide(H)");
            open.Click += new EventHandler(open_Click);

            //sptarator 1
            MenuItem sptarator1 = new MenuItem("-");

            //setting
            MenuItem setting = new MenuItem("Setting(S)");
            setting.Click += new EventHandler(setting_Click);

            //about 
            MenuItem about = new MenuItem("About(A)");
            about.Click += new EventHandler(about_Click);

            //sptarator 2
            MenuItem sptarator2 = new MenuItem("-");

            //Exit 
            MenuItem exit = new MenuItem("Exit(E)");
            exit.Click += new EventHandler(exit_Click);

            //bind notify icom with menu  
            MenuItem[] childen = new MenuItem[] { open, sptarator1, setting, about, sptarator2, exit };
            notifyIcon.ContextMenu = new ContextMenu(childen);

            //bind dict window state change even to system tray notify icon  
            StateChanged += new EventHandler(SysTray_StateChanged);
        }


        /// <summary>  
        /// mouse click event
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //如果鼠标左键单击  
            if (e.Button == MouseButtons.Left)
            {
                if (WindowState == WindowState.Minimized)
                {
                    WindowState = WindowState.Normal;
                    ShowInTaskbar = true;
                    Show();
                }
                else
                {
                    WindowState = WindowState.Minimized;
                    ShowInTaskbar = false;
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                if (WindowState == WindowState.Normal)
                {
                    notifyIcon.ContextMenu.MenuItems[0].Text = "Hide(H)";
                }
                else
                {
                    notifyIcon.ContextMenu.MenuItems[0].Text = "Open(O)";
                }
            }
        }



        /// <summary>  
        /// iDict window state change event  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void SysTray_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                notifyIcon.ContextMenu.MenuItems[0].Text = "Open(O)";
                ShowInTaskbar = false;
            }
            else
            {
                notifyIcon.ContextMenu.MenuItems[0].Text = "Hide(H)";
                ShowInTaskbar = true;

            }
        }



        /// <summary>
        /// notify icon menu Open event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void open_Click(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
                ShowInTaskbar = true;
                Show();
            }
            else
            {
                WindowState = WindowState.Minimized;
                ShowInTaskbar = false;
            }
        }


        /// <summary>
        /// setting menu click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setting_Click(object sender, EventArgs e)
        {
            Setting st = new Setting();
            st.Show();
        }


        /// <summary>
        /// about menu click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void about_Click(object sender, EventArgs e)
        {
            About ab = new About();
            ab.Show();
        }


        /// <summary>  
        /// exit menu click event
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void exit_Click(object sender, EventArgs e)
        {
            //System.Windows.Application.Current.Shutdown();
            App.Current.Shutdown();
            //destroy notify icon from system tray
            notifyIcon.Dispose();
        }



        /// <summary>
        /// mouse move event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            string selectedText = System.Windows.Clipboard.GetText().Trim().ToLower();
            string selectedTextD = "";
            if ((selectedText != "") && (!selectedText.StartsWith("http")) && (selectedText.Length <= 20) && (listDict.Count > 0))
            {
                textBoxWord.Text = selectedText;
                selectedTextD = selectedText + "\n" + textBoxDetail.Text.Trim();
                if (selectedTextD.Length > 63)
                {
                    selectedTextD = selectedTextD.Substring(0, 62);
                }
                if (textBoxDetail.Text.Trim().Length == 0)
                {
                    textBoxWord.Text = "";
                    selectedTextD = "iDict";
                }
                notifyIcon.Text = selectedTextD;
            }
            else
            {
                notifyIcon.Text = "iDict";
            }
        }
        #endregion
    }
}
