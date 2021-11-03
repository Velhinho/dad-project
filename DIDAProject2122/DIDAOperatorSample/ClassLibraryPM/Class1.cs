using System;
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
                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                    schedulerURL = words[2];

                    startInfo = new ProcessStartInfo("Scheduler.exe"); //set do .exe do processo

                    //args a passar
                    //adicionar aqui informacoes de storage, workers, o necessario
                    args.Clear();
                    args.Add(schedulerURL); // definir os argumentos
                    foreach (workerStruct worker in WorkersList)
                    {
                        args.Add(worker.name + "#" + worker.url);
                    }

                    startInfo.Arguments = string.Join(" ", args.ToArray()); //passa como string mas vê como array no processo iniciado

                    Process.Start(startInfo);

                    ComandosLidos = ComandosLidos + "schedulerURL = " + schedulerURL + "\r\n";
                    GrpcChannel scheduler_channel = GrpcChannel.ForAddress(schedulerURL);
                    Scheduler_Service = new DIDASchedulerServerService.DIDASchedulerServerServiceClient(scheduler_channel);

                    List<string> requestList = new List<string>();

                    foreach(StorageStruct storage in StorageList)
                    {
                        GrpcChannel storage_channel = GrpcChannel.ForAddress(storage.url);
                        var Storage_Service = new DIDAStorageService.DIDAStorageServiceClient(storage_channel);
                        StorageChannelDict.Add(storage.name, Storage_Service);
                        string aux = storage.name + "#" + storage.url;
                        requestList.Add(aux);
                    }
                    DIDAStorageInfoRequest infoRequest = new DIDAStorageInfoRequest {  };
                    infoRequest.StorageList.Add(requestList);
                    foreach(KeyValuePair<string, DIDAStorageService.DIDAStorageServiceClient> item in StorageChannelDict)
                    {
                        item.Value.sendStorageInfo(infoRequest);
                    }


                    break;
                case "worker":
                    workerStruct auxWorker = new workerStruct();
                    auxWorker.name = words[1];
                    auxWorker.url = words[2];
                    auxWorker.gossipDelay = words[3];

                    WorkersList.Add(auxWorker);

                    startInfo = new ProcessStartInfo("Worker.exe"); //set do .exe do processo

                    //args a passar, adicionar o necessario 
                    args.Clear();
                    args.Add(debugModeString + " " + auxWorker.name + " " + auxWorker.url + " " + auxWorker.gossipDelay);
                    foreach (StorageStruct storage in StorageList)
                    {
                        args.Add(storage.name + "#" + storage.url);
                    }
                    startInfo.Arguments = string.Join(" ", args.ToArray());

                    aux_To_add_list = Process.Start(startInfo);
                    list_of_processes.Add(auxWorker.name, aux_To_add_list);

                    ComandosLidos = ComandosLidos + "worker node = " + auxWorker.name + "--" + auxWorker.url + "--" + auxWorker.gossipDelay + "\r\n";

                    break;
                case "storage":
                    StorageStruct auxStorage = new StorageStruct();
                    auxStorage.name = words[1];
                    auxStorage.url = words[2];
                    auxStorage.gossipDelay = words[3];

                    StorageList.Add(auxStorage);

                    startInfo = new ProcessStartInfo("Storage.exe"); //set do .exe do processo
                                                                     //args a passar, adicionar o necessario 
                    args.Add(auxStorage.name + " " + auxStorage.url + " " + auxStorage.gossipDelay);
                    startInfo.Arguments = string.Join(" ", args.ToArray());

                    aux_To_add_list = Process.Start(startInfo);
                    list_of_processes.Add(auxStorage.name, aux_To_add_list);

                    ComandosLidos = ComandosLidos + "storage node = " + auxStorage.name + "--" + auxStorage.url + "--" + auxStorage.gossipDelay + "\r\n";

                    break;
                case "populate":
                    populate_file = words[1];

                    ComandosLidos = ComandosLidos + "populate file: " + populate_file + "\r\n";

                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

                    var pop_path = Path.Combine(Environment.CurrentDirectory, @"files\", populate_file);
                    string[] data = System.IO.File.ReadAllLines(pop_path);

                    foreach (StorageStruct storageInfo in StorageList)
                    {
                        GrpcChannel storage_channel = GrpcChannel.ForAddress(storageInfo.url);
                        var storageNode = new DIDAStorageService.DIDAStorageServiceClient(storage_channel);
                        foreach (string data_line in data)
                        {
                            string[] data_keyvalue = data_line.Split(",");
                            DIDAWriteRequest request = new DIDAWriteRequest { Id = data_keyvalue[0], Val = data_keyvalue[1] };
                            DIDAVersion reply = storageNode.write(request);
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
                    
                    var empty = Scheduler_Service.RcvClientRequest(clientRequest);

                    //quando chegamos aqui sabemos que já estão lidos todos os storage, workers e shceduler
                    //podemos então criar um dicionario com ligacoes rpc para eles. 
                    
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

                    Process to_kill = list_of_processes[to_crash];
                    to_kill.Kill();

                    list_of_processes.Remove(to_crash);

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
