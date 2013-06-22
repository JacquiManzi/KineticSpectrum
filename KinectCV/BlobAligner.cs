using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RevKitt.ks.KinectCV
{
    class BlobAligner
    {
        private List<Blob> _previousBlobs = new List<Blob>();
 
        /// <summary>
        /// Given a list of blobs, matches up with blobs from the previous frame, returning a list of blob pairs that represent identical blobs from different frames
        /// </summary>
        /// <param name="newBlobs"></param>
        /// <returns>A list of blob pairs, with each pair representing the the same blob from different frames. The key is the blob from the previous frame, while the value is the
        /// blob from the current frame.</returns>
        public Dictionary<Blob, Blob> AlignBlobs(IList<Blob> newBlobs )
        {
            List<Blob> matchingBlobs = new List<Blob>(15);
            var pairs = new Dictionary<Blob, Blob>();
            foreach (var newBlob in newBlobs)
            {
                double maxScore = 0;
                Blob maxBlob = null;
                foreach (var oldBlob in _previousBlobs)
                {
                    double score = newBlob.Score(oldBlob);
                    double areaDiff = Math.Abs(oldBlob.Area - newBlob.Area);

                    //if we have the best score and the areas are at least comperable, make it the closest match
                    if(score > maxScore && areaDiff < oldBlob.Area/3 && areaDiff < newBlob.Area/3)
                    {
                        maxScore = score;
                        maxBlob = oldBlob;
                    }
                    if(score > 0)
                    {
                        matchingBlobs.Add(oldBlob);
                    }
                }

                if(maxBlob != null && matchingBlobs.Count < 2)
                {
                    pairs.Add(newBlob, maxBlob);
                }
                matchingBlobs.Clear();
            }
            _previousBlobs = new List<Blob>(newBlobs);
            return pairs;
        }
    }
}
