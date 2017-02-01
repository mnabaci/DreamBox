using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
    class NoItemFoundException : Exception
    {

    }
    public class ImageCollection
    {
        public Bitmap ButtonImage { get; set; }
        public Bitmap BackgroundImage {get;set;}
        public Bitmap ForeImage { get; set; }
        public Bitmap PersonImage { get; set; }
        public ImageCollection() { }
        public ImageCollection(Bitmap buttonImage, Bitmap backgroundImage, Bitmap foreImage,Bitmap personImage= null) 
        { 
            ButtonImage = buttonImage; 
            BackgroundImage = backgroundImage;
            ForeImage = foreImage;
            PersonImage = personImage;
        }
    }
    public class ImageList
    {
        public List<ImageCollection> Collection { get; private set; }
        int startIndex;
        public int[] CurrentItemIndexes { get { return GetCurrentItemsIndexes(); } }
        public ImageCollection[] CurrentItems { get { return GetCurrentItems(); } }
        int viewCount;
        public int ViewCount { get { return viewCount; } }
        public ImageList(int viewCount)
        {
            Collection = new List<ImageCollection>();
            startIndex = 0;
            this.viewCount = viewCount < 1 ? 1:viewCount;
        }
        public void Add(ImageCollection collection)  
        {
            Collection.Add(collection);
        }
        public ImageCollection[] Next()
        {
            if (Collection.Count == 0) throw new NoItemFoundException();
            IncreaseIndex();
            ImageCollection[] collection = new ImageCollection[viewCount];
            int[] indexes = GetCurrentItemsIndexes();
            for (int i = 0; i < viewCount; i++)
                collection[i] = Collection[indexes[i]];
            return collection;

        }
        public ImageCollection[] Previous()
        {
            if (Collection.Count == 0) throw new NoItemFoundException();
            DecreaseIndex();
            ImageCollection[] collection = new ImageCollection[viewCount];
            int[] indexes = GetCurrentItemsIndexes();
            for (int i = 0; i < viewCount; i++)
                collection[i] = Collection[indexes[i]];
            return collection;
        }
        void IncreaseIndex()
        {
            if ((startIndex + 1) >= Collection.Count)
                startIndex = 0;
            else
                startIndex++;
        }
        void DecreaseIndex()
        {
            if ((startIndex - 1) <= 0)
                startIndex = Collection.Count == 0 ? 0 : Collection.Count-1;
            else
                startIndex--;
        }
        int[] GetCurrentItemsIndexes()
        {
            if (Collection.Count == 0) throw new NoItemFoundException();
            int[] list = new int[viewCount];
            switch (Collection.Count)
            {
                case 0:
                    for (int i = 0; i < viewCount; i++)
                        list[i] = 0;
                    break;
                default:
                    list[0] = startIndex;
                    for(int i=1;i<viewCount; i++)
                    list[i] = ((list[i-1] + 1) >= Collection.Count) ? 0 : list[i-1] + 1;
                    break;
            }
            return list;
        }
        ImageCollection[] GetCurrentItems()
        {
            if (Collection.Count == 0) throw new NoItemFoundException();
            int[] indexes = GetCurrentItemsIndexes();
            ImageCollection[] collection = new ImageCollection[viewCount];
            for (int i = 0; i < viewCount; i++)
                collection[i] = Collection[indexes[i]];
            return collection;
        }

    }
}
