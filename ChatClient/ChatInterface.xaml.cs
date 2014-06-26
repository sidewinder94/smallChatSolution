using ChatDllContracts;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// Logique d'interaction pour ChatInterface.xaml
    /// </summary>
    public partial class ChatInterface : UserControl
    {
        public Client pair { get; private set; }
        private MainWindow parent;
        public ChatInterface(MainWindow parent, Client pair)
        {
            InitializeComponent();
            this.parent = parent;
            this.pair = pair;
        }

        public void ReceivedMessage(Message msg)
        {
            var receivedMessageLabel = new Label();
            Thickness margin = new Thickness(20, 0, 0, 0);
            receivedMessageLabel.Margin = margin;
            receivedMessageLabel.Content = msg;
            receivedMessageLabel.Background = Brushes.LightYellow;
            this.messageList.Children.Add(receivedMessageLabel);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var message = new Message();
            message.Body = this.Message.Text;
            message.Sender = this.parent.me;
            message.Receiver = this.pair;
            this.parent.service.SendMessageToClient(message);
            var sentMessageLabel = new Label();
            sentMessageLabel.Content = message;
            sentMessageLabel.Background = Brushes.AliceBlue;
            this.messageList.Children.Add(sentMessageLabel);
            this.Message.Text = String.Empty;
        }
    }
}
