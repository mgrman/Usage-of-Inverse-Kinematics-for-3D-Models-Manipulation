using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingRelay_Utils
{

    /// <summary>
    /// Collection of messages that stores messages in collection of static containers, messages themselves are stored in another collection.
    /// This was in databinding when message is added or removed then bound collection itself isnt modified, only values in containers are changed.
    /// </summary>
    public class MessageCollection : IEnumerable<MessageContainer>
    {
        public List<MessageContainer> Containers { get; private set; }

        public List<string> Messages { get; private set; }

        public MessageCollection(int capacity)
        {
            Containers = new List<MessageContainer>(capacity);
            Messages = new List<string>(capacity);

            for (int i = 0; i < capacity; i++)
            {
                Messages.Add("");
                Containers.Add(new MessageContainer(this, i));
            }
        }

        public IEnumerator<MessageContainer> GetEnumerator()
        {
            return Containers.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Containers.GetEnumerator();
        }

        public string this[int i]
        {
            get
            {
                return Messages[i];
            }
        }

        public void AddMessage(string message)
        {
            Messages.RemoveAt(Messages.Count - 1);
            Messages.Insert(0, message);
        }

        public void UpdateContainers()
        {
            foreach (var cont in Containers)
            {
                cont.FirePropertyChanged();
            }
        }
    }


    public class MessageContainer : INotifyPropertyChanged
    {
        public MessageCollection ParentCol { get; private set; }

        public int MessageIndex { get; private set; }

        public string Message
        {
            get
            {
                return ParentCol[MessageIndex];
            }
        }

        public MessageContainer(MessageCollection parent, int messageIndex)
        {
            ParentCol = parent;
            MessageIndex = messageIndex;
        }

        public void FirePropertyChanged()
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs("Message"));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

}
