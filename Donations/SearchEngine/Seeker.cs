using Donations.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Donations.SearchEngine
{
    public class Seeker
    {
        public static BinaryFormatter binFormatter = new BinaryFormatter();        

        public static Node FindNode(string id)
        {
            var page = FindPage(id, Page.RootPagePath);
            var current = page.Head;
            while(current != null)
            {
                if (current.Key == id)
                    return current;

                current = current.Next;
            }

            throw new ArgumentException($"Index doesn't not contain value {id}");
        }

        public static Node Find(string id, string fileName)
        {
            Page page = null;
            var binFormatter = new BinaryFormatter();
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                page = (Page)binFormatter.Deserialize(fs);
            }

            Node node = null;
            return node;
        }

        public static Page FindPage(string value, string rootPagePath)
        {            
            using (var fs = new FileStream(rootPagePath, FileMode.Open))
            {
                var page = (Page)binFormatter.Deserialize(fs);

                if (page.Count == 0)
                    return page;
                
                var currentNode = page.Head;
                if (currentNode.NextGreater == null && currentNode.NextLess == null)
                    return page;

                var compared = string.Compare(value, currentNode.Key);
                if (compared < 0)
                    return FindPage(value, currentNode.NextLess.FileName);

                while (currentNode.Next != null)
                {
                    var compared1 = string.Compare(value, currentNode.Key);                    
                    var compared2 = string.Compare(value, currentNode.Next.Key);
                    if (compared1 >= 0 && compared2 < 0)
                    {
                        if (currentNode.NextGreater != null)
                            return FindPage(value, currentNode.NextGreater.FileName);
                        else
                            return page;
                    }

                    currentNode = currentNode.Next;
                }

                return FindPage(value, currentNode.NextGreater.FileName);
            }            
        }
    }
}
