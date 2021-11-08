using Grpc.Net.Client;
using System;
using System.Collections.Generic;

namespace Storage
{
    class StorageNode
    {
        private readonly RecordStorage recordStorage;
        private readonly DIDAVersion emptyVersion;
        private readonly DIDARecord emptyRecord;
        private readonly int replicaId;
        private readonly string url;
        private readonly List<DIDAStorageService.DIDAStorageServiceClient> clients = 
            new List<DIDAStorageService.DIDAStorageServiceClient>();
        private readonly Log log;

        internal RecordStorage RecordStorage => recordStorage;

        public DIDAVersion EmptyVersion => emptyVersion;

        public DIDARecord EmptyRecord => emptyRecord;

        public int ReplicaId => replicaId;

        public List<DIDAStorageService.DIDAStorageServiceClient> Clients => clients;

        internal Log Log => log;

        public string Url => url;

        public StorageNode(int replicaId, string url)
        {
            emptyVersion = new DIDAVersion { ReplicaId = -1, VersionNumber = -1 };
            emptyRecord = new DIDARecord { Id = "", Val = "", Version = emptyVersion };
            this.recordStorage = new RecordStorage();
            this.replicaId = replicaId;
            log = new Log();
            this.url = url;
        }

        public DIDARecord Read(string id, DIDAVersion version)
        {
            lock (RecordStorage)
            {
                if (version.Equals(EmptyVersion))
                {
                    return RecordStorage.GetRecord(id);
                }
                else
                {
                    return RecordStorage.GetRecord(id, version);
                }
            }
        }

        public DIDAVersion Write(string id, string val)
        {
            lock(RecordStorage)
            {
                Log.AddWriteEntry(id, val);
                return RecordStorage.WriteRecord(id, val);
            }
        }

        public DIDAVersion UpdateIfValueIs(string id, string oldValue, string newValue)
        {
            lock(RecordStorage)
            {
                Log.AddUpdateIfEntry(id, newValue, oldValue);
                return RecordStorage.UpdateIfValue(id, oldValue, newValue);
            }
        }

        public void AddClients(List<string> storageList)
        {
            foreach (string line in storageList)
            {
                string[] parsedLine = line.Split("#");
                if (!parsedLine[0].Equals(ReplicaId))
                {
                    var url = parsedLine[1];
                    if (url == this.Url)
                    {
                        continue;
                    }
                    Console.WriteLine("Client: " + url);
                    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                    var channel = GrpcChannel.ForAddress(url);
                    Clients.Add(new DIDAStorageService.DIDAStorageServiceClient(channel));
                }
            }
        }

        public DIDAVersion Populate(string id, string val)
        {
            lock (RecordStorage)
            {
                Console.WriteLine("Populate");
                return RecordStorage.WriteRecord(id, val);
            }
        }

        public void MergeGossip(List<LogEntry> entries)
        {
            lock(RecordStorage)
            {
                Console.WriteLine("received log from gossip baby");
                if (entries.Count == 0)
                {
                    Console.WriteLine("Empty gossip");
                }
                foreach (var entry in entries)
                {
                    if (entry.Type == LogEntryType.Write)
                    {
                        RecordStorage.WriteRecord(entry.Id, entry.NewValue);
                    } 
                    else
                    {
                        RecordStorage.UpdateIfValue(entry.Id, entry.OldValue, entry.NewValue);
                    }
                    Console.WriteLine("Gossip entry: " + entry);
                }
            }
        }

        public void SendGossip()
        {
            lock(RecordStorage)
            {
                if ( Clients.Count == 0)
                {
                    return;
                }
                foreach (var client in Clients)
                {
                    var request = new DIDAPushGossipRequest { };
                    request.Log.Add(Log.ToList());
                    client.pushGossipAsync(request);
                }
                Console.WriteLine("sending logs to other clients baby");
                Console.WriteLine("Number of entries " + Log.LogEntries.Count);
                Log.Clear();
            }
        }
    }
}
