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
            while (current != null)
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

        public static Page FindLeafPage(Node value, Page page)
        {
            if (page.IsLeaf)
                return page;

            var currentNode = page.Head;
            var compared = string.Compare(value.Key, currentNode.Key);
            if (compared < 0)
                return FindLeafPage(value, currentNode.NextLess);

            while (currentNode.Next != null)
            {
                currentNode = currentNode.Next;
                if (string.Compare(value.Key, currentNode.Key) < 0)
                    return FindLeafPage(value, currentNode.NextLess);
            }

            return currentNode.NextGreater;
        }

        public static Page FindPage(string value, string page)
        {
            throw new NotSupportedException("");
        }
    }
}
