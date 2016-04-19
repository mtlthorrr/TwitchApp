using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;



namespace TwitchAppV2
{
    public class ChannelModel : ObservableObject
    {
        public enum ChannelService
        {
            Twitch,
            MLGTV
        }
        
        #region Fields

        private string _name;
        private string _displayName;
        private bool _online;
        private string _game;
        private TwitchChannelToken _channelToken;
        private BitmapImage _channelPreviewImage;
        private string _requestError;
        private ChannelService _channelType;

        public ObservableCollection<Stream> Streams { get; set; }
        

        #endregion Fields

        #region Properties;

        public string Name
        {
            get { return _name; }

            set
            {
                if (value != _name)
                {
                    _name = value;
                    RaisePropertyChangedEvent("Name");
                }
            }
        }
        
        public string DisplayName
        {
            get { return _displayName; }

            set
            {
                if (value != _displayName)
                {
                    _displayName = value;
                    RaisePropertyChangedEvent("DisplayName");
                }
            }
        }

        public bool Online
        {
            get { return _online; }

            set
            {
                if (value != _online)
                {
                    _online = value;
                    RaisePropertyChangedEvent("Online");
                }
            }
        }

        public string Game
        {
            get { return _game; }

            set
            {
                if (value != _game)
                {
                    _game = value;
                    RaisePropertyChangedEvent("Game");
                }
            }
        }

        public TwitchChannelToken ChannelToken
        {
            get { return _channelToken; }

            set
            {
                if (value != _channelToken )
                {
                    _channelToken = value;
                    RaisePropertyChangedEvent("ChannelProperty");
                }
            }
        }

        public BitmapImage ChannelPreviewImage
        {
            get { return _channelPreviewImage; }

            set
            {
                if (value != _channelPreviewImage)
                {
                    _channelPreviewImage = value;
                    RaisePropertyChangedEvent("ChannelPreviewImage");
                }
            }
        }

        public string RequestError
        {
            get { return _requestError; }
            set
            {
                if (value != _requestError)
                {
                    _requestError = value;
                    RaisePropertyChangedEvent("RequestError");
                }
            }
        }

        public ChannelService ChannelType
        {
            get { return _channelType; }
            set 
            {
                if (value != _channelType)
                {
                    _channelType = value;
                    RaisePropertyChangedEvent("ChannelType");
                }

            }
        }

        #endregion


    }
}
