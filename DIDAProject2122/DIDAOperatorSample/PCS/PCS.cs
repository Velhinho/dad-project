using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Grpc.Core;

namespace PCS {
    class PCS {

       

        class PCSServerService : DIDAPCSServerService.DIDAPCSServerServiceBase {

            public override Task<DIDAEmpty> CreateRemoteScheduler(DIDAcreateScheduler request, ServerCallContext context) {
                 //return base.CreateRemoteScheduler(request, context);

                return Task.FromResult<DIDAEmpty> (CreateRemoteSchedulerImpl(request));
            }

            private DIDAEmpty CreateRemoteSchedulerImpl(DIDAcreateScheduler request) {
                
                string schedulerURL = request.Url;
                ProcessStartInfo startInfo = new ProcessStartInfo("Scheduler.exe"); //set do .exe do processo
                List<string> args = new List<string>();
                
                args.Add(schedulerURL); // definir os argumentos

                var workers = request.Workers;

                foreach(string w in workers) {
                    args.Add(w);
                }
                
                startInfo.Arguments = string.Join(" ", args.ToArray()); //passa como string mas vê como array no processo iniciado
                
                Process.Start(startInfo);

                Console.WriteLine("Scheduler Criado");

                return new DIDAEmpty { };
            }

            public override Task<DIDAEmpty> CreateRemoteStorage(DIDAcreateStorage request, ServerCallContext context) {
                //return base.CreateRemoteStorage(request, context);

                return Task.FromResult<DIDAEmpty>(CreateRemoteStorageImpl(request));
            }

            private DIDAEmpty CreateRemoteStorageImpl(DIDAcreateStorage request) {

                return new DIDAEmpty { };
            }

            public override Task<DIDAEmpty> CreateRemoteWorker(DIDAcreateWorker request, ServerCallContext context) {
                //return base.CreateRemoteWorker(request, context);

                return Task.FromResult<DIDAEmpty>(CreateRemoteWorkerImpl(request));
            }

            private DIDAEmpty CreateRemoteWorkerImpl(DIDAcreateWorker request) {

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
