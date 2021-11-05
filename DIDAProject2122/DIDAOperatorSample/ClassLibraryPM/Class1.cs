﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Grpc.Net.Client;

namespace ClassLibraryPM {
    public class PM_logic {
       
        //private ou public???
        private string schedulerURL;
        List<workerStruct> WorkersList = new List<workerStruct>();
        List<StorageStruct> StorageList = new List<StorageStruct>();
        int replicaIdAutoIncrement = 0;
        Dictionary<string, DIDAStorageService.DIDAStorageServiceClient> StorageChannelDict = 
                new Dictionary<string, DIDAStorageService.DIDAStorageServiceClient>();
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
        string ComandosLidos = "Commands\r\n";


        //comunicar com scheduler 
        DIDASchedulerServerService.DIDASchedulerServerServiceClient Scheduler_Service;


        Dictionary<string, DIDAPCSServerService.DIDAPCSServerServiceClient> PCSs_dict =
            new Dictionary<string, DIDAPCSServerService.DIDAPCSServerServiceClient>();

        Dictionary<string, string> Map_name_to_ip = new Dictionary<string, string>(); //mapeia o nome do node com o ip do node, para usar no crash

        List<string> PCSs_urls = new List<string> { "http://localhost:10000" };

        public PM_logic() {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            //inicializar canais para os pcs

            foreach(string url in PCSs_urls) {
                GrpcChannel pcs_channel = GrpcChannel.ForAddress(url);
                var pcs_Service = new DIDAPCSServerService.DIDAPCSServerServiceClient(pcs_channel);

                Uri uri = new Uri(url);
                string ip = uri.Host;

                PCSs_dict.Add(ip, pcs_Service);
            }
        }

