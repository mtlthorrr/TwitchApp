using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

using Newtonsoft.Json;
namespace TwitchAppV2
{
    public class Settings  : ObservableObject
    {
        [XmlArrayItem("StreamCommand")]
        public ObservableCollection<String> LaunchStreamCommandList;
    }
}
