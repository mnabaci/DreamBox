using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
    public class ImageRead
    {
        public ImageCollection CurrentImage { get; private set; }
        string _path;
        int _count;
        public ImageRead(string path) { this._path = path; _count = 0; }
        public bool GetImage()
        {
            try
            {
                
                System.IO.StreamWriter file =
                    new System.IO.StreamWriter("test.txt", true);
                file.WriteLine(_path + "/" + _count + "/" + "bg.png");
                file.Close();
                if (Directory.Exists(_path + "/" + _count))
                {
                    CurrentImage = new ImageCollection();
                    CurrentImage.BackgroundImage = new Bitmap(Image.FromFile(_path + "/" + _count + "/" + "bg.png"));
                    CurrentImage.ButtonImage = new Bitmap(Image.FromFile(_path + "/" + _count + "/" + "fm.png"));
                    CurrentImage.ForeImage = new Bitmap(Image.FromFile(_path + "/" + _count + "/" + "fi.png"));
                }
                else return false;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            _count++; 
            return true;
        }
    }
}
