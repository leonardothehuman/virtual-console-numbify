using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Virtual_Console_Numbify_fw.StepGenerators;

namespace Virtual_Console_Numbify_fw.Models
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        public VirtualConsoleOptionsManager virtualConsoleOptionsManager = new VirtualConsoleOptionsManager();
        private delegate Task runAsync();
        public Command Inject { get; set; }
        public Command BrowseFile { get; set; }

        public Command BrowseRom { get; set; }
        private string baseWad = "";
        public string BaseWad {
            get { return baseWad; }
            set {
                baseWad = value;
                validateForm();
                NotyfyChange();
            }
        }

        private Console? selectedConsole = null;
        public Console? SelectedConsole
        {
            get { return selectedConsole; }
            set {
                selectedConsole = value;
                validateForm();
                NotyfyChange();
            }
        }

        private string romFileCompletePath = "";
        public string RomFileCompletePath {
            get { return romFileCompletePath; }
            set {
                romFileCompletePath = value;
                validateForm();
                NotyfyChange();
            }
        }

        private string bannerImageCompletePath = "";
        public string BannerImageCompletePath{
            get { return bannerImageCompletePath; }
            set
            {
                bannerImageCompletePath = value;
                validateForm();
                NotyfyChange();
            }
        }

        private string iconImageCompletePath = "";
        public string IconImageCompletePath{
            get { return iconImageCompletePath; }
            set {
                iconImageCompletePath = value;
                validateForm();
                NotyfyChange();
            }
        }

        private string saveIconCompletePath = "";
        public string SaveIconCompletePath{
            get { return saveIconCompletePath; }
            set {
                saveIconCompletePath = value;
                validateForm();
                NotyfyChange();
            }
        }

        private string channelNameTitle = "";
        public string ChannelNameTitle{
            get { return channelNameTitle; }
            set {
                string toSet = Helpers.AllowOnlyOneCircunflexDiacritic(value);
                channelNameTitle = Helpers.TruncateString(toSet, 20);
                validateForm();
                NotyfyChange();
            }
        }

        private string bannerTitle = "";
        public string BannerTitle{
            get { return bannerTitle; }
            set {
                string toSet = Helpers.AllowOnlyOneCircunflexDiacritic(value);
                bannerTitle = Helpers.TruncateString(toSet, 20);
                validateForm();
                NotyfyChange();
            }
        }

        private string bannerYear = "";
        public string BannerYear{
            get { return bannerYear; }
            set {
                string toSet = Helpers.ForceChars(value, "01234567890");
                toSet = Helpers.RemoveFromBeginning(toSet, '0');
                toSet = Helpers.TruncateString(toSet, 4);
                bannerYear = toSet;

                validateForm();
                NotyfyChange();
            }
        }

        private string bannerMaximumPlayerCount = "";
        public string BannerMaximumPlayerCount {
            get { return bannerMaximumPlayerCount; }
            set {
                string toSet = Helpers.ForceChars(value, "01234567890");
                toSet = Helpers.RemoveFromBeginning(toSet, '0');
                toSet = Helpers.TruncateString(toSet, 1);
                bannerMaximumPlayerCount = toSet;
                validateForm();
                NotyfyChange();
            }
        }

        private string saveName = "";
        public string SaveName{
            get { return saveName; }
            set {
                string toSet = Helpers.AllowOnlyOneCircunflexDiacritic(value);
                saveName = Helpers.TruncateString(toSet, 20);
                validateForm();
                NotyfyChange();
            }
        }

        private string newId = "";
        public string NewId{
            get { return newId; }
            set {
                newId = Helpers.ForceWadId(value);
                validateForm();
                NotyfyChange();
            }
        }

        private bool allowEditing = false;
        public bool AllowEditing{
            get { return allowEditing; }
            set {
                allowEditing = value;
                validateForm();
                NotyfyChange();
            }
        }

        private bool useLz7 = false;
        public bool UseLz7{
            get { return useLz7; }
            set {
                useLz7 = value;
                validateForm();
                NotyfyChange();
            }
        }

        private bool pauseOnEveryStep = false;
        public bool PauseOnEveryStep{
            get { return pauseOnEveryStep; }
            set {
                pauseOnEveryStep = value;
                validateForm();
                NotyfyChange();
            }
        }

        private bool disableAutoitXAlert = false;
        public bool DisableAutoitXAlert{
            get { return disableAutoitXAlert; }
            set {
                disableAutoitXAlert = value;
                validateForm();
                NotyfyChange();
            }
        }

        private float progress = 0;
        public float Progress{
            get { return progress; }
            set { 
                progress = value;
                NotyfyChange();
            }
        }

        private string status = "";
        public string Status{
            get { return status; }
            set { 
                status = value;
                NotyfyChange();
            }
        }

        private bool allFieldsAreEnabled = true;
        public bool AllFieldsAreEnabled{
            get { return allFieldsAreEnabled; }
            set {
                allFieldsAreEnabled = value;
                validateForm();
                NotyfyChange();
            }
        }
        private void NotyfyChange([CallerMemberName] string controlName = ""){
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(controlName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void validateForm(){
            Inject.ChangeCanExecute();
            BrowseFile.ChangeCanExecute();
            BrowseRom.ChangeCanExecute();
        }

        public void ResetAllFields(){
            BaseWad = "";
            SelectedConsole = null;
            RomFileCompletePath = "";
            BannerImageCompletePath = "";
            IconImageCompletePath = "";
            SaveIconCompletePath = "";
            ChannelNameTitle = "";
            BannerTitle = "";
            BannerYear = "";
            BannerMaximumPlayerCount = "";
            SaveName = "";
            NewId = "";
            AllowEditing = false;
            UseLz7 = false;
            PauseOnEveryStep = false;
            DisableAutoitXAlert = false;
        }

        
        MainWindowComunicator.ShowFrontendMessage frontendMessageDelegate = null;
        public delegate Task<string> BrowseFileDelegate(string fileFilter);
        public delegate Task<string> BrowseDirectoryDelegate(string title);
        private BrowseFileDelegate browseFileDelegate;
        private BrowseDirectoryDelegate browseDirectoryDelegate;
        public MainWindowModel(MainWindowComunicator.ShowFrontendMessage del, BrowseFileDelegate bfd, BrowseDirectoryDelegate bdd)
        {
            frontendMessageDelegate = del;
            browseFileDelegate = bfd;
            browseDirectoryDelegate = bdd;

            Dictionary<string, string> fieldFileTypes = new Dictionary<string, string>();
            fieldFileTypes.Add("BaseWad", "WiiWare file (*.wad)|*.wad|All files (*.*)|*.*");
            fieldFileTypes.Add("BannerImageCompletePath", "Image File (*.bmp; *.jpg; *.png; *.gif; *.jpeg)|*.bmp;*.jpg;*.png;*.gif;*.jpeg|All files (*.*)|*.*");
            fieldFileTypes.Add("IconImageCompletePath", "Image File (*.bmp; *.jpg; *.png; *.gif; *.jpeg)|*.bmp;*.jpg;*.png;*.gif;*.jpeg|All files (*.*)|*.*");
            fieldFileTypes.Add("SaveIconCompletePath", "Image File (*.bmp; *.jpg; *.png; *.gif; *.jpeg)|*.bmp;*.jpg;*.png;*.gif;*.jpeg|All files (*.*)|*.*");


            BrowseFile = new Command((object obj) => {
                runAsync f = async delegate () {
                    string FieldToFill = (string)obj;
                    PropertyInfo propertyInfo = this.GetType().GetProperty(FieldToFill);
                    string toSet = await browseFileDelegate(fieldFileTypes[FieldToFill]);
                    if (toSet == "") return;
                    propertyInfo.SetValue(this, Convert.ChangeType(toSet, propertyInfo.PropertyType), null);
                };
                f();
            }, (object obj) =>{
                if (AllFieldsAreEnabled == false) return false;
                return true;
            });

            BrowseRom = new Command((object obj) => {
                runAsync f = async delegate () {
                    string result = "";
                    if (selectedConsole == Console.NGAES){
                        result = await browseDirectoryDelegate("Select a Neo-Geo AES rom directory");
                    }else{
                        string filter = virtualConsoleOptionsManager.defaultExtensions;
                        if (selectedConsole != null){
                            filter = virtualConsoleOptionsManager.extensions[(Console)SelectedConsole];
                        }
                        result = await browseFileDelegate(filter);
                    }

                    if (result == "") return;
                    RomFileCompletePath = result;
                };
                f();
            }, (object obj) => {
                if (AllFieldsAreEnabled == false) return false;
                return true;
            });

            Inject = new Command((object obj) => {
                runAsync f = async delegate () {
                    AllFieldsAreEnabled = false;
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "WiiWare file (*.wad)|*.wad|All files (*.*)|*.*";
                    saveFileDialog1.Title = "Where to save the injected file ...";

                    if (saveFileDialog1.ShowDialog() != true)
                    {
                        AllFieldsAreEnabled = true;
                        return;
                    }

                    try
                    {
                        InjectionEnviorunment enviorunment = new InjectionEnviorunment();
                        enviorunment.externalToolsBasePath = @"C:\Users\Leonardo\Desktop";
                        enviorunment.finalWadFile = saveFileDialog1.FileName;
                        enviorunment.console = (Console)SelectedConsole;
                        try { File.Delete(System.IO.Path.Combine(enviorunment.autoinjectwadPath, "initial.wad")); } catch { }
                        await Helpers.CopyFileAsync(
                            BaseWad,
                            System.IO.Path.Combine(enviorunment.autoinjectwadPath, "initial.wad")
                        );
                        enviorunment.workingWad = System.IO.Path.Combine(enviorunment.autoinjectwadPath, "initial.wad");

                        VirtualConsoleInjectionRecipe recipe = new VirtualConsoleInjectionRecipe(enviorunment, PauseOnEveryStep);
                        recipe.progressReported += (object sender2, ProgressReportEventArgs args) => {
                            Status = args.progressMessage;
                            Progress = args.progressNumber;
                        };
                        recipe.setFrontendMessageDelegate(frontendMessageDelegate);
                        

                        if (enviorunment.console != Console.SMS)
                        {
                            recipe.addStep(InjectNewRomGenerator.generate(RomFileCompletePath));
                        }
                        recipe.addStep(CustomizeGeneratedWadGenerator.generate(
                            ChannelNameTitle, NewId, UseLz7, BannerImageCompletePath, IconImageCompletePath,
                            BannerTitle, Int32.Parse(BannerYear), Int32.Parse(BannerMaximumPlayerCount)
                        ));
                        recipe.addStep(ExtractWadGenerator.generate());
                        recipe.addStep(ExtractZeroFiveGenerator.generate());

                        if (enviorunment.console == Console.NGAES)
                            recipe.addStep(FindNeoGeoBannerBinGenerator.generate());

                        if (enviorunment.console == Console.SMD || enviorunment.console == Console.SMS)
                        {
                            recipe.addStep(ExtractDataCcfGenerator.generate(AllowEditing, 0, false, DisableAutoitXAlert));
                            recipe.addStep(ExtractDataCcfGenerator.generate(AllowEditing, 1, false, DisableAutoitXAlert));
                        }
                        if (enviorunment.console == Console.SMS)
                        {
                            recipe.addStep(ReplaceSMSRom.generate(RomFileCompletePath));
                        }

                        if (enviorunment.console == Console.NGAES)
                        {
                            recipe.addStep(GenerateNeoGeoBannerGenerator.generate(SaveIconCompletePath, SaveName));
                        }
                        else
                        {
                            recipe.addStep(ReplaceIconFromExtracted.generate(
                                SaveIconCompletePath, SaveName, AllowEditing, DisableAutoitXAlert
                            ));
                        }
                        recipe.addStep(RemoveManualFromExtractedGenerator.generate());

                        if (enviorunment.console == Console.SMD || enviorunment.console == Console.SMS)
                        {
                            recipe.addStep(ExtractDataCcfGenerator.generate(AllowEditing, 1, true, DisableAutoitXAlert));
                            recipe.addStep(ExtractDataCcfGenerator.generate(AllowEditing, 0, true, DisableAutoitXAlert));
                        }

                        if (enviorunment.console == Console.NGAES)
                            recipe.addStep(PackNeoGeoBannerIfInADifferentFile.generate());

                        recipe.addStep(PackZeroFiveGenerator.generate());
                        recipe.addStep(PackExtractedWadGenerator.generate());
                        await recipe.executeSteps();

                        await frontendMessageDelegate(
                            "Your injected virtual console wad has been generated, the nintendo wii is " +
                            "a very fragile system, so please, no matter how perfect the injection tool " +
                            "claims to be, always test the wad file on an emunand before installing it on " +
                            "a real nand, and, before installing it on real nand, be sure that you have " +
                            "priiloader installed and it is working properly !!!",
                            "Finished !!!",
                            RecipeButtonsType.ok
                        );
                        ResetAllFields();
                    }
                    catch (Exception ex)
                    {
                        await frontendMessageDelegate(
                            ex.Message,
                            "Error !!!",
                            RecipeButtonsType.ok
                        );
                    }

                    //Status = "Idle";
                    Progress = 0;
                    AllFieldsAreEnabled = true;
                };
                Task t = f();
            }, (object obj) => {
                if (AllFieldsAreEnabled == false) return false;
                if (!Helpers.IsAValidWinPath(BaseWad.Trim())){
                    Status = "You must specify a Wad file";
                    return false;
                }
                if (SelectedConsole == null){
                    Status = "You must specify a console for the injection";
                    return false;
                }
                if (!Helpers.IsAValidWinPath(RomFileCompletePath.Trim())){
                    Status = "You must specify a Rom file";
                    return false;
                }
                if (!Helpers.IsAValidWinPath(BannerImageCompletePath.Trim())){
                    Status = "You must specify an image to use on banner";
                    return false;
                }
                if (!Helpers.IsAValidWinPath(IconImageCompletePath.Trim())){
                    Status = "You must specify an image to use as icon";
                    return false;
                }
                if (!Helpers.IsAValidWinPath(SaveIconCompletePath.Trim())){
                    Status = "You must specify an image to use as save icon";
                    return false;
                }

                if (ChannelNameTitle.Trim().Length <= 0){
                    Status = "You must specify a the channel title";
                    return false;
                }
                if (BannerTitle.Trim().Length <= 0){
                    Status = "You must specify a text to appear on the banner";
                    return false;
                }
                if (BannerYear.Trim().Length < 4){
                    Status = "You must specify the game's release year";
                    return false;
                }
                if (BannerMaximumPlayerCount.Trim().Length <= 0){
                    Status = "You must specify the game's player count";
                    return false;
                }
                if (SaveName.Trim().Length <= 0){
                    Status = "You must specify the name of the save file";
                    return false;
                }
                if (NewId.Trim().Length > 0 && NewId.Length < 4){
                    Status = "You must specify a valid Wad id or leave it empty";
                    return false;
                }

                Status = "You can now inject the rom if everything is correct";

                return true;
            });
        }
    }
}
