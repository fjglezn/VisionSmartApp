using System;
using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace VisionSmartApp
{
    public partial class FormVideo : Form
    {
        Image<Bgr, Byte> imgOrg; //image type RGB (or Bgr as we say in Open CV)
        private Capture capturecam;
        public static int Counter = 0;

        public FormVideo()
        {
            InitializeComponent();
        }

        private void FormVideo_Load(object sender, EventArgs e)
        {
            try
            {
                capturecam = new Capture();
            }
            catch (NullReferenceException exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }
            Application.Idle += new EventHandler(ProcessFunction);
        }

        private void ProcessFunction(object sender, EventArgs arg)
        {
            imgOrg = capturecam.QueryFrame().ToImage<Bgr, Byte>(); // error line
            FindPedestrian.Find(imgOrg);
            imageBox1.Image = imgOrg;
            txtPersonasDetectadas.Text = Counter.ToString();
        }
    }
    public static class FindPedestrian
    {
        /// <summary>
        /// Find the pedestrian in the image
        /// </summary>
        /// <param name="image">The image</param>
        /// <param name="processingTime">The pedestrian detection time in milliseconds</param>
        /// <returns>The image with pedestrian highlighted.</returns>
        public static Image<Bgr, Byte> Find(Image<Bgr, Byte> image)
        {
            Rectangle[] regions = new Rectangle[5];
            //this is the CPU version
            using (HOGDescriptor des = new HOGDescriptor())
            {
                des.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());

                Emgu.CV.Structure.MCvObjectDetection[] objects = des.DetectMultiScale(image);
                
                for(int i = 0; i < objects.Length; i++)
                {
                    regions[i] = objects[i].Rect;
                    if (objects[i].Score > 0.50)
                        FormVideo.Counter++;
                }
            }
            foreach (Rectangle pedestrain in regions)
            {
                image.Draw(pedestrain, new Bgr(Color.Red), 1);
            }
            return image;
        }
    }
}
