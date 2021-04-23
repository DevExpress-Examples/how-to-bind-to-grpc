using System;
using Grpc.Core;
using IssuesData;

namespace IssuesServer {
    class Program {
        const int Port = 30051;

        public static void Main(string[] args) {
            Server server = new Server {
                Services = {
                    IssuesService.BindService(new IssuesServiceImpl())
                },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Issues server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
