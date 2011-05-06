using System;
using Emgu.CV;
using Emgu.CV.Structure;

namespace KinectDisplay
{
    public class Background
    {
        private readonly byte[,] _lowerBound;
        private readonly byte[,] _upperBound;
        private readonly byte[,] _base;
        private readonly int[,] _duration;
        private readonly float[,] _percentBlack;
        private readonly int _tolerance;
        private readonly int _updateTime;

        public Background(int width, int height, int tolerance = 3, int updateTime = 200)
        {
            _lowerBound = new byte[height,width];
            _upperBound = new byte[height, width];
            _duration = new int[height, width];
            _percentBlack = new float[height, width];
            _base = new byte[height,width];
            _duration.Initialize();
            _percentBlack.Initialize();
            _tolerance = tolerance;
            _updateTime = updateTime;
        }

        public bool Update(byte val, int wPos, int hPos)
        {
            if(_percentBlack[hPos,wPos] > 0)
            {
                _percentBlack[hPos, wPos] -= 0.5f;
            }
            else
            {
                _percentBlack[hPos, wPos] += 0.5f;
            }
            if (ForegroundDetector<Gray>.CompareDoubles(_base[hPos, wPos], val, _tolerance))
            {
                return true;
            }

            if(val > _base[hPos,wPos] && _base[hPos,wPos] != 0)
            {
                _base[hPos, wPos] = val;
                return true;
            }

            if (val == 0)
            {
                _percentBlack[hPos, wPos]++;
                if(_percentBlack[hPos,wPos] > _updateTime/8.0)
                {
                    _base[hPos, wPos] = 0;
                }
                return true;
            }
            
            _percentBlack[hPos, wPos]--;
            if(val > _upperBound[hPos,wPos] )
            {
                //_base[hPos, wPos] = val;
                _upperBound[hPos, wPos] = val;
                if(val - _lowerBound[hPos,wPos] > 4)
                {
                    _lowerBound[hPos, wPos] = (byte) (val - 4);
                    _duration[hPos,wPos] = 0;
                    //_percentBlack[hPos, wPos] = 0;
                }
            }
            else if(val < _lowerBound[hPos,wPos])
            {
                _lowerBound[hPos, wPos] = val;
                if(_upperBound[hPos,wPos] - val > 4 )
                {
                    int upper = val + 4;
                    _upperBound[hPos, wPos] = (byte) (upper > 255 ? 255 : upper);
                    _duration[hPos, wPos] = 0;
                    //_percentBlack[hPos,wPos] = 0;
                }
            }
            else
            {
                _duration[hPos, wPos]++;
                if(_duration[hPos,wPos] > _updateTime)
                {
                    _base[hPos, wPos] = (byte) (_lowerBound[hPos, wPos] +
                                        ((_upperBound[hPos, wPos] - _lowerBound[hPos, wPos])/2));
                }
                else if(_percentBlack[hPos,wPos] > _updateTime)
                {
                    _base[hPos, wPos] = 0;
                }
            }
            return false;
            
        }

        public byte this[int r,int c]
        {
            get { return _base[r, c]; }
        }

        public void Reset(object obj)
        {
            Image<Gray, byte> baseImg = (Image<Gray, byte>) obj;
            byte[,,] data = baseImg.Data;
            int height = baseImg.Height, width = baseImg.Width;

            for(int i=0; i<height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    byte val = _base[i,j] = data[i, j, 0];
//                    if(val==0)
//                    {
                    _lowerBound[i, j] = _upperBound[i, j] = val;
//                    }
//                    else
//                    {
//                       _lowerBound[i, j] = (byte) (data[i, j, 0] - _tolerance/2);
//                        _upperBound[i, j] = (byte) (data[i, j, 0] + _tolerance/2);
//                        if (_upperBound[i, j] < _tolerance) _upperBound[i, j] = 255;
//                    }
                }
            }
        }


    }
}
