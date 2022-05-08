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

namespace Virtual_Console_Numbify
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VirtualConsoleOptionsManager virtualConsoleOptionsManager = new();

        private delegate Task runAsync();
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            consoleSelect.ItemsSource = virtualConsoleOptionsManager.supportedConsoles;
        }

        private System.Drawing.Image resizeImage(System.Drawing.Image img, int x, int y)
        {
            System.Drawing.Image newimage = new Bitmap(x, y);
            using (Graphics gfx = Graphics.FromImage(newimage))
            {
                gfx.DrawImage(img, 0, 0, x, y);
            }
            return newimage;
        }

        public static async Task CopyFileAsync(string sourceFile, string destinationFile)
        {
            using (var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
            using (var destinationStream = new FileStream(destinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
                await sourceStream.CopyToAsync(destinationStream);
        }

        private void consoleSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "WiiWare file (*.wad)|*.wad|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                wadFile.Text = openFileDialog.FileName;
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "WiiWare file (*.wad)|*.wad|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                romFile.Text = openFileDialog.FileName;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Jpeg File (*.jpg)|*.wad|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                bannerImageFile.Text = openFileDialog.FileName;
        }

        private void replace_tpl_image(WAD wadFile, string binContainerName, string tplFileName, string newImagePath)
        {
            U8 binContainer = new();
            if (!wadFile.HasBanner)
                throw new Exception("CustomizeMii only edits Channel WADs!");

            for (int i = 0; i < wadFile.BannerApp.NumOfNodes; i++)
            {
                if (wadFile.BannerApp.StringTable[i].ToLower() == binContainerName)
                {
                    binContainer.LoadFile(wadFile.BannerApp.Data[i]);
                }
            }

            List<string> bannerTpls = new List<string>();
            TPL_TextureFormat bannerFormat = TPL_TextureFormat.CI8;
            TPL_PaletteFormat pFormat = TPL_PaletteFormat.RGB5A3;
            string casedTplFileName = "";
            for (int i = 0; i < binContainer.NumOfNodes; i++)
            {
                if (binContainer.StringTable[i].ToLower().EndsWith(".tpl"))
                {
                    try
                    {
                        TPL tmpTpl = TPL.Load(binContainer.Data[i]);

                        string formatString = tmpTpl.GetTextureFormat(0).ToString();
                        if (formatString.StartsWith("CI")) formatString += "+" + tmpTpl.GetPaletteFormat(0);

                        if (binContainer.StringTable[i].ToLower() == tplFileName)
                        {
                            bannerFormat = tmpTpl.GetTextureFormat(0);
                            pFormat = tmpTpl.GetPaletteFormat(0);
                            casedTplFileName = binContainer.StringTable[i];
                        }

                        bannerTpls.Add(binContainer.StringTable[i]);
                    }
                    catch { }
                }
            }

            TPL tplFile;
            System.Drawing.Image img;

            tplFile = TPL.Load(binContainer.Data[binContainer.GetNodeIndex(casedTplFileName)]);

            img = System.Drawing.Image.FromFile(newImagePath);

            if (img.Width != tplFile.GetTextureSize(0).Width || img.Height != tplFile.GetTextureSize(0).Height)
                img = resizeImage(img, tplFile.GetTextureSize(0).Width, tplFile.GetTextureSize(0).Height);

            tplFile.RemoveTexture(0);
            tplFile.AddTexture(img, bannerFormat, pFormat);

            binContainer.ReplaceFile(binContainer.GetNodeIndex(casedTplFileName), tplFile.ToByteArray());

            binContainer.AddHeaderImd5();
            binContainer.Lz77Compress = true;

            for (int i = 0; i < wadFile.BannerApp.NumOfNodes; i++)
                if (wadFile.BannerApp.StringTable[i].ToLower() == binContainerName)
                { wadFile.BannerApp.ReplaceFile(i, binContainer.ToByteArray()); break; }
        }

        private void extractBannerBrlyt(WAD wadFile)
        {
            string fileNameToExtract = "banner.brlyt";

            U8 banner = new();
            if (!wadFile.HasBanner)
                throw new Exception("CustomizeMii only edits Channel WADs!");

            for (int i = 0; i < wadFile.BannerApp.NumOfNodes; i++)
            {
                if (wadFile.BannerApp.StringTable[i].ToLower() == "banner.bin")
                {
                    banner.LoadFile(wadFile.BannerApp.Data[i]);
                }
            }

            try
            {
                File.WriteAllBytes(
                    @"C:\Users\Leonardo\Desktop\autoinjectwad\banner.brlyt",
                    banner.Data[banner.GetNodeIndex(fileNameToExtract)]
                 );
            }
            catch (Exception ex) { }
        }

        private void replaceBannerBrlyt(WAD wadFile)
        {
            string fileNameToReplace = "banner.brlyt";

            U8 banner = new();
            if (!wadFile.HasBanner)
                throw new Exception("CustomizeMii only edits Channel WADs!");

            for (int i = 0; i < wadFile.BannerApp.NumOfNodes; i++)
            {
                if (wadFile.BannerApp.StringTable[i].ToLower() == "banner.bin")
                {
                    banner.LoadFile(wadFile.BannerApp.Data[i]);
                }
            }

            try
            {
                banner.ReplaceFile(
                    banner.GetNodeIndex(fileNameToReplace),
                    File.ReadAllBytes(@"C:\Users\Leonardo\Desktop\HowardC_Tools\VCbrlyt9.0\banner.brlyt")
                );

                for (int i = 0; i < wadFile.BannerApp.NumOfNodes; i++)
                    if (wadFile.BannerApp.StringTable[i].ToLower() == "banner.bin")
                    { wadFile.BannerApp.ReplaceFile(i, banner.ToByteArray()); break; }
            }
            catch (Exception ex) { }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            runAsync f = async delegate ()
            {
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

                WAD w = new();
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

            };
            f();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //runAsync f = async delegate (){
                AutoItX3 saveIconEditor = new AutoItX3();
                int pid = saveIconEditor.Run(
                    @"C:\Users\Leonardo\Desktop\HowardC_Tools\VCSaveInject5.0\vc_save_inject.exe",
                    @"C:\Users\Leonardo\Desktop\HowardC_Tools\VCSaveInject5.0\"
                );
                
                saveIconEditor.WinWaitActive("[CLASS: ThunderRT6FormDC]");
                saveIconEditor.ControlCommand(
                    "[CLASS:ThunderRT6FormDC]", "",
                    "[CLASS:ThunderRT6ComboBox; INSTANCE:1]",
                    "Selectstring",
                    "Master System"
                );
                //saveIconEditor.  ThunderRT6ComboBox [CLASS:ThunderRT6FormDC] [CLASS:ThunderRT6ComboBox; INSTANCE:1]
            //};
            //f();
        }
    }
}
