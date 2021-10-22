using System;
using Grpc.Core;

namespace Worker {

    class WorkerServerService : DIDAWorkerServerService.DIDAWorkerServerServiceBase
    {
        string worker_id;
        public WorkerServerService(string id)
        {
            worker_id = id;
        }
    }

    class Worker {
        static void Main(string[] args) {

            foreach (var item in args) {
                Console.WriteLine(item.ToString());
            }
            string id = args[1];
            Uri uri = new Uri(args[2]);
            int port = uri.Port;
            Server server = new Server
            {
                Services = { DIDAWorkerServerService.BindService(new WorkerServerService(id)) },
                Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("Server " + id + " started on port " + port);
            Console.ReadKey();
            server.ShutdownAsync().Wait();

        }
    }
}
