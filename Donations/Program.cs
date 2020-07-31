using Donations.BuildingIndex;
using Donations.Nodes;
using Donations.SearchEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Donations
{
    class Program
    {
        static void Main(string[] args)
        {
            var indexBuilder = new IndexBuilder();
            indexBuilder.Build("Source.txt");

            var states = new Dictionary<string, int>();

            using(var fs = new FileStream("Donations.txt", FileMode.Open))
            {
                using(var sr = new StreamReader(fs))
                {                    
                    using(var donorReader = new StreamReader("Source.txt"))
                    {
                        while (!sr.EndOfStream)
                        {
                            var donation = sr.ReadLine().Split(',');                            
                            var node = Seeker.FindNode(donation[0]);
                            
                            donorReader.BaseStream.Position = node.Offset;
                            donorReader.DiscardBufferedData();

                            var buffer = new char[node.Length];
                            if (donorReader.Read(buffer, 0, buffer.Length) == -1)
                                break;

                            
                            var line = string.Concat(buffer);
                            var state = line.Split(',')[1].Trim();
                            if (!states.ContainsKey(state))
                                states[state] = 0;
                            states[state] += int.Parse(donation[1]);
                        }
                    }
                }                
            }

            foreach (var state in states)
                Console.WriteLine($"{state.Key} - {state.Value}");            
        }
    }
}
