using System;
using System.Threading.Tasks;
using Grpc.Core;
using System.Collections.Generic;
using Grpc.Net.Client;
using System.Timers;
using System.Linq;

namespace Storage
{
    class StorageService : DIDAStorageService.DIDAStorageServiceBase {
        private readonly string storageId;
        private readonly int gossipDelay;
        private readonly StorageNode StorageNode;

        public string StorageId => storageId;

        public int GossipDelay => gossipDelay;

        public StorageService(string id, int delay, StorageNode storageNode)
        {
            storageId = id;
            gossipDelay = delay;
            this.StorageNode = storageNode;
        }

        public override Task<DIDAReadReply> read(DIDAReadRequest request, ServerCallContext context)
        {
            var record = StorageNode.Read(request.Id, request.Version);
            return Task.FromResult<DIDAReadReply>(new DIDAReadReply { Record = record });
        }

        public override Task<DIDAVersion> updateIfValueIs(DIDAUpdateIfRequest request, ServerCallContext context)
        {
            Console.WriteLine("Received a updateIfValue request!");
            Console.WriteLine("Request: " + request);
            var version = StorageNode.UpdateIfValueIs(request.Id, request.Oldvalue, request.Newvalue);
            return Task.FromResult<DIDAVersion>(version);
        }

        public override Task<DIDAVersion> write(DIDAWriteRequest request, ServerCallContext context)
        {
            Console.WriteLine("Received a write request!");
            Console.WriteLine("Request: " + request);
            var version = StorageNode.Write(request.Id, request.Val);
            Console.WriteLine("Version " + version);
            return Task.FromResult<DIDAVersion>(version);
        }

        public override Task<DIDAPushGossipReply> pushGossip(DIDAPushGossipRequest request, ServerCallContext context)
        {
            StorageNode.MergeGossip(request.Log.ToList());
            return Task.FromResult(new DIDAPushGossipReply { });
        }

        public override Task<DIDAPrintReply> printStatus(DIDAPrintRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }
        
        public override Task<DIDAListServerReply> listServer(DIDAListServerRequest request, ServerCallContext context)
        {
            Console.WriteLine(StorageNode.RecordStorage.ToString());
            return Task.FromResult(new DIDAListServerReply { });
        }

        public override Task<DIDAStorageInfoReply> sendStorageInfo(DIDAStorageInfoRequest request, ServerCallContext context)
        {
            StorageNode.AddClients(request.StorageList.ToList());
            return Task.FromResult(new DIDAStorageInfoReply { });
        }

        public override Task<DIDAVersion> populate(DIDAWriteRequest request, ServerCallContext context)
        {
            StorageNode.Populate(request.Id, request.Val);
            return Task.FromResult(new DIDAVersion { });
        }
    }
}
