using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Scheduler;

namespace ClassLibraryPM {
    public class PM_logic {
       
        //private ou public???
        private string schedulerURL;
        List<workerStruct> WorkersList = new List<workerStruct>();
        List<StorageStruct> StorageList = new List<StorageStruct>();
        string populate_file;
        bool debugMode = false;
        string debugModeString = "noDebug";
        ClientStruct client = new ClientStruct();

        /* le o ficheiro de config
        ler nos de storage, workers, etc
        ordem no ficheiro:
            0 - debug
            1 - storage
            2 - workers
            3 - scheduler
            4 - o resto 
        */
        public void readConfigFile(string file_name) {
            
            string path = Path.Combine(Environment.CurrentDirectory, @"files\", file_name);
            string[] lines = System.IO.File.ReadAllLines(path);
            string[] words;
            // infos para inicio de processos
            ProcessStartInfo startInfo;
            string[] args;

            Dictionary<string, Process> list_of_processes = new Dictionary<string, Process>();
            Process aux_To_add_list; 

            foreach (string line in lines) {

                words = line.Split(' ');
                string word = words[0];

                switch (word) {
                    case "scheduler":
                        schedulerURL = words[1];

                        startInfo = new ProcessStartInfo("Scheduler.exe"); //set do .exe do processo

                        //args a passar
                        //adicionar aqui informacoes de storage, workers, o necessario
                        args = new string[] { schedulerURL}; // definir os argumentos
                        startInfo.Arguments = string.Join(" ", args); //passa como string mas vê como array no processo iniciado

                        Process.Start(startInfo);

                        break;
                    case "worker":
                        workerStruct auxWorker = new workerStruct();
                        auxWorker.name = words[1];
                        auxWorker.url = words[2];
                        auxWorker.gossipDelay = words[3];

                        WorkersList.Add(auxWorker);

                        startInfo = new ProcessStartInfo("Worker.exe"); //set do .exe do processo

                        //args a passar, adicionar o necessario 
                        args = new string[] { debugModeString, auxWorker.name, auxWorker.url, auxWorker.gossipDelay}; 
                        startInfo.Arguments = string.Join(" ", args);

                        aux_To_add_list = Process.Start(startInfo);
                        list_of_processes.Add(auxWorker.name, aux_To_add_list);
                        break;
                    case "storage":
                        StorageStruct auxStorage = new StorageStruct();
                        auxStorage.name = words[1];
                        auxStorage.url = words[2];
                        auxStorage.gossipDelay = words[3];

                        StorageList.Add(auxStorage);

                        startInfo = new ProcessStartInfo("Storage.exe"); //set do .exe do processo
                        //args a passar, adicionar o necessario 
                        args = new string[] { auxStorage.name, auxStorage.url, auxStorage.gossipDelay };
                        startInfo.Arguments = string.Join(" ", args);

                        aux_To_add_list = Process.Start(startInfo);
                        list_of_processes.Add(auxStorage.name, aux_To_add_list);

                        break;
                    case "populate":
                        populate_file = words[1];
                        break;
                    case "client":
                        client.app_file = words[2];
                        client.input = words[1]; 
                        break;
                    case "debug":
                        debugMode = true;
                        debugModeString = "debug";
                        break;
                    case "status":

                        break;
                    case "listServer":

                        break;
                    case "listGlobal":

                        break;
                    case "crash":

                        string to_crash = words[1];

                        Process to_kill = list_of_processes[to_crash];
                        to_kill.Kill();

                        list_of_processes.Remove(to_crash);

                        break;
                    case "wait":

                        int wait_interval = Int32.Parse(words[1]);

                        System.Threading.Thread.Sleep(wait_interval);
                        break;
                }     
            }
        }

        public string listarConfig() {

            string s = "Readed Configs\r\nschedulerURL = " + schedulerURL + "\r\n";
            //add storage nodes
            foreach (StorageStruct aux in StorageList) {
                s = s + "storage node = " + aux.name + "--" + aux.url + "--" + aux.gossipDelay + "\r\n";
            }
            //add worker nodes
            foreach (workerStruct aux in WorkersList) {
                s = s + "worker node = " + aux.name + "--" + aux.url + "--" + aux.gossipDelay + "\r\n";
            }

            s = s + "populate file: " + populate_file + "\r\n";

            s = s + "client_input = " + client.input + "--client_app_file = " + client.app_file + "\r\n";

            return s;
        }
    }

    //struct de worker
    struct workerStruct {
        public string name, url, gossipDelay; 
    }

    //struct de storage
    struct StorageStruct {
        public string name, url, gossipDelay;
    }

    //struct de client, é apenas uma linha portanto nao existem varias instancias
    //provavelmente duas varias seria suficiente
    struct ClientStruct {
        public string input, app_file;
    }
}
