using System;
using Grpc.Core;

namespace Worker {

    class WorkerServerService : DIDAWorkerServerService.DIDAWorkerServerServiceBase
    {
        string worker_id;
        int gossip_delay;
        public WorkerServerService(string id, int delay)
        {
            worker_id = id;
            gossip_delay = delay;
        }
    }

    class Worker {
        static void Main(string[] args) {

            foreach (var item in args) {
                Console.WriteLine(item.ToString());
            }
            string id = args[1];
            Uri uri = new Uri(args[2]);
            int gossip_delay = Int32.Parse(args[3]);
            int port = uri.Port;
            string host = uri.Host;
            Server server = new Server
            {
                Services = { DIDAWorkerServerService.BindService(new WorkerServerService(id, gossip_delay)) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("Server " + id + " started on port " + port);
            Console.ReadKey();
            server.ShutdownAsync().Wait();

        }
    }
}
