using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace KinectAndGLTest.Kinect
{

    

    internal class CameraRef : SafeHandle
    {
        #region [ Native ]
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool UnmapViewOfFile(IntPtr hMap);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hHandle);
        #endregion

        public const int DEFAULT_TIMEOUT = 50;

        private          IntPtr       _map;
        private          IntPtr       _section;
        private readonly Device       _refDevice;
        private readonly Dispatcher   _dispatcher;
        private readonly CameraType   _type;
        private readonly int          _timeout;
        
        private readonly WeakReference _image;

        public bool IsAlive
        {
            get { return _image.IsAlive; }
        }


        private static readonly Vector Resolution = new Vector(640, 480);

        internal CameraRef(CameraType type, Device refrencingCamera, Dispatcher dispatcher, int timeout) : base(IntPtr.Zero, true)
        {
            _dispatcher = dispatcher;
            _refDevice = refrencingCamera;
            _timeout = timeout;
            _type = type;
            _image = new WeakReference(null, false);
            _map = IntPtr.Zero;
            _section = IntPtr.Zero;
        }

        private static int CalcImageSize(CameraType type, int height, int width)
        {
            int stride = CalculateStride(type, width);
            return stride * height;
        }

        internal Boolean Update()
        {
            if(!IsAlive) return false;
            
            switch (_type)
            {
                case CameraType.ColorRaw:
                    KinectDevice.GetCameraColorFrameRAW(_refDevice.CameraPointer, _map, _timeout);
                    break;
                case CameraType.ColorRgb24:
                    KinectDevice.GetCameraColorFrameRGB24(_refDevice.CameraPointer, _map, _timeout);
                    break;
                case CameraType.ColorRgb32:
                    KinectDevice.GetCameraColorFrameRGB32(_refDevice.CameraPointer, _map, _timeout);
                    break;
                case CameraType.DepthRaw:
                    KinectDevice.GetCameraDepthFrameRAW(_refDevice.CameraPointer, _map, _timeout);
                    break;
                case CameraType.DepthCorrected8:
                    KinectDevice.GetCameraDepthFrameCorrected8(_refDevice.CameraPointer, _map, _timeout);
                    break;
                case CameraType.DepthCorrected12:
                    KinectDevice.GetCameraDepthFrameCorrected12(_refDevice.CameraPointer, _map, _timeout);
                    break;
                case CameraType.DepthRgb32:
                    KinectDevice.GetCameraDepthFrameRGB32(_refDevice.CameraPointer, _map, _timeout);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _dispatcher.BeginInvoke((Action) (()=>BitmapSource.Invalidate()));
            //_handler.Invoke(BitmapSource, EventArgs.Empty);)
            return true;
        }

        public InteropBitmap BitmapSource
        {
           get
           {
               InteropBitmap local;
               if (!_image.IsAlive)
               {
                   var imageSize = (uint) CalcImageSize(_type, (int) Resolution.Y, (int) Resolution.X);
                   int stride = CalculateStride(_type, (int) Resolution.X);
                   if (_map == IntPtr.Zero)
                   {
                       _section = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, imageSize, null);
                       _map = MapViewOfFile(_section, 0xF001F, 0, 0, imageSize);
                   }
                   local =
                       Imaging.CreateBitmapSourceFromMemorySection(_section, (int) Resolution.X, (int) Resolution.Y,
                                                                   PixelFormats.Bgr32, stride, 0) as InteropBitmap;
                   _image.Target = local;
               }
               else
               {
                   local = (InteropBitmap) _image.Target;
               }
               return local;
           }
        }

        private static int CalculateStride(CameraType type, int width)
        {
            int stride;
            switch (type)
            {
                case CameraType.ColorRaw:
                case CameraType.DepthRaw:
                    throw new ArgumentException("Don't know how to handle the raw types yet, sorry");
                case CameraType.ColorRgb24:
                    stride = (width * 3 + 3) & ~3;
                    break;
                case CameraType.DepthCorrected12:
                    stride = (width * 2 + 3) & ~3;
                    break;
                case CameraType.ColorRgb32:
                case CameraType.DepthRgb32:
                    stride = width * 4;
                    break;
                case CameraType.DepthCorrected8:
                    stride = (width + 3) & ~3;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
            return stride;
        }

        #region SafeHandle Implementaion
        protected override bool ReleaseHandle()
        {
            if(_map != IntPtr.Zero)
            {
                UnmapViewOfFile(_map);
                _map = IntPtr.Zero;
            }
            if(_section != IntPtr.Zero)
            {
                CloseHandle(_section);
                _section = IntPtr.Zero;
            }
            return true;
        }

        public override bool IsInvalid
        {
            get { return _map == IntPtr.Zero || _section == IntPtr.Zero; }
        }

        #endregion
    }
}
