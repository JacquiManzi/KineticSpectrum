using System.Collections.Generic;
using RevKitt.ks.KinectCV;

namespace KinectDisplay
{
    public class ObjectTracker
    {
        private IList<Blob> _blobs;
        private static int _objCount = default(int);


        public ObjectTracker()
        {
           _blobs = new List<Blob>();
        }

        public void Track(IList<Blob> newBlobs)
        {
            IList<Blob> bList = new List<Blob>();
            foreach(Blob newBlob in newBlobs)
            {
                Blob closestMatch = null;
                double maxScore=0;

                foreach(Blob blob in _blobs)
                {
                    double score = blob.Score(newBlob);
                    
                    if(score > maxScore)
                    {
                        maxScore = score;
                        closestMatch = blob;
                    }
                }
                if(closestMatch == null)
                {
                    newBlob.Name = (_objCount++).ToString();
                    bList.Add(newBlob);
                }
                else
                {
                    closestMatch.AbsorbDimentionsOf(newBlob);
                    bList.Add(closestMatch);
                }
            }
            _blobs = bList;
        }

        public IList<Blob> Blobs { get { return _blobs; } }
    }

    class Overlap
    {
        
    }
}
