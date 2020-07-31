using Donations.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donations
{
    [Serializable]
    public class Node
    {
        public string Value { get; set; }
        public Node Next { get; set; }
        public Page NextLess { get; set; }
        public Page NextGreater { get; set; }
        public Page Page { get; set; }
        public long Offset { get; set; }
        public long Length { get; set; }
    }
}
