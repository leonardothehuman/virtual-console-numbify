using AutoItX3Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virtual_Console_Numbify_fw.StepGenerators{
    internal class PackDataCcfGenerator{
        public static VirtualConsoleInjectionStep generate(bool manualInject, int inceptionLevel){
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            if (inceptionLevel == 0){
                toReturn.pauseStartMessage = "Will pack data.ccf";
                toReturn.pauseFinishedMessage = "Finished packing data.ccf file";
            }
            if (inceptionLevel == 1){
                toReturn.pauseStartMessage = "Will pack misc.ccf.zlib";
                toReturn.pauseFinishedMessage = "Finished packing misc.ccf.zlib file";
            }
            toReturn.milestoneList = new object[]{
                new object()
            };
            toReturn.process = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                string ccfFile = Path.Combine(new string[] { env.workingExtracted05, "data.ccf" });
                string outDir = Path.Combine(new string[] { env.workingExtracted05, @"data_ccf_OUT\" });
                string verificationText = ccfFile + @" successfully extracted!";
                if (inceptionLevel == 1){
                    com.reportProgress("Extracting misc.ccf.zlib ...", toReturn.milestoneList[0]);
                    ccfFile = Path.Combine(new string[] { env.workingExtractedCcf, "misc.ccf.zlib" });
                    outDir = Path.Combine(new string[] { env.workingExtractedCcf, @"misc_ccf_zlib_OUT\" });
                    await com.showFrontendMessage(
                        "Now, it's time to extract misc.ccf.zlib\n" +
                        "\n" +
                        "This step is automated with AutoItX, don't move the mouse until further instructions ...", "Misc.ccf.zlib", RecipeButtonsType.ok);
                }else{
                    com.reportProgress("Extracting data.ccf ...", toReturn.milestoneList[0]);
                    await com.showFrontendMessage(
                        "Now, it's time to extract data.ccf\n" +
                        "\n" +
                        "This step is automated with AutoItX, don't move the mouse until further instructions ...", "Data.ccf", RecipeButtonsType.ok);
                }

                AutoItX3 saveIconEditor = new AutoItX3();
                saveIconEditor.AutoItSetOption("MouseCoordMode", 2);
                int pid = saveIconEditor.Run(
                    Path.Combine(env.ccftool, "CCF_tool.exe"),
                    env.ccftool
                ); ;

                saveIconEditor.WinWaitActive("[CLASS:ThunderRT6FormDC]");

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

                //Optionally stops here
                if (manualInject == false){
                    saveIconEditor.ControlClick(
                        "[CLASS:ThunderRT6FormDC]", "",
                        "[CLASS:ThunderRT6CommandButton; INSTANCE:4]",
                        "primary"
                    );

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

                        throw new Exception("Failed to extract data");
                    }
                }else{
                    bool cok = await com.showFrontendMessage(
                        "A new window have appeared, it may be behind Virtual Console Numbify\n" +
                        "Only click yes or no on this dialog after this steps\n" +
                        "1. Be sure that Path to file is set to \"" + ccfFile + "\"\n" +
                        "2. Be sure that Path to folder is set to \"" + outDir + "\"\n" +
                        "3. Click extract\n" +
                        "4. Close the ccf tool window\n" +
                        "\n" +
                        "Was everythong ok with CCF tool ?", "CCF Tool", RecipeButtonsType.yesno
                    );
                    if (cok == false){
                        throw new Exception("User reported that something bad happened :-(");
                    }
                }

                if (inceptionLevel == 1){
                    env.workingExtractedCcf2 = outDir;
                }else{
                    env.workingExtractedCcf = outDir;
                }
            };
            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.removeAllDirectoriesFromDirectory(env.autoinjectwadPath);
            };
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.removeAllDirectoriesFromDirectory(env.autoinjectwadPath);
            };
            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
            };
            return toReturn;
        }
    }
}
