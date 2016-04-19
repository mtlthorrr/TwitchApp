using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;
using System.Windows; // For the error msg Boxes. Needs to be refactored..
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Diagnostics; // For Process




namespace TwitchAppV2
{
    public class ChannelViewModel : ObservableObject
    {
        #region Fields

        private string _selectedStream;
        private ChannelModel _selectedChannel;

        public ObservableCollection<ChannelModel> _channels {get; set; }

        Settings Settings = new Settings();
        TwitchLib TwitchLib = new TwitchLib("cv076dh7b6ejrsxj8cgahdaipdg15sc");
        MlgLib MlgLib = new MlgLib();
        BitmapImage offlineImage;
        

        #endregion

        #region Properties/Commands
        public string SelectedStream
        {
            get { return _selectedStream; }
            set
            {
                if (value != _selectedStream)
                {
                    _selectedStream = value;
                    RaisePropertyChangedEvent("SelectedStream");
                }
            }
        }

        public ChannelModel SelectedChannel
        {
            get { return _selectedChannel; }
            set
            {
                if (value != _selectedChannel)
                {
                    _selectedChannel = value;
                    RaisePropertyChangedEvent("SelectedChannel");
                }
            }
        }

        #endregion

        public ChannelViewModel()
        {

            _channels = new ObservableCollection<ChannelModel>();
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string configFile = String.Format("{0}\\TwitchAPP.xml", appPath);

            configFile = @"C:\Users\thorrr\desktop\TwitchApp.xml"; // For dev set the location manually
            //offlineImage = new BitmapImage(new Uri(@"C:\Users\thorrr\Documents\Visual Studio 2013\Projects\StreamWatcher\Offline.jpg"));
            offlineImage = new BitmapImage();

            XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            XmlAttributes attributesToOverride = new XmlAttributes();
            attributesToOverride.XmlIgnore = true;
            overrides.Add(typeof(ChannelModel), "ChannelPreviewImage", attributesToOverride);

            if (File.Exists(configFile))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(ObservableCollection<ChannelModel>), overrides, null, new XmlRootAttribute("Channels"), null);
                TextReader textReader = new StreamReader(configFile);

                //XmlSerializer settingsDeserializer = new XmlSerializer(typeof(Settings), overrides, null, new XmlRootAttribute("Channels"), null);

                try
                {
                    _channels = (ObservableCollection<ChannelModel>)deserializer.Deserialize(textReader);
                    int a = 1;
                    //Settings = (Settings)settingsDeserializer.Deserialize(textReader);
                }

                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(String.Format("Error parsing config file: {0}", ex.InnerException));
                }
                
                textReader.Close();
                foreach (ChannelModel channel in _channels)
                {
                    channel.Streams = new ObservableCollection<Stream>();
                }

                RefreshFavorites(null);
            }
            
            else
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<ChannelModel>), overrides, null, new XmlRootAttribute("Channels"), null);
                TextWriter textWriter = new StreamWriter(configFile);
                serializer.Serialize(textWriter, _channels);
                textWriter.Close();
            }
        }

        public ICommand RefreshFavoritesCommand
        {
            get { return new DelegateCommand<object>(RefreshFavorites);  }
        }

        private async void RefreshFavorites(object parameter)
        {
            
            foreach (ChannelModel channel in _channels)
            {

                
                switch (channel.ChannelType)
                {
                    case ChannelModel.ChannelService.Twitch:
                        try
                        {
                            var streamData = await TwitchLib.KrakenRequest(String.Format("streams/{0}", channel.Name));
                            channel.ChannelToken = await TwitchLib.GetChannelToken(channel.Name);

                            if (streamData["stream"].HasValues != false)
                            {
                                channel.ChannelPreviewImage = GetChannelPreview((string)streamData["stream"]["preview"]["medium"]);

                                //var channelAvailableStreams = await TwitchLib.UsherRequest(String.Format("select/{0}.json?nauthsig={1}&nauth={2}&allow_source=true", channel.Name, channel.ChannelToken._Sig, channel.ChannelToken._Token), "select");
                                var channelAvailableStreams = await TwitchLib.UsherRequest(String.Format("api/channel/hls/{0}.json?sig={1}&token={2}&allow_source=true", channel.Name, channel.ChannelToken._Sig, channel.ChannelToken._Token), "select");

                                List<string> m3u = new List<string>(Regex.Split(channelAvailableStreams, "\n"));

                                ParseM3U parser = new ParseM3U();
                                channel.Streams.Clear();
                                parser.Parse(m3u, channel.Streams);
                            }

                            else
                            {
                                channel.ChannelPreviewImage = offlineImage;
                            }
                        }

                        catch (HttpRequestException hre)
                        {
                            channel.RequestError = hre.Message;
                        }
                        break;


                    case ChannelModel.ChannelService.MLGTV:
                        try 
                        {
                           List<string> m3uList = await MlgLib.GetStreams(channel.Name);
                            ParseM3U parser = new ParseM3U();
                            channel.Streams.Clear();
                            parser.Parse(m3uList, channel.Streams, "mlg");
                            
                        }

                        catch (HttpRequestException hre)
                        {
                            channel.RequestError = hre.Message;
                        }

                        break;

                }
            }
        }

        public ICommand RunStreamCommand
        {
            get { return new DelegateCommand<ChannelModel>(RunStream); }
        }

        private void RunStream(ChannelModel parameter)
        {
            if (parameter.Streams.Count == 0)
            {
                MessageBox.Show("Channel has no active streams");
                return;
            }

            if (SelectedStream == null || SelectedStream == "")
            {
                MessageBox.Show("Please select a strem quality");
                return;
            }

            Process.Start(@"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe", SelectedStream);
        }

        private BitmapImage GetChannelPreview(string url)
        {
            var image = new BitmapImage();
            int BytesToRead = 100;

            try
            {
                WebRequest request = WebRequest.Create(new Uri(url, UriKind.Absolute));
                WebResponse response = request.GetResponse();
                var responseStream = response.GetResponseStream();
                BinaryReader reader = new BinaryReader(responseStream);
                MemoryStream memoryStream = new MemoryStream();

                byte[] bytebuffer = new byte[BytesToRead];
                int bytesRead = reader.Read(bytebuffer, 0, BytesToRead);

                while (bytesRead > 0)
                {
                    memoryStream.Write(bytebuffer, 0, bytesRead);
                    bytesRead = reader.Read(bytebuffer, 0, BytesToRead);
                }

                image.BeginInit();
                memoryStream.Seek(0, SeekOrigin.Begin);

                image.StreamSource = memoryStream;
                image.EndInit();
                response.Close();
            }

            catch (WebException wex)
            {

            }
            
            return image;
        }
    }
}
