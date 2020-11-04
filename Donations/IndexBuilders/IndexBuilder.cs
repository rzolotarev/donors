using Donations.Nodes;
using Donations.SearchEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Donations.IndexBuilders
{
    public class IndexBuilder
    {
        public void Build(string donorsFileName)
        {
            var rootPage = new Page();
            rootPage.IsLeaf = true;
            Page.RootPagePath = rootPage.FileName;
            rootPage.Save();

            using (var fs = new FileStream(donorsFileName, FileMode.Open))
            {                
                using (var sr = new StreamReader(fs))
                {
                    var line = new StringBuilder();
                    long position = 0;
                    while(!sr.EndOfStream)
                    {
                        position = sr.BaseStream.Position;

                        line.Append(sr.ReadLine());
                        var splitted = line.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                        var newNode = new Node() { Key = splitted[0], Position = position };

                        var page = Seeker.FindLeafPage(newNode, rootPage);
                        page.Add(newNode);
                        line.Clear();
                    }
                }
            }                
        }
    }
}
