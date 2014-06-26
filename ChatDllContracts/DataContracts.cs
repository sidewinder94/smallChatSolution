using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChatDllContracts
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public String Body;
        [DataMember]
        public Client Sender;
        [DataMember]
        public Client Receiver;

        public override string ToString()
        {
            return Sender.Name + ": \n" + Body;
        }
    }
    [DataContract]
    public class Client
    {
        [DataMember]
        public String Name;
        [DataMember]
        public String id;


        public Client() { }
        public Client(string name, string id)
        {
            this.Name = name;
            this.id = id;
        }

        public override bool Equals(object obj)
        {
            Client newObj = obj as Client;
            if (newObj == null)
            {
                return false;
            }
            else
            {
                if (this.id == newObj.id &&
                   this.Name == newObj.Name)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
