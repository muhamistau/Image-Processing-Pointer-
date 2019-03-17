using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessingPointer
{
    public partial class Form1 : Form
    {

        Bitmap bmp;

        public Form1()
        {
            InitializeComponent();
        }

        private void greyscaleConvertion(Bitmap bmp)
        {
            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* p = (byte*)(void*)bitmapData.Scan0.ToPointer();
                int stopAddress = (int)p + bitmapData.Stride * bitmapData.Height;
                while ((int)p != stopAddress)
                {
                    p[0] = (byte)(.299 * p[2] + .587 * p[1] + .114 * p[0]);
                    p[1] = p[0];
                    p[2] = p[0];
                    p += 3;
                }
            }
            bmp.UnlockBits(bitmapData);
        }

        private void brightnessConvertion(Bitmap bmp, int value)
        {
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                        ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int nOffset = data.Stride - data.Width * 3, nVal;
            int nWidth = data.Width * 3;

            unsafe
            {
                byte* ptr = (byte*)(data.Scan0);
                for (int y = 0; y < data.Height; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        nVal = (int)(ptr[0] + value);
                        if (nVal < 0) nVal = 0;
                        if (nVal > 255) nVal = 255;
                        ptr[0] = (byte)nVal;
                        ++ptr;
                    }
                    ptr += nOffset;
                }
            }
            bmp.UnlockBits(data);
        }

        private void invertConvertion(Bitmap bmp)
        {
            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* p = (byte*)(void*)bitmapData.Scan0.ToPointer();
                int stopAddress = (int)p + bitmapData.Stride * bitmapData.Height;
                while ((int)p != stopAddress)
                {
                    p[0] = (byte)(255 - p[0]);
                    p[1] = (byte)(255 - p[1]);
                    p[2] = (byte)(255 - p[2]);
                    p += 3;
                }
            }
            bmp.UnlockBits(bitmapData);
        }

        private void thresholdConvertion(Bitmap bmp, int value)
        {
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                        ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* p = (byte*)(void*)data.Scan0.ToPointer();
                int stopAddress = (int)p + data.Stride * data.Height;
                while ((int)p != stopAddress)
                {
                    if ((byte)(.299 * p[2] + .587 * p[1] + .114 * p[0]) > value)
                    {
                        p[0] = 255;
                    }
                    else
                        p[0] = 0;
                    p[1] = p[0];
                    p[2] = p[0];
                    p += 3;
                }
            }
            bmp.UnlockBits(data);
        }

        private void buttonGrayscale_Click(object sender, EventArgs e)
        {
            
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Please open an image first");
                return;
            }
            else
            {
                var time = System.Diagnostics.Stopwatch.StartNew(); // start timer

                Cursor = Cursors.WaitCursor;
                bmp = (Bitmap)pictureBox1.Image;
                greyscaleConvertion(bmp);
                pictureBox2.Image = bmp;
                Cursor = Cursors.Default;

                time.Stop();
                executionTime.Text = time.ElapsedMilliseconds.ToString() + " milliseconds";
            }
        }

        private void buttonBrightness_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Please open an image first");
            }
            else
            {
                if (textBoxBrightness.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("Please enter something in the textbox");
                    return;
                }
                else
                {
                    var time = System.Diagnostics.Stopwatch.StartNew();

                    Cursor = Cursors.WaitCursor;
                    Bitmap bmp = (Bitmap)pictureBox1.Image;
                    int value = Convert.ToInt16(textBoxBrightness.Text);
                    brightnessConvertion(bmp, value);
                    pictureBox2.Image = bmp;
                    Cursor = Cursors.Default;

                    time.Stop();
                    executionTime.Text = time.ElapsedMilliseconds.ToString() + " milliseconds";
                }
            }
        }

        private void buttonInvert_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Please open an image first");
                return;
            }
            else
            {
                var time = System.Diagnostics.Stopwatch.StartNew(); // start timer

                Cursor = Cursors.WaitCursor;
                bmp = (Bitmap)pictureBox1.Image;
                invertConvertion(bmp);
                pictureBox2.Image = bmp;
                Cursor = Cursors.Default;

                time.Stop();
                executionTime.Text = time.ElapsedMilliseconds.ToString() + " milliseconds";
            }
        }

        private void buttonThreshold_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Please open an image first");
            }
            else
            {
                if (textBoxThreshold.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("Please enter something in the textbox");
                    return;
                }
                else
                {
                    var time = System.Diagnostics.Stopwatch.StartNew();

                    Cursor = Cursors.WaitCursor;

                    Bitmap bmp = (Bitmap)pictureBox1.Image;
                    int value = Convert.ToInt16(textBoxThreshold.Text);
                    thresholdConvertion(bmp, value);
                    Cursor = Cursors.Default;
                    pictureBox2.Image = bmp;

                    time.Stop();
                    executionTime.Text = time.ElapsedMilliseconds.ToString() + " milliseconds";
                }
            }
        }

        private void openImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.jpg; *.bmp)|*.jpg; *.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openFileDialog.FileName);
            }
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Image Files (*.jpg; *.png;)|*.jpg; *.png";
            if (save.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image.Save(save.FileName);
            }
        }

        private static float Px(int init, int end, int[] hist)
        {
            int sum = 0;
            int i;

            for (i = init; i <= end; i++)
                sum += hist[i];

            return (float)sum;
        }

        private static float Mx(int init, int end, int[] hist)
        {
            int sum = 0;
            int i;

            for (i = init; i <= end; i++)
                sum += i * hist[i];

            return (float)sum;
        }

        private static int FindMax(float[] vec, int n)
        {
            float maxVec = 0;
            int idx = 0;
            int i;

            for (i = 1; i < n - 1; i++)
            {
                if (vec[i] > maxVec)
                {
                    maxVec = vec[i];
                    idx = i;
                }
            }

            return idx;
        }

        unsafe private static void GetHistogram(byte* p, int w, int h, int ws, int[] hist)
        {
            hist.Initialize();

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w * 3; j += 3)
                {
                    int index = i * ws + j;
                    hist[p[index]]++;
                }
            }
        }

        private void buttonOtsu_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Please open an image first");
                return;
            }
            else
            {
                var time = System.Diagnostics.Stopwatch.StartNew();

                //Search Otsu's Treshold Value 
                Cursor = Cursors.WaitCursor;
                bmp = (Bitmap)pictureBox1.Image;
                byte t = 0;
                float[] vet = new float[256];
                int[] hist = new int[256];
                vet.Initialize();

                float p1, p2, p12;
                int k;

                BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* p = (byte*)(void*)bmData.Scan0.ToPointer();

                    GetHistogram(p, bmp.Width, bmp.Height, bmData.Stride, hist);

                    for (k = 1; k != 255; k++)
                    {
                        p1 = Px(0, k, hist);
                        p2 = Px(k + 1, 255, hist);
                        p12 = p1 * p2;
                        if (p12 == 0)
                            p12 = 1;
                        float diff = (Mx(0, k, hist) * p2) - (Mx(k + 1, 255, hist) * p1);
                        vet[k] = (float)diff * diff / p12;
                    }
                }


                t = (byte)FindMax(vet, 256);
                textBoxOtsu.Text = "" + t.ToString();

                //Tresholding

                unsafe
                {
                    byte* p = (byte*)(void*)bmData.Scan0.ToPointer();
                    int stopAddress = (int)p + bmData.Stride * bmData.Height;
                    while ((int)p != stopAddress)
                    {
                        if ((byte)(p[0] + p[1] + p[2]) > t)
                            p[0] = 255;
                        else
                            p[0] = 0;
                        p[1] = p[0];
                        p[2] = p[0];
                        p += 3;
                    }
                }
                bmp.UnlockBits(bmData);

                Cursor = Cursors.Default;
                pictureBox2.Image = bmp;

                time.Stop();
                executionTime.Text = time.ElapsedMilliseconds.ToString() + " milliseconds";
            }
        }
    }
}
