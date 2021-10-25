using System;
using System.Threading.Tasks;
using Grpc.Core;
using System.Collections.Generic;

namespace Storage {

    class StorageService : DIDAStorageService.DIDAStorageServiceBase
    {
        string storage_id;
        int gossip_delay;
        Dictionary<string, string> items = new Dictionary<string, string>();

        public StorageService(string id, int delay)
        {
            storage_id = id;
            gossip_delay = delay;
        }

        public override Task<DIDARecordReply> read(DIDAReadRequest request, ServerCallContext context)
        {
            return Task.FromResult<DIDARecordReply>(ReadImpl(request));
        }

        private DIDARecordReply ReadImpl(DIDAReadRequest request)
        {
            return new DIDARecordReply
            {
                Id = request.Id,
                Val = "1",
                Version = new DIDAVersion { ReplicaId = 1, VersionNumber = 1 }
            };
        }

        public override Task<DIDAVersion> updateIfValueIs(DIDAUpdateIfRequest request, ServerCallContext context)
        {
            return Task.FromResult<DIDAVersion>(UpdateImpl(request));
        }

        private DIDAVersion UpdateImpl(DIDAUpdateIfRequest request)
        {
            return new DIDAVersion { ReplicaId = -1, VersionNumber = -1 };
        }

        public override Task<DIDAVersion> write(DIDAWriteRequest request, ServerCallContext context)
        {
            return Task.FromResult<DIDAVersion>(WriteImpl(request));
        }

        private DIDAVersion WriteImpl(DIDAWriteRequest request)
        {
            return new DIDAVersion { ReplicaId = -1, VersionNumber = -1 };
        }
        public override Task<DIDAPrintReply> printStatus(DIDAPrintRequest request, ServerCallContext context)
        {
            
            return Task.FromResult<DIDAPrintReply>(new DIDAPrintReply());
        }
        public override Task<DIDAListServerReply> listServer(DIDAListServerRequest request, ServerCallContext context)
        {
            foreach (var thing in items)
            {
                Console.WriteLine(thing.Key + ": " + thing.Value);
            }
            return Task.FromResult<DIDAListServerReply>(new DIDAListServerReply());
        }
    }

    class Storage {
        static void Main(string[] args) {

            foreach (var item in args) {
                Console.WriteLine(item.ToString());
            }
            string id = args[0];
            Uri uri = new Uri(args[1]);
            int gossip_delay = Int32.Parse(args[2]);
            int port = uri.Port;
            string host = uri.Host;
            Server server = new Server
            {
                Services = { DIDAStorageService.BindService(new StorageService(id, gossip_delay)) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("Server " + id + " started on port " + port);
            Console.ReadKey();
            server.ShutdownAsync().Wait();

        }
    }
}
