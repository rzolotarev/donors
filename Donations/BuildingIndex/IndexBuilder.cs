using Donations.Nodes;
using Donations.SearchEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Donations.BuildingIndex
{
    public class IndexBuilder
    {
        public void Build(string donorsFileName)
        {
            var rootPage = new Page();
            Page.RootPagePath = rootPage.FileName;
            rootPage.Save();            
            using (var fs = new FileStream(donorsFileName, FileMode.Open))
            {
                long position = 0;
                using (var sr = new StreamReader(fs))
                {
                    var line = new StringBuilder();
                    while (true)
                    {
                        var resp = fs.ReadByte();
                        if (resp == -1)
                            break;

                        char c = (char)resp;
                        if (c == '\n')
                        {
                            var splitted = line.ToString().Split(',');
                            var currentNode = new Node() { Value = splitted[0].Last().ToString(), Offset = position, Length = line.Length - splitted[0].Length + 1};
                            var page = Seeker.FindPage(splitted[0], Page.RootPagePath);
                            page.Add(currentNode);                            
                            line.Clear();
                            position = fs.Position;
                        }
                        else
                        {
                            line.Append(c);                            
                        }
                    }                                                                                            
                }
            }                        
        }
    }
}
