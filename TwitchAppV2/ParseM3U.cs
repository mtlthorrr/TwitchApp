using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Windows; // For the error msg Boxes. Needs to be refactored..
using System.Collections.ObjectModel;


namespace TwitchAppV2
{
    class ParseM3U
    {
        private const string pattern = @"([A-Z\-]+)=(""(.+?)""|\d+\.\d+|\d+x\d+|\d+|[0-9A-z\-]+|0x[0-9A-z]+)";
        
        public void Parse(List<string> m3u, ObservableCollection<Stream> streams, string type = "")
        {
            bool incomingUrl = false;
            Regex rgx = new Regex(pattern);

            Stream workingStream = new Stream();

            if (m3u.First() != "#EXTM3U")
            {
                // Need better way to do this!
                MessageBox.Show("EXTM3U not properly formatted, unable to parse");
            }

            foreach (string line in m3u)
            {
                if (line.StartsWith("#EXTM3U"))
                {
                    continue;
                }

                else if (line.StartsWith("#EXT-X-MEDIA:"))
                {
                    var tagRemoved = line.Replace("#EXT-X-MEDIA:", "");
                    var matches = rgx.Matches(tagRemoved);

                    foreach (Match match in matches)
                    {
                        if (match.Groups[1].Value == "NAME")
                        {
                            workingStream.Quality = match.Groups[3].Value;
                        }
                    }
                }

                else if (line.StartsWith("#EXT-X-STREAM-INF"))
                {

                    incomingUrl = true;

                    var tagRemoved = line.Replace("#EXT-X-STREAM-INF", "");
                    var matches = rgx.Matches(tagRemoved);
                    int bw = 0;
                    string res = null;
                    foreach (Match match in matches)
                    {
                        if (match.Groups[1].Value == "BANDWIDTH")
                        {
                            bw = Convert.ToInt32(match.Groups[2].Value) / 1000;
                        }

                        if (match.Groups[1].Value == "RESOLUTION")
                        {
                            res = match.Groups[2].Value;
                        }
                    }

                    workingStream.Quality = String.Format("{0}k - {1}", bw, res);
                }

                else if (incomingUrl)
                {
                    workingStream.Url = line;
                    streams.Add(workingStream);

                    workingStream = new Stream();
                    incomingUrl = false;
                }
            }
        }
    }
}
