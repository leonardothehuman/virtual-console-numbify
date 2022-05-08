using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Virtual_Console_Numbify_fw {
    /// <summary>
    /// Lógica interna para AlertWindow.xaml
    /// </summary>
    public partial class AlertWindow : Window {
        public AlertWindow() {
            InitializeComponent();
        }
        private void Hyperlink_MouseLeftButtonDown(object sender, MouseEventArgs e) {
            var hyperlink = (Hyperlink)sender;
            Process.Start(hyperlink.NavigateUri.ToString());
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            System.Environment.Exit(0);
        }
    }
}
