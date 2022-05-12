using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virtual_Console_Numbify_fw.InjectionModels {

    internal class VirtualConsoleInjectionStep
    {
        public delegate Task Process(InjectionEnviorunment env, MainWindowComunicator com);
        public delegate Task ErrorCleanup(InjectionEnviorunment env, MainWindowComunicator com);
        public delegate Task ProcessCleanup(InjectionEnviorunment env, MainWindowComunicator com);
        public delegate Task PreEverythingCleanup(InjectionEnviorunment env, MainWindowComunicator com);

        //PreEverythingCleanup
        //PostEverythingCleanup

        public Process process;
        public ErrorCleanup errorCleanup;
        public ProcessCleanup processCleanup;
        public PreEverythingCleanup preEverythingCleanup;
        public object[] milestoneList;

        public string pauseFinishedMessage = "";
        public string pauseStartMessage = "";
    }
}
