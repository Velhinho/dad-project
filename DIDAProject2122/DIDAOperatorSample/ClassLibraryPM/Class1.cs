using System;
using System.Collections.Generic;
using System.IO;

namespace ClassLibraryPM {
    public class PM_logic {
       
        //private ou public???
        private string schedulerURL;
        List<workerStruct> WorkersList = new List<workerStruct>();
        List<StorageStruct> StorageList = new List<StorageStruct>();
        string populate_file;
        bool debugMode = false;
        ClientStruct client = new ClientStruct();

        // le o ficheiro de config
        // ler nos de storage, workers, etc
        public void readConfigFile(string file_name) {
            
            string path = Path.Combine(Environment.CurrentDirectory, @"files\", file_name);
            string[] lines = System.IO.File.ReadAllLines(path);
            string[] words;
            foreach (string line in lines) {

                words = line.Split(' ');
                string word = words[0];

                switch (word) {
                    case "scheduler":
                        schedulerURL = words[1];
                        break;
                    case "worker":
                        workerStruct auxWorker = new workerStruct();
                        auxWorker.name = words[1];
                        auxWorker.url = words[2];
                        auxWorker.gossipDelay = words[3];

                        WorkersList.Add(auxWorker);

                        break;
                    case "storage":
                        StorageStruct auxStorage = new StorageStruct();
                        auxStorage.name = words[1];
                        auxStorage.url = words[2];
                        auxStorage.gossipDelay = words[3];

                        StorageList.Add(auxStorage);
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
                        break;
                    case "status":

                        break;
                    case "listServer":

                        break;
                    case "listGlobal":

                        break;
                    case "crash":

                        break;
                    case "wait":

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
