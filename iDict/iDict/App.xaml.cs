﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace iDict
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex;

        public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            // avoid app be opened more than twice
            bool ret;
            mutex = new System.Threading.Mutex(true, "iDict", out ret);

            if (!ret)
            {
                Environment.Exit(0);
            }

        }
    }
}
