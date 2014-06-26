using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(ChatDll.Service));
            host.Open();
            Console.WriteLine("Server Started");
            Console.ReadLine();
        }
    }
}
