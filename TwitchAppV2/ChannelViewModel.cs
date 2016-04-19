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
        private string _configFile;
        private XmlAttributeOverrides _overrides = new XmlAttributeOverrides();
        private XmlAttributes _attributesToOverride = new XmlAttributes();

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
            _configFile = String.Format("{0}\\TwitchAPP.xml", appPath);
            _configFile = @"C:\Users\thorrr\desktop\TwitchApp.xml"; // For dev set the location manually
            const string _imgbase64 = "/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAAkGBwgHBgkICAgKCgkLDhcPDg0NDhwUFREXIh4jIyEeICAlKjUtJScyKCAgLj8vMjc5PDw8JC1CRkE6RjU7PDn/2wBDAQoKCg4MDhsPDxs5JiAmOTk5OTk5OTk5OTk5OTk5OTk5OTk5OTk5OTk5OTk5OTk5OTk5OTk5OTk5OTk5OTk5OTn/wgARCADIAUADAREAAhEBAxEB/8QAGgABAQEBAQEBAAAAAAAAAAAAAAEGBQMHBP/EABQBAQAAAAAAAAAAAAAAAAAAAAD/2gAMAwEAAhADEAAAAPhoAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABqDlH0MwpojyMMbYz51T8Z5nEAAAAAAAAAAAAABvjlFOWQ0hlDbHGId85piAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACkAAAAAAAAAAAAAAAAAAAKACFIUgKACFIUEKAQpAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//xAAsEAABBAIBAwIEBwEAAAAAAAACAQMEBQYRABITIQcxFEFRgBAiQEJDYHFy/9oACAEBAAE/APvHrMHsptYxYvzKusiyd9gp8wGFe17qKLy7orChtVrbFlGX/CovWigQl7Eheyjy2xnHsRxaHJmQam6mOsI8ZrbkBL1FpOy2GusE5htEOR5FDrTlsRW3XBQzddQFUepE0G/c/onPUuHQUr51dZUQG3QdXUpi0OS5ofGnA9gJeelUOjur9iit6UJay1PUnvuATSC2Rewr+EylOJ6cJKSJSSdyWzObHldclnrHw2Y8xvHrHJZ5watoXZAsm90KWtoPLTALmtgSpbrkB44XmXGYlg69G/7FOUGJ2N7EfnNHDiQGTRs5c18WWuv5BtfnzIsZn0CRnJKxn4srasSoryOsu699En62a7RZTQUIu37FVMrIvwjzMphwt6XaGJAi89QbeBaWcBqreN+JW1zEAXzDo73b/eg8zW2g2cDGmob/AHThVYMP+FToNCLmJy2IGU002UfbjxpzDrp/QBNFXmUy2J+T3EyKfcjyJrzrR/USNVReek79RUZLFvLS6jQwiq4PYNp0jNCaId+B1y2hxoMzsxbFiwa0i95gTEf80YovKgKEcAm072UwGpc59mT5Yf01pPIroOenlxCpLSyfmSOyLlbIYaPz5cIfy8wm2g1sPJWJz/a+NqnGWPCr1u7FRHmB5JSMYZKorJa0JKT/AIxpbOI4/HPYIH8fkS5n90zOrq2BEk0JxWDcNGqqK+zpS15Lufcgn9e//8QAFBEBAAAAAAAAAAAAAAAAAAAAkP/aAAgBAgEBPwB6P//EABQRAQAAAAAAAAAAAAAAAAAAAJD/2gAIAQMBAT8Aej//2Q==";
            byte[] byteBuffer = Convert.FromBase64String(_imgbase64);
            using (MemoryStream _memStream = new MemoryStream(byteBuffer))
            {
                offlineImage = new BitmapImage();
                offlineImage.BeginInit();
                offlineImage.StreamSource = _memStream;
                offlineImage.CacheOption = BitmapCacheOption.OnLoad;
                offlineImage.EndInit();
                offlineImage.Freeze();
            }
            byteBuffer = null;

            
            _attributesToOverride.XmlIgnore = true;
            _overrides.Add(typeof(ChannelModel), "ChannelPreviewImage", _attributesToOverride);

            ReloadChannelConfig(null);
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

        public ICommand ReloadChannelConfigCommand
        {
            get { return new DelegateCommand<object>(ReloadChannelConfig);  }
        }
        
        private void ReloadChannelConfig(object paremeter)
        {
            if (File.Exists(_configFile))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(ObservableCollection<ChannelModel>), _overrides, null, new XmlRootAttribute("Channels"), null);
                using (TextReader textReader = new StreamReader(_configFile))

                    //XmlSerializer settingsDeserializer = new XmlSerializer(typeof(Settings), overrides, null, new XmlRootAttribute("Channels"), null);

                    try
                    {
                        _channels.Clear();
                        var channelBuffer = (ObservableCollection<ChannelModel>)deserializer.Deserialize(textReader);
                        foreach (ChannelModel channel in channelBuffer)
                        {
                            _channels.Add(channel);
                        }
                        //Settings = (Settings)settingsDeserializer.Deserialize(textReader);
                    }

                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(String.Format("Error parsing config file: {0}", ex.InnerException));
                    }

                foreach (ChannelModel channel in _channels)
                {
                    channel.Streams = new ObservableCollection<Stream>();
                }

                RefreshFavorites(null);
            }

            else
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<ChannelModel>), _overrides, null, new XmlRootAttribute("Channels"), null);
                TextWriter textWriter = new StreamWriter(_configFile);
                serializer.Serialize(textWriter, _channels);
                textWriter.Close();
            }
        }
    }
}
