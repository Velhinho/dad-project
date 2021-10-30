using System;
using Grpc.Core;
using System.Collections.Generic;
using Grpc.Net.Client;

namespace Scheduler
{
    class SchedulerServerService : DIDASchedulerServerService.DIDASchedulerServerServiceBase
    {

    }
    public class Scheduler
    {
        static void Main(string[] args)
        {
            List<workerStruct> WorkersList = new List<workerStruct>();
            foreach (var item in args) {
                Console.WriteLine(item.ToString());
            }
            Uri uri = new Uri(args[0]);
            int port = uri.Port;
            string host = uri.Host;
            for(var i = 1; i < args.Length; i++)
            {
                string[] workerInfo = args[i].Split("#");
                workerStruct auxWorker = new workerStruct();
                auxWorker.name = workerInfo[0];
                auxWorker.url = workerInfo[1];
                WorkersList.Add(auxWorker);
            }
            Server server = new Server
            {
                Services = { DIDASchedulerServerService.BindService(new SchedulerServerService()) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("Server started on port " + port);
            DIDARequest request = new DIDARequest
            {
                Input = "Epic",
                Next = 0,
                Meta = new DIDAMetaRecord { Id = 1 }
            };
            request.Chain.Add(new DIDAAssignment { Output = "", Operator = new DIDAOperatorID { Classname = "AddOperator" } });
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            GrpcChannel channel = GrpcChannel.ForAddress(WorkersList[0].url);
            var worker = new DIDAWorkerServerService.DIDAWorkerServerServiceClient(channel);
            DIDAReply result = worker.work(request);
            Console.ReadKey();
            server.ShutdownAsync().Wait();
            }
        struct workerStruct
        {
            public string name, url;
        }
    }
}
