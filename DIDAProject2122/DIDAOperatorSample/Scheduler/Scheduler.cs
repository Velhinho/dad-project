using System;
using Grpc.Core;
using System.Collections.Generic;
using Grpc.Net.Client;
using System.Threading.Tasks;

namespace Scheduler
{
    class SchedulerServerService : DIDASchedulerServerService.DIDASchedulerServerServiceBase {

        //construtor a completar
        public SchedulerServerService() {

        }

        public override Task<DIDAEmptyReply> RcvClientRequest(DIDAClientRequest request, ServerCallContext context) {
            //return base.RcvClientRequest(request, context);

            return Task.FromResult<DIDAEmptyReply>(RcvClientRequestImpl(request));
        }

        private DIDAEmptyReply RcvClientRequestImpl(DIDAClientRequest request) {

            string input = request.Input;
            List<string> list_of_commands = new List<string>();
            foreach(string command in request.Commands) {
                list_of_commands.Add(command);
            }

            Console.WriteLine("input = " + input);
            foreach(string i in list_of_commands) {
                Console.WriteLine("Command: " + i);
            }

            return new DIDAEmptyReply { };
        }

        //recebe um DIDAPopData que contem uma lista com todas as informacoes, linha a linha do ficheiro
        public override Task<DIDAEmptyReply> RcvPopulateData(DIDAPopData request, ServerCallContext context) {
            //return base.RcvClientRequest(request, context);

            return Task.FromResult<DIDAEmptyReply>(RcvPopulateDataIpml(request));
        }

        private DIDAEmptyReply RcvPopulateDataIpml(DIDAPopData request) {

            
            Dictionary<string, string> DataDic = new Dictionary<string, string>();
            foreach( string line in request.Data) {
                string[] aux = line.Split(",");
                DataDic.Add(aux[0], aux[1]);
            }

            Console.WriteLine("Data dictionary:");
            foreach (KeyValuePair<string, string> item in DataDic) {
                Console.WriteLine("id = {0}, data = {1}", item.Key, item.Value);
            }

            return new DIDAEmptyReply { };
        }


    }
    public class Scheduler
    {
        static void Main(string[] args)
        {
            List<workerStruct> WorkersList = new List<workerStruct>();
            foreach (var item in args) {
                Console.WriteLine(item.ToString());
            }
            Uri uri = new Uri(args[0]);
            int port = uri.Port;
            string host = uri.Host;
            for(var i = 1; i < args.Length; i++)
            {
                string[] workerInfo = args[i].Split("#");
                workerStruct auxWorker = new workerStruct();
                auxWorker.name = workerInfo[0];
                auxWorker.url = workerInfo[1];
                WorkersList.Add(auxWorker);
            }
            Server server = new Server
            {
                Services = { DIDASchedulerServerService.BindService(new SchedulerServerService()) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("Server started on port " + port);
            
            /*DIDARequest request = new DIDARequest
            {
                Input = "Epic",
                Next = 0,
                Meta = new DIDAMetaRecord { Id = 1 }
            };
            request.Chain.Add(new DIDAAssignment { Output = "", Operator = new DIDAOperatorID { Classname = "AddOperator" } });
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            GrpcChannel channel = GrpcChannel.ForAddress(WorkersList[0].url);
            var worker = new DIDAWorkerServerService.DIDAWorkerServerServiceClient(channel);
            DIDAReply result = worker.work(request);*/
            Console.ReadKey();
            server.ShutdownAsync().Wait();
        }
        struct workerStruct
        {
            public string name, url;
        }
    }
}
