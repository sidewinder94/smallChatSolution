using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ChatDllContracts
{
    /// <summary>
    /// Callback type
    /// </summary>
    public interface IClientCallback
    {
        //Callback function
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(Message newMessage);

        //Callback function
        [OperationContract(IsOneWay = true)]
        void NotifyNewClients(List<Client> clients);

        //Callback function
        [OperationContract(IsOneWay = true)]
        void NotifyClientID(String newID);
    }


    [ServiceContract(CallbackContract = typeof(IClientCallback))]
    public interface IService
    {
        [OperationContract]
        List<Client> SubscribeClient(Client newClient);

        [OperationContract(IsOneWay = true)]
        void SendMessageToClient(Message message);

        [OperationContract(IsOneWay = true)]
        void UnsubsribeClient(Client client);

    }
}
