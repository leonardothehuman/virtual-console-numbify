﻿using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Virtual_Console_Numbify_fw.InjectionModels;
using Virtual_Console_Numbify_fw.StepGenerators;

namespace Virtual_Console_Numbify_fw.ViewModels {
    public class MainWindowViewModel : INotifyPropertyChanged{
        public VirtualConsoleOptionsManager virtualConsoleOptionsManager = new VirtualConsoleOptionsManager();
        private delegate Task runAsync();
        private delegate void runSync();
        public Command Inject { get; set; }
        public Command BrowseFile { get; set; }
        public Command BrowseRom { get; set; }
        public Command DownloadUpdate { get; set; }
        private string baseWad = "";
        public string BaseWad {
            get { return baseWad; }
            set {
                baseWad = value;
                validateForm();
                NotyfyChange();
            }
        }

        private GameConsole? selectedConsole = null;
        public GameConsole? SelectedConsole
        {
            get { return selectedConsole; }
            set {
                selectedConsole = value;
                SaveName = SaveName;
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
                string toSet = Helpers.BlockCircunflexDiacritic(value);
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
                if(
                    SelectedConsole == GameConsole.SMS ||
                    SelectedConsole == GameConsole.SMD
                //|| SelectedConsole == GameConsole.MSX
                ) {
                    toSet = Helpers.BlockCircunflexDiacritic(toSet);
                }
                saveName = Helpers.TruncateString(toSet, 20);
                validateForm();
                NotyfyChange();
            }
        }

        private string saveNameLabel = "";
        public string SaveNameLabel {
            get { return saveNameLabel; }
            set {
                saveNameLabel = value;
                NotyfyChange();
            }
        }

