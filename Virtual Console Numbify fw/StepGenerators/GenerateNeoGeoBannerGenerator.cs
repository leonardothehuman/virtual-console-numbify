using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virtual_Console_Numbify_fw.StepGenerators{
    internal class GenerateNeoGeoBannerGenerator{
        public static VirtualConsoleInjectionStep generate(string iconFilePath, string title){
            FileStream destinationStream = null;
            FileStream originBanner = null;
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            toReturn.pauseStartMessage = "Will inject save banner";
            toReturn.pauseFinishedMessage = "Save banner injection finnished";
            toReturn.milestoneList = new object[]{
                new object(),
                new object()
            };
            toReturn.process = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                com.reportProgress("Generating icon ...", toReturn.milestoneList[0]);
                Directory.CreateDirectory(Path.Combine(env.autoinjectwadPath, "icons"));
                if (env.workingNeoGeoBannerContainer == ""){
                    await com.showFrontendMessage(
                        "The base wad file don't have a banner.bin, it's not possible to inject a save icon",
                        "Warning", RecipeButtonsType.ok
                    );
                    return;
                }
                
                await Helpers.ExecuteExternalProcess(
                    Path.Combine(env.VCiconPath, "VC_Icon_Gen.exe"),
                    env.VCiconPath,
                    new string[] {
                        "-sys", "neogeo",
                        "-s", "\""+iconFilePath+"\"",
                        "-d", "\""+Path.Combine(env.autoinjectwadPath, @"icons\")+"\"",
                        "-m", "s"
                    },
                    true
                );
                string destinationFile = Path.Combine(env.workingNeoGeoBannerContainer, "banner.bin");
                File.Delete(destinationFile);

                com.reportProgress("Injecting banner ...", toReturn.milestoneList[1]);
                byte[] bannerBeginning = new byte[10] { 0x57, 0x49, 0x42, 0x4E, 0x00, 0x00, 0x00, 0x10, 0xFF, 0xFF };
                destinationStream = new FileStream(destinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
                await destinationStream.WriteAsync(bannerBeginning, 0, bannerBeginning.Length);
                for(int i = 0; i < 23; i++){
                    destinationStream.WriteByte(0x00);
                }
                byte[] stringToInsert = Encoding.Unicode.GetBytes(title);
                await destinationStream.WriteAsync(stringToInsert, 0, stringToInsert.Length);
                destinationStream.WriteByte(0x0D);
                int insertedBytes = bannerBeginning.Length + 23 + stringToInsert.Length + 1;
                for (int i = 0; i < 160 - insertedBytes; i++){
                    destinationStream.WriteByte(0x00);
                }

                originBanner = new FileStream(
                    Path.Combine(new string[] { env.autoinjectwadPath, @"icons", "banner.tpl" }),
                    FileMode.Open, FileAccess.Read, FileShare.Read,
                    4096, FileOptions.Asynchronous | FileOptions.SequentialScan
                );
                originBanner.Seek(0x1A0, SeekOrigin.Begin);
                await originBanner.CopyToAsync(destinationStream);
                await destinationStream.FlushAsync();
                destinationStream.Dispose();
                destinationStream = null;
                originBanner.Dispose();
                originBanner = null;

            };
            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                if(destinationStream != null){
                    destinationStream.Dispose();
                }
                if (originBanner != null){
                    originBanner.Dispose();
                }
                Helpers.removeAllDirectoriesFromDirectory(env.autoinjectwadPath);
            };
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Helpers.removeAllDirectoriesFromDirectory(env.autoinjectwadPath);
            };
            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                Directory.Delete(Path.Combine(env.autoinjectwadPath, @"icons"), true);
            };
            return toReturn;
        }
    }
}
