﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dialogowe_pkp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DbConnector connector = new DbConnector();
            DispatchSync(connector.retrieve);

            Content = new WelcomePage(this, connector);
        }
        protected void DispatchSync(Action action)
        {
            Dispatcher.Invoke(action);
        }
    }
}
