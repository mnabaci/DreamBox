using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing.Printing;
using Nikon;
namespace ImageProcessing
{
    public partial class TakePhoto : Form
    {
        System.Timers.Timer timer;
        ImageCollection Image;
        Counter counter;
        GreenBox test;
        PrintDocument printDocument1;
        private NikonDevice device;
        System.Timers.Timer closingTimer;
        Bitmap printImage;
        Boolean ClosingTimerStarted = false;
        public TakePhoto(ImageCollection image, NikonDevice device)
        {
            InitializeComponent();
            Cursor.Hide();
            this.DoubleBuffered = true;

            CenterComponents();
            // Hook up device capture events
            this.device = device;
            device.ImageReady += new ImageReadyDelegate(device_ImageReady);
            device.CaptureComplete += new CaptureCompleteDelegate(device_CaptureComplete);

            counter = new Counter();
            Image = image;
            this.BackgroundImage = new Bitmap(Image.BackgroundImage);
            printImage = new Bitmap(Image.BackgroundImage);
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(CountDown);

            closingTimer = new System.Timers.Timer(6000);

            CounterItem cItem = counter.CountDown();
            pictureBox1.Image = cItem.Number;
            pictureBox2.Image = cItem.Smile;
            timer.Start();
        }
        void CenterComponents()
        {
            pictureBox1.Height = Height / 4;
            pictureBox2.Height = Height / 8;
            pictureBox3.Height = Height / 8;
            pictureBox1.Width = Width / 2;
            pictureBox2.Width = Width / 2;
            pictureBox3.Width = Width / 2;
            pictureBox3.Top = Height/15;
            pictureBox1.Top = pictureBox3.Top+pictureBox3.Height +Height/6;
            pictureBox2.Top = pictureBox1.Top + pictureBox1.Height;
            pictureBox1.Left = (ClientSize.Width - pictureBox1.Width) / 2;
            pictureBox2.Left = (ClientSize.Width - pictureBox1.Width) / 2;
            pictureBox3.Left = (ClientSize.Width - pictureBox1.Width) / 2;
        }
        void device_ImageReady(NikonDevice sender, NikonImage image)
        {
            if (ClosingTimerStarted)
            {
                closingTimer.Elapsed -= new ElapsedEventHandler(CloseForm);
                closingTimer.Stop();
                ClosingTimerStarted = false;
            }
            closingTimer.Elapsed += new ElapsedEventHandler(CloseForm);
            closingTimer.Start();
            ClosingTimerStarted = true;
            pictureBox1.Invoke((MethodInvoker)delegate
            {
                pictureBox1.Image = ImageProcessing.Properties.Resources.bekleme;
                pictureBox2.Image = null;
            });
            Image.PersonImage = new Bitmap(new MemoryStream(image.Buffer));

        }
        void device_CaptureComplete(NikonDevice sender, int data)
        {
            if (ClosingTimerStarted)
            {
                closingTimer.Elapsed -= new ElapsedEventHandler(CloseForm);
                closingTimer.Stop();
                ClosingTimerStarted = false;
            }
            test = new GreenBox(Image, Position.BottomLeft);
            test.Hue = 2;
            printImage = test.CImage;
            this.BackgroundImage = new Bitmap(printImage);
            pictureBox1.Image = null;
            Print(printImage);
        }
        void CloseForm(object o, ElapsedEventArgs a)
        {

            closingTimer.Stop();
            closingTimer.Elapsed -= new ElapsedEventHandler(CloseForm);
            this.Invoke((MethodInvoker)delegate
            {
                this.Close();
            });
        }
        void CountDown(object o, ElapsedEventArgs a)
        {
            CounterItem cItem = counter.CountDown();
            if (cItem.Count == 0)
            {
                timer.Stop(); 
                
                if (device == null)
                {

                    return;
                }

                try
                {
                    pictureBox1.Image = ImageProcessing.Properties.Resources.smile;
                    pictureBox2.Image = null;
                    if (ClosingTimerStarted)
                    {
                        closingTimer.Elapsed -= new ElapsedEventHandler(CloseForm);
                        closingTimer.Stop();
                        ClosingTimerStarted = false;
                    }
                    closingTimer.Elapsed += new ElapsedEventHandler(CloseForm);
                    closingTimer.Start();
                    ClosingTimerStarted = true;
                    device.Capture();

                }
                catch (NikonException ex)
                {
                    if (ex.ErrorCode == eNkMAIDResult.kNkMAIDResult_OutOfFocus)
                    {
                        pictureBox3.Image = null;
                        pictureBox1.Image = ImageProcessing.Properties.Resources.focuserror;
                        pictureBox2.Image = null;
                    }
                    else if (ex.ErrorCode == eNkMAIDResult.kNkMAIDResult_BatteryDontWork)
                    {
                        pictureBox3.Image = null;
                        pictureBox1.Image = ImageProcessing.Properties.Resources.lowbattery;
                        pictureBox2.Image = null;
                    }
                    else if (ex.ErrorCode == eNkMAIDResult.kNkMAIDResult_UnexpectedError)
                    {
                        pictureBox3.Image = null;
                        pictureBox1.Image = ImageProcessing.Properties.Resources.unknownerror;
                        pictureBox2.Image = null;
                    }
                    counter = new Counter();
                    timer.Enabled = true;
                    timer.Start();
                }
                return;
            }
            pictureBox1.Image = cItem.Number;
            pictureBox2.Image = cItem.Smile;

        }
        void Print(Bitmap Image)
        {   
            printDocument1 = new System.Drawing.Printing.PrintDocument();
            printDocument1.PrintPage += new PrintPageEventHandler(this.printDocument1_PrintPage);
            printDocument1.EndPrint += new PrintEventHandler(this.printDocument1_EndPrint);
            System.Drawing.Printing.PrintController printController = new System.Drawing.Printing.StandardPrintController();
            printDocument1.PrintController = printController;
            if (!File.Exists("data.bin"))
            {
                PrintDialog printDialog1 = new PrintDialog();
                DialogResult dr = printDialog1.ShowDialog();
                using (Stream stream = System.IO.File.Open("data.bin", FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, printDialog1.PrinterSettings);
                }
            }
            using (Stream stream = File.Open("data.bin", FileMode.Open))
            {
                BinaryFormatter bin = new BinaryFormatter();

                System.Drawing.Printing.PrinterSettings setting = (System.Drawing.Printing.PrinterSettings)bin.Deserialize(stream);
                printDocument1.PrinterSettings = setting;
            }
            printDocument1.Print();
        }
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            float oran = (float)e.PageSettings.PaperSize.Width / (float)printImage.Width;
            float width = e.PageSettings.PaperSize.Width;
            float height = printImage.Height * oran;
            e.Graphics.DrawImage(printImage, (e.PageSettings.PaperSize.Width - width) / 2.0f, (e.PageSettings.PaperSize.Height - height) / 2.0f, width, height);
        }
        private void printDocument1_EndPrint(object sender,PrintEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                pictureBox3.Image = ImageProcessing.Properties.Resources.alin;
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                closingTimer.Elapsed += new ElapsedEventHandler(CloseForm);
                closingTimer.Start();
            });
        }

        private void TakePhoto_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    Close();
                    break;
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void TakePhoto_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cursor.Show();
            timer.Elapsed -=  new ElapsedEventHandler(CountDown);
            timer.Stop();
            // Hook up device capture events
            device.ImageReady -= new ImageReadyDelegate(device_ImageReady);
            device.CaptureComplete -= new CaptureCompleteDelegate(device_CaptureComplete);
        }

        private void TakePhoto_ResizeEnd(object sender, EventArgs e)
        {
            CenterComponents();
        }

        private void TakePhoto_ClientSizeChanged(object sender, EventArgs e)
        {
            CenterComponents();
        }
    }
}
