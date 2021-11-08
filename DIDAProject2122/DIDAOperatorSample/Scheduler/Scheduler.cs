using System;
using Grpc.Core;
using System.Collections.Generic;
using Grpc.Net.Client;
using System.Threading.Tasks;
using System.Linq;

namespace Scheduler
{
    class SchedulerServerService : DIDASchedulerServerService.DIDASchedulerServerServiceBase {

        int id_increment = 0;
        int worker_increment = 0;

        List<workerStruct> workersList;
        //construtor a completar
        public SchedulerServerService(List<workerStruct> workersList) {
            this.workersList = workersList;
        }

        public override Task<DIDAEmptyReply> RcvClientRequest(DIDAClientRequest request, ServerCallContext context) {
            //return base.RcvClientRequest(request, context);

            return Task.FromResult<DIDAEmptyReply>(RcvClientRequestImpl(request));
        }

        private DIDAOperatorID OperatorID(string operatorString)
        {
            var elems = operatorString.Split(" ");
            return new DIDAOperatorID { Classname = elems[1], Order = Int32.Parse(elems[2]) };
        }


        private DIDAEmptyReply RcvClientRequestImpl(DIDAClientRequest request) {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            string input = request.Input;
            List<string> list_of_commands = new List<string>();
            foreach (string command in request.Commands) {
                list_of_commands.Add(command);
            }

            Console.WriteLine("input = " + input);
            foreach (string i in list_of_commands) {
                Console.WriteLine("Command: " + i);
            }

            DIDARequest megaRequest = new DIDARequest
            {
                Meta = new DIDAMetaRecord { Id = id_increment++ },
                Input = request.Input,
                Next = 0,
                ChainSize = list_of_commands.Count
            };

            var operatorIds = request.Commands.Select(OperatorID);

            foreach (DIDAOperatorID opId in operatorIds)
            {
                var url = workersList[worker_increment++ % workersList.Count].url;
                var uri = new Uri(url);
                var host = uri.Host;
                var port = uri.Port;
                var output = "";
                var assignment = new DIDAAssignment { Operator = opId, Host = host, Port = port, Output = output };
                megaRequest.Chain.Add(assignment);
            }
            var firstHost = megaRequest.Chain[0].Host;
            var firstPort = megaRequest.Chain[0].Port;
            var channel = Grpc.Net.Client.GrpcChannel.ForAddress("http://" + firstHost + ":" + firstPort);
            var worker = new DIDAWorkerServerService.DIDAWorkerServerServiceClient(channel);
            worker.workAsync(megaRequest);

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
                Services = { DIDASchedulerServerService.BindService(new SchedulerServerService(WorkersList)) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("Server started on port " + port);
          
            Console.ReadKey();
            server.ShutdownAsync().Wait();
        }
    }
        struct workerStruct
        {
            public string name, url;
        }
}
