using libWiiSharp;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Drawing;
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
using AutoItX3Lib;
using Virtual_Console_Numbify_fw.StepGenerators;
using System.Text.RegularExpressions;

namespace Virtual_Console_Numbify_fw
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate Task runAsync();
        public MainWindow(){
            DataContext = this;
            InitializeComponent();
            var mainWindowModel = new MainWindowViewModel(async (string message, string title, RecipeButtonsType type) => {
                this.Activate();
                if (type == RecipeButtonsType.ok){
                    MessageBox.Show(message, title);
                    return true;
                } else if (type == RecipeButtonsType.yesno){
                    //this.Activate();
                    MessageBoxResult dialogResult = MessageBox.Show(message, title, MessageBoxButton.YesNo);
                    if (dialogResult == MessageBoxResult.Yes){
                        return true;
                    } else if (dialogResult == MessageBoxResult.No){
                        return false;
                    }
                }
                return false;
            }, async (string filter) =>{
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = filter;
                if (openFileDialog.ShowDialog() == true) return openFileDialog.FileName;
                return "";
            }, async (string t) => {
                using (var directoryDialog = new System.Windows.Forms.FolderBrowserDialog()){
                    directoryDialog.Description = t;
                    directoryDialog.ShowNewFolderButton = false;
                    System.Windows.Forms.DialogResult result = directoryDialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK){
                        return directoryDialog.SelectedPath;
                    }
                }
                return "";
            }, async (string f, string t) => {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "WiiWare file (*.wad)|*.wad|All files (*.*)|*.*";
                saveFileDialog1.Title = "Where to save the injected file ...";
                if (saveFileDialog1.ShowDialog() != true) {
                    return "";
                }
                return saveFileDialog1.FileName;
            });
            this.DataContext = mainWindowModel;
            consoleSelect.ItemsSource = mainWindowModel.virtualConsoleOptionsManager.supportedConsoles;
        }
    }
}
