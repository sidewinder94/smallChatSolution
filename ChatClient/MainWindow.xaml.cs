using ChatDllContracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatClient
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    class Callback : IClientCallback
    {
        public delegate void UpdateArgs(List<Client> list);
        public delegate void ReceivedMessage(Message message);
        public event UpdateArgs updateEvent;
        public event ReceivedMessage receivedEvent;
        MainWindow parent;
        public Callback(MainWindow parent)
        {
            this.parent = parent;
        }
        public void ReceiveMessage(Message newMessage)
        {
            this.receivedEvent(newMessage);
        }

        public void NotifyNewClients(List<Client> clients)
        {
            this.updateEvent(clients);
        }

        public void NotifyClientID(string newID)
        {
            parent.me.id = newID;
        }

    }
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    ////[CallbackBehavior(UseSynchronizationContext = false)]
    public partial class MainWindow : Window
    {
        private DuplexChannelFactory<IService> channel;
        public IService service { get; private set; }
        public Client me { get; private set; }



        public MainWindow(String nick)
        {

            Callback callback = new Callback(this);

            #region callbacks implementation
            //Mise à jour de la liste des clients
            callback.updateEvent += updatedClientList;

            callback.receivedEvent += receivedMessage;

            //Réception d'un message


            #endregion
            InstanceContext context = new InstanceContext(this);
            channel = new DuplexChannelFactory<IService>(callback, "default");
            service = channel.CreateChannel();
            me = new Client(nick, null);


            InitializeComponent();

            this.clientListDisplay.ItemsSource = service.SubscribeClient(me)
                                                        .Where(x => !x.Equals(me))
                                                        .ToList();
            this.Title = nick;
        }

        private void updatedClientList(List<Client> list)
        {
            Action<List<Client>> action = delegate(List<Client> ClientList)
            {
                this.clientListDisplay.ItemsSource = ClientList.Where(x => !x.Equals(me));
            };
            Dispatcher.Invoke(action, list);
        }

        private void receivedMessage(Message message)
        {
            Action<Message> action = delegate(Message newMessage)
            {
                TabItem display = this.Tabs.Items.Cast<TabItem>()
                                                       .Where(x => x.Content.GetType() == typeof(ChatInterface))
                                                       .Where(x => ((ChatInterface)x.Content).pair.id == newMessage.Sender.id)
                                                       .FirstOrDefault();
                if (display != null)
                {
                    ((ChatInterface)display.Content).ReceivedMessage(newMessage);
                }
                else
                {
                    display = new TabItem();
                    display.Header = newMessage.Sender.Name;
                    var displayChat = new ChatInterface(this, newMessage.Sender);
                    display.Content = displayChat;
                    displayChat.ReceivedMessage(newMessage);
                    this.Tabs.Items.Add(display);
                }
                this.Tabs.SelectedItem = display;
            };

            Dispatcher.Invoke(action, message);
        }

        private void clientListDisplay_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (clientListDisplay.SelectedItem != null)
            {
                Client selected = clientListDisplay.SelectedItem as Client;
                TabItem display = this.Tabs.Items.Cast<TabItem>()
                                                  .Where(x => x.Content.GetType() == typeof(ChatInterface))
                                                  .Where(x => ((ChatInterface)x.Content).pair.id == selected.id)
                                                  .FirstOrDefault();

                if (display == null)
                {
                    var tab = new TabItem();
                    tab.Header = selected.Name;
                    var newDisplay = new ChatInterface(this, selected);
                    tab.Content = newDisplay;
                    this.Tabs.Items.Add(tab);
                    this.Tabs.SelectedItem = tab;
                }
                else
                {
                    this.Tabs.SelectedItem = display;
                }
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            service.UnsubsribeClient(me);
            channel.Close();
        }
    }
}
