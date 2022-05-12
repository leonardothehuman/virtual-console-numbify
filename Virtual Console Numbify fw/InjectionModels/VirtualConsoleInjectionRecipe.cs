using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Virtual_Console_Numbify_fw.InjectionModels;

namespace Virtual_Console_Numbify_fw.InjectionModels {
    public enum RecipeButtonsType{
        ok = 0,
        yesno = 1
    }
    public class MainWindowComunicator{
        public delegate void ReportProgress(string progressMessage, object milestone);
        public delegate Task<bool> ShowFrontendMessage(string message, string title, RecipeButtonsType type);
        public ReportProgress reportProgress;
        public ShowFrontendMessage showFrontendMessage;
    }

    public class ProgressReportEventArgs : EventArgs{
        public string progressMessage;
        public float progressNumber;
    }
    internal class VirtualConsoleInjectionRecipe
    {
        private readonly List<VirtualConsoleInjectionStep> steps = new List<VirtualConsoleInjectionStep>();
        private readonly InjectionEnviorunment injectionEnviorunment;
        private readonly MainWindowComunicator comunicator;
        private readonly List<object> milestoneList;

        public delegate void ProgressReportEventHandler(object sender, ProgressReportEventArgs args);
        public event ProgressReportEventHandler progressReported;
        bool pause;
        public VirtualConsoleInjectionRecipe(InjectionEnviorunment env, bool _pause){
            pause = _pause;
            injectionEnviorunment = env;
            comunicator = new MainWindowComunicator();
            comunicator.reportProgress = (string progressMessage, object milestone) => {
                progressReported?.Invoke(this, new ProgressReportEventArgs(){
                    progressMessage = progressMessage,
                    progressNumber = milestoneList.IndexOf(milestone) * 100 / (milestoneList.Count - 1)
                });
            };
            comunicator.showFrontendMessage = async (string message, string title, RecipeButtonsType type) => { return false; };
            milestoneList = new List<object>();
        }
        public void SetFrontendMessageDelegate(MainWindowComunicator.ShowFrontendMessage del){
            comunicator.showFrontendMessage = del;
        }
        public void AddStep(VirtualConsoleInjectionStep toAdd)
        {
            steps.Add(toAdd);
            foreach (var item in toAdd.milestoneList){
                milestoneList.Add(item);
            }
        }
        public async Task ExecuteSteps(){
            foreach (VirtualConsoleInjectionStep step in steps){
                await step.preEverythingCleanup(injectionEnviorunment, comunicator);
            }
            for (int i = 0; i < steps.Count; i++){
                VirtualConsoleInjectionStep step = steps[i];
                try{
                    await step.process(injectionEnviorunment, comunicator);
                    if (pause == true){
                        if(i == steps.Count - 1){
                            await comunicator.showFrontendMessage(step.pauseFinishedMessage, "Pause", RecipeButtonsType.ok);
                        }else{
                            await comunicator.showFrontendMessage(step.pauseFinishedMessage + "\n" + steps[i+1].pauseStartMessage, "Pause", RecipeButtonsType.ok);
                        }                        
                    }
                    await step.processCleanup(injectionEnviorunment, comunicator);
                }
                catch(Exception ex){
                    await step.errorCleanup(injectionEnviorunment, comunicator);
                    throw ex;
                }
            }
        }
    }
}