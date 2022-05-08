using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace Virtual_Console_Numbify_fw.StepGenerators{
    internal class InjectNewRomGenerator{
        public static VirtualConsoleInjectionStep generate(string romFilePath){
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            toReturn.pauseStartMessage = "Will inject a rom";
            toReturn.pauseFinishedMessage = "Rom injection finnished";
            toReturn.milestoneList = new object[]{
                new object(),
                new object(),
                new object()
            };
            string romName = "";
            toReturn.process = async(InjectionEnviorunment env, MainWindowComunicator com) =>{
                string consoleCode = "";
                romName = "rom"+Path.GetExtension(romFilePath);
                romName = romName.ToLower();
                bool devilken = false;
                bool devilkencopydir = false;
                switch (env.console){
                    case Console.NES:
                        consoleCode = "5";
                    break;
                    case Console.SNES:
                        consoleCode = "1";
                    break;
                    case Console.N64:
                        consoleCode = "2";
                        devilken = true;
                        string v = Path.GetExtension(romFilePath).ToLower();
                        if (v != ".n64" && v != "z64" && v != "v64"){
                            throw new Exception("Nintendo 64 roms must have .n64, .v64 or .z64 extension ...");
                        }
                    break;
                    case Console.SMD:
                        consoleCode = "6";
                    break;
                    case Console.PCE:
                        consoleCode = "3";
                    break;
                    case Console.NGAES:
                        devilken = true;
                        devilkencopydir = true;
                    break;
                    default:
                        throw new NotImplementedException("The selected console is not supported on InjectNewRom step ...");
                }
                com.reportProgress("Copying rom ...", toReturn.milestoneList[0]);
                if (devilken){
                    await com.showFrontendMessage(
                        "The title id that you will have to type on terminal will not be used, " +
                        "it will use your choice on main interface instead", "Alert", RecipeButtonsType.ok
                    );
                    Helpers.clearDirectory(Path.Combine(env.devilkenInjectorPath, "ROMS"));
                    if (devilkencopydir){
                        await Helpers.copyAllFilesOnDirectory(
                            romFilePath, Path.Combine(env.devilkenInjectorPath, "ROMS"), true
                        );
                    }else{
                        await Helpers.CopyFileAsync(romFilePath, Path.Combine(new string[]{
                            env.devilkenInjectorPath, "ROMS", romName
                        }));
                    }
                    
                }else{
                    try { File.Delete(Path.Combine(env.autoinjectwadPath, romName)); } catch { }
                    await Helpers.CopyFileAsync(romFilePath, Path.Combine(env.autoinjectwadPath, romName));
                }
                
                com.reportProgress("Copying base wad ...", toReturn.milestoneList[1]);
                if (devilken){
                    await Helpers.CopyFileAsync(env.workingWad, Path.Combine(env.devilkenInjectorPath, "ww.wad"));
                    File.Delete(env.workingWad);
                    env.workingWad = Path.Combine(env.devilkenInjectorPath, "ww.wad");
                    com.reportProgress("Injecting rom ...", toReturn.milestoneList[2]);
                    await Helpers.ExecuteExternalProcess(
                        Path.Combine(env.devilkenInjectorPath, "VC.exe"),
                        env.devilkenInjectorPath,
                        new string[]{
                            "ww.wad",
                            "ROMS"
                        }
                    );

                    File.Move(
                        Path.Combine(env.devilkenInjectorPath, "title.wad"),
                        Path.Combine(env.autoinjectwadPath, "VC-newinjection in ww.wad")
                    );
                }else{
                    await Helpers.CopyFileAsync(env.workingWad, Path.Combine(env.autoinjectwadPath, "ww.wad"));
                    File.Delete(env.workingWad);
                    env.workingWad = Path.Combine(env.autoinjectwadPath, "ww.wad");
                    com.reportProgress("Injecting rom ...", toReturn.milestoneList[2]);
                    await Helpers.ExecuteExternalProcess(
                        Path.Combine(env.autoinjectwadPath, "injectuwad.exe"),
                        env.autoinjectwadPath,
                        new string[]{
                            romName,
                            "ww.wad",
                            "newinjection",
                            consoleCode,
                            "n"
                        }
                    );
                    File.Delete(env.workingWad);
                }
                env.workingWad = Path.Combine(env.autoinjectwadPath, "VC-newinjection in ww.wad");
            };
            
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.removeAllFilesWithAnSpecificExtensionFromDirectory(env.autoinjectwadPath, ".wad", new string[]{
                    env.workingWad
                });
                Helpers.removeAllDirectoriesFromDirectory(env.autoinjectwadPath);
                await toReturn.processCleanup(env, com);
            };

            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                try { File.Delete(env.workingWad); } catch { };
                await toReturn.preEverythingCleanup(env, com);
            };

            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.removeAllFilesWithAnSpecificExtensionFromDirectory(env.autoinjectwadPath, ".app");
                string[] allFiles = Directory.GetFiles(env.autoinjectwadPath);
                List<string> toDelete = new List<string>();
                foreach (string file in allFiles){
                    switch (Path.GetFileName(file).ToLower()){
                        case "rom.bin":
                        case "rom.gen":
                        case "rom.pce":
                        case "rom.smd":
                        case "rom.md":
                        case "rom.nes":
                        case "rom.smc":
                        case "rom.sfc":
                        case "rom.n64":
                        case "rom.v64":
                        case "rom.z64":
                        case "ww.wad":
                        case "title.cert":
                        case "title.tik":
                        case "title.tmd":
                        case "sha1out.txt":
                        case "injectorlog.txt":
                            toDelete.Add(file);
                        break;
                    }
                }
                foreach (string file in toDelete){
                    File.Delete(file);
                }
                Helpers.removeAllDirectoriesFromDirectory(env.devilkenInjectorPath);
                Helpers.removeAllFilesWithAnSpecificExtensionFromDirectory(env.devilkenInjectorPath, ".wad", new string[]{
                    env.workingWad
                });
                Directory.CreateDirectory(Path.Combine(env.devilkenInjectorPath, "ROMS"));
            };
            return toReturn;
        }
    }
}
