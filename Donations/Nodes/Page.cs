using Donations.SearchEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Donations.Nodes
{
    [Serializable]
    public class Page
    {
        public int Count { get; set; }
        public Node Head { get; set; }
        public Node Parent { get; set; }
        public string FileName { get; set; }
        public int PageCapacity = 3;

        public static string RootPagePath { get; set; }        

        public Page(int count = 0)
        {
            var guid = Guid.NewGuid();
            FileName = guid.ToString();
            
            Count = count;
        }

        private void SimpleAdd(Node node, Page page)
        {
            node.Page = this;
            var current = page.Head;

            if (current == null)
            {
                Count++;
                page.Head = node;                
                return;
            }
            if (current.Next == null)
            {
                Count++;
                var compared = string.Compare(current.Key, node.Key);
                if (compared <= 0)
                {
                    page.Head.Next = node;
                }
                else
                {
                    node.Next = page.Head;
                    page.Head = node;
                }
                return;
            }
            while (current.Next != null)
            {
                var comparedLess = string.Compare(node.Key, current.Key);
                var comparedGreater = string.Compare(node.Key, current.Next.Key);
                if (comparedLess < 0 && current == page.Head)
                {
                    Count++;
                    page.Head = node;
                    node.Next = current;
                    return;
                }
                if (comparedLess >= 0 && comparedGreater <= 0)
                {
                    Count++;
                    node.Next = current.Next;
                    current.Next = node;
                    return;
                }

                current = current.Next;
            }

            current.Next = node;
        }

        private void AddWithSplitting(Node node)
        {
            var edge = Count % 2 == 0 ? Count / 2 : Count / 2 + 1;
            var counter = 0;
            var current = Head;
            Node prev = null;
            while (counter < edge)
            {                
                counter++;
                prev = current;
                current = current.Next;
            }

            var newPage = new Page(Count - edge);            
            newPage.Head = current;
            prev.Next = null;
            newPage.Save();

            Count = edge;

            // creating new node
            var parentNode = new Node() { Key = newPage.Head.Key };
            parentNode.NextLess = this;
            parentNode.NextGreater = newPage;

            // setting refs to the new parent node
            if (Parent == null)
            {
                Parent = parentNode;
            }
            
            if (newPage.Parent == null)
                newPage.Parent = parentNode;

            // added new node
            var compared = string.Compare(newPage.Head.Key, node.Key);
            if (compared > 0)
                SimpleAdd(node, this);
            else
                SimpleAdd(node, newPage);

            // adding parent node to the parent page
            if (Parent.Page == null)
            {                
                Parent.Page = new Page(1); // node - page
                RootPagePath = Parent.Page.FileName;
            }
            Parent.Page.Add(parentNode); // page - node
            
        }

        //TODO: set count to 500
        public void Add(Node node)
        {
            if (Count < PageCapacity)
                SimpleAdd(node, this);            
            else            
                AddWithSplitting(node);

            Save();
        }

        public static void AddNode(Node node)
        {
            var page = Seeker.FindPage(node.Key, RootPagePath);
            page.Add(node);
        }


        public void Save()
        {
            var binFormatter = new BinaryFormatter();
            using(var fs = new FileStream(FileName, FileMode.OpenOrCreate))
            {
                binFormatter.Serialize(fs, this);
            }            
        }

    }
}
