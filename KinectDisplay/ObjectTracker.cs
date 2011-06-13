using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;

namespace KinectDisplay
{
    public class ObjectTracker
    {
        private IList<Blob> _blobs;
        private int _objCount = 0;


        public ObjectTracker()
        {
           _blobs = new List<Blob>();
        }

        public void Track(IList<Blob> newBlobs)
        {
            foreach(Blob blob in _blobs)
            {
                Blob closestMatch = null;
                int minScore = int.MaxValue;

                foreach(Blob newBlob in newBlobs)
                {
//                    int score = (int) ( (blob.OverlapPercent(newBlob) + 
//                                         newBlob.OverlapPercent(blob)  ) * 100 );
                    int score = blob.Score(newBlob);
                    if(score < minScore)
                    {
                        minScore = score;
                        closestMatch = blob;
                    }
                }
                if(closestMatch != null)
                {
                    closestMatch.Name = blob.Name;
                }
                
//                blob.Name = (closestMatch == null ? (_objCount++).ToString() : closestMatch.Name);
            }
            foreach(Blob newBlob in newBlobs)
            {
                if (newBlob.Name == null)
                {
                    newBlob.Name = (_objCount++).ToString();
                }
            }
            _blobs = newBlobs;
        }

        public IList<Blob> Blobs { get { return _blobs; } }
    }

    class Overlap
    {
        
    }
}
