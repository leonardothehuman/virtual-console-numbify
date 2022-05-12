using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Virtual_Console_Numbify_fw.InjectionModels;

namespace Virtual_Console_Numbify_fw.StepGenerators{
    internal class FindNeoGeoBannerBinGenerator{
        public static VirtualConsoleInjectionStep Generate(){
            VirtualConsoleInjectionStep extractZeroSix = ExtractZeroFiveGenerator.Generate("00000006");
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            toReturn.pauseStartMessage = "Will find neogeo's banner.bin";
            toReturn.pauseFinishedMessage = "banner.bin search finished";
            toReturn.milestoneList = extractZeroSix.milestoneList;
            toReturn.process = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                if (File.Exists(
                    Path.Combine(
                        env.WorkingExtracted05,
                        "banner.bin"
                    )
                )){
                    env.WorkingNeoGeoBannerContainer = env.WorkingExtracted05;
                    return;
                }
                await extractZeroSix.process(env, com);
                env.WorkingNeoGeoBannerContainer = Path.Combine(
                    new string[] { env.WorkingExtracted, @"00000006_app_OUT\" }
                );
                if (File.Exists(
                    Path.Combine(
                        env.WorkingNeoGeoBannerContainer,
                        "banner.bin"
                    )
                )){
                    return;
                }
                //await com.showFrontendMessage("Will delete 6", "6", RecipeButtonsType.ok);
                Directory.Delete(Path.Combine(new string[] { env.WorkingExtracted, @"00000006_app_OUT\" }), true);
                env.WorkingNeoGeoBannerContainer = "";
            };
            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                await extractZeroSix.errorCleanup(env, com);
            };
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                await extractZeroSix.preEverythingCleanup(env, com);
            };
            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                await extractZeroSix.processCleanup(env, com);
            };
            return toReturn;
        }
    }
}
