using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using System.IO;
namespace ImageProcessing
{
    public enum Position
    {
        TopLeft,
        TopCenter,
        TopRight,
        CenterLeft,
        Center,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    public class GreenBox
    {
        ImageCollection Image { get; set; }
        QRCodeEncoder encoder;
        public Bitmap BRForeImage
        {
            get
            {
                return RemoveGreenBackground(Image.PersonImage);
            }
        }
        public Bitmap CImage
        {
            get
            {
                return CombineImages();
            }
        }
        Bitmap ResultImage { get; set; }
        public Position Position { get; set; }
        public int Hue { get; set; }
        float saturation;
        public float Saturation {
            get { return saturation; } 
            set {
                if (value > 1)
                    saturation = 1;
                else if (value < 0)
                    saturation = 0;
                else
                    saturation = value;
            } 
        }
        float luminance;
        public float Luminance {
            get { return luminance; }
            set
            {
                if (value > 1)
                    luminance = 1;
                else if (value < 0)
                    luminance = 0;
                else
                    luminance = value;
            } 
        }
        public GreenBox(Bitmap background,Bitmap personImage, Bitmap foreImage, Position position = ImageProcessing.Position.BottomLeft)
        {
            encoder = new QRCodeEncoder();
            Image.BackgroundImage = background;
            Image.ButtonImage = personImage;
            Image.ForeImage = foreImage;
            Position = position;
            Hue = 0;
            Saturation = 0;
            Luminance = 0;
        }
        public GreenBox(ImageCollection image, Position position = ImageProcessing.Position.BottomLeft)
        {
            encoder = new QRCodeEncoder();
            Image = image;
            Position = position;
            Hue = 0;
            Saturation = 0;
            Luminance = 0;
        }
        Bitmap RemoveGreenBackground(Bitmap image)
        {
            AForge.Imaging.Filters.HSLFiltering filter = new AForge.Imaging.Filters.HSLFiltering();
            if (Hue == 0)
                filter.UpdateHue = false;
            else
                filter.Hue = new AForge.IntRange(Hue, 0);
            if (Saturation == 0)
                filter.UpdateSaturation = false;
            else
                filter.Saturation = new AForge.Range((float)Saturation, 1);
            if (Luminance == 0)
                filter.UpdateLuminance = false;
            else
                filter.Luminance = new AForge.Range(Luminance, 1);
            //image = ResizeBitmap(image, 1620, 1080);
            Bitmap bmp = new Bitmap(image);
            filter.ApplyInPlace(bmp);
            Bitmap output = new Bitmap(bmp.Width, bmp.Height);
            // Iterate over all piels from top to bottom...
            for (int y = 0; y < output.Height; y++)
            {
                // ...and from left to right
                for (int x = 0; x < output.Width; x++)
                {
                    // Determine the pixel color
                    Color camColor = bmp.GetPixel(x, y);

                    // Every component (red, green, and blue) can have a value from 0 to 255, so determine the extremes
                    byte max = Math.Max(Math.Max(camColor.R, camColor.G), camColor.B);
                    byte min = Math.Min(Math.Min(camColor.R, camColor.G), camColor.B);

                    // Should the pixel be masked/replaced?
                    bool replace =
                        camColor.G != min // green is not the smallest value
                        && (camColor.G == max // green is the biggest value
                        || max - camColor.G < 2) // or at least almost the biggest value
                        && (max - min) > 40; // minimum difference between smallest/biggest value (avoid grays)

                    if (replace)
                        camColor = Color.Transparent;

                    // Set the output pixel
                    output.SetPixel(x, y, camColor);
                }
            }
            output = TrimBitmap(output);
            return output;
        }
        
        Bitmap CombineImages()
        {
            Bitmap bitmapResult = new Bitmap(Image.BackgroundImage.Width, Image.BackgroundImage.Height, Image.BackgroundImage.PixelFormat);
            Graphics g = Graphics.FromImage(bitmapResult);
            g.DrawImage(Image.BackgroundImage, 0, 0, Image.BackgroundImage.Width, Image.BackgroundImage.Height);
            Bitmap br = BRForeImage;
            br = ScaleImage(br, Math.Floor(bitmapResult.Width * 0.75), Math.Floor(bitmapResult.Height * 0.6));
            switch (Position)
            {
                case ImageProcessing.Position.TopLeft:
                    g.DrawImage(br, 0 + bitmapResult.Width/5, 0, br.Width, br.Height);
                    break;
                case ImageProcessing.Position.TopCenter:
                    g.DrawImage(br, bitmapResult.Width / 2 - br.Width / 2, 0, br.Width, br.Height);
                    break;
                case ImageProcessing.Position.TopRight:
                    g.DrawImage(br, bitmapResult.Width - br.Width - bitmapResult.Width / 5, 0, br.Width, br.Height);
                    break;
                case ImageProcessing.Position.CenterLeft:
                    g.DrawImage(br, 0 + bitmapResult.Width/5, bitmapResult.Height / 2 - br.Height / 2, br.Width, br.Height);
                    break;
                case ImageProcessing.Position.Center:
                    g.DrawImage(br, bitmapResult.Width / 2 - br.Width / 2, bitmapResult.Height / 2 - br.Height / 2, br.Width, br.Height);
                    break;
                case ImageProcessing.Position.CenterRight:
                    g.DrawImage(br, bitmapResult.Width - br.Width - bitmapResult.Width / 5, bitmapResult.Height / 2 - br.Height / 2, br.Width, br.Height);
                    break;
                case ImageProcessing.Position.BottomLeft:
                    g.DrawImage(br, 0 + bitmapResult.Width/5, bitmapResult.Height - br.Height - bitmapResult.Height/20, br.Width, br.Height);
                    break;
                case ImageProcessing.Position.BottomCenter:
                    g.DrawImage(br, bitmapResult.Width / 2 - br.Width / 2, bitmapResult.Height - br.Height, br.Width, br.Height);
                    break;
                case ImageProcessing.Position.BottomRight:
                    g.DrawImage(br, bitmapResult.Width - br.Width - bitmapResult.Width / 5, bitmapResult.Height - br.Height, br.Width, br.Height);
                    break;
            }
            if(Image.ForeImage != null)
                g.DrawImage(Image.ForeImage,0,bitmapResult.Height-Image.ForeImage.Height,Image.ForeImage.Width,Image.ForeImage.Height);
            Guid guid;
            string generatedID = generateID(Settings.Instance.PhotoURL,out guid);
            Bitmap QRCode = encoder.Encode(generatedID);
            if (!Directory.Exists(@"output/combined"))
                Directory.CreateDirectory(@"output/combined");
            if (!Directory.Exists(@"output/raw"))
                Directory.CreateDirectory(@"output/raw");
            g.DrawImage(QRCode, bitmapResult.Width - 96 - bitmapResult.Width / 10, bitmapResult.Height - 96 - bitmapResult.Height / 15 , 96, 96);
            string sFormat = GetImageFormatName(ImageFormat.Jpeg);
            bitmapResult.Save(string.Format(@"output/combined/{0:N}.{1}", guid,sFormat), ImageFormat.Jpeg);
            Image.PersonImage.Save(string.Format(@"output/raw/{0:N}.{1}", guid, sFormat), ImageFormat.Jpeg);
            SQL sql = new SQL();
            while (!sql.AddImage(string.Format("{0:N}", guid), sFormat)) ;
            return bitmapResult;
        }
        private static readonly Dictionary<Guid, string> _knownImageFormats =
             (from p in typeof(ImageFormat).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
              where p.PropertyType == typeof(ImageFormat)
              let value = (ImageFormat)p.GetValue(null, null)
              select new { Guid = value.Guid, Name = value.ToString() })
             .ToDictionary(p => p.Guid, p => p.Name);

        private static string GetImageFormatName(ImageFormat format)
        {
            string name;
            if (_knownImageFormats.TryGetValue(format.Guid, out name))
                return name;
            return null;
        }
        public string generateID(string sourceUrl,out Guid id)
        {
            id =Guid.NewGuid();
            return string.Format("{0}?id={1:N}", sourceUrl, id);
        }
        static public Bitmap ScaleImage(Image image, double maxWidth, double maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            Bitmap bmp = new Bitmap(newImage);

            return bmp;
        }
        static Bitmap TrimBitmap(Bitmap source)
        {
            Rectangle srcRect = default(Rectangle);
            BitmapData data = null;
            try
            {
                data = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                byte[] buffer = new byte[data.Height * data.Stride];
                System.Runtime.InteropServices.Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
                int xMin = int.MaxValue;
                int xMax = 0;
                int yMin = int.MaxValue;
                int yMax = 0;
                for (int y = 0; y < data.Height; y++)
                {
                    for (int x = 0; x < data.Width; x++)
                    {
                        byte alpha = buffer[y * data.Stride + 4 * x + 3];
                        if (alpha != 0)
                        {
                            if (x < xMin) xMin = x;
                            if (x > xMax) xMax = x;
                            if (y < yMin) yMin = y;
                            if (y > yMax) yMax = y;
                        }
                    }
                }
                if (xMax < xMin || yMax < yMin)
                {
                    // Image is empty...
                    return null;
                }
                srcRect = Rectangle.FromLTRB(xMin, yMin, xMax, yMax);
            }
            finally
            {
                if (data != null)
                    source.UnlockBits(data);
            }

            Bitmap dest = new Bitmap(srcRect.Width, srcRect.Height);
            Rectangle destRect = new Rectangle(0, 0, srcRect.Width, srcRect.Height);
            using (Graphics graphics = Graphics.FromImage(dest))
            {
                graphics.DrawImage(source, destRect, srcRect, GraphicsUnit.Pixel);
            }
            return dest;
        }
        private static Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(sourceBMP, 0, 0, width, height);
            return result;
        }
    }
}
