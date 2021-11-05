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
        private string storage_id;
        private int gossip_delay;
        private readonly StorageNode StorageNode;
        private List<StorageStruct> storageInfo = new List<StorageStruct>();
        List<DIDAStorageService.DIDAStorageServiceClient> _clients = new List<DIDAStorageService.DIDAStorageServiceClient>();
        List<Grpc.Net.Client.GrpcChannel> _channels = new List<Grpc.Net.Client.GrpcChannel>();
        List<DIDAWriteRequest> _writeLog = new List<DIDAWriteRequest>();
        List<DIDAUpdateIfRequest> _updateLog = new List<DIDAUpdateIfRequest>();

        public StorageService(string id, int delay, StorageNode storageNode)
        {
            storage_id = id;
            gossip_delay = delay;
            this.StorageNode = storageNode;
            Task.FromResult(push_gossip());
        }

        public override Task<DIDAReadReply> read(DIDAReadRequest request, ServerCallContext context)
        {
            var record = StorageNode.read(request.Id, request.Version);
            return Task.FromResult<DIDAReadReply>(new DIDAReadReply { Record = record });
        }

        public override Task<DIDAVersion> updateIfValueIs(DIDAUpdateIfRequest request, ServerCallContext context)
        {
            var version = StorageNode.updateIfValueIs(request.Id, request.Oldvalue, request.Newvalue);
            lock(_updateLog) { 
                _updateLog.Add(request);
            }
            return Task.FromResult<DIDAVersion>(version);
        }

        public override Task<DIDAVersion> write(DIDAWriteRequest request, ServerCallContext context)
        {
            Console.WriteLine("Received a write request!");
            Console.WriteLine("Request: " + request);
            var version = StorageNode.write(request.Id, request.Val);
            Console.WriteLine("Version " + version);
            lock (_writeLog)
            {
                _writeLog.Add(request);
            }
            return Task.FromResult<DIDAVersion>(version);
        }

        public async Task push_gossip()
        {
            while(true)
            {
                Console.WriteLine("it's gossip time baby");
                var delayTask = Task.Delay(gossip_delay);
                lock(_writeLog)
                {
                    lock(_updateLog)
                    {
                        if (_writeLog.Count > 0 || _updateLog.Count > 0)
                        {
                            foreach (var client in _clients)
                            {
                                var request = new DIDAPushGossipRequest { };
                                request.WriteList.Add(_writeLog);
                                request.UpdateList.Add(_updateLog);
                                client.pushGossip(request);
                            }
                            Console.WriteLine("sending logs to other clients baby");
                            _writeLog.Clear();
                            _updateLog.Clear();
                        }
                    }
                }
                await delayTask;
            }
        }

        public override Task<DIDAPushGossipReply> pushGossip(DIDAPushGossipRequest request, ServerCallContext context)
        {
            Console.WriteLine("received log from gossip baby");
            foreach(var writeReq in request.WriteList)
            {
                StorageNode.write(writeReq.Id, writeReq.Val);
            }

            foreach(var updateIfReq in request.UpdateList)
            {
                StorageNode.updateIfValueIs(updateIfReq.Id, updateIfReq.Oldvalue, updateIfReq.Newvalue);
            }

            return Task.FromResult(new DIDAPushGossipReply { });
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

        public override Task<DIDAVersion> populate(DIDAWriteRequest request, ServerCallContext context)
        {
            StorageNode.write(request.Id, request.Val);
            return Task.FromResult(new DIDAVersion { });
        }

        struct StorageStruct
        {
            public string name, url;
        }
    }
}
