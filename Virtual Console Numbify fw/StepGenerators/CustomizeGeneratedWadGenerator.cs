using libWiiSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Virtual_Console_Numbify_fw.InjectionModels;
using Virtual_Console_Numbify_fw.ViewModels;

namespace Virtual_Console_Numbify_fw.StepGenerators
{
    internal class CustomizeGeneratedWadGenerator{
        public static VirtualConsoleInjectionStep Generate(
            string wadTitleName,
            string wadTitleId,
            bool useCompression,

            string bannerImageFilePath,
            string iconFilePath,
            string bannerGameTitle,
            int bannerYear,
            int bannerMaximumPlayerCount
        ){
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            toReturn.pauseStartMessage = "Will start to customize wad";
            toReturn.pauseFinishedMessage = "Finished wad customization";
            toReturn.milestoneList = new object[]{
                new object(),
                new object(),
                new object(),
                new object(),
                new object(),
                new object(),
                new object(),
                new object()
            };
            toReturn.process = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                WAD w = new WAD();
                com.reportProgress("Loading wad with injected rom ...", toReturn.milestoneList[0]);
                w.LoadFile(env.WorkingWad);

                com.reportProgress("Replacing banner image ...", toReturn.milestoneList[1]);
                Helpers.Replace_tpl_image(w, "banner.bin", "vcpic.tpl", bannerImageFilePath);

                com.reportProgress("Replacing icon ...", toReturn.milestoneList[2]);
                Helpers.Replace_tpl_image(w, "icon.bin", "iconvcpic.tpl", iconFilePath);

                com.reportProgress("Extracting banner.brlyt ...", toReturn.milestoneList[3]);
                Helpers.ExtractBannerBrlyt(w, Path.Combine(env.AutoinjectwadPath, "banner.brlyt"));

                await Helpers.CopyFileAsync(
                    Path.Combine(env.AutoinjectwadPath, "banner.brlyt"),
                    Path.Combine(env.VCbrlytPath, "banner.brlyt")
                );

                com.reportProgress("Injecting info on banner ...", toReturn.milestoneList[4]);
                await Helpers.ExecuteExternalProcess(
                    Path.Combine(env.VCbrlytPath, "vcbrlyt.exe"),
                    env.VCbrlytPath,
                    new string[]{
                        "banner.brlyt",
                        "-Title", bannerGameTitle,
                        "-YEAR", bannerYear.ToString(),
                        "-Play", bannerMaximumPlayerCount.ToString(),
                        "-H_T_ESRB_ENG", " ",
                        "-H_T_ESRB_FRA", " ",
                        "-H_T_ESRB_SPA", " ",
                        "-H_T_ESRB_GER", " ",
                        "-H_T_ESRB_ITA", " ",
                        "-H_T_ESRB_NED", " ",
                        "-H_T_ESRB_JPN", " ",
                        "-H_T_ESRB_NONE", " "
                    }
                );

                com.reportProgress("Replacing banner ...", toReturn.milestoneList[5]);
                Helpers.ReplaceBannerBrlyt(w, Path.Combine(env.VCbrlytPath, "banner.brlyt"));
                w.ChangeChannelTitles(new string[]{
                    wadTitleName,wadTitleName,wadTitleName,wadTitleName,
                    wadTitleName,wadTitleName,wadTitleName,wadTitleName
                });

                com.reportProgress("Changing wad info ...", toReturn.milestoneList[6]);
                if (wadTitleId.Trim() != ""){
                    w.ChangeTitleID(LowerTitleID.Channel, wadTitleId.Trim());
                }else if(env.gameConsole == GameConsole.SMS || env.gameConsole == GameConsole.N64 || env.gameConsole == GameConsole.NGAES){
                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    string newId = "L";
                    if (env.gameConsole == GameConsole.N64) newId = "N";
                    if (env.gameConsole == GameConsole.NGAES) newId = "E";
                    var random = new System.Random();
                    for (int i = 0; i < 3; i++){
                        newId += chars[random.Next(chars.Length)];
                    }
                    w.ChangeTitleID(LowerTitleID.Channel, newId);
                }
                w.Lz77CompressBannerAndIcon = useCompression;
                w.Lz77DecompressBannerAndIcon = !useCompression;

                com.reportProgress("Saving wad ...", toReturn.milestoneList[7]);
                w.Save(Path.Combine(env.AutoinjectwadPath, "bannerReplaced.wad"));
                w.Dispose();
                File.Delete(env.WorkingWad);
                env.WorkingWad = Path.Combine(env.AutoinjectwadPath, "bannerReplaced.wad");
            };
            toReturn.errorCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                try { File.Delete(env.WorkingWad); } catch { };
                await toReturn.preEverythingCleanup(env, com);
            };
            toReturn.preEverythingCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) =>{
                Helpers.RemoveAllFilesWithAnSpecificExtensionFromDirectory(env.AutoinjectwadPath, ".wad", new string[]{
                    env.WorkingWad
                });
                Helpers.RemoveAllFilesWithAnSpecificExtensionFromDirectory(env.AutoinjectwadPath, ".brlyt");
                Helpers.RemoveAllFilesWithAnSpecificExtensionFromDirectory(env.VCbrlytPath, ".brlyt");
                Helpers.RemoveAllDirectoriesFromDirectory(env.AutoinjectwadPath);
            };
            toReturn.processCleanup = async (InjectionEnviorunment env, MainWindowComunicator com) => {
                File.Delete(Path.Combine(env.AutoinjectwadPath, "banner.brlyt"));
                File.Delete(Path.Combine(env.VCbrlytPath, "banner.brlyt"));
            };
            return toReturn;
        }
    }
}
