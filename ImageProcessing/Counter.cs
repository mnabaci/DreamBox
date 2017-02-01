using ImageProcessing.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ImageProcessing
{
    public struct CounterItem{
        public Bitmap Number {get;set;}
        public Bitmap Smile {get;set;}
        public int Count {get;set;}
    }
    public class Counter
    {
        Bitmap[] numbers = new Bitmap[10];
        Bitmap smile;
        CounterItem cItem;
        public Bitmap[] Numbers { get { return numbers; } }
        public Bitmap Smile { get { return smile; } }
        int count;
        public Counter()
        {
            numbers[0] = Resources._0;
            numbers[1] = Resources._1;
            numbers[2] = Resources._2;
            numbers[3] = Resources._3;
            numbers[4] = Resources._4;
            numbers[5] = Resources._5;
            numbers[6] = Resources._6;
            numbers[7] = Resources._7;
            numbers[8] = Resources._8;
            numbers[9] = Resources._9;
            smile = Resources.smile;
            cItem = new CounterItem();
            count = 7;
        }
        public CounterItem CountDown()
        {
            count = (count <= 0) ? -1 : count - 1;
            cItem.Number = numbers[count==-1 ? 0 : count];
            cItem.Smile = smile;
            cItem.Count = count;
            return cItem;
        }
    }
}