        //le um ficheiro
        public void readFile(string file_name) {
            path = Path.Combine(Environment.CurrentDirectory, @"files\", file_name);
            lines = System.IO.File.ReadAllLines(path);
        }

        // handle de um linha do ficheiro de configs
        public void HandleNextLine(string line) {

            // infos para inicio de processos
            ProcessStartInfo startInfo;
            List<string> args = new List<string>();

            Process aux_To_add_list;

            string[] words = line.Split(' ');
            string word = words[0];

            switch (word) {
                case "scheduler":
                    string schedulerName = words[1];
                    schedulerURL = words[2];
                    Uri scheduler_uri = new Uri(schedulerURL);
                    string scheduler_IP = scheduler_uri.Host;

                    DIDAcreateScheduler createSchedulerRequest = new DIDAcreateScheduler();
                    createSchedulerRequest.Name = words[1];
                    createSchedulerRequest.Url = schedulerURL;
                    
                    foreach (workerStruct worker in WorkersList) {
                        createSchedulerRequest.Workers.Add(worker.name + "#" + worker.url);
                    }

                    PCSs_dict[scheduler_IP].CreateRemoteSchedulerAsync(createSchedulerRequest);
                    Map_name_to_ip.Add(schedulerName, scheduler_IP);

                    ComandosLidos = ComandosLidos + "schedulerURL = " + schedulerURL + "\r\n";
                    GrpcChannel scheduler_channel = GrpcChannel.ForAddress(schedulerURL);
                    Scheduler_Service = new DIDASchedulerServerService.DIDASchedulerServerServiceClient(scheduler_channel);

                    List<string> requestList = new List<string>();

                    foreach(StorageStruct storage in StorageList) {
                        GrpcChannel storage_channel = GrpcChannel.ForAddress(storage.url);
                        var Storage_Service = new DIDAStorageService.DIDAStorageServiceClient(storage_channel);
                        StorageChannelDict.Add(storage.name, Storage_Service);
                        string aux = storage.name + "#" + storage.url;
                        requestList.Add(aux);
                    }
                    DIDAStorageInfoRequest infoRequest = new DIDAStorageInfoRequest {  };
                    infoRequest.StorageList.Add(requestList);
                    foreach(KeyValuePair<string, DIDAStorageService.DIDAStorageServiceClient> item in StorageChannelDict) {
                        item.Value.sendStorageInfoAsync(infoRequest);
                    }

                    break;
                case "worker":
                    workerStruct auxWorker = new workerStruct();
                    auxWorker.name = words[1];
                    auxWorker.url = words[2];
                    auxWorker.gossipDelay = words[3];

                    WorkersList.Add(auxWorker);

                    DIDAcreateWorker createWorkerRequest = new DIDAcreateWorker();
                    createWorkerRequest.Name = auxWorker.name;
                    createWorkerRequest.Gossip = auxWorker.gossipDelay;
                    createWorkerRequest.Url = auxWorker.url;
                    createWorkerRequest.Debug = debugModeString;
                    
                    Uri worker_uri = new Uri(auxWorker.url);
                    string worker_IP = worker_uri.Host;

                    foreach (StorageStruct storage in StorageList) {
                        createWorkerRequest.Storage.Add(storage.name + "#" + storage.url);
                    }

                    PCSs_dict[worker_IP].CreateRemoteWorkerAsync(createWorkerRequest);

                    Map_name_to_ip.Add(auxWorker.name, worker_IP);

                    ComandosLidos = ComandosLidos + "worker node = " + auxWorker.name + "--" + auxWorker.url + "--" + auxWorker.gossipDelay + "\r\n";

                    break;
                case "storage":
                    StorageStruct auxStorage = new StorageStruct();
                    auxStorage.name = words[1];
                    auxStorage.url = words[2];
                    auxStorage.gossipDelay = words[3];

                    StorageList.Add(auxStorage);

                    DIDAcreateStorage createStrorageRequest = new DIDAcreateStorage();
                    createStrorageRequest.Debug = debugModeString;
                    createStrorageRequest.Gossip = auxStorage.gossipDelay;
                    createStrorageRequest.Name = auxStorage.name;
                    createStrorageRequest.Url = auxStorage.url;
                    createStrorageRequest.ReplicaIdAutoIncrement = replicaIdAutoIncrement++;

                    Uri storage_uri = new Uri(auxStorage.url);
                    string storage_IP = storage_uri.Host;

                    PCSs_dict[storage_IP].CreateRemoteStorageAsync(createStrorageRequest);

                    Map_name_to_ip.Add(auxStorage.name, storage_IP);

                    ComandosLidos = ComandosLidos + "storage node = " + auxStorage.name + "--" + auxStorage.url + "--" + auxStorage.gossipDelay + "\r\n";

                    break;
                case "populate":
                    populate_file = words[1];

                    ComandosLidos = ComandosLidos + "populate file: " + populate_file + "\r\n";

                    var pop_path = Path.Combine(Environment.CurrentDirectory, @"files\", populate_file);
                    string[] data = System.IO.File.ReadAllLines(pop_path);

                    foreach (StorageStruct storageInfo in StorageList) {
                        GrpcChannel storage_channel = GrpcChannel.ForAddress(storageInfo.url);
                        var storageNode = new DIDAStorageService.DIDAStorageServiceClient(storage_channel);
                        foreach (string data_line in data)
                        {
                            string[] data_keyvalue = data_line.Split(",");
                            DIDAWriteRequest request = new DIDAWriteRequest { Id = data_keyvalue[0], Val = data_keyvalue[1] };
                            storageNode.populateAsync(request);
                        }
                    }

                    break;
                case "client":
                    client.app_file = words[2];
                    client.input = words[1];

                    ComandosLidos = ComandosLidos + "client_input = " + client.input + "--client_app_file = " + client.app_file + "\r\n";


                    var client_app_file_path = Path.Combine(Environment.CurrentDirectory, @"files\", client.app_file);
                    string[] commands = System.IO.File.ReadAllLines(client_app_file_path);
                    
                    var clientRequest = new DIDAClientRequest { Input = client.input };
                    clientRequest.Commands.Add(commands);
                    
                    Scheduler_Service.RcvClientRequestAsync(clientRequest);
                    
                    break;
                case "debug":
                    debugMode = true;
                    debugModeString = "debug";

                    ComandosLidos = ComandosLidos + "Debug Mode On \r\n";

                    break;
                case "status":

                    ComandosLidos = ComandosLidos + "Status\r\n";

                    break;
                case "listServer":

                    string server_id = words[1];

                    ComandosLidos = ComandosLidos + "List server " + server_id + "\r\n";

                    break;
                case "listGlobal":

                    ComandosLidos = ComandosLidos + "List Global\r\n";

                    break;
                case "crash":

                    string to_crash = words[1];

                    /*Process to_kill = list_of_processes[to_crash];
                    to_kill.Kill();

                    list_of_processes.Remove(to_crash);*/

                    string to_crash_ip = Map_name_to_ip[to_crash];

                    DIDAServer server_to_crash = new DIDAServer();
                    server_to_crash.Name = to_crash;

                    PCSs_dict[to_crash_ip].KillServerAsync(server_to_crash);

                    ComandosLidos = ComandosLidos + "Crash server " + to_crash + "\r\n";

                    break;
                case "wait":

                    int wait_interval = Int32.Parse(words[1]);

                    System.Threading.Thread.Sleep(wait_interval);

                    ComandosLidos = ComandosLidos + "Wait " + words[1] + " milliseconds\r\n";
                    break;
                default:
                    ComandosLidos = ComandosLidos + "Something went wrong! Command readed: " + line + "\r\n";
                    break;
            }
        }


        /*ler o ficheiro todo de uma vez
         0 - debug
         1 - storage
         2 - worker 
         3 - storage
         4 - populate
         5 - client
         6 - o resto 
        */
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

        public string listarComandos() {

            return ComandosLidos;
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
