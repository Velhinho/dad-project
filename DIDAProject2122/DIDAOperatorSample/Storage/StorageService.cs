using System;
using System.Threading.Tasks;
using Grpc.Core;
using System.Collections.Generic;
using Grpc.Net.Client;

namespace Storage
{
    class StorageService : DIDAStorageService.DIDAStorageServiceBase {
        private string storage_id;
        private int gossip_delay;
        private readonly StorageNode StorageNode;
        private List<StorageStruct> storageInfo = new List<StorageStruct>();
        List<DIDAStorageService.DIDAStorageServiceClient> _clients = new List<DIDAStorageService.DIDAStorageServiceClient>();
        List<Grpc.Net.Client.GrpcChannel> _channels = new List<Grpc.Net.Client.GrpcChannel>();

        public StorageService(string id, int delay, StorageNode storageNode)
        {
            storage_id = id;
            gossip_delay = delay;
            this.StorageNode = storageNode;
        }

        public override Task<DIDAReadReply> read(DIDAReadRequest request, ServerCallContext context)
        {
            var record = StorageNode.read(request.Id, request.Version);
            return Task.FromResult<DIDAReadReply>(new DIDAReadReply { Record = record });
        }

        public override Task<DIDAVersion> updateIfValueIs(DIDAUpdateIfRequest request, ServerCallContext context)
        {
            var version = StorageNode.updateIfValueIs(request.Id, request.Oldvalue, request.Newvalue);
            return Task.FromResult<DIDAVersion>(version);
        }

        public override Task<DIDAVersion> write(DIDAWriteRequest request, ServerCallContext context)
        {
            Console.WriteLine("Received a write request!");
            Console.WriteLine("Request: " + request);
            var version = StorageNode.write(request.Id, request.Val);
            Console.WriteLine("Version " + version);
            return Task.FromResult<DIDAVersion>(version);
        }

        public override Task<DIDAPrintReply> printStatus(DIDAPrintRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }
        public override Task<DIDAListServerReply> listServer(DIDAListServerRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }
        public override Task<DIDAStorageInfoReply> sendStorageInfo(DIDAStorageInfoRequest request, ServerCallContext context)
        {
            foreach (string line in request.StorageList)
            {
                string[] parsedLine = line.Split("#");
                if (!parsedLine[0].Equals(storage_id))
                {
                    storageInfo.Add(new StorageStruct { name = parsedLine[0], url = parsedLine[1] });
                }
            }
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            foreach (StorageStruct n in storageInfo)
            {
                var channel = Grpc.Net.Client.GrpcChannel.ForAddress(n.url);
                _channels.Add(channel);
                _clients.Add(new DIDAStorageService.DIDAStorageServiceClient(channel));
            }
            return Task.FromResult<DIDAStorageInfoReply>(new DIDAStorageInfoReply { });
        }

        struct StorageStruct
        {
            public string name, url;
        }
    }
}
