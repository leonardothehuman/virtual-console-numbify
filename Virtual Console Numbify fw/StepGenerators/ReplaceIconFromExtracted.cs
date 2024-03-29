﻿using AutoItX3Lib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Virtual_Console_Numbify_fw.InjectionModels;
using Virtual_Console_Numbify_fw.ViewModels;

namespace Virtual_Console_Numbify_fw.StepGenerators{
    internal class ReplaceIconFromExtracted{
        public static VirtualConsoleInjectionStep Generate(string iconFilePath, string saveName, bool manualInject, bool disableAlert){
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            string tempIcon = "tempicon" + Path.GetExtension(iconFilePath);
            toReturn.pauseStartMessage = "Will replace icon";
            toReturn.pauseFinishedMessage = "Icon replaced";
            toReturn.milestoneList = new object[]{
                new object(),
                new object()
            };
            toReturn.process = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                string tplFile = Path.Combine(Path.Combine(env.AutoinjectwadPath, @"icons\banner.tpl"));
                string appFile = Path.Combine(new string[]{env.WorkingExtracted, "00000001.app"});
                com.reportProgress("Generating save icons animations ...", toReturn.milestoneList[0]);
                string consoleCode = "";
                string dropdownString = "";
                string verificationText0 = "";
                string verificationText1 = "";
                string verificationText2 = "";
                switch (env.gameConsole)
                {
                    case GameConsole.NES:
                        consoleCode = "nes";
                        dropdownString = "NES /FC";
                        verificationText1 = @"Tpl File:  " + tplFile + @" successfully injected into " + appFile + @".";
                        verificationText2 = saveName + @" successflly injected into " + appFile;
                    break;
                    case GameConsole.SNES:
                        consoleCode = "snes";
                        dropdownString = "SNES /SFC";
                        verificationText1 = @"Tpl File:  " + tplFile + @" successfully copied to " + env.WorkingExtracted05 + @"banner.tpl.";
                        verificationText2 = saveName + @" successflly injected into " + appFile;
                    break;
                    case GameConsole.N64:
                        consoleCode = "n64";
                        dropdownString = "N64";
                        tplFile = Path.Combine(Path.Combine(env.AutoinjectwadPath, @"icons\save_banner.tpl"));
                        //appFile = Path.Combine(new string[] { env.workingExtracted, @"00000005_app_out" })+"\\";
                        appFile = env.WorkingExtracted05;
                        verificationText1 = @"Tpl File:  " + tplFile + @" successfully injected into save_banner.tpl.";
                        verificationText2 = @"N64 titles successfully Injected!";
                    break;
                    case GameConsole.SMD:
                        consoleCode = "smd";
                        dropdownString = "Genesis/ Mega Drive";
                        tplFile = Path.Combine(Path.Combine(env.AutoinjectwadPath, @"icons")) + "\\";
                        appFile = Path.Combine(new string[] { env.WorkingExtractedCcf2, @"banner.cfg.txt" });
                        Helpers.NotmalizeLineEnd(Path.Combine(env.WorkingExtractedCcf2, "banner.cfg.txt"));
                        verificationText0 = env.WorkingExtractedCcf2;
                        verificationText1 = @"WTE Files Successfully Copied!";
                        verificationText2 = @"Titles Successfully Injected into "+ appFile + @"!";
                    break;
                    case GameConsole.SMS:
                        consoleCode = "sms";
                        dropdownString = "Master System";
                        tplFile = Path.Combine(Path.Combine(env.AutoinjectwadPath, @"icons")) + "\\";
                        appFile = Path.Combine(new string[] { env.WorkingExtractedCcf2, @"banner.cfg.txt" });
                        Helpers.NotmalizeLineEnd(Path.Combine(env.WorkingExtractedCcf2, "banner.cfg.txt"));
                        if(File.Exists(Path.Combine(env.WorkingExtractedCcf2, "comment"))) {
                            File.Delete(Path.Combine(env.WorkingExtractedCcf2, "comment"));
                            using (
                                FileStream destinationStream = new FileStream(
                                    Path.Combine(env.WorkingExtractedCcf2, "comment"), FileMode.CreateNew, FileAccess.Write,
                                    FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan
                                )
                            ) {
                                byte[] stringToInsert = Encoding.UTF8.GetBytes(saveName);
                                await destinationStream.WriteAsync(stringToInsert, 0, stringToInsert.Length);
                                destinationStream.WriteByte(0x0A);
                                await destinationStream.WriteAsync(
                                    Encoding.UTF8.GetBytes("MASTER SYSTEM"), 0, 13
                                );
                                destinationStream.WriteByte(0x0A);
                                await destinationStream.FlushAsync();
                            }
                        }
                        verificationText0 = env.WorkingExtractedCcf2;
                        verificationText1 = @"WTE Files Successfully Copied!";
                        verificationText2 = @"Titles Successfully Injected into " + appFile + @"!";
                    break;
                    case GameConsole.PCE:
                        consoleCode = "pce";
                        dropdownString = "Tg-16 / PCE";
                        appFile = Path.Combine(new string[] { env.WorkingExtracted05, @"TITLE.TXT" });
                        //Helpers.notmalizeLineEnd(Path.Combine(new string[] { env.workingExtracted05, @"TITLE.TXT" }));
                        verificationText1 = tplFile + @" successfully copied to " + env.WorkingExtracted05 + @"savedata.tpl!";
                        verificationText2 = @"Titles Successfully Injected into " + appFile + @"!";
                    break;
                    case GameConsole.NGAES:
                        consoleCode = "neogeo";
                        dropdownString = "NeoGeo";
                        appFile = Path.Combine(new string[] { env.WorkingExtracted, @"00000006_app_out" }) + "\\";
                        break;
                    default:
                        throw new System.NotImplementedException("The selected console is not supported yet ...");
                }
                Directory.CreateDirectory(Path.Combine(env.AutoinjectwadPath, "icons"));

                string tempIconDest = Path.Combine(env.AutoinjectwadPath, tempIcon);
                await Helpers.CopyFileAsync(iconFilePath, tempIconDest);

                await Helpers.ExecuteExternalProcess(
                    Path.Combine(env.VCiconPath, "VC_Icon_Gen.exe"),
                    env.VCiconPath,
                    new string[] {
                        "-sys", consoleCode,
                        "-s", "\""+tempIconDest+"\"",
                        "-d", "\""+Path.Combine(env.AutoinjectwadPath, @"icons\")+"\"",
                        "-m", "s"
                    },
                    true
                );

                com.reportProgress("Replacing save icons ...", toReturn.milestoneList[1]);
                
                if(disableAlert == false){
                    await com.showFrontendMessage(
                        "Now, it's time to inject the save icon\n" +
                        "\n" +
                        "This step is automated with AutoItX, don't move the mouse until further instructions ...", "Save icon", RecipeButtonsType.ok
                    );
                }
                
                AutoItX3 saveIconEditor = new AutoItX3();
                saveIconEditor.AutoItSetOption("MouseCoordMode", 2);
                int pid = saveIconEditor.Run(
                    Path.Combine(env.VCsaveInjectPath, "vc_save_inject.exe"),
                    env.VCsaveInjectPath
                ); ;

                saveIconEditor.WinWaitActive("[CLASS:ThunderRT6FormDC]");
                string hwnd = saveIconEditor.WinGetHandle("[CLASS:ThunderRT6FormDC]");
                //Select the console from select box
                saveIconEditor.ControlCommand(
                    "[CLASS:ThunderRT6FormDC]", "",
                    "[CLASS:ThunderRT6ComboBox; INSTANCE:2]",
                    "Selectstring",
                    dropdownString
                );

                //Where the icons was saved
                saveIconEditor.ControlSetText(
                    "[CLASS:ThunderRT6FormDC]", "",
                    "[CLASS:ThunderRT6TextBox; INSTANCE:2]",
                    tplFile
                );

                //The extracted file that contains the icon
                saveIconEditor.ControlSetText(
                    "[CLASS:ThunderRT6FormDC]", "",
                    "[CLASS:ThunderRT6TextBox; INSTANCE:1]",
                    appFile
                );

                //Sets the save name
                saveIconEditor.MouseClick("primary", 148, 82);
                saveIconEditor.Send("{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}");
                saveIconEditor.Send("{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}");
                saveIconEditor.Send(saveName, 1);
                saveIconEditor.Sleep(3000);
                saveIconEditor.ControlClick(
                    "[CLASS:ThunderRT6FormDC]", "",
                    "[CLASS:ThunderRT6CommandButton; INSTANCE:1]",
                    "primary"
                );

                //Optionally stops here
                if (manualInject == false)
                {
                    //Click to inject
                    saveIconEditor.ControlClick(
                        "[CLASS:ThunderRT6FormDC]", "",
                        "[CLASS:ThunderRT6CommandButton; INSTANCE:5]",
                        "primary"
                    );

                    string successText2;
                    string[] successTexts2 = new string[] {"", ""};
                    if (verificationText0.Trim() != ""){
                        //Verify the injection confirmation dialog
                        saveIconEditor.WinWaitActive("[CLASS:#32770]");
                        successText2 = saveIconEditor.WinGetText("[CLASS:#32770]");
                        successTexts2 = successText2.Split('\n');
                        //Click OK
                        saveIconEditor.ControlClick(
                            "[CLASS:#32770]", "",
                            "[CLASS:Button; INSTANCE:1]",
                            "primary"
                        );
                    }

                    //Verify the injection confirmation dialog
                    saveIconEditor.WinWaitActive("[CLASS:#32770]");
                    string successText = saveIconEditor.WinGetText("[CLASS:#32770]");
                    string[] successTexts = successText.Split('\n');
                    //Click OK
                    saveIconEditor.ControlClick(
                        "[CLASS:#32770]", "",
                        "[CLASS:Button; INSTANCE:1]",
                        "primary"
                    );
                    //Close the injector
                    saveIconEditor.WinClose("[CLASS:ThunderRT6FormDC]");

                    if (verificationText0.Trim() != ""){
                        if (successTexts2[1].Trim().ToLower() == verificationText0.Trim().ToLower()){
                            //MessageBox.Show("Injected");
                        }else{
                            throw new System.Exception("Failed to inject icon");
                        }
                    }

                    if (
                        successTexts[1].Trim().ToLower() == verificationText1.Trim().ToLower() &&
                        successTexts[2].Trim().ToLower() == verificationText2.Trim().ToLower()
                    ){
                        //MessageBox.Show("Injected");
                    }else{
                        MessageBox.Show(successTexts[1].Trim().ToLower());
                        MessageBox.Show(verificationText1.Trim().ToLower());

                        MessageBox.Show(successTexts[2].Trim().ToLower());
                        MessageBox.Show(verificationText2.Trim().ToLower());
                        throw new System.Exception("Failed to inject icon");
                    }
                }
                else
                {
                    bool cok = await com.showFrontendMessage(
                        "A new window has appeared, it may be behind Virtual Console Numbify\n" +
                        "Only click yes or no on this dialog after these steps\n" +
                        "1. Be sure that the correct console is selected\n" +
                        "2. Be sure that save title is set to \"" + saveName + "\" and click Set All\n" +
                        "3. Be sure that source tpl is set to \"" + tplFile + "\"\n" +
                        "4. Be sure that file to inject is set to \"" + appFile + "\"\n" +
                        "5. Click inject\n" +
                        "6. Be sure that we had no errors while injecting\n" +
                        "7. Close the icon injector window\n" +
                        "\n" +
                        "Was everything ok with icon injector ?", "Save icon", RecipeButtonsType.yesno
                    );
                    if(cok == false)
                    {
                        throw new System.Exception("User reported that something bad happened :-(");
                    }
                }
            };
            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.RemoveAllFilesWithAnSpecificExtensionFromDirectory(env.VCiconPath, ".tga");
                Helpers.RemoveAllDirectoriesFromDirectory(env.AutoinjectwadPath);
                File.Delete(Path.Combine(env.AutoinjectwadPath, tempIcon));
            };
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.RemoveAllFilesWithAnSpecificExtensionFromDirectory(env.VCiconPath, ".tga");
                Helpers.RemoveAllDirectoriesFromDirectory(env.AutoinjectwadPath);
                File.Delete(Path.Combine(env.AutoinjectwadPath, tempIcon));
            };
            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Directory.Delete(Path.Combine(env.AutoinjectwadPath, @"icons"), true);
                File.Delete(Path.Combine(env.AutoinjectwadPath, tempIcon));
            };
            return toReturn;
        }
    }
}
