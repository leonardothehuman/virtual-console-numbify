using AutoItX3Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Virtual_Console_Numbify_fw.StepGenerators{
    internal class ExtractDataCcfGenerator{
        public static VirtualConsoleInjectionStep Generate(bool manualInject, int inceptionLevel, bool reverse, bool disableAlert){
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            if(inceptionLevel == 0){
                toReturn.pauseStartMessage = "Will extract data.ccf";
                toReturn.pauseFinishedMessage = "Finished extracting data.ccf file";
            }
            if (inceptionLevel == 1){
                toReturn.pauseStartMessage = "Will extract misc.ccf.zlib";
                toReturn.pauseFinishedMessage = "Finished extracting misc.ccf.zlib file";
            }
            if(reverse == true){
                if (inceptionLevel == 0){
                    toReturn.pauseStartMessage = "Will pack data.ccf";
                    toReturn.pauseFinishedMessage = "Finished packing data.ccf file";
                }if (inceptionLevel == 1){
                    toReturn.pauseStartMessage = "Will pack misc.ccf.zlib";
                    toReturn.pauseFinishedMessage = "Finished packing misc.ccf.zlib file";
                }
            }
            toReturn.milestoneList = new object[]{
                new object()
            };
            toReturn.process = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                string ccfFile = Path.Combine(new string[] { env.WorkingExtracted05, "data.ccf" });
                string outDir = Path.Combine(new string[] { env.WorkingExtracted05, @"data_ccf_OUT\" });
                string verificationText = ccfFile + @" successfully extracted!";
                string oldFile = "";
                if (reverse == true){
                    if (inceptionLevel == 1){
                        com.reportProgress("Packing misc.ccf.zlib ...", toReturn.milestoneList[0]);
                        ccfFile = Path.Combine(new string[] { env.WorkingExtractedCcf, "misc.ccf.zlib" });
                        outDir = Path.Combine(new string[] { env.WorkingExtractedCcf, @"misc_ccf_zlib_OUT\" });
                        oldFile = Path.Combine(new string[] { env.WorkingExtractedCcf, "misc.ccf.zlib.old" });
                        File.Move(ccfFile, oldFile);
                        if (disableAlert == false){
                            await com.showFrontendMessage(
                                "Now, it's time to pack misc.ccf.zlib\n" +
                                "\n" +
                                "This step is automated with AutoItX, don't move the mouse until further instructions ...", "Misc.ccf.zlib", RecipeButtonsType.ok
                            );
                        }
                    }else{
                        com.reportProgress("Packing data.ccf ...", toReturn.milestoneList[0]);
                        oldFile = Path.Combine(new string[] { env.WorkingExtracted05, "data.ccf.old" });
                        File.Move(ccfFile, oldFile);
                        if (disableAlert == false){
                            await com.showFrontendMessage(
                                "Now, it's time to pack data.ccf\n" +
                                "\n" +
                                "This step is automated with AutoItX, don't move the mouse until further instructions ...", "Data.ccf", RecipeButtonsType.ok
                            );
                        }
                    }
                    verificationText = ccfFile + @" successfully created!";
                }
                else{
                    if (inceptionLevel == 1){
                        com.reportProgress("Extracting misc.ccf.zlib ...", toReturn.milestoneList[0]);
                        ccfFile = Path.Combine(new string[] { env.WorkingExtractedCcf, "misc.ccf.zlib" });
                        outDir = Path.Combine(new string[] { env.WorkingExtractedCcf, @"misc_ccf_zlib_OUT\" });
                        if (disableAlert == false){
                            await com.showFrontendMessage(
                                "Now, it's time to extract misc.ccf.zlib\n" +
                                "\n" +
                                "This step is automated with AutoItX, don't move the mouse until further instructions ...", "Misc.ccf.zlib", RecipeButtonsType.ok
                            );
                        }
                    }else{
                        com.reportProgress("Extracting data.ccf ...", toReturn.milestoneList[0]);
                        if (disableAlert == false){
                            await com.showFrontendMessage(
                                "Now, it's time to extract data.ccf\n" +
                                "\n" +
                                "This step is automated with AutoItX, don't move the mouse until further instructions ...", "Data.ccf", RecipeButtonsType.ok
                            );
                        }
                    }
                    verificationText = ccfFile + @" successfully extracted!";
                }

                AutoItX3 saveIconEditor = new AutoItX3();
                saveIconEditor.AutoItSetOption("MouseCoordMode", 2);
                int pid = saveIconEditor.Run(
                    Path.Combine(env.Ccftool, "CCF_tool.exe"),
                    env.Ccftool
                ); ;

                saveIconEditor.WinWaitActive("[CLASS:ThunderRT6FormDC]");
                saveIconEditor.Sleep(1000);

                saveIconEditor.ControlSetText(
                    "[CLASS:ThunderRT6FormDC]", "",
                    "[CLASS:ThunderRT6TextBox; INSTANCE:2]",
                    ccfFile
                );

                saveIconEditor.ControlSetText(
                    "[CLASS:ThunderRT6FormDC]", "",
                    "[CLASS:ThunderRT6TextBox; INSTANCE:1]",
                    outDir
                );
                saveIconEditor.Sleep(1500);

                //Optionally stops here
                if (manualInject == false){
                    if (reverse == true){
                        saveIconEditor.ControlClick(
                            "[CLASS:ThunderRT6FormDC]", "",
                            "[CLASS:ThunderRT6CommandButton; INSTANCE:3]",
                            "primary"
                        );
                    }else{
                        saveIconEditor.ControlClick(
                            "[CLASS:ThunderRT6FormDC]", "",
                            "[CLASS:ThunderRT6CommandButton; INSTANCE:4]",
                            "primary"
                        );
                    }

                    saveIconEditor.WinWaitActive("[CLASS:#32770]");
                    string successText = saveIconEditor.WinGetText("[CLASS:#32770]");
                    string[] successTexts = successText.Split('\n');
                    //Click OK
                    saveIconEditor.ControlClick(
                        "[CLASS:#32770]", "",
                        "[CLASS:Button; INSTANCE:1]",
                        "primary"
                    );
                    saveIconEditor.WinClose("[CLASS:ThunderRT6FormDC]");

                    if (successTexts[1].Trim().ToLower() == verificationText.Trim().ToLower()){
                        //MessageBox.Show("Injected");
                    }else{
                        //MessageBox.Show(successTexts[1].Trim().ToLower());
                        //MessageBox.Show(verificationText.Trim().ToLower());
                        if (reverse == true){
                            throw new Exception("Failed to pack data");
                        }else{
                            throw new Exception("Failed to extract data");
                        }
                    }
                }else{
                    string ex = "3. Click extract\n";
                    if (reverse == true) ex = "3. Click pack\n";
                    bool cok = await com.showFrontendMessage(
                        "A new window has appeared, it may be behind Virtual Console Numbify\n" +
                        "Only click yes or no on this dialog after these steps\n" +
                        "1. Be sure that Path to file is set to \"" + ccfFile + "\"\n" +
                        "2. Be sure that Path to folder is set to \"" + outDir + "\"\n" +
                        ex +
                        "4. Close the CCF tool window\n" +
                        "\n" +
                        "Was everything ok with CCF tool ?", "CCF Tool", RecipeButtonsType.yesno
                    );
                    if (cok == false){
                        throw new Exception("User reported that something bad happened :-(");
                    }
                }


                if (reverse == true){
                    if (inceptionLevel == 1){
                        Directory.Delete(env.WorkingExtractedCcf2, true);
                        env.WorkingExtractedCcf2 = "";
                    }
                    else{
                        Directory.Delete(env.WorkingExtractedCcf, true);
                        env.WorkingExtractedCcf = "";
                    }
                    File.Delete(oldFile);
                }
                else{
                    if (inceptionLevel == 1){
                        env.WorkingExtractedCcf2 = outDir;
                    }else{
                        env.WorkingExtractedCcf = outDir;
                    }
                }
                //await com.showFrontendMessage("nothing", "", RecipeButtonsType.ok);
            };
            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.RemoveAllDirectoriesFromDirectory(env.AutoinjectwadPath);
            };
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.RemoveAllDirectoriesFromDirectory(env.AutoinjectwadPath);
            };
            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
            };
            return toReturn;
        }
    }
}
