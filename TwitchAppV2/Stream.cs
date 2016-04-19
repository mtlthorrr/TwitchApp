using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchAppV2
{
    public class Stream : ObservableObject
    {
        #region Fields

        private string _type;
        private string _provider;
        private string _previewImage; // Is this needed as we have this on the Channel..
        private string _url;
        private string _quality;

        #endregion

        #region Properties

        public string Type
        {
            get { return _type; }

            set
            {
                if (value != _type)
                {
                    _type = value;
                    RaisePropertyChangedEvent("Type");
                }
            }
        }

        public string Provider
        {
            get { return _provider; }

            set
            {
                if (value != _provider)
                {
                    _provider = value;
                    RaisePropertyChangedEvent("Provider");
                }
            }
        }

        public string PreviewImage
        {
            get { return _previewImage; }

            set
            {
                if (value != _previewImage)
                {
                    _previewImage = value;
                    RaisePropertyChangedEvent("PerviewImage");                }
            }
        }

        public string Url
        {
            get { return _url; }
            set
            {
                if (value != _url)
                {
                    _url = value;
                    RaisePropertyChangedEvent("Url");
                }
            }
        }

        public string Quality
        {
            get { return _quality; }
            set
            {
                if (value != _quality)
                {
                    _quality = value;
                    RaisePropertyChangedEvent("Quality");
                }
            }
        }

        #endregion Properties



    }

    public class HLS_Stream : ObservableObject
    {
        #region Fields
        #endregion

        #region Properties
        #endregion
    }
}
