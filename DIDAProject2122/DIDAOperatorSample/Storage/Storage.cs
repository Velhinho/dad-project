using System;
using Grpc.Core;
using System.Timers;

namespace Storage
{
    class Storage {
        public static void Main(string[] args)
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

            var storageNode = new StorageNode(replicaId, uri.AbsoluteUri[0..^1]);
            Server server = new Server
            {
                Services = { DIDAStorageService.BindService(new StorageService(id, gossip_delay, storageNode)) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("Server " + id + " started on port " + port);

            var timer = new Timer(gossip_delay);
            timer.Elapsed += (s, e) => Gossip(storageNode);
            timer.AutoReset = true;
            timer.Enabled = true;

            Console.ReadKey();
            server.ShutdownAsync().Wait();

        }

        public static void Gossip(StorageNode storageNode)
        {
            Console.WriteLine("it's gossip time baby");
            storageNode.SendGossip();
        }
    }
}
