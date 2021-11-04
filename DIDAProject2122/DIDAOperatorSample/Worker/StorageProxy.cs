using System;
using System.Collections.Generic;
using DIDAWorker;

namespace Worker
{
    public class StorageProxy : IDIDAStorage
    {
        // dictionary with storage gRPC client objects for all storage nodes
        List<DIDAStorageService.DIDAStorageServiceClient> _clients = new List<DIDAStorageService.DIDAStorageServiceClient>();
        // dictionary with storage gRPC channel objects for all storage nodes
        List<Grpc.Net.Client.GrpcChannel> _channels = new List<Grpc.Net.Client.GrpcChannel>();
        // metarecord for the request that this storage proxy is handling
        DIDAMetaRecord _meta;
        int _replicaNumber;
        int _replicaLeader = 0;

        // The constructor of a storage proxy.
        // The storageNodes parameter lists the nodes that this storage proxy needs to be aware of to perform
        // read, write and updateIfValueIs operations.
        // The metaRecord identifies the request being processed by this storage proxy object
        // and allows the storage proxy to request data versions previously accessed by the request
        // and to inform operators running on the following (downstream) workers of the versions it accessed.
        public StorageProxy(DIDAStorageNode[] storageNodes, DIDAMetaRecord metaRecord)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            foreach (DIDAStorageNode n in storageNodes)
            {
                var channel = Grpc.Net.Client.GrpcChannel.ForAddress("http://" + n.host + ":" + n.port);
                _channels.Add(channel);
                _clients.Add(new DIDAStorageService.DIDAStorageServiceClient(channel));
            }
            _meta = metaRecord;
            _replicaNumber = _clients.Count;
        }

        // THE FOLLOWING 3 METHODS ARE THE ESSENCE OF A STORAGE PROXY
        // IN THIS EXAMPLE THEY ARE JUST CALLING THE STORAGE 
        // IN THE COMLPETE IMPLEMENTATION THEY NEED TO:
        // 1) LOCATE THE RIGHT STORAGE SERVER
        // 2) DEAL WITH FAILED STORAGE SERVERS
        // 3) CHECK IN THE METARECORD WHICH ARE THE PREVIOUSLY READ VERSIONS OF DATA 
        // 4) RECORD ACCESSED DATA INTO THE METARECORD

        public virtual DIDARecordReply read(DIDAWorker.DIDAReadRequest r)
        {
            while (true)
            {
                try
                {
                    var version = new DIDAVersion { VersionNumber = r.Version.VersionNumber, ReplicaId = r.Version.ReplicaId };
                    var request = new DIDAReadRequest { Id = r.Id, Version =  version};
                    var res = _clients[_replicaLeader].read(request);
                    Console.WriteLine(_replicaLeader + " finished reading");
                    Console.WriteLine(res);
                    var versionReply = new DIDAWorker.DIDAVersion { ReplicaId = res.Record.Version.ReplicaId, VersionNumber = res.Record.Version.VersionNumber };
                    Console.WriteLine("converted version correctly");
                    return new DIDARecordReply { Id = res.Record.Id, Val = res.Record.Val, Version = versionReply };
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    _replicaLeader += 1;
                    _replicaLeader %= _replicaNumber;
                }
            }
        }

        public virtual DIDAWorker.DIDAVersion write(DIDAWorker.DIDAWriteRequest r)
        {
            while(true)
            {
                try
                {
                    var res = _clients[_replicaLeader].write(new DIDAWriteRequest { Id = r.Id, Val = r.Val });
                    return new DIDAWorker.DIDAVersion { VersionNumber = res.VersionNumber, ReplicaId = res.ReplicaId };
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    _replicaLeader += 1;
                    _replicaLeader %= _replicaNumber; 
                }
            }
        }

        public virtual DIDAWorker.DIDAVersion updateIfValueIs(DIDAWorker.DIDAUpdateIfRequest r)
        {
            while (true)
            {
                try
                {
                    var req = new DIDAUpdateIfRequest { Id = r.Id, Newvalue = r.Newvalue, Oldvalue = r.Oldvalue };
                    var res = _clients[_replicaLeader].updateIfValueIs(req);
                    return new DIDAWorker.DIDAVersion { VersionNumber = res.VersionNumber, ReplicaId = res.ReplicaId };
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    _replicaLeader += 1;
                    _replicaLeader %= _replicaNumber;
                }
            }
        }
    }
}
