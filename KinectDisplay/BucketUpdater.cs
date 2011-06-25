using System;
using System.Collections.Generic;
using System.Windows.Media;
using Emgu.CV;
using Emgu.CV.Structure;
using KineticControl;

namespace KinectDisplay
{
    public class BucketUpdater
    {
        private readonly IList<ColorData> _colorData;
        private readonly int _bucketEntries;
        private DateTime _lastUpdate;
        Patterns pattern = new Patterns();
        private Random rand = new Random();
        private Boolean change = true;
        private Color colorState = Colors.Blue;
        private int inc = 0;
        private int count = 0;
        private int colorCount = 0;
        Random random = new Random();


        public BucketUpdater(IList<ColorData> colorData)
        {
            for (int i = 0; i < colorData.Count; i++)
            {
                foreach(ColorData cdata in colorData)
                {
                    cdata[i] = Colors.Black;
                }
            }
            _colorData = colorData;
            _lastUpdate = DateTime.Now;
        }


        public void UpdateBuckets(Image<Gray,byte> mask, Image<Gray,byte> distance)
        {
            UpdateColors();
            byte[,,] maskD = mask.Data;
            byte[,,] dist = distance.Data;
            int height = mask.Height;
            int width = mask.Width;

            for (int i = 0; i < height; i++)
            {
                for (int j = 100; j < width-100; j++)
                {
                    if (maskD[i, j, 0] == 255)
                    {
                        UpdateBucket(dist[i, j, 0]);
                    }
                }
            }
        }

        private void UpdateColors()
        {
            TimeSpan delta = DateTime.Now - _lastUpdate;
            _lastUpdate = DateTime.Now;
            double adjfactor = Math.Pow(4, delta.TotalSeconds);
            foreach (var colorData in _colorData)
            {
                pattern.ParanoidZen(ref colorState, adjfactor, colorData);
               // pattern.HappyTrails(colorData, adjfactor);
               // pattern.GetBlues();
            }
            foreach (ColorData colorData in _colorData)
            {
                //pattern.HappyTrails(colorData, adjfactor);
                
            }

            if(colorCount >= 1000)
            {
                colorState = pattern.GetColors()[random.Next(0, pattern.GetColors().Count)];
                colorCount = 0;
            }
            colorCount++;
        }

        private const int adjust = 6;
        private void UpdateBucket(byte intensity)
        {
            
           RainbowMaddness(intensity);

        }

        public void normalRed(byte intensity)
        {
            int bucket = (int)(0.360493 * Math.Pow(1.01928, intensity));
            //            int start = 90;
            //            int end = 255;
            //            int pos = (int)Math.Floor((intensity-50.0)/(255.0-50)*49);
            bucket = (int)Math.Round(bucket - adjust + bucket * adjust / 46.0);
            if (bucket < 0) return;
            int i = 0;
            foreach (ColorData colorData in _colorData)
            {
                int thisBucket = bucket * 4 / colorData.Spacing + colorData.Initial;
                if (colorData.Spacing == 12)
                    colorData[thisBucket] = Colors.Red;
                else
                    //colorData[thisBucket] = Colors.Red;
                    colorData[thisBucket] = pattern.GetReds()[4];
                i++;
                if (thisBucket == 46)
                {
                    colorData[47] = Colors.Red;
                    colorData[48] = Colors.Red;
                    colorData[49] = Colors.Red;
                }
            }
        }

        public void RainbowMaddness(byte intensity)
        {
            int bucket = (int)(0.360493 * Math.Pow(1.01928, intensity));
            bucket = (int)Math.Round(bucket - adjust + bucket * adjust / 46.0);
            if (bucket < 0) return;
            foreach (ColorData colorData in _colorData)
            {
                int thisBucket = bucket * 4 / colorData.Spacing + colorData.Initial;
                if (colorData.Spacing == 12)
                    colorData[thisBucket] = Colors.Red;
                else
                    colorData[thisBucket] = pattern.GetColors()[inc];
                if (thisBucket == 46)
                {
                    colorData[47] = pattern.GetColors()[inc];
                    colorData[48] = pattern.GetColors()[inc];
                    colorData[49] = pattern.GetColors()[inc];
                }
                if (count > 10000000)
                {
                    inc++;
                    count = 0;
                }
                
                if(inc >= pattern.GetColors().Count)
                {
                    inc = 0;
                }

                count++;
            }
        }

        public IList<ColorData> AllColorData { get { return _colorData; } }
    }
}
