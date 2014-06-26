using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatDllContracts;
using System.ServiceModel;
using System.Collections;

namespace ChatDll
{
    public class Service : IService
    {
        public static readonly Hashtable CallbackChannels = new Hashtable();
        public static readonly List<Client> Clients = new List<Client>();
        public static readonly Random rdm = new Random(DateTime.Now.Millisecond);
        public List<Client> SubscribeClient(Client newClient)
        {
            Console.WriteLine("Tried to connect : {0}", newClient);
            var channel = OperationContext.Current.GetCallbackChannel<IClientCallback>();

            newClient.id = rdm.Next().ToString();
            bool alreadyUsed = true;
            while (alreadyUsed)
            {
                channel.NotifyClientID(newClient.id);
                int count = Clients.Where(x => x.id == newClient.id).Count();
                if (count == 0)
                {
                    alreadyUsed = false;
                    break;
                }
            }
            if (!Clients.Contains(newClient))
            {
                Clients.Add(newClient);
            }

            foreach (IClientCallback callback in CallbackChannels.Values.Cast<IClientCallback>())
            {
                callback.NotifyNewClients(Clients);
            }

            if (!CallbackChannels.Contains(channel))
            {
                CallbackChannels.Add(newClient.id, channel);
            }

            Console.WriteLine("Connected : {0}", newClient);

            return Clients;
        }

        public void SendMessageToClient(Message message)
        {
            IClientCallback channel = CallbackChannels[message.Receiver.id] as IClientCallback;
            if (channel != null)
            {
                Console.WriteLine("{0} sent {1} to {2}", message.Sender,
                                                         message.Body,
                                                         message.Receiver);
                channel.ReceiveMessage(message);
            }
        }


        public void UnsubsribeClient(Client client)
        {
            if (Clients.Contains(client))
            {
                Clients.Remove(client);
                CallbackChannels.Remove(client.id);
                Console.WriteLine("{0} disconnected", client);

            }
            foreach (IClientCallback callback in CallbackChannels.Values.Cast<IClientCallback>())
            {
                callback.NotifyNewClients(Clients);
            }
        }
    }
}
