using System;
using System.Windows.Media;
using Emgu.CV;
using Emgu.CV.Structure;
using KineticControl;

namespace KinectDisplay
{
    public class BucketUpdater
    {
        private readonly ColorData _colorData1;
        private readonly ColorData _colorData2;
        private readonly int _bucketEntries;
        private DateTime _lastUpdate;
        Patterns pattern = new Patterns();
        private Random rand = new Random();
        private Boolean change = true;
        private Color colorState = Colors.Blue;


        public BucketUpdater(ColorData colorData1, ColorData colorData2)
        {
            for (int i = 0; i < colorData1.Count; i++)
            {
                colorData1[i] = Colors.Black;
                colorData2[i] = Colors.Black;
            }
            _lastUpdate = DateTime.Now;
            _colorData1 = colorData1;
            _colorData2 = colorData2;
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
            pattern.ParanoidZen(ref colorState, adjfactor, _colorData);
            //pattern.HappyTrails(_colorData, adjfactor);

 


        }

        private void UpdateBucket(byte intensity)
        {
            

            //int bucket = (int) (52.36*Math.Log(intensity, Math.E) + 53.422);
            int bucket = (int)(0.360493 * Math.Pow(1.01928, intensity));
            //            int start = 90;
            //            int end = 255;
            //            int pos = (int)Math.Floor((intensity-50.0)/(255.0-50)*49);
            if (bucket < 0) return;
            _colorData[bucket] = pattern.GetColors()[rand.Next(pattern.GetColors().Count - 1)];
            _colorData[bucket] = pattern.GetColors()[rand.Next(pattern.GetColors().Count - 1)];
            //_colorData[bucket] = Colors.SlateBlue;
            //_colorData[bucket] = Colors.Red;

        }

        public ColorData ColorData1 { get { return _colorData1; } }
        public ColorData ColorData2 { get { return _colorData2; } }
    }
}
