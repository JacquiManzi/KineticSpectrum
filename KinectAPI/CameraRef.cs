using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace KinectAPI
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
        //private readonly Dispatcher   _dispatcher;
        private readonly CameraType   _type;
        private readonly int          _timeout;

        private volatile WeakReference _image;
        private volatile WeakReference _matrix;


        private static readonly Size Resolution = new Size(640, 480);

        internal CameraRef(CameraType type, Device refrencingCamera, Dispatcher dispatcher, int timeout) : base(IntPtr.Zero, true)
        {
            Dispatcher = dispatcher;
            _refDevice = refrencingCamera;
            _timeout = timeout;
            _type = type;
            _image = null;
            _map = IntPtr.Zero;
            _section = IntPtr.Zero;
        }

        internal Dispatcher Dispatcher { get; set; }
        internal Boolean HasDispatcher { get { return Dispatcher != null; } }

        private static int CalcImageSize(CameraType type, int height, int width)
        {
            int stride = CalculateStride(type, width);
            return stride * height;
        }

        internal Boolean Update()
        {
            if (_image == null && _matrix == null) return true;

            Object bitmap = null;
            Object matrix = null;
            lock (this)
            {
                if(_image != null)
                    bitmap = _image.Target;
                if(_matrix != null)
                    matrix = _matrix.Target;
                if (bitmap == null && matrix == null) return false;
                if (_map == IntPtr.Zero) throw new InvalidAsynchronousStateException("Not expecting _map to be null");
            }

            lock (this)
            {
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
            }
            if(Dispatcher != null && bitmap != null)
                Dispatcher.BeginInvoke((Action)(() => ((InteropBitmap) bitmap).Invalidate() ));
            //_handler.Invoke(BitmapSource, EventArgs.Empty);)
            return true;
        }

        

        public DepthMatrix DepthMatrix
        {
            get
            {
                lock(this)
                {
                    DepthMatrix local;
                    if (_matrix == null || !_matrix.IsAlive)
                    {
                        var imageSize = (uint)CalcImageSize(_type, (int)Resolution.Height, (int)Resolution.Width);
                        if (_map.Equals(IntPtr.Zero))
                        {
                            _section = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, imageSize, null);
                            _map = MapViewOfFile(_section, 0xF001F, 0, 0, imageSize);
                        }
                        local = new DepthMatrix(_map, (int)Resolution.Width, (int)Resolution.Height );
                         
                        _matrix = new WeakReference(local);
                    }
                    else if (_matrix.Target is DepthMatrix)
                    {
                        local = _matrix.Target as DepthMatrix;
                    }
                    else
                    {
                        throw new ConstraintException("This Camera has already been allocated as a '" + _image.Target.GetType() + "'");
                    }
                    return local;
                }
            }
        } 

        public InteropBitmap BitmapSource
        {
           get
           {
               lock (this)
               {
                   if (_image == null || !_image.IsAlive)
                   {
                       InteropBitmap local = BuildBitmap();
                       _image = new WeakReference(local);
                       return local;
                   }
                   return _image.Target as InteropBitmap ?? BuildBitmap();
               }
           }
        }
        
        private InteropBitmap BuildBitmap()
        {
            var imageSize = (uint) CalcImageSize(_type, (int) Resolution.Height, (int) Resolution.Width);
            int stride = CalculateStride(_type, (int) Resolution.Width);
            if (_map.Equals(IntPtr.Zero))
            {
                _section = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, imageSize, null);
                _map = MapViewOfFile(_section, 0xF001F, 0, 0, imageSize);
            }
            PixelFormat format = GetFormat(_type);

            return  Imaging.CreateBitmapSourceFromMemorySection(_section, (int) Resolution.Width, (int) Resolution.Height,
                                                            format, stride, 0) as InteropBitmap;
        }


        private static PixelFormat GetFormat(CameraType type)
        {
            switch (type)
            {
                case CameraType.ColorRgb32:
                case CameraType.DepthRgb32:
                    return PixelFormats.Bgr32;
                case CameraType.ColorRgb24:
                    return PixelFormats.Bgr24;
                case CameraType.DepthCorrected8:
                    return PixelFormats.Gray8;
//                case CameraType.DepthCorrected12:
//                    return PixelFormats.Gray16;
                case CameraType.ColorRaw:
                case CameraType.DepthRaw:
                    return PixelFormats.Gray32Float;

            }
            throw new ArgumentException("Unsupported Camera Type: " + type, "type");
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
            lock (this)
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
            }
            return true;
        }

        public override bool IsInvalid
        {
            get {
                lock (this)
                {
                    return _map == IntPtr.Zero || _section == IntPtr.Zero;
                }
            }
        }

        #endregion
    }
}
