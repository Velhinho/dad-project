using System;
using System.Threading.Tasks;
using Grpc.Core;
using System.Collections.Generic;

namespace Storage
{
    class StorageService : DIDAStorageService.DIDAStorageServiceBase {
        private string storage_id;
        private int gossip_delay;
        private readonly StorageNode StorageNode;
        
        public StorageService(string id, int delay, StorageNode storageNode)
        {
            storage_id = id;
            gossip_delay = delay;
            this.StorageNode = storageNode;
        }

        public override Task<DIDAReadReply> read(DIDAReadRequest request, ServerCallContext context)
        {
            Console.WriteLine("Received a read request!");
            var record = StorageNode.read(request.Id, request.Version);
            return Task.FromResult<DIDAReadReply>(new DIDAReadReply {Record = record});
        }

        public override Task<DIDAVersion> updateIfValueIs(DIDAUpdateIfRequest request, ServerCallContext context)
        {
            var version = StorageNode.updateIfValueIs(request.Id, request.Oldvalue, request.Newvalue);
            return Task.FromResult<DIDAVersion>(version);
        }

        public override Task<DIDAVersion> write(DIDAWriteRequest request, ServerCallContext context)
        {
            var version = StorageNode.write(request.Id, request.Val);
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
    }
}
