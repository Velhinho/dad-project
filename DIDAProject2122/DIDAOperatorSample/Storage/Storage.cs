using System;
using Grpc.Core;

namespace Storage
{
    class Storage {
        static void Main(string[] args)
        {
            foreach (var item in args)
            {
                Console.WriteLine(item.ToString());
            }
            string id = args[0];
            Uri uri = new Uri(args[1]);
            int gossip_delay = Int32.Parse(args[2]);
            int replicaId = Int32.Parse(args[3]);
            int port = uri.Port;
            string host = uri.Host;
            Server server = new Server
            {
                Services = { DIDAStorageService.BindService(new StorageService(id, gossip_delay, new StorageNode(replicaId))) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("Server " + id + " started on port " + port);
            Console.ReadKey();
            server.ShutdownAsync().Wait();

        }
    }
}
