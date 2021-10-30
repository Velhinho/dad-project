using System;
using System.Threading.Tasks;
using Grpc.Core;
using System.Reflection;
using System.IO;
using DIDAWorker;
using System.Collections.Generic;

namespace Worker
{

    class WorkerServerService : DIDAWorkerServerService.DIDAWorkerServerServiceBase
    {
        string worker_id;
        int gossip_delay;
        StorageProxy storageProxy;
        public WorkerServerService(string id, int delay, StorageProxy storageProxy)
        {
            worker_id = id;
            gossip_delay = delay;
            this.storageProxy = storageProxy;
        }

        public override Task<DIDAReply> work(DIDARequest request, ServerCallContext context)
        {
            return Task.FromResult<DIDAReply>(WorkImpl(request));
        }

        private DIDAReply WorkImpl(DIDARequest request)
        {
            string class_to_load = request.Chain[request.Next].Operator.Classname;
            Console.WriteLine("Received request to use " + class_to_load);
            IDIDAOperator opObj = createOpInstance(class_to_load);
            opObj.ConfigureStorage(storageProxy);
            Console.WriteLine("Configured Storage!");
            var meta = new DIDAWorker.DIDAMetaRecord { Id = request.Meta.Id };
            opObj.ProcessRecord(meta, request.Input, request.Chain[request.Next].Output);
            Console.WriteLine("Finished processing record helll yeah");
            return new DIDAReply
            {

            };
        }

        private IDIDAOperator createOpInstance(string class_to_load)
        {
            string _dllNameTermination = ".dll";
            string _currWorkingDir = Directory.GetCurrentDirectory();
            string opPath = Path.Combine(_currWorkingDir, "..\\..\\..\\..", "OperatorLibrary");
            Console.WriteLine("Current working directory (cwd): " + opPath);
            IDIDAOperator _objLoadedByReflection;
            foreach (string filename in Directory.EnumerateFiles(opPath))
            {
                Console.WriteLine("file in cwd: " + filename);
                if (filename.EndsWith(_dllNameTermination))
                {
                    Console.WriteLine("loading assembly");
                    Assembly _dll = Assembly.LoadFrom(filename);
                    Console.WriteLine("getting types");
                    Type[] _typeList = _dll.GetTypes();
                    foreach (Type type in _typeList)
                    {
                        Console.WriteLine("typename = " + type.Name);
                        if (type.Name == class_to_load)
                        {
                            Console.WriteLine("Found type to load dynamically: " + class_to_load);
                            _objLoadedByReflection = (IDIDAOperator) Activator.CreateInstance(type);
                            Console.WriteLine("success!");
                            return _objLoadedByReflection;
                        }
                    }
                }
            }
            Console.WriteLine("Unable to find class " + class_to_load);
            return null;
        }
    }

    class Worker {

        static List<DIDAStorageNode> GetDIDAStorages(string[] args)
        {
            List<DIDAStorageNode> nodes = new List<DIDAStorageNode>();

            for (int i = 4; i < args.Length; i++)
            {
                string[] words = args[i].Split("#");
                Uri storageUri = new Uri(words[1]);
                nodes.Add(new DIDAStorageNode { host = storageUri.Host, port = storageUri.Port, serverId = words[0] });
            }
            return nodes;
        }

        static void Main(string[] args) {

            foreach (var item in args) {
                Console.WriteLine(item.ToString());
            }
            string id = args[1];
            Uri uri = new Uri(args[2]);
            int gossip_delay = Int32.Parse(args[3]);
            int port = uri.Port;
            string host = uri.Host;
            var nodes = GetDIDAStorages(args);
            StorageProxy storageProxy = new StorageProxy(nodes.ToArray(), new DIDAMetaRecord { Id = 1 });
            
            Server server = new Server
            {
                Services = { DIDAWorkerServerService.BindService(new WorkerServerService(id, gossip_delay, storageProxy)) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("Server " + id + " started on port " + port);
            Console.ReadKey();
            server.ShutdownAsync().Wait();

        }
    }
}
