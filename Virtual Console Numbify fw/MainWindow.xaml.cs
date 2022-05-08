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
using Virtual_Console_Numbify_fw.Models;

namespace Virtual_Console_Numbify_fw
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VirtualConsoleOptionsManager virtualConsoleOptionsManager = new VirtualConsoleOptionsManager();

        private delegate Task runAsync();
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            var mainWindowModel = new MainWindowModel(async (string message, string title, RecipeButtonsType type) => {
                this.Activate();
                if (type == RecipeButtonsType.ok)
                {
                    MessageBox.Show(message, title);
                    return true;
                }
                else if (type == RecipeButtonsType.yesno)
                {
                    //this.Activate();
                    MessageBoxResult dialogResult = MessageBox.Show(message, title, MessageBoxButton.YesNo);
                    if (dialogResult == MessageBoxResult.Yes)
                    {
                        return true;
                    }
                    else if (dialogResult == MessageBoxResult.No)
                    {
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
            });
            this.DataContext = mainWindowModel;
            consoleSelect.ItemsSource = mainWindowModel.virtualConsoleOptionsManager.supportedConsoles;
        }

        private void consoleSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "WiiWare file (*.wad)|*.wad|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                wadFile.Text = openFileDialog.FileName;
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(consoleSelect.SelectedItem == null){
                openFileDialog.Filter = virtualConsoleOptionsManager.defaultExtensions;
            }else{
                Console selectedConsole = ((KeyValuePair<Console, string>)consoleSelect.SelectedItem).Key;
                if(selectedConsole == Console.NGAES){
                    using (var directoryDialog = new System.Windows.Forms.FolderBrowserDialog()){
                        directoryDialog.Description = "Select a Neo-Geo AES rom directory";
                        directoryDialog.ShowNewFolderButton = false;
                        System.Windows.Forms.DialogResult result = directoryDialog.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.OK){
                            romFile.Text = directoryDialog.SelectedPath;
                        }
                    }
                    return;
                }
                openFileDialog.Filter = virtualConsoleOptionsManager.extensions[selectedConsole];
            }
            
            if (openFileDialog.ShowDialog() == true)
                romFile.Text = openFileDialog.FileName;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image File (*.bmp; *.jpg; *.png; *.gif; *.jpeg)|*.bmp;*.jpg;*.png;*.gif;*.jpeg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                bannerImageFile.Text = openFileDialog.FileName;
        }
        
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            runAsync f = async delegate (){
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "WiiWare file (*.wad)|*.wad|All files (*.*)|*.*";
                saveFileDialog1.Title = "Where to save the injected file ...";

                if (saveFileDialog1.ShowDialog() != true){
                    return;
                }

                try
                {
                    InjectionEnviorunment enviorunment = new InjectionEnviorunment();
                    enviorunment.externalToolsBasePath = @"C:\Users\Leonardo\Desktop";
                    enviorunment.finalWadFile = saveFileDialog1.FileName;
                    enviorunment.console = ((KeyValuePair<Console, string>)consoleSelect.SelectedItem).Key;
                    try { File.Delete(System.IO.Path.Combine(enviorunment.autoinjectwadPath, "initial.wad")); } catch { }
                    await Helpers.CopyFileAsync(
                        wadFile.Text,
                        System.IO.Path.Combine(enviorunment.autoinjectwadPath, "initial.wad")
                    );
                    enviorunment.workingWad = System.IO.Path.Combine(enviorunment.autoinjectwadPath, "initial.wad");

                    bool disabreAutoitAlert;
                    if (disableAutoitBl.IsChecked == null || disableAutoitBl.IsChecked == false){
                        disabreAutoitAlert = false;
                    }else{
                        disabreAutoitAlert = true;
                    }

                    bool pause;
                    if (pauseBl.IsChecked == null || pauseBl.IsChecked == false){
                        pause = false;
                    } else{
                        pause = true;
                    }
                    VirtualConsoleInjectionRecipe recipe = new VirtualConsoleInjectionRecipe(enviorunment, pause);
                    recipe.progressReported += (object sender2, ProgressReportEventArgs args) => {
                        statusLabel.Content = args.progressMessage;
                        progressBar.Value = args.progressNumber;
                    };
                    recipe.setFrontendMessageDelegate(async (string message, string title, RecipeButtonsType type) =>{
                        if(type == RecipeButtonsType.ok){
                            MessageBox.Show(message, title);
                            return true;
                        }else if(type == RecipeButtonsType.yesno){
                            this.Activate();
                            MessageBoxResult dialogResult = MessageBox.Show(message, title, MessageBoxButton.YesNo);
                            if (dialogResult == MessageBoxResult.Yes){
                                return true;
                            }else if (dialogResult == MessageBoxResult.No){
                                return false;
                            }
                        }
                        return false;
                    });
                    
                    bool lzEnabled;
                    if (enableLz7.IsChecked == null || enableLz7.IsChecked == false){
                        lzEnabled = false;
                    }else{
                        lzEnabled = true;
                    }
                    
                    bool manualEnabled;
                    if (manualSave.IsChecked == null || manualSave.IsChecked == false){
                        manualEnabled = false;
                    }else{
                        manualEnabled = true;
                    }

                    if (enviorunment.console != Console.SMS){
                        recipe.addStep(InjectNewRomGenerator.generate(romFile.Text));
                    }
                    recipe.addStep(CustomizeGeneratedWadGenerator.generate(
                        channelName.Text, changeIdBox.Text, lzEnabled, bannerImageFile.Text,
                        iconImageFile.Text, bannerTitle.Text, Int32.Parse(bannerYear.Text),
                        Int32.Parse(playerCount.Text)
                    ));
                    recipe.addStep(ExtractWadGenerator.generate());
                    recipe.addStep(ExtractZeroFiveGenerator.generate());

                    if(enviorunment.console == Console.NGAES)
                        recipe.addStep(FindNeoGeoBannerBinGenerator.generate());

                    if (enviorunment.console == Console.SMD || enviorunment.console == Console.SMS){
                        recipe.addStep(ExtractDataCcfGenerator.generate(manualEnabled, 0, false, disabreAutoitAlert));
                        recipe.addStep(ExtractDataCcfGenerator.generate(manualEnabled, 1, false, disabreAutoitAlert));
                    }
                    if(enviorunment.console == Console.SMS){
                        recipe.addStep(ReplaceSMSRom.generate(romFile.Text));
                    }

                    if (enviorunment.console == Console.NGAES){
                        recipe.addStep(GenerateNeoGeoBannerGenerator.generate(saveIcon.Text, saveName.Text));
                    }else{
                        recipe.addStep(ReplaceIconFromExtracted.generate(saveIcon.Text, saveName.Text, manualEnabled, disabreAutoitAlert));
                    }
                    recipe.addStep(RemoveManualFromExtractedGenerator.generate());

                    if (enviorunment.console == Console.SMD || enviorunment.console == Console.SMS){
                        recipe.addStep(ExtractDataCcfGenerator.generate(manualEnabled, 1, true, disabreAutoitAlert));
                        recipe.addStep(ExtractDataCcfGenerator.generate(manualEnabled, 0, true, disabreAutoitAlert));
                    }

                    if (enviorunment.console == Console.NGAES)
                        recipe.addStep(PackNeoGeoBannerIfInADifferentFile.generate());

                    recipe.addStep(PackZeroFiveGenerator.generate());
                    recipe.addStep(PackExtractedWadGenerator.generate());
                    await recipe.executeSteps();
                    MessageBox.Show(
                        "Your injected virtual console wad has been generated, the nintendo wii is " +
                        "a very fragile system, so please, no matter how perfect the injection tool " +
                        "claims to be, always test the wad file on an emunand before installing it on " +
                        "a real nand, and, before installing it on real nand, be sure that you have " +
                        "priiloader installed and it is working properly !!!"
                    );
                }
                catch (Exception ex)
                {
                    this.Activate();
                    MessageBox.Show(ex.Message);
                }

                statusLabel.Content = "Idle";
                progressBar.Value = 0;
                
                
                
                /*
                    await CopyFileAsync(romFile.Text, @"C:\Users\Leonardo\Desktop\autoinjectwad\rom.bin");
                    await CopyFileAsync(wadFile.Text, @"C:\Users\Leonardo\Desktop\autoinjectwad\ww.wad");
                    string command = "rom.bin ww.wad " + channelName.Text + " 5 n";
                    //command += "\"" + romFile.Text + "\"" + " ";
                    //command += "\"" + wadFile.Text + "\"" + " ";
                    //command += "\"" + channelName.Text + "\"" + " 5 n";
                    using (System.Diagnostics.Process pProcess = new System.Diagnostics.Process())
                    {
                        pProcess.StartInfo.FileName = @"C:\Users\Leonardo\Desktop\autoinjectwad\injectuwad.exe";
                        pProcess.StartInfo.Arguments = command; //argument
                        pProcess.StartInfo.WorkingDirectory = @"C:\Users\Leonardo\Desktop\autoinjectwad\";
                        pProcess.StartInfo.UseShellExecute = false;
                        //pProcess.StartInfo.RedirectStandardOutput = true;
                        //pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        //pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                        pProcess.Start();
                        //string output = pProcess.StandardOutput.ReadToEnd(); //The output result
                        pProcess.WaitForExit();
                    }
                */

                /*
                WAD w = new WAD();
                w.LoadFile(@"C:\Users\Leonardo\Desktop\autoinjectwad\VC-TextBox in ww.wad");
                replace_tpl_image(w, "banner.bin", "vcpic.tpl", bannerImageFile.Text);
                replace_tpl_image(w, "icon.bin", "iconvcpic.tpl", bannerImageFile.Text);
                extractBannerBrlyt(w);

                await CopyFileAsync(
                    @"C:\Users\Leonardo\Desktop\autoinjectwad\banner.brlyt",
                    @"C:\Users\Leonardo\Desktop\HowardC_Tools\VCbrlyt9.0\banner.brlyt"
                );

                using (System.Diagnostics.Process pProcess = new System.Diagnostics.Process())
                {
                    pProcess.StartInfo.FileName = @"C:\Users\Leonardo\Desktop\HowardC_Tools\VCbrlyt9.0\vcbrlyt.exe";
                    pProcess.StartInfo.Arguments = "banner.brlyt -Title \"" + bannerTitle.Text + "\" -YEAR " + bannerYear.Text + " -Play " + playerCount.Text; //argument
                    pProcess.StartInfo.WorkingDirectory = @"C:\Users\Leonardo\Desktop\HowardC_Tools\VCbrlyt9.0\";
                    pProcess.StartInfo.UseShellExecute = false;
                    //pProcess.StartInfo.RedirectStandardOutput = true;
                    //pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    //pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                    pProcess.Start();
                    //string output = pProcess.StandardOutput.ReadToEnd(); //The output result
                    pProcess.WaitForExit();
                }

                replaceBannerBrlyt(w);

                w.Save(@"C:\Users\Leonardo\Desktop\autoinjectwad\bannerReplaced.wad");

                using (System.Diagnostics.Process pProcess = new System.Diagnostics.Process())
                {
                    pProcess.StartInfo.FileName = @"C:\Users\Leonardo\Desktop\autoinjectwad\wadunpacker.exe";
                    pProcess.StartInfo.Arguments = "bannerReplaced.wad"; //argument
                    pProcess.StartInfo.WorkingDirectory = @"C:\Users\Leonardo\Desktop\autoinjectwad\";
                    pProcess.StartInfo.UseShellExecute = false;
                    //pProcess.StartInfo.RedirectStandardOutput = true;
                    //pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    //pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                    pProcess.Start();
                    //string output = pProcess.StandardOutput.ReadToEnd(); //The output result
                    pProcess.WaitForExit();
                }

                string titleId = w.TitleID.ToString().PadLeft(16, '0');

                using (System.Diagnostics.Process pProcess = new System.Diagnostics.Process())
                {
                    pProcess.StartInfo.FileName = @"C:\Users\Leonardo\Desktop\HowardC_Tools\VCIcon8.0\VC_Icon_Gen.exe";
                    pProcess.StartInfo.Arguments = "-sys nes -s \"" + bannerImageFile.Text + "\" -d \"C:\\Users\\Leonardo\\Desktop\\autoinjectwad\\\" -m s"; //argument
                    pProcess.StartInfo.WorkingDirectory = @"C:\Users\Leonardo\Desktop\HowardC_Tools\VCIcon8.0\";
                    pProcess.StartInfo.UseShellExecute = false;
                    //pProcess.StartInfo.RedirectStandardOutput = true;
                    //pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    //pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                    pProcess.Start();
                    //string output = pProcess.StandardOutput.ReadToEnd(); //The output result
                    pProcess.WaitForExit();
                }
                */
            };
            Task t = f();
            
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //runAsync f = async delegate (){
            AutoItX3 saveIconEditor = new AutoItX3();
            saveIconEditor.AutoItSetOption("MouseCoordMode", 2);
            int pid = saveIconEditor.Run(
                @"C:\Users\Leonardo\Desktop\HowardC_Tools\VCSaveInject5.0\vc_save_inject.exe",
                @"C:\Users\Leonardo\Desktop\HowardC_Tools\VCSaveInject5.0\"
            );

            saveIconEditor.WinWaitActive("[CLASS:ThunderRT6FormDC]");
            string hwnd = saveIconEditor.WinGetHandle("[CLASS:ThunderRT6FormDC]");
            saveIconEditor.ControlCommand(
                "[CLASS:ThunderRT6FormDC]", "",
                "[CLASS:ThunderRT6ComboBox; INSTANCE:2]",
                "Selectstring",
                "NES /FC"
            );

            string tplFile = @"C:\Users\Leonardo\Desktop\autoinjectwad\banner.tpl";
            saveIconEditor.ControlSetText(
                "[CLASS:ThunderRT6FormDC]", "",
                "[CLASS:ThunderRT6TextBox; INSTANCE:2]",
                tplFile
            );

            string appFile = @"C:\Users\Leonardo\Desktop\autoinjectwad\0001000146584947\00000001.app";
            saveIconEditor.ControlSetText(
                "[CLASS:ThunderRT6FormDC]", "",
                "[CLASS:ThunderRT6TextBox; INSTANCE:1]",
                appFile
            );

            saveIconEditor.MouseClick("primary", 148, 82);
            saveIconEditor.Send("{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}");
            saveIconEditor.Send("{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}");
            saveIconEditor.Send(bannerTitle.Text, 1);

            saveIconEditor.ControlClick(
                "[CLASS:ThunderRT6FormDC]", "",
                "[CLASS:ThunderRT6CommandButton; INSTANCE:1]",
                "primary"
            );

            //Optionally stops here

            saveIconEditor.ControlClick(
                "[CLASS:ThunderRT6FormDC]", "",
                "[CLASS:ThunderRT6CommandButton; INSTANCE:5]",
                "primary"
            );

            string verificationText1 = @"Tpl File:  "+ tplFile + @" successfully injected into "+ appFile + @".";
            string verificationText2 = bannerTitle.Text + @" successflly injected into " + appFile;

            saveIconEditor.WinWaitActive("[CLASS:#32770]");

            string successText = saveIconEditor.WinGetText("[CLASS:#32770]");

            string[] successTexts = successText.Split('\n');

            saveIconEditor.ControlClick(
                "[CLASS:#32770]", "",
                "[CLASS:Button; INSTANCE:1]",
                "primary"
            );

            saveIconEditor.WinClose("[CLASS:ThunderRT6FormDC]");

            if (successTexts[1].Trim() == verificationText1.Trim() && successTexts[2].Trim() == verificationText2.Trim()){
                MessageBox.Show("Injected");
            }
            else{
                MessageBox.Show("Error");
            }

            
            //saveIconEditor.  ThunderRT6ComboBox [CLASS:ThunderRT6FormDC] [CLASS:ThunderRT6ComboBox; INSTANCE:1]
            //};
            //f();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            using (System.Diagnostics.Process pProcess = new System.Diagnostics.Process()){
                pProcess.StartInfo.FileName = @"C:\Users\Leonardo\Desktop\autoinjectwad\wadpacker.exe";
                pProcess.StartInfo.Arguments = "0001000146584947.tik 0001000146584947.tmd 0001000146584947.cert out.wad -sign";
                pProcess.StartInfo.WorkingDirectory = @"C:\Users\Leonardo\Desktop\autoinjectwad\0001000146584947";
                pProcess.StartInfo.UseShellExecute = false;
                //pProcess.StartInfo.RedirectStandardOutput = true;
                //pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                //pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                pProcess.Start();
                //string output = pProcess.StandardOutput.ReadToEnd(); //The output result
                pProcess.WaitForExit();
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image File (*.bmp; *.jpg; *.png; *.gif; *.jpeg)|*.bmp;*.jpg;*.png;*.gif;*.jpeg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                iconImageFile.Text = openFileDialog.FileName;
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image File (*.bmp; *.jpg; *.png; *.gif; *.jpeg)|*.bmp;*.jpg;*.png;*.gif;*.jpeg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                saveIcon.Text = openFileDialog.FileName;
        }

        private void channelName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        private void bannerYear_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (bannerYear.Text.Length >= 4)
            {
                e.Handled = true;
                return;
            }
            e.Handled = !IsTextAllowed(e.Text);
        }
        private void BannerYearPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
                if(text.Length > 4)
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void playerCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (playerCount.Text.Length >= 1)
            {
                e.Handled = true;
                return;
            }
            e.Handled = !IsTextAllowed(e.Text);
        }
        private void PlayerCountPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
                if (text.Length > 1)
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            //GenerateNeoGeoBannerGenerator.generate().process(new InjectionEnviorunment(), new MainWindowComunicator());
        }
    }
}
