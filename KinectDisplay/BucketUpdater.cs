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
            }
            foreach (ColorData colorData in _colorData)
            {
                //pattern.HappyTrails(colorData, adjfactor);
            }
        }

        private const int adjust = 6;
        private void UpdateBucket(byte intensity)
        {
            
            
            //int bucket = (int) (52.36*Math.Log(intensity, Math.E) + 53.422);
            int bucket = (int)(0.360493 * Math.Pow(1.01928, intensity));
            //            int start = 90;
            //            int end = 255;
            //            int pos = (int)Math.Floor((intensity-50.0)/(255.0-50)*49);
            bucket = (int) Math.Round(bucket - adjust + bucket*adjust/46.0);
            if (bucket < 0) return;

            foreach (ColorData colorData in _colorData)
            {
                int thisBucket = bucket*4/colorData.Spacing + colorData.Initial;
                if (colorData.Spacing == 12)
                    colorData[thisBucket] = Colors.Red;
                else
                    colorData[thisBucket] = Colors.Red;
                if (thisBucket == 46)
                {
                    colorData[47] = Colors.Red;
                    colorData[48] = Colors.Red;
                    colorData[49] = Colors.Red;
                }
            }
            //_colorData[bucket] = Colors.SlateBlue;
            //_colorData[bucket] = Colors.Red;

        }

        public IList<ColorData> AllColorData { get { return _colorData; } }
    }
}
