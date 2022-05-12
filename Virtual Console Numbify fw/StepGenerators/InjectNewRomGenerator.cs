using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Virtual_Console_Numbify_fw.InjectionModels;
using Virtual_Console_Numbify_fw.ViewModels;

namespace Virtual_Console_Numbify_fw.StepGenerators{
    internal class InjectNewRomGenerator{
        public static VirtualConsoleInjectionStep Generate(string romFilePath){
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
                switch (env.gameConsole){
                    case GameConsole.NES:
                        consoleCode = "5";
                    break;
                    case GameConsole.SNES:
                        consoleCode = "1";
                    break;
                    case GameConsole.N64:
                        consoleCode = "2";
                        devilken = true;
                        string v = Path.GetExtension(romFilePath).ToLower();
                        if (v != ".n64" && v != "z64" && v != "v64"){
                            throw new System.Exception("Nintendo 64 roms must have .n64, .v64 or .z64 extension ...");
                        }
                    break;
                    case GameConsole.SMD:
                        consoleCode = "6";
                    break;
                    case GameConsole.PCE:
                        consoleCode = "3";
                    break;
                    case GameConsole.NGAES:
                        devilken = true;
                        devilkencopydir = true;
                    break;
                    default:
                        throw new System.NotImplementedException("The selected console is not supported on InjectNewRom step ...");
                }
                com.reportProgress("Copying rom ...", toReturn.milestoneList[0]);
                if (devilken){
                    await com.showFrontendMessage(
                        "The title id that you will have to type on the terminal will not be used, " +
                        "it will use your choice on the main interface instead", "Alert", RecipeButtonsType.ok
                    );
                    Helpers.ClearDirectory(Path.Combine(env.DevilkenInjectorPath, "ROMS"));
                    if (devilkencopydir){
                        await Helpers.CopyAllFilesOnDirectory(
                            romFilePath, Path.Combine(env.DevilkenInjectorPath, "ROMS"), true
                        );
                    }else{
                        await Helpers.CopyFileAsync(romFilePath, Path.Combine(new string[]{
                            env.DevilkenInjectorPath, "ROMS", romName
                        }));
                    }
                    
                }else{
                    try { File.Delete(Path.Combine(env.AutoinjectwadPath, romName)); } catch { }
                    await Helpers.CopyFileAsync(romFilePath, Path.Combine(env.AutoinjectwadPath, romName));
                }
                
                com.reportProgress("Copying base wad ...", toReturn.milestoneList[1]);
                if (devilken){
                    await Helpers.CopyFileAsync(env.WorkingWad, Path.Combine(env.DevilkenInjectorPath, "ww.wad"));
                    File.Delete(env.WorkingWad);
                    env.WorkingWad = Path.Combine(env.DevilkenInjectorPath, "ww.wad");
                    com.reportProgress("Injecting rom ...", toReturn.milestoneList[2]);
                    await Helpers.ExecuteExternalProcess(
                        Path.Combine(env.DevilkenInjectorPath, "VC.exe"),
                        env.DevilkenInjectorPath,
                        new string[]{
                            "ww.wad",
                            "ROMS"
                        }
                    );

                    File.Move(
                        Path.Combine(env.DevilkenInjectorPath, "title.wad"),
                        Path.Combine(env.AutoinjectwadPath, "VC-newinjection in ww.wad")
                    );
                }else{
                    await Helpers.CopyFileAsync(env.WorkingWad, Path.Combine(env.AutoinjectwadPath, "ww.wad"));
                    File.Delete(env.WorkingWad);
                    env.WorkingWad = Path.Combine(env.AutoinjectwadPath, "ww.wad");
                    com.reportProgress("Injecting rom ...", toReturn.milestoneList[2]);
                    await Helpers.ExecuteExternalProcess(
                        Path.Combine(env.AutoinjectwadPath, "injectuwad.exe"),
                        env.AutoinjectwadPath,
                        new string[]{
                            romName,
                            "ww.wad",
                            "newinjection",
                            consoleCode,
                            "n"
                        }
                    );
                    File.Delete(env.WorkingWad);
                }
                env.WorkingWad = Path.Combine(env.AutoinjectwadPath, "VC-newinjection in ww.wad");
            };
            
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.RemoveAllFilesWithAnSpecificExtensionFromDirectory(env.AutoinjectwadPath, ".wad", new string[]{
                    env.WorkingWad
                });
                Helpers.RemoveAllDirectoriesFromDirectory(env.AutoinjectwadPath);
                await toReturn.processCleanup(env, com);
            };

            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                try { File.Delete(env.WorkingWad); } catch { };
                await toReturn.preEverythingCleanup(env, com);
            };

            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.RemoveAllFilesWithAnSpecificExtensionFromDirectory(env.AutoinjectwadPath, ".app");
                string[] allFiles = Directory.GetFiles(env.AutoinjectwadPath);
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
                Helpers.RemoveAllDirectoriesFromDirectory(env.DevilkenInjectorPath);
                Helpers.RemoveAllFilesWithAnSpecificExtensionFromDirectory(env.DevilkenInjectorPath, ".wad", new string[]{
                    env.WorkingWad
                });
                Directory.CreateDirectory(Path.Combine(env.DevilkenInjectorPath, "ROMS"));
            };
            return toReturn;
        }
    }
}
