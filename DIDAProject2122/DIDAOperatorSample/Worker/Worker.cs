using System;
using System.Threading.Tasks;
using Grpc.Core;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using DIDAWorker;

namespace Worker {

    class WorkerServerService : DIDAWorkerServerService.DIDAWorkerServerServiceBase
    {
        string worker_id;
        int gossip_delay;
        public WorkerServerService(string id, int delay)
        {
            worker_id = id;
            gossip_delay = delay;
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
        static void Main(string[] args) {

            foreach (var item in args) {
                Console.WriteLine(item.ToString());
            }
            string id = args[1];
            Uri uri = new Uri(args[2]);
            int gossip_delay = Int32.Parse(args[3]);
            int port = uri.Port;
            string host = uri.Host;
            Server server = new Server
            {
                Services = { DIDAWorkerServerService.BindService(new WorkerServerService(id, gossip_delay)) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("Server " + id + " started on port " + port);
            Console.ReadKey();
            server.ShutdownAsync().Wait();

        }
    }
}