        private string newId = "";
        public string NewId{
            get { return newId; }
            set {
                newId = Helpers.ForceWadId(value);
                NotyfyChange();
                if (
                    newId.Length > 0 && (
                        newId[0] != 'C' &&
                        newId[0] != 'E' &&
                        newId[0] != 'F' &&
                        newId[0] != 'J' &&
                        newId[0] != 'L' &&
                        newId[0] != 'M' &&
                        newId[0] != 'N' &&
                        newId[0] != 'P' &&
                        newId[0] != 'Q' &&
                        newId[0] != 'X'
                    )
                ) {
                    frontendMessageDelegate(
                        "Only C, E, F, J, L, M, N, P, Q or X are allowed as first char on Title ID",
                        "Warning", RecipeButtonsType.ok
                    );
                    newId = "";
                }
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

        private bool statusAlert = false;
        public bool StatusAlert {
            get { return statusAlert; }
            set {
                statusAlert = value;
                NotyfyChange();
            }
        }

        private bool allFieldsAreEnabled = false;
        public bool AllFieldsAreEnabled{
            get { return allFieldsAreEnabled; }
            set {
                allFieldsAreEnabled = value;
                validateForm();
                NotyfyChange();
            }
        }

        private string updateText = "";
        public string UpdateText {
            get { return updateText; }
            set {
                updateText = value;
                NotyfyChange();
            }
        }

        private string updateButton = "";
        public string UpdateButton {
            get { return updateButton; }
            set {
                updateButton = value;
                NotyfyChange();
            }
        }

        private string updateUrl = "";
        public string UpdateUrl {
            get { return updateUrl; }
            set {
                updateUrl = value;
                NotyfyChange();
            }
        }
        private void NotyfyChange([CallerMemberName] string controlName = ""){
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(controlName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void validateForm(){
            if (
                    SelectedConsole == GameConsole.SMS ||
                    SelectedConsole == GameConsole.SMD
                //|| SelectedConsole == GameConsole.MSX
                ) {
                SaveNameLabel = "Save Name:";
            } else {
                SaveNameLabel = "Save Name:    (use ^ to break line)";
            }
            Inject.ChangeCanExecute();
            BrowseFile.ChangeCanExecute();
            BrowseRom.ChangeCanExecute();
            DownloadUpdate.ChangeCanExecute();
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
        public delegate Task<string> SaveFileDelegate(string fileFilter, string title);
        private BrowseFileDelegate browseFileDelegate;
        private BrowseDirectoryDelegate browseDirectoryDelegate;
        private SaveFileDelegate saveFileDelegate;
        public MainWindowViewModel(
            MainWindowComunicator.ShowFrontendMessage del, BrowseFileDelegate bfd,
            BrowseDirectoryDelegate bdd, SaveFileDelegate sfd
        ){
            frontendMessageDelegate = del;
            browseFileDelegate = bfd;
            browseDirectoryDelegate = bdd;
            saveFileDelegate = sfd;

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
                    if (selectedConsole == GameConsole.NGAES){
                        result = await browseDirectoryDelegate("Select a Neo-Geo AES rom directory");
                    }else{
                        string filter = virtualConsoleOptionsManager.defaultExtensions;
                        if (selectedConsole != null){
                            filter = virtualConsoleOptionsManager.extensions[(GameConsole)SelectedConsole];
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
                    try{
                        AllFieldsAreEnabled = false;

                        BaseWad = BaseWad.Trim();
                        RomFileCompletePath = RomFileCompletePath.Trim();
                        BannerImageCompletePath = BannerImageCompletePath.Trim();
                        IconImageCompletePath = IconImageCompletePath.Trim();
                        SaveIconCompletePath = SaveIconCompletePath.Trim();

                        ChannelNameTitle = ChannelNameTitle.Trim();
                        BannerTitle = BannerTitle.Trim();
                        BannerYear = BannerYear.Trim();
                        BannerMaximumPlayerCount = BannerMaximumPlayerCount.Trim();
                        SaveName = SaveName.Trim();
                        NewId = NewId.Trim();

                        if (!File.Exists(BaseWad)) { throw new Exception("The specified base wad file does not exists"); }
                        if (selectedConsole == GameConsole.NGAES) {
                            if (!Directory.Exists(RomFileCompletePath)) {
                                throw new Exception("The specified rom directory does not exists");
                            }
                            if(!Helpers.CheckIfDirectoryHasAtLeastOneFileWithAnSpecifiedExtension(romFileCompletePath, ".bin")) {
                                throw new Exception(
                                    "The selected directory does not seem to be a Neo Geo Aes rom directory"
                                );
                            }
                        } else {
                            if (!File.Exists(RomFileCompletePath)) {
                                throw new Exception("The specified rom file does not exists");
                            }
                        }
                        if (!File.Exists(BannerImageCompletePath)) { throw new Exception("The specified banner image does not exists"); }
                        if (!File.Exists(IconImageCompletePath)) { throw new Exception("The specified icon image does not exists"); }
                        if (!File.Exists(SaveIconCompletePath)) { throw new Exception("The specified save icon does not exist"); }

                        string fileToSave = await saveFileDelegate(
                            "WiiWare file (*.wad)|*.wad|All files (*.*)|*.*",
                            "Where to save the injected file ..."
                        );

                        if (fileToSave == "") {
                            AllFieldsAreEnabled = true;
                            return;
                        }
                        if (File.Exists(fileToSave)) { throw new Exception("You can not specify an existing output file"); }
                        InjectionEnviorunment enviorunment = new InjectionEnviorunment();
                        //enviorunment.ExternalToolsBasePath = Path.Combine(Helpers.GetExeDirectory(), "externalTools");
                        //enviorunment.ExternalToolsBasePath = @"C:\Users\Leonardo\Desktop";
                        enviorunment.FinalWadFile = fileToSave;
                        enviorunment.gameConsole = (GameConsole)SelectedConsole;
                        try { File.Delete(System.IO.Path.Combine(enviorunment.AutoinjectwadPath, "initial.wad")); } catch { }
                        await Helpers.CopyFileAsync(
                            BaseWad,
                            System.IO.Path.Combine(enviorunment.AutoinjectwadPath, "initial.wad")
                        );
                        enviorunment.WorkingWad = System.IO.Path.Combine(enviorunment.AutoinjectwadPath, "initial.wad");

                        VirtualConsoleInjectionRecipe recipe = new VirtualConsoleInjectionRecipe(enviorunment, PauseOnEveryStep);
                        recipe.progressReported += (object sender2, ProgressReportEventArgs args) => {
                            StatusAlert = false;
                            Status = args.progressMessage;
                            Progress = args.progressNumber;
                        };
                        recipe.SetFrontendMessageDelegate(frontendMessageDelegate);

                        if (enviorunment.gameConsole != GameConsole.SMS){
                            recipe.AddStep(InjectNewRomGenerator.Generate(RomFileCompletePath));
                        }
                        recipe.AddStep(CustomizeGeneratedWadGenerator.Generate(
                            ChannelNameTitle, NewId, UseLz7, BannerImageCompletePath, IconImageCompletePath,
                            BannerTitle, Int32.Parse(BannerYear), Int32.Parse(BannerMaximumPlayerCount)
                        ));
                        recipe.AddStep(ExtractWadGenerator.Generate());
                        recipe.AddStep(ExtractZeroFiveGenerator.Generate());

                        if (enviorunment.gameConsole == GameConsole.NGAES)
                            recipe.AddStep(FindNeoGeoBannerBinGenerator.Generate());

                        if (enviorunment.gameConsole == GameConsole.SMD || enviorunment.gameConsole == GameConsole.SMS){
                            recipe.AddStep(ExtractDataCcfGenerator.Generate(AllowEditing, 0, false, DisableAutoitXAlert));
                            recipe.AddStep(ExtractDataCcfGenerator.Generate(AllowEditing, 1, false, DisableAutoitXAlert));
                        }if (enviorunment.gameConsole == GameConsole.SMS){
                            recipe.AddStep(ReplaceSMSRom.Generate(RomFileCompletePath));
                        }

                        if (enviorunment.gameConsole == GameConsole.NGAES){
                            recipe.AddStep(GenerateNeoGeoBannerGenerator.Generate(SaveIconCompletePath, SaveName));
                        }else{
                            recipe.AddStep(ReplaceIconFromExtracted.Generate(
                                SaveIconCompletePath, SaveName, AllowEditing, DisableAutoitXAlert
                            ));
                        }
                        recipe.AddStep(RemoveManualFromExtractedGenerator.Generate());

                        if (enviorunment.gameConsole == GameConsole.SMD || enviorunment.gameConsole == GameConsole.SMS){
                            recipe.AddStep(ExtractDataCcfGenerator.Generate(AllowEditing, 1, true, DisableAutoitXAlert));
                            recipe.AddStep(ExtractDataCcfGenerator.Generate(AllowEditing, 0, true, DisableAutoitXAlert));
                        }

                        if (enviorunment.gameConsole == GameConsole.NGAES)
                            recipe.AddStep(PackNeoGeoBannerIfInADifferentFile.Generate());

                        recipe.AddStep(PackZeroFiveGenerator.Generate());
                        recipe.AddStep(PackExtractedWadGenerator.Generate());
                        await recipe.ExecuteSteps();

                        await frontendMessageDelegate(
                            "Your injected virtual GameConsole wad has been generated, the Nintendo Wii is " +
                            "a very fragile system, so please, no matter how perfect the injection tool " +
                            "claims to be, always test the wad file on an emunand before installing it on " +
                            "a real nand, and, before installing it on real nand, be sure that you have " +
                            "priiloader and bootmii installed and they are working properly !!!",
                            "Finished !!!",
                            RecipeButtonsType.ok
                        );
                        ResetAllFields();
                    }catch (Exception ex){
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
                    StatusAlert = true;
                    Status = "You must specify a Wad file";
                    return false;
                }
                if (SelectedConsole == null){
                    StatusAlert = true;
                    Status = "You must specify a GameConsole for the injection";
                    return false;
                }
                if (!Helpers.IsAValidWinPath(RomFileCompletePath.Trim())){
                    StatusAlert = true;
                    if (selectedConsole == GameConsole.NGAES) {
                        Status = "You must specify a rom directory";
                    } else {
                        Status = "You must specify a rom file";
                    }
                    return false;
                }
                if (!Helpers.IsAValidWinPath(BannerImageCompletePath.Trim())){
                    StatusAlert = true;
                    Status = "You must specify an image to use on the banner";
                    return false;
                }
                if (!Helpers.IsAValidWinPath(IconImageCompletePath.Trim())){
                    StatusAlert = true;
                    Status = "You must specify an image to use as an icon";
                    return false;
                }
                if (!Helpers.IsAValidWinPath(SaveIconCompletePath.Trim())){
                    StatusAlert = true;
                    Status = "You must specify an image to use as a save icon";
                    return false;
                }

                if (ChannelNameTitle.Trim().Length <= 0){
                    StatusAlert = true;
                    Status = "You must specify the channel title";
                    return false;
                }
                if (BannerTitle.Trim().Length <= 0){
                    StatusAlert = true;
                    Status = "You must specify a text to appear on the banner";
                    return false;
                }
                if (BannerYear.Trim().Length < 4){
                    StatusAlert = true;
                    Status = "You must specify the game's release year";
                    return false;
                }
                if (BannerMaximumPlayerCount.Trim().Length <= 0){
                    StatusAlert = true;
                    Status = "You must specify the game's player count";
                    return false;
                }
                if (SaveName.Trim().Length <= 0){
                    StatusAlert = true;
                    Status = "You must specify the name of the save file";
                    return false;
                }
                if (NewId.Trim().Length > 0 && NewId.Trim().Length < 4){
                    StatusAlert = true;
                    Status = "You must specify a valid Wad id or leave it empty";
                    return false;
                }

                StatusAlert = false;
                Status = "You can now inject the rom if everything is correct";

                return true;
            });

            runAsync f2 = async delegate () {
                try {
                    if (!File.Exists(Path.Combine(Helpers.GetExeDirectory(), "common-key.bin"))) {
                        StatusAlert = true;
                        Status = "common-key.bin not found";
                        await frontendMessageDelegate(
                            "The common-key.bin file was not found, please, put this file on the program's directory and try again\n" +
                            "\n" +
                            //"But, where can you find this file ?, well, the only legalized criminal gang in this world does not allow me to redistribute this file, so you have to find it on your own, in the same place that you got the rom (I know that it was not on a physical cartridge that you own :-) ), you also have the option to extract it from your own Wii, which is the harder, but the legalized method of doing that ...\n" +
                            "But, where can you find this file? well, you have to find it on your own for legal reasons. You should extract it from your own Wii ...\n" +
                            "\n" +
                            "You can also put an empty file instead of the real common-key.bin, but the injection process will fail :-(", "Error: common-key.bin not found", RecipeButtonsType.ok);
                        return;
                    };
                    string dir = Helpers.GetExeDirectory();
                    char[] validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123456789 \\/:".ToCharArray();
                    string invalidList = "";
                    for (int i = 0; i < dir.Length; i++) {
                        if(!Array.Exists(validChars, e => e == dir[i])) {
                            invalidList += dir[i];
                        }
                    }
                    if(invalidList != "") {
                        await frontendMessageDelegate(
                            "Sorry, but some third-party tools that this program uses are very picky with the chars " +
                            "that are allowed on the path, otherwise, it will not work correctly so, I decided to only " +
                            "allow this program to run if extracted to a path that only contains alphanumeric " +
                            "chars, number and spaces ... \n" +
                            "\n" +
                            "The current path is: "+ dir + "\n" +
                            "\n" +
                            "The invalid chars on this path "+ 
                            Helpers.IsOrAre(Helpers.UniqueChars(invalidList).Length) + ": "+
                            Helpers.SplitCharsIntoHumanReadbleList(Helpers.UniqueChars(invalidList)) + "\n" +
                            "\n" +
                            "Sorry Japanese users :-(",
                            "Error",
                            RecipeButtonsType.ok
                        );
                        StatusAlert = true;
                        Status = "You must move Virtual GameConsole Numbify to a valid path.";
                        return;
                    }
                    InjectionEnviorunment env = new InjectionEnviorunment();
                    File.Delete(Path.Combine(env.AutoinjectwadPath, "common-key.bin"));
                    //if (!File.Exists(Path.Combine(env.AutoinjectwadPath, "common-key.bin"))) {
                    await Helpers.CopyFileAsync(
                        Path.Combine(Helpers.GetExeDirectory(), "common-key.bin"),
                        Path.Combine(env.AutoinjectwadPath, "common-key.bin")
                    );
                    //}
                    File.Delete(Path.Combine(env.DevilkenInjectorPath, "common-key.bin"));
                    //if (!File.Exists(Path.Combine(env.DevilkenInjectorPath, "common-key.bin"))) {
                    await Helpers.CopyFileAsync(
                        Path.Combine(Helpers.GetExeDirectory(), "common-key.bin"),
                        Path.Combine(env.DevilkenInjectorPath, "common-key.bin")
                    );
                   // }
                    if(
                        !File.Exists(Path.Combine(Environment.SystemDirectory, "FM20.DLL")) ||
                        !File.Exists(Path.Combine(Environment.SystemDirectory, "msvbvm60.dll"))
                    ) {
                        await frontendMessageDelegate(
                            "Don't forget to read the Readme-Virtual-GameConsole-Numbify.txt file to ensure that all required libraries are installed.\n" +
                            "\n" +
                            "It seems like one of the prerequisites is not installed, but, since I am not sure if the checks are 100% correct, I will let you use the application, just, read the readme.txt file again if something goes wrong and ALWAYS TEST THE OUPUT WAD on an emunand before installing it on the real nand.",
                            "Alert",
                            RecipeButtonsType.ok
                        );
                    }
                    AllFieldsAreEnabled = true;
                } catch (Exception ex) {
                    await frontendMessageDelegate(
                        ex.Message,
                        "Error !!!",
                        RecipeButtonsType.ok
                    );
                }
            };
            f2();

            DownloadUpdate = new Command((object obj) => {
                Process.Start(UpdateUrl);
            }, (object obj) => {
                if (AllFieldsAreEnabled == false) return false;
                return true;
            });

            runAsync f3 = async delegate () {
                try {
                    var currentTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                    int currentVersionInt = Config.CurrentVersion;
                    string checkFile = Path.Combine(Helpers.GetExeDirectory(), "lastUpdateCheck.json");
                    var checkFileDefinition = new {
                        lastCheckTimeStamp = 0,
                        serverString = ""
                    };
                    var updateDefinition = new {
                        latestVersionInteger = 0,
                        downloadUrl = "",
                        updateMessage = "",
                        updateButton = ""
                    };


                    long lastCheckTimeStamp = 0;
                    string serverString = "";
                    runSync checkLocalCache = () => {
                        if (File.Exists(checkFile)) {
                            try {
                                var checkData = JsonConvert.DeserializeAnonymousType(
                                    File.ReadAllText(checkFile), checkFileDefinition
                                );
                                lastCheckTimeStamp = checkData.lastCheckTimeStamp;
                                serverString = checkData.serverString;
                            } catch (Exception ex) {
                                lastCheckTimeStamp = 0;
                                serverString = "";
                            }
                        }
                    };
                    checkLocalCache();
                    
                    if (currentTimeStamp > lastCheckTimeStamp + Config.UpdateCheckInterval) {
                        try {
                            serverString = await Helpers.GetStringFromInternet(Config.UpdateUrl);
                            lastCheckTimeStamp = currentTimeStamp;
                        } catch (Exception ex) {
                            checkLocalCache();
                        }
                    }
                    if (serverString == "") return;

                    try {
                        var updateJson = JsonConvert.DeserializeAnonymousType(serverString, updateDefinition);
                        if (currentVersionInt < updateJson.latestVersionInteger) {
                            UpdateText = updateJson.updateMessage;
                            UpdateUrl = updateJson.downloadUrl;
                            UpdateButton = updateJson.updateButton;
                        }

                        File.WriteAllText(checkFile, JsonConvert.SerializeObject(new {
                            lastCheckTimeStamp = lastCheckTimeStamp,
                            serverString = serverString
                        }));
                    } catch (Exception ex) {
                        File.Delete(checkFile);
                        throw ex;
                    }
                } catch (Exception ex) {
                }
            };
            f3();
        }
    }
}
