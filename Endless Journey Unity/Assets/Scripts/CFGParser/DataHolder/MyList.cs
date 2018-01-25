using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.CFGParser.DataHolder
{
    public class MyList<T>
    {
        LinkedList<T> collection;

        public MyList()
        {
            collection = new LinkedList<T>();
        }

        public T Last
        {
            get
            {
                return collection.Last.Value;
            }
        }

        public void Enqueue(T data)
        {
            lock (collection)
            {
                collection.AddLast(data);
            }
        }

        public T Dequeue()
        {
            lock (collection)
            {
                T result = collection.First.Value;
                collection.RemoveFirst();
                return result;
            }
        }
    }
}
