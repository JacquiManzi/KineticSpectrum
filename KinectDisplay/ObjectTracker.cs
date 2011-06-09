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
            foreach(Blob newBlob in newBlobs)
            {
                Blob closestMatch = null;
                int maxScore=0;

                foreach(Blob blob in _blobs)
                {
                    int score = (int) ( (blob.OverlapPercent(newBlob) + 
                                         newBlob.OverlapPercent(blob)  ) * 100 );
                    if(score > maxScore)
                    {
                        maxScore = score;
                        closestMatch = blob;
                    }
                }

                newBlob.Name = (closestMatch == null ? (_objCount++).ToString() : closestMatch.Name);
            }
            _blobs = newBlobs;
        }

        public IList<Blob> Blobs { get { return _blobs; } }
    }

    class Overlap
    {
        
    }
}
