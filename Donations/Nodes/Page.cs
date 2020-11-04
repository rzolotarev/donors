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
        public int PageCapacity = 4; // for comport splitting it's useful to have even number of nodes
        public bool IsLeaf { get; set; }
        public Page ParentPage { get; set; }

        public static string RootPagePath { get; set; }        

        public Page()
        {
            var guid = Guid.NewGuid();
            FileName = guid.ToString();
            Head = new Node();
            Count = 0;
        }

        public void MarkPageForNodes(Node node)
        {
            while(node != null)
            {
                node.OwningPage = this;
                node = node.Next;
            }
        }

        private void SimpleAdd(Node node)
        {
            node.OwningPage = this;
            if (Head.Next == null)
            {
                Head.Next = node;
                node.Prev = Head;
                return;
            }

            var current = Head;
            while(current.Next != null)
            {
                current = current.Next;
                if(string.Compare(current.Key, node.Key) < 0)
                {
                    node.Next = current;
                    node.Prev = current.Prev;
                    current.Prev = node;
                    return;
                }                
            }

            node.Prev = current;
            current.Next = node;
        }

        private void AddWithSplitting(Node node)
        {
            SimpleAdd(node);
            var edge = PageCapacity / 2;

            var current = this.Head;
            for(var i = 0; i <= edge; i++)
                current = current.Next;

            var newPage = new Page();
            newPage.Add(current.Next);
            newPage.MarkPageForNodes(current.Next);

            node.NextLess = node.OwningPage;
            node.NextGreater = newPage;
            if (ParentPage == null)
            {
                ParentPage = new Page();
                node.OwningPage = ParentPage;
                node.NextLess = this;
                node.NextGreater = newPage;
                return;
            }

            ParentPage.Add(current);            
        }
        
        public void Add(Node node)
        {
            if (Count == PageCapacity)
                AddWithSplitting(node);
            else
                SimpleAdd(node);            
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
