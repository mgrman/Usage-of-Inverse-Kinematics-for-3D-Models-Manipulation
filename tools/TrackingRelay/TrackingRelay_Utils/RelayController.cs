using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingRelay_Utils
{
    /// <summary>
    /// Controler which starts with program and sets itself according to commandline args
    /// </summary>
    /// <typeparam name="CONTYPE">Type of IConnection used </typeparam>
    public class RelayController<CONTYPE> : IServer where CONTYPE : IConnection, new()
    {
        IServer _server;
        public RelayController(string[] args)
        {
            if (!args.Where(o => o.ToLower() == "noconsole").Any())
            {
                ConsoleUtils.ShowConsole();
            }

            if (args.Where(o => o.ToLower() == "serverrelay").Any())
            {
                int pars;
                int port = args
                    .Where(o => Int32.TryParse(o, out pars))
                    .Select(o => Int32.Parse(o))
                    .DefaultIfEmpty(4242)
                    .First();

                _server = new SocketController<CONTYPE>(port) as IServer;
                return;
            }



            Console.WriteLine("Do you want TCP relay [t]?");
            var c = Char.ToLower(Console.ReadKey().KeyChar);

            Console.WriteLine();

            if (c == 't')
            {
                Console.WriteLine("Input Port:");
                int port;
                while(!Int32.TryParse(Console.ReadLine().Trim(),out port))
                {   }

                _server = new SocketController<CONTYPE>(port) as IServer;
            }

        }

        public void RunServerLoop()
        {
            _server.RunServerLoop();
        }
    }
}

