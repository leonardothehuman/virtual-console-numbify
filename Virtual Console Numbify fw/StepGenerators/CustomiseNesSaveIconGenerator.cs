using AutoItX3Lib;
using libWiiSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Virtual_Console_Numbify_fw.StepGenerators
{
    internal class CustomiseNesSaveIconGenerator{
        /*public static VirtualConsoleInjectionStep generate(string iconFilePath, string saveName, bool manualInject){
            VirtualConsoleInjectionStep toReturn = new VirtualConsoleInjectionStep();
            string titleId = "";
            toReturn.process = async (InjectionEnviorunment env) => {
                await Helpers.ExecuteExternalProcess(
                    Path.Combine(env.autoinjectwadPath, "wadunpacker.exe"),
                    env.autoinjectwadPath,
                    new string[]{"bannerReplaced.wad"}
                );
                WAD w = new WAD();
                w.LoadFile(Path.Combine(env.autoinjectwadPath, "bannerReplaced.wad"));
                titleId = w.TitleID.ToString("X").PadLeft(16, '0').ToLower();
                w.Dispose();

                Directory.CreateDirectory(Path.Combine(env.autoinjectwadPath, "icons"));

                await Helpers.ExecuteExternalProcess(
                    Path.Combine(env.VCiconPath, "VC_Icon_Gen.exe"),
                    env.VCiconPath,
                    new string[] {
                        "-sys", "nes",
                        "-s", "\""+iconFilePath+"\"",
                        "-d", "\""+Path.Combine(env.autoinjectwadPath, @"icons\")+"\"",
                        "-m", "s"
                    },
                    true
                );

                AutoItX3 saveIconEditor = new AutoItX3();
                saveIconEditor.AutoItSetOption("MouseCoordMode", 2);
                int pid = saveIconEditor.Run(
                    Path.Combine(env.VCsaveInjectPath, "vc_save_inject.exe"),
                    env.VCsaveInjectPath
                ); ;

                saveIconEditor.WinWaitActive("[CLASS:ThunderRT6FormDC]");
                string hwnd = saveIconEditor.WinGetHandle("[CLASS:ThunderRT6FormDC]");
                saveIconEditor.ControlCommand(
                    "[CLASS:ThunderRT6FormDC]", "",
                    "[CLASS:ThunderRT6ComboBox; INSTANCE:2]",
                    "Selectstring",
                    "NES /FC"
                );

                string tplFile = Path.Combine(Path.Combine(env.autoinjectwadPath, @"icons\banner.tpl"));
                saveIconEditor.ControlSetText(
                    "[CLASS:ThunderRT6FormDC]", "",
                    "[CLASS:ThunderRT6TextBox; INSTANCE:2]",
                    tplFile
                );

                string appFile = Path.Combine(new string[]{
                    env.autoinjectwadPath, titleId, "00000001.app"
                });

                saveIconEditor.ControlSetText(
                    "[CLASS:ThunderRT6FormDC]", "",
                    "[CLASS:ThunderRT6TextBox; INSTANCE:1]",
                    appFile
                );

                saveIconEditor.MouseClick("primary", 148, 82);
                saveIconEditor.Send("{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}{BS}");
                saveIconEditor.Send("{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}{DEL}");
                saveIconEditor.Send(saveName, 1);

                saveIconEditor.ControlClick(
                    "[CLASS:ThunderRT6FormDC]", "",
                    "[CLASS:ThunderRT6CommandButton; INSTANCE:1]",
                    "primary"
                );

                //Optionally stops here
                if (manualInject == false){
                    saveIconEditor.ControlClick(
                        "[CLASS:ThunderRT6FormDC]", "",
                        "[CLASS:ThunderRT6CommandButton; INSTANCE:5]",
                        "primary"
                    );

                    string verificationText1 = @"Tpl File:  " + tplFile + @" successfully injected into " + appFile + @".";
                    string verificationText2 = saveName + @" successflly injected into " + appFile;

                    saveIconEditor.WinWaitActive("[CLASS:#32770]");

                    string successText = saveIconEditor.WinGetText("[CLASS:#32770]");

                    string[] successTexts = successText.Split('\n');

                    saveIconEditor.ControlClick(
                        "[CLASS:#32770]", "",
                        "[CLASS:Button; INSTANCE:1]",
                        "primary"
                    );

                    saveIconEditor.WinClose("[CLASS:ThunderRT6FormDC]");

                    if (successTexts[1].Trim() == verificationText1.Trim() && successTexts[2].Trim() == verificationText2.Trim()){
                        MessageBox.Show("Injected");
                    }else{
                        throw new Exception("Failed to inject icon");
                    }
                }else{
                    MessageBox.Show("Click ok when icon is ready");
                }

                await Helpers.ExecuteExternalProcess(
                    Path.Combine(env.U8tool, "U8Tool.exe"),
                    env.U8tool,
                    new string[] {
                        "-file", "\""+Path.Combine(new string[]{env.autoinjectwadPath, titleId, "00000005.app"})+"\"",
                        "-folder", "\""+Path.Combine(new string[]{env.autoinjectwadPath, titleId, @"00000005_app_OUT\"})+"\"",
                        "-source", "\""+Path.Combine(new string[]{env.autoinjectwadPath, titleId, "00000005.app"})+"\"",
                        "-extract"
                    },
                    true
                );
                File.Delete(
                    Path.Combine(new string[] { env.autoinjectwadPath, titleId, @"00000005_app_OUT", "emanual.arc" })
                );
                File.Move(
                    Path.Combine(new string[] { env.autoinjectwadPath, titleId, "00000005.app" }),
                    Path.Combine(new string[] { env.autoinjectwadPath, titleId, "00000005.old.app" })
                );
                await Helpers.ExecuteExternalProcess(
                    Path.Combine(env.U8tool, "U8Tool.exe"),
                    env.U8tool,
                    new string[] {
                        "-file", "\""+Path.Combine(new string[]{env.autoinjectwadPath, titleId, "00000005.app"})+"\"",
                        "-folder", "\""+Path.Combine(new string[]{env.autoinjectwadPath, titleId, @"00000005_app_OUT\"})+"\"",
                        "-source", "\""+Path.Combine(new string[]{env.autoinjectwadPath, titleId, "00000005.old.app"})+"\"",
                        "-pack"
                    },
                    true
                );
                File.Move(
                    Path.Combine(new string[] { env.autoinjectwadPath, titleId, "00000004.app" }),
                    Path.Combine(new string[] { env.autoinjectwadPath, titleId, "00000004.old.app" })
                );
                await Helpers.CopyFileAsync(
                    env.zeroFourApp, Path.Combine(new string[] { env.autoinjectwadPath, titleId, "00000004.app" })
                );

                await Helpers.CopyFileAsync(
                    Path.Combine(env.autoinjectwadPath, "common-key.bin"),
                    Path.Combine(new string[]{
                        env.autoinjectwadPath, titleId, "common-key.bin"
                    })
                );

                try
                {
                    await Helpers.ExecuteExternalProcess(
                        Path.Combine(env.autoinjectwadPath, "wadpacker.exe"),
                        Path.Combine(env.autoinjectwadPath, titleId),
                        new string[] {
                            titleId+".tik",
                            titleId+".tmd",
                            titleId+".cert",
                            "out.wad",
                            "-sign"
                        }, false, 10
                    );
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                

                await Helpers.CopyFileAsync(
                    Path.Combine(new string[]{
                        env.autoinjectwadPath, titleId, "out.wad"
                    }),
                    Path.Combine(new string[]{
                        env.autoinjectwadPath, "Injected.wad"
                    })
                );

            };
            toReturn.errorCleanup = async (InjectionEnviorunment env) => {
            };
            toReturn.processCleanup = async (InjectionEnviorunment env) => {
                File.Delete(Path.Combine(env.autoinjectwadPath, "bannerReplaced.wad"));
                Directory.Delete(Path.Combine(env.autoinjectwadPath, "icons"), true);
                if(titleId != "") Directory.Delete(Path.Combine(env.autoinjectwadPath, titleId), true);
            };
            return toReturn;
    }*/
    }
}
