using libWiiSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Virtual_Console_Numbify_fw{
    internal class Helpers{

        public static string RemoveProtocolFromBase(string p) {
            string[] paths = p.Split(':');
            return paths[1][paths[1].Length - 1] + ":" + paths[2];
        }

        public static string RemoveFromBeginning(string s, char c){
            string toReturn = "";
            bool allRemoved = false;
            for (int i = 0; i < s.Length; i++){
                if (s[i] == c && allRemoved == false) continue;
                allRemoved = true;
                toReturn += s[i];
            }
            return toReturn;
        }
        public static string ForceWadId(string s){
            string toReturn = s.ToUpper();
            toReturn = ForceChars(toReturn, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
            toReturn = TruncateString(toReturn, 4);
            return toReturn;
        }

        public static string ForceChars(string s, string c){
            var chars = c.ToCharArray();
            string toReturn = "";
            for (int i = 0; i < s.Length; i++){
                if (!Array.Exists(chars, e => e == s[i])) continue;
                toReturn += s[i];
            }
            return toReturn;
        }

        public static string UniqueChars(string s) {
            Dictionary<char, char> charList= new Dictionary<char, char>();
            for (int i = 0; i < s.Length; i++) {
                charList[s[i]] = s[i];
            }
            string toReturn = "";
            foreach (char c in charList.Keys) {
                toReturn += c;
            }
            return toReturn;
        }

        public static string IsOrAre(int n) {
            if (n == 1) return "is";
            return "are";
        }

        public static string SplitCharsIntoHumanReadbleList(string s) {
            char[] l = s.ToCharArray();
            string toReturn = "";
            for (int i = 0; i < l.Length; i++) {
                if (toReturn.Length == 0)
                    toReturn += l[i];
                else if (i != l.Length - 1)
                    toReturn += ", " + l[i];
                else
                    toReturn += " and " + l[i];
            }
            return toReturn;
        }
        public static string AllowOnlyOneCircunflexDiacritic(string s){
            string toReturn = "";
            bool diacriticInserted = false;
            for (int i = 0; i < s.Length; i++){
                if (s[i] == '^' && diacriticInserted == true) continue;
                if (s[i] == '^') diacriticInserted = true;
                toReturn += s[i];
            }
            return toReturn;
        }
        public static string BlockCircunflexDiacritic(string s) {
            string toReturn = "";
            for (int i = 0; i < s.Length; i++) {
                if (s[i] == '^') continue;
                toReturn += s[i];
            }
            return toReturn;
        }
        public static string TruncateString(string s, int c){
            if (s.Length < c) c = s.Length;
            return s.Substring(0, c);
        }
        public static bool IsAValidWinPath(string p){
            if(Path.IsPathRooted(p) == false) return false;
            try{
                Path.GetFullPath(p);
            }catch{
                return false;
            }
            try{
                if (p.ToCharArray()[1] != ':') return false;
                if (p.ToCharArray()[2] != '\\') return false;
                if (p.Length < 4) return false;
            }catch{
                return false;
            }
            return true;
        }
        public static async Task CopyFileAsync(string sourceFile, string destinationFile){
            using (var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan)) {
                using (var destinationStream = new FileStream(destinationFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan)) {
                    await sourceStream.CopyToAsync(destinationStream);
                    await sourceStream.FlushAsync();
                    await destinationStream.FlushAsync();
                }
            }
        }
        public static string JoinArguments(string[] arguments, bool noQuote){
            string toReturn = "";
            bool firstRun = true;
            foreach (string arg in arguments){
                if (firstRun == false) toReturn += " ";
                firstRun = false;
                if (arg.IndexOf(' ') >= 0 && noQuote == false){
                    toReturn += "\"" + arg + "\"";
                }else{
                    toReturn += arg;
                }
            }
            return toReturn;
        }
        public static Task ExecuteExternalProcess(string process, string workingDir, string[] arguments, bool noQuote = false, int exitCode = 0){
            var toReturn = new TaskCompletionSource<int>();
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = process;
            pProcess.StartInfo.Arguments = JoinArguments(arguments, noQuote); //argument
            pProcess.StartInfo.WorkingDirectory = workingDir;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.EnableRaisingEvents = true;
            pProcess.Exited += (object sender, EventArgs args) =>{
                if(pProcess.ExitCode == exitCode){
                    toReturn.SetResult(pProcess.ExitCode);
                }else{
                    toReturn.SetException(
                        new Exception(
                            "Error: external process exited with non zero code -> " + pProcess.ExitCode.ToString()
                        )
                    );
                }
                pProcess.Dispose();
            };
            //pProcess.StartInfo.RedirectStandardOutput = true;
            //pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
            pProcess.Start();
            //string output = pProcess.StandardOutput.ReadToEnd(); //The output result
            return toReturn.Task;
        }
        public static void ExtractBannerBrlyt(WAD wadFile, string destinationPath){
            string fileNameToExtract = "banner.brlyt";

            U8 banner = new U8();
            if (!wadFile.HasBanner)
                throw new Exception("The base WAD is not a Channel WADs!");

            bool bannerFound = false;
            for (int i = 0; i < wadFile.BannerApp.NumOfNodes; i++){
                if (wadFile.BannerApp.StringTable[i].ToLower() == "banner.bin"){
                    banner.LoadFile(wadFile.BannerApp.Data[i]);
                    bannerFound = true;
                }
            }

            if(bannerFound == false){
                throw new Exception("Banner.bin not found on WAD file");
            }
            
            File.WriteAllBytes(destinationPath, banner.Data[banner.GetNodeIndex(fileNameToExtract)]);
        }

        public static void ReplaceBannerBrlyt(WAD wadFile, string replacementPath){
            string fileNameToReplace = "banner.brlyt";

            U8 banner = new U8();
            if (!wadFile.HasBanner)
                throw new Exception("The base WAD is not a Channel WADs!");

            bool bannerFound = false;
            for (int i = 0; i < wadFile.BannerApp.NumOfNodes; i++){
                if (wadFile.BannerApp.StringTable[i].ToLower() == "banner.bin"){
                    banner.LoadFile(wadFile.BannerApp.Data[i]);
                    bannerFound = true;
                }
            }
            if (bannerFound == false){
                throw new Exception("Banner.bin not found on WAD file");
            }

            //@"C:\Users\Leonardo\Desktop\HowardC_Tools\VCbrlyt9.0\banner.brlyt"
            banner.ReplaceFile(
                banner.GetNodeIndex(fileNameToReplace),
                File.ReadAllBytes(replacementPath)
            );

            for (int i = 0; i < wadFile.BannerApp.NumOfNodes; i++)
                if (wadFile.BannerApp.StringTable[i].ToLower() == "banner.bin")
                { wadFile.BannerApp.ReplaceFile(i, banner.ToByteArray()); break; }
            
        }
        public static Image ResizeImage(Image img, int x, int y){
            Image newimage = new Bitmap(x, y);
            using (Graphics gfx = Graphics.FromImage(newimage)){
                gfx.DrawImage(img, 0, 0, x, y);
            }
            return newimage;
        }
        public static void Replace_tpl_image(WAD wadFile, string binContainerName, string tplFileName, string newImagePath){
            U8 binContainer = new U8();
            if (!wadFile.HasBanner)
                throw new Exception("The base WAD is not a Channel WADs!");

            bool bannerFound = false;
            for (int i = 0; i < wadFile.BannerApp.NumOfNodes; i++){
                if (wadFile.BannerApp.StringTable[i].ToLower() == binContainerName){
                    binContainer.LoadFile(wadFile.BannerApp.Data[i]);
                    bannerFound = true;
                }
            }
            if (bannerFound == false){
                throw new Exception(binContainerName + " not found on WAD file");
            }

            TPL_TextureFormat bannerFormat = TPL_TextureFormat.CI8;
            TPL_PaletteFormat pFormat = TPL_PaletteFormat.RGB5A3;
            string casedTplFileName = "";
            for (int i = 0; i < binContainer.NumOfNodes; i++){
                if (binContainer.StringTable[i].ToLower().EndsWith(".tpl")){
                    if (binContainer.StringTable[i].ToLower() == tplFileName){
                        casedTplFileName = binContainer.StringTable[i];
                    }
                }
            }
            if (casedTplFileName.Trim() == ""){
                throw new Exception(tplFileName + " not found on " + binContainerName);
            }

            TPL tplFile;
            Image img;

            tplFile = TPL.Load(binContainer.Data[binContainer.GetNodeIndex(casedTplFileName)]);
            bannerFormat = tplFile.GetTextureFormat(0);
            pFormat = tplFile.GetPaletteFormat(0);

            img = Image.FromFile(newImagePath);

            if (img.Width != tplFile.GetTextureSize(0).Width || img.Height != tplFile.GetTextureSize(0).Height)
                img = ResizeImage(img, tplFile.GetTextureSize(0).Width, tplFile.GetTextureSize(0).Height);

            tplFile.RemoveTexture(0);
            tplFile.AddTexture(img, bannerFormat, pFormat);

            binContainer.ReplaceFile(binContainer.GetNodeIndex(casedTplFileName), tplFile.ToByteArray());

            binContainer.AddHeaderImd5();
            binContainer.Lz77Compress = true;

            for (int i = 0; i < wadFile.BannerApp.NumOfNodes; i++)
                if (wadFile.BannerApp.StringTable[i].ToLower() == binContainerName)
                { wadFile.BannerApp.ReplaceFile(i, binContainer.ToByteArray()); break; }
        }

        public static void clearDirectory(string dir){
            RemoveAllDirectoriesFromDirectory(dir);
            string[] allFiles = Directory.GetFiles(dir);
            foreach (string file in allFiles){
                File.Delete(file);
            }
        }

        public static void RemoveAllFilesWithAnSpecificExtensionFromDirectory(string dir, string ext, string[] _exception = null){
            string[] allFiles = Directory.GetFiles(dir);
            if(_exception == null){
                _exception = new string[] { };
            }
            foreach (string file in allFiles){
                if (Path.GetExtension(file).ToLower() == ext && Array.IndexOf(_exception, file.Trim()) < 0){
                    File.Delete(file);
                }
            }
        }

        public static bool CheckIfDirectoryHasAtLeastOneFileWithAnSpecifiedExtension(string dir, string ext) {
            string[] allFiles = Directory.GetFiles(dir);
            foreach (string file in allFiles) {
                if (Path.GetExtension(file).ToLower() == ext) {
                    return true;
                }
            }
            return false;
        }

        public static async Task CopyAllFilesOnDirectory(string src, string dest, bool sub){
            string[] allFiles = Directory.GetFiles(src);
            foreach (string file in allFiles){
                await CopyFileAsync(file, Path.Combine(dest, Path.GetFileName(file)));
            }
            if (sub == false) return;
            string[] dirr = Directory.GetDirectories(src);
            foreach (string d in dirr){
                Directory.CreateDirectory(Path.Combine(dest, Path.GetFileName(d)));
                await CopyAllFilesOnDirectory(d, Path.Combine(dest, Path.GetFileName(d)), true);
            }
        }

        public static void RemoveAllDirectoriesFromDirectory(string dir) {
            string[] dirr = Directory.GetDirectories(dir);
            foreach (string d in dirr){
                Directory.Delete(d, true);
            }
        }
        public static void NotmalizeLineEnd(string file){
            string[] lines = File.ReadAllLines(file);
            List<string> list_of_string = new List<string>();
            foreach (string line in lines){
                list_of_string.Add(line.Trim());
            }
            File.WriteAllLines(file, list_of_string);
        }
        public static string GetExeDirectory() {
            string exePath = Helpers.RemoveProtocolFromBase(Assembly.GetExecutingAssembly().GetName().CodeBase);
            return Path.GetDirectoryName(exePath);
        }
    }
}
