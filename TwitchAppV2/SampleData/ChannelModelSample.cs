using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections.ObjectModel;



using TwitchAppV2;


namespace TwitchAppV2.SampleData
{
    public class ChannelModelSample
    {   
        public BitmapImage offlineImage { get; set; }
        public ChannelModel channel { get; set; }
        public ObservableCollection<ChannelModel> _channels { get; set; }

        public ChannelModelSample()
        {

            _channels = new ObservableCollection<ChannelModel>();
            
            channel = new ChannelModel();
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

            channel.ChannelPreviewImage = offlineImage;
            channel.Name = "Thorrr's Channel";
            channel.StatusTitle = "Thorrr's Streaming Some shit!!!!!";
            channel.Streams = new ObservableCollection<Stream>();
            Stream stream1 = new Stream();
            stream1.Quality = "4765k - 1920x1080";
            channel.Streams.Add(stream1);

            _channels.Add(channel);
            _channels.Add(channel);

        }
    }
}
