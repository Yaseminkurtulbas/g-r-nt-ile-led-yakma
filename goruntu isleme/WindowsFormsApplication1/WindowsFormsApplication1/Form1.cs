using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;
using System.IO.Ports;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        string[] portlar = SerialPort.GetPortNames();
        private FilterInfoCollection VideoCapTureDevices;
        private VideoCaptureDevice Finalvideo;
       

        public Form1()
        {
            InitializeComponent();
        }    
       
        private void Form1_Load(object sender, EventArgs e)
        {           
            VideoCapTureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo VideoCaptureDevice in VideoCapTureDevices)
            {
                comboBox1.Items.Add(VideoCaptureDevice.Name);
            }

            comboBox1.SelectedIndex = 0;
            foreach (string port in portlar)
            {
                comboBox2.Items.Add(port);
            }
            comboBox2.SelectedIndex = 0;
            
            // label3.Text = "Bağlantı Kapalı";

            serialPort2.PortName = comboBox2.Text;
            serialPort2.Open();
            if (serialPort2.IsOpen == true)
            {
                label3.Text = "Bağlantı Açık";

            }
            

        }        

        private void button1_Click(object sender, EventArgs e)
        {
            Finalvideo = new VideoCaptureDevice(VideoCapTureDevices[comboBox1.SelectedIndex].MonikerString);
            Finalvideo.NewFrame += new NewFrameEventHandler(Finalvideo_NewFrame);
            Finalvideo.DesiredFrameRate = 20; //görüntü sayısı 
            Finalvideo.DesiredFrameSize = new Size(320, 240); //görüntü boyutu
            Finalvideo.Start();                                       

        }
        private void button2_Click(object sender, EventArgs e)
        {

            if (Finalvideo.IsRunning)
            {
                Finalvideo.Stop();

            }
        }
        private void button3_Click(object sender, EventArgs e)
        {

            if (Finalvideo.IsRunning)
            {
                Finalvideo.Stop();

            }

            Application.Exit();
        }
        
        void Finalvideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {

            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            Bitmap image1 = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = image;

           
            EuclideanColorFiltering filter = new EuclideanColorFiltering();
            
            filter.CenterColor = new RGB(Color.FromArgb(255, 20, 147));
            filter.Radius = 100;
          
            filter.ApplyInPlace(image1);

            nesnebul(image1);                  

        }

        public void nesnebul(Bitmap image)
        {
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.MinWidth = 5;
            blobCounter.MinHeight = 5;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;
           

            BitmapData objectsData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            
            Grayscale grayscaleFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            UnmanagedImage grayImage = grayscaleFilter.Apply(new UnmanagedImage(objectsData));            
            image.UnlockBits(objectsData);
            blobCounter.ProcessImage(image);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            Blob[] blobs = blobCounter.GetObjectsInformation();
            pictureBox2.Image = image;
            foreach (Rectangle recs in rects)
            {
                if (rects.Length >= 0)
                {
                    Rectangle objectRect = rects[0];
                    ;
                    Graphics g = pictureBox1.CreateGraphics();
                    using (Pen pen = new Pen(Color.FromArgb(252, 3, 26), 2))
                    {
                        g.DrawRectangle(pen, objectRect);
                    }
                   
                    int objectX = objectRect.X + (objectRect.Width / 2);
                    int objectY = objectRect.Y + (objectRect.Height / 2);

                    g.Dispose();

                  
                        this.Invoke((MethodInvoker)delegate
                        {
                            richTextBox1.Text = objectRect.Location.ToString() + "\n" + richTextBox1.Text + "\n"; ;
                        });

                    if ((0 <= objectX && objectX < 105) && (0 <= objectY && objectY < 80))
                    {
                        serialPort2.Write("2");
                    }
                    if ((105 <= objectX && objectX < 210) && (0 < objectY && objectY < 80))
                    {
                        serialPort2.Write("3");
                    }
                    if ((210 <= objectX && objectX <= 320) && (0 < objectY && objectY < 80))
                    {
                        serialPort2.Write("4");
                    }


                    if ((0 <= objectX && objectX < 105) && (80 <= objectY && objectY < 160))
                    {
                        serialPort2.Write("5");
                    }
                    if ((105 <= objectX && objectX < 210) && (80 <= objectY && objectY < 160))
                    {
                        serialPort2.Write("6");
                    }
                    if ((210 <= objectX && objectX <= 320) && (80 <= objectY && objectY < 160))
                    {
                        serialPort2.Write("7");
                    }


                    if ((0 < objectX && objectX < 105) && (160 <= objectY && objectY <= 240))
                    {
                        serialPort2.Write("8");
                    }
                    if ((105 <= objectX && objectX < 210) && (160 <= objectY && objectY <= 240))
                    {
                        serialPort2.Write("9");
                    }
                    if ((210 < objectX && objectX <320) && (160 < objectY && objectY < 240))
                    {
                        serialPort2.Write("1");
                        Console.WriteLine("text");
                    }


                }

            }

        }            

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void rdiobtnMavi_CheckedChanged(object sender, EventArgs e)
        {

        }
    }


}


