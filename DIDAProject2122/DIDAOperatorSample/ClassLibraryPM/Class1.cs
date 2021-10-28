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

        Dictionary<string, Process> list_of_processes = new Dictionary<string, Process>();

        bool readingFile = false;
        int currentLine = 0;
        //aux vars para ler ficheiro, quer completo quer linha a linha
        string path;
        string[] lines;
        string[] words;

        //contem as configuracoes lidas ate ao momento
        string ConfigReaded = "Readed Configs\r\n";

        //le um ficheiro
        public void readFile(string file_name) {
            path = Path.Combine(Environment.CurrentDirectory, @"files\", file_name);
            lines = System.IO.File.ReadAllLines(path);
        }

        // handle de um linha do ficheiro de configs
        public void HandleNextLine(string line) {
            
            // infos para inicio de processos
            ProcessStartInfo startInfo;
            string[] args;

            Process aux_To_add_list;

            string[] words = line.Split(' ');
            string word = words[0];

            switch (word) {
                case "scheduler":
                    schedulerURL = words[1];

                    startInfo = new ProcessStartInfo("Scheduler.exe"); //set do .exe do processo

                    //args a passar
                    //adicionar aqui informacoes de storage, workers, o necessario
                    args = new string[] { schedulerURL }; // definir os argumentos
                    startInfo.Arguments = string.Join(" ", args); //passa como string mas vê como array no processo iniciado

                    Process.Start(startInfo);

                    ConfigReaded = ConfigReaded + "schedulerURL = " + schedulerURL + "\r\n";

                    break;
                case "worker":
                    workerStruct auxWorker = new workerStruct();
                    auxWorker.name = words[1];
                    auxWorker.url = words[2];
                    auxWorker.gossipDelay = words[3];

                    WorkersList.Add(auxWorker);

                    startInfo = new ProcessStartInfo("Worker.exe"); //set do .exe do processo

                    //args a passar, adicionar o necessario 
                    args = new string[] { debugModeString, auxWorker.name, auxWorker.url, auxWorker.gossipDelay };
                    startInfo.Arguments = string.Join(" ", args);

                    aux_To_add_list = Process.Start(startInfo);
                    list_of_processes.Add(auxWorker.name, aux_To_add_list);

                    ConfigReaded = ConfigReaded + "worker node = " + auxWorker.name + "--" + auxWorker.url + "--" + auxWorker.gossipDelay + "\r\n";

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

                    ConfigReaded = ConfigReaded + "storage node = " + auxStorage.name + "--" + auxStorage.url + "--" + auxStorage.gossipDelay + "\r\n";

                    break;
                case "populate":
                    populate_file = words[1];

                    ConfigReaded = ConfigReaded + "populate file: " + populate_file + "\r\n";

                    break;
                case "client":
                    client.app_file = words[2];
                    client.input = words[1];

                    ConfigReaded = ConfigReaded + "client_input = " + client.input + "--client_app_file = " + client.app_file + "\r\n";

                    break;
                case "debug":
                    debugMode = true;
                    debugModeString = "debug";

                    ConfigReaded = ConfigReaded + "Debug Mode On \r\n";

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

                    ConfigReaded = ConfigReaded + "Crash server " + to_crash + "\r\n";

                    break;
                case "wait":

                    int wait_interval = Int32.Parse(words[1]);

                    System.Threading.Thread.Sleep(wait_interval);

                    ConfigReaded = ConfigReaded + "Wait " + words[1] + " milliseconds\r\n";
                    break;
            }
        }


        //ler o ficheiro todo de uma vez
        public void readConfigFile(string file_name) {
            if (!readingFile) {
                readFile(file_name);
                readingFile = true;
            }

            for (int i = currentLine; i < lines.Length; i++) {

                HandleNextLine(lines[i]);
            }
        }

        public bool readNextLine(string file_name) {

            bool end_of_file = false;
            if (!readingFile) {
                readFile(file_name);
                readingFile = true;
            }


            string line_to_process = lines[currentLine];
            currentLine++;
            if (currentLine == lines.Length) {
                end_of_file = true;
            }

            HandleNextLine(line_to_process);

            return end_of_file;
        }

        public string listarConfig() {

            return ConfigReaded;
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
