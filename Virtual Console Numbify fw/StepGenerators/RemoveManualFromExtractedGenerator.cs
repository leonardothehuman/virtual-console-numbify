using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virtual_Console_Numbify_fw.StepGenerators
{
    internal class RemoveManualFromExtractedGenerator
    {
        public static VirtualConsoleInjectionStep generate()
        {
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            toReturn.pauseStartMessage = "Will delete manual";
            toReturn.pauseFinishedMessage = "Manual deleted";
            toReturn.milestoneList = new object[]{
                new object(),
                new object()
            };
            toReturn.process = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                com.reportProgress("Deleting manual file ...", toReturn.milestoneList[0]);
                switch (env.console)
                {
                    case Console.NES:
                    case Console.SNES:
                        File.Delete(
                            Path.Combine(new string[] { env.workingExtracted05, "emanual.arc" })
                        );
                    break;
                    case Console.N64:
                    case Console.PCE:
                    case Console.NGAES:
                        File.Delete(
                            Path.Combine(new string[] { env.workingExtracted05, "html.arc" })
                        );
                    break;
                    case Console.SMD:
                    case Console.SMS:
                        File.Delete(
                            Path.Combine(new string[] { env.workingExtractedCcf, "man.arc.zlib" })
                        );
                    break;
                    default:
                        throw new NotImplementedException("The selected console is not supported yet ...");
                }
                
                com.reportProgress("Replacing 00000004.app ...", toReturn.milestoneList[1]);
                File.Move(
                    Path.Combine(new string[] { env.workingExtracted, "00000004.app" }),
                    Path.Combine(new string[] { env.workingExtracted, "00000004.old.app" })
                );
                await Helpers.CopyFileAsync(
                    env.zeroFourApp, Path.Combine(new string[] { env.workingExtracted, "00000004.app" })
                );
            };
            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.removeAllDirectoriesFromDirectory(env.autoinjectwadPath);
            };
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.removeAllDirectoriesFromDirectory(env.autoinjectwadPath);
            };
            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                File.Delete(Path.Combine(new string[] { env.workingExtracted, "00000004.old.app" }));
            };
            return toReturn;
        }
    }
}
