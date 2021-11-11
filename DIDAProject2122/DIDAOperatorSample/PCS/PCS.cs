using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Grpc.Core;

namespace PCS {
    class PCS {

        class PCSServerService : DIDAPCSServerService.DIDAPCSServerServiceBase {

            private Dictionary<string, Process> dict_of_processes = new Dictionary<string, Process>();

            public override Task<DIDAEmpty> CreateRemoteScheduler(DIDAcreateScheduler request, ServerCallContext context) {
                 //return base.CreateRemoteScheduler(request, context);

                return Task.FromResult<DIDAEmpty> (CreateRemoteSchedulerImpl(request));
            }

            private DIDAEmpty CreateRemoteSchedulerImpl(DIDAcreateScheduler request) {
                
                string schedulerURL = request.Url;
                ProcessStartInfo startInfo = new ProcessStartInfo("Scheduler.exe"); //set do .exe do processo
                List<string> args = new List<string>();

                args.Add(request.Debug);
                args.Add(schedulerURL); // definir os argumentos

                var workers = request.Workers;

                foreach(string w in workers) {
                    args.Add(w);
                }
                
                startInfo.Arguments = string.Join(" ", args.ToArray()); //passa como string mas vê como array no processo iniciado
                startInfo.UseShellExecute = true;
                Process p = Process.Start(startInfo);

                dict_of_processes.Add(request.Name, p);

                Console.WriteLine("Scheduler Criado");

                return new DIDAEmpty { };
            }

            public override Task<DIDAEmpty> CreateRemoteStorage(DIDAcreateStorage request, ServerCallContext context) {
                //return base.CreateRemoteStorage(request, context);

                return Task.FromResult<DIDAEmpty>(CreateRemoteStorageImpl(request));
            }

            private DIDAEmpty CreateRemoteStorageImpl(DIDAcreateStorage request) {

                ProcessStartInfo startInfo = new ProcessStartInfo("Storage.exe"); //set do .exe do processo
                List<string> args = new List<string>();

                args.Clear();
                args.Add(request.Name + " " + request.Url + " " + request.Gossip + " " + request.ReplicaIdAutoIncrement);
                startInfo.Arguments = string.Join(" ", args.ToArray()); //passa como string mas vê como array no processo iniciado
                startInfo.UseShellExecute = true;
                Process p = Process.Start(startInfo);
                dict_of_processes.Add(request.Name, p);

                Console.WriteLine("Storage Criado");

                return new DIDAEmpty { };
            }

            public override Task<DIDAEmpty> CreateRemoteWorker(DIDAcreateWorker request, ServerCallContext context) {
                //return base.CreateRemoteWorker(request, context);

                return Task.FromResult<DIDAEmpty>(CreateRemoteWorkerImpl(request));
            }

            private DIDAEmpty CreateRemoteWorkerImpl(DIDAcreateWorker request) {

                string workerURL = request.Url;
                ProcessStartInfo startInfo = new ProcessStartInfo("Worker.exe"); //set do .exe do processo
                List<string> args = new List<string>();

                args.Clear();
                args.Add(request.Debug + " " + request.Name + " " + request.Url + " " + request.Gossip);

                var storage = request.Storage;

                foreach (string s in storage) {
                    args.Add(s);
                }

                startInfo.Arguments = string.Join(" ", args.ToArray()); //passa como string mas vê como array no processo iniciado
                startInfo.UseShellExecute = true;
                Process p = Process.Start(startInfo);
                dict_of_processes.Add(request.Name, p);
                Console.WriteLine("Worker Criado");

                return new DIDAEmpty { };
            }

            public override Task<DIDAEmpty> KillServer(DIDAServer request, ServerCallContext context) {
                //return base.KillServer(request, context);
              
                return Task.FromResult<DIDAEmpty>(KillServerImpl(request));
            }

            private DIDAEmpty KillServerImpl(DIDAServer request) {

                string to_crash = request.Name;

                Process to_kill = dict_of_processes[to_crash];
                to_kill.Kill();

                dict_of_processes.Remove(to_crash);

                return new DIDAEmpty { };
            }
        }

        static void Main(string[] args) {
            Console.WriteLine("Hello World!");

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            string host = "localhost";
            int port = 10000;

            Server server = new Server {
                Services = { DIDAPCSServerService.BindService(new PCSServerService()) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("Server started on port " + port);

            Console.ReadKey();
            server.ShutdownAsync().Wait();
        }
    }
}
