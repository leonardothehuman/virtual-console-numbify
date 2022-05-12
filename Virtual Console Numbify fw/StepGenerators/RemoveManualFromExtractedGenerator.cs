using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Virtual_Console_Numbify_fw.InjectionModels;
using Virtual_Console_Numbify_fw.ViewModels;

namespace Virtual_Console_Numbify_fw.StepGenerators{
    internal class RemoveManualFromExtractedGenerator{
        public static VirtualConsoleInjectionStep Generate(){
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            VirtualConsoleInjectionStep extractZeroFour = ExtractZeroFiveGenerator.Generate("00000004");
            VirtualConsoleInjectionStep packZeroFour = PackZeroFiveGenerator.Generate("00000004");
            toReturn.pauseStartMessage = "Will delete manual";
            toReturn.pauseFinishedMessage = "Manual deleted";
            List<object> ml = new List<object>();
            ml.Add(new object());
            ml.Add(new object());
            for (int i = 0; i < extractZeroFour.milestoneList.Length; i++) {
                ml.Add(extractZeroFour.milestoneList[i]);
            }
            for (int i = 0; i < packZeroFour.milestoneList.Length; i++) {
                ml.Add(packZeroFour.milestoneList[i]);
            }
            toReturn.milestoneList = ml.ToArray();
            toReturn.process = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                com.reportProgress("Deleting manual file ...", toReturn.milestoneList[0]);
                switch (env.gameConsole){
                    case GameConsole.NES:
                    case GameConsole.SNES:
                        File.Delete(
                            Path.Combine(new string[] { env.WorkingExtracted05, "emanual.arc" })
                        );
                    break;
                    case GameConsole.N64:
                    case GameConsole.PCE:
                    case GameConsole.NGAES:
                        File.Delete(
                            Path.Combine(new string[] { env.WorkingExtracted05, "html.arc" })
                        );
                    break;
                    case GameConsole.SMD:
                    case GameConsole.SMS:
                        File.Delete(
                            Path.Combine(new string[] { env.WorkingExtractedCcf, "man.arc.zlib" })
                        );
                    break;
                    default:
                        throw new System.NotImplementedException("The selected console is not supported yet ...");
                }
                
                com.reportProgress("Replacing 00000004.app ...", toReturn.milestoneList[1]);
                /*File.Move(
                    Path.Combine(new string[] { env.WorkingExtracted, "00000004.app" }),
                    Path.Combine(new string[] { env.WorkingExtracted, "00000004.old.app" })
                );
                await Helpers.CopyFileAsync(
                    env.ZeroFourApp, Path.Combine(new string[] { env.WorkingExtracted, "00000004.app" })
                );*/
                await extractZeroFour.process(env, com);
                Helpers.ClearDirectory(Path.Combine(env.WorkingExtracted, @"00000004_app_OUT\HomeButton3"));
                await Helpers.CopyAllFilesOnDirectory(
                    Path.Combine(env.WorkingExtracted, @"00000004_app_OUT\HomeButton2"),
                    Path.Combine(env.WorkingExtracted, @"00000004_app_OUT\HomeButton3"),
                    true
                );
                await packZeroFour.process(env, com);
            };
            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.RemoveAllDirectoriesFromDirectory(env.AutoinjectwadPath);
                await extractZeroFour.errorCleanup(env, com);
                await packZeroFour.errorCleanup(env, com);
            };
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.RemoveAllDirectoriesFromDirectory(env.AutoinjectwadPath);
                await extractZeroFour.preEverythingCleanup(env, com);
                await packZeroFour.preEverythingCleanup(env, com);
            };
            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                File.Delete(Path.Combine(new string[] { env.WorkingExtracted, "00000004.old.app" }));
                await extractZeroFour.processCleanup(env, com);
                await packZeroFour.processCleanup(env, com);
            };
            return toReturn;
        }
    }
}
