using System;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System.Windows.Interop;

// Please write coder @ wiredprairie dot us if you use this code or find it useful!


namespace JFKCommonLibrary.WPF
{

    internal class IconHandle : SafeHandle
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DestroyIcon(IntPtr hIcon);

        internal IconHandle() : this(IntPtr.Zero) { }
        internal IconHandle(IntPtr ptr)
            : base(IntPtr.Zero, true)
        {
            base.handle = ptr;
        }

        protected override bool ReleaseHandle()
        {
            return DestroyIcon(base.handle);
        }

        public override bool IsInvalid
        {
            get
            {
                return (base.handle == IntPtr.Zero);
            }
        }

    }

    internal class BitmapHandle : SafeHandle
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr hObject);

        internal BitmapHandle() : this(IntPtr.Zero) { }
        internal BitmapHandle(IntPtr ptr)
            : base(IntPtr.Zero, true)
        {
            base.handle = ptr;
        }

        protected override bool ReleaseHandle()
        {
            return DeleteObject(base.handle);
        }

        public override bool IsInvalid
        {
            get
            {
                return (base.handle == IntPtr.Zero);
            }
        }

    }



    public class GhostCursor
    {
        #region Native Win32 stuff

        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BITMAPV5HEADER pbmi,
           uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        public const uint BI_BITFIELDS = 3;
        public const uint LCS_WINDOWS_COLOR_SPACE = 2;
        public const uint LCS_GM_IMAGES = 4;
        public const uint CF_DIBV5 = 17;
        public const uint GMEM_MOVEABLE = 0x00000002;
        public const uint GMEM_ZEROINIT = 0x00000040;
        public const uint GMEM_DDESHARE = 0x00002000;
        public const uint GHND = GMEM_MOVEABLE | GMEM_ZEROINIT;


        [StructLayout(LayoutKind.Sequential)]
        public struct CIEXYZ
        {
            public uint ciexyzX; //FXPT2DOT30 
            public uint ciexyzY; //FXPT2DOT30 
            public uint ciexyzZ; //FXPT2DOT30 
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct CIEXYZTRIPLE
        {
            public CIEXYZ ciexyzRed;
            public CIEXYZ ciexyzGreen;
            public CIEXYZ ciexyzBlue;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct BITFIELDS
        {
            public uint BlueMask;
            public uint GreenMask;
            public uint RedMask;
        }


        [StructLayout(LayoutKind.Explicit)]
        public struct BITMAPV5HEADER
        {
            public BITMAPV5HEADER(int width, int height, ushort bpp)
            {
                Size = (uint)Marshal.SizeOf(typeof(BITMAPV5HEADER));
                Width = width;
                Height = height;
                Planes = 1;
                BitCount = bpp;
                Compression = BI_BITFIELDS;
                RedMask = 0x00FF0000;
                GreenMask = 0x0000FF00;
                BlueMask = 0x000000FF;
                AlphaMask = 0xFF000000;

                // zeroed by default per struct needs ...
                SizeImage = 0;
                XPelsPerMeter = 0;
                YPelsPerMeter = 0;
                ClrUsed = 0;
                ClrImportant = 0;
                CSType = LCS_WINDOWS_COLOR_SPACE;
                Endpoints.ciexyzBlue.ciexyzX = 0;
                Endpoints.ciexyzBlue.ciexyzY = 0;
                Endpoints.ciexyzBlue.ciexyzZ = 0;
                Endpoints.ciexyzGreen.ciexyzX = 0;
                Endpoints.ciexyzGreen.ciexyzY = 0;
                Endpoints.ciexyzGreen.ciexyzZ = 0;
                Endpoints.ciexyzRed.ciexyzX = 0;
                Endpoints.ciexyzRed.ciexyzY = 0;
                Endpoints.ciexyzRed.ciexyzZ = 0;
                GammaRed = 0;
                GammaGreen = 0;
                GammaBlue = 0;
                Intent = LCS_GM_IMAGES;
                ProfileData = 0;
                ProfileSize = 0;
                Reserved = 0;
            }

            [FieldOffset(0)]
            public uint Size;
            [FieldOffset(4)]
            public int Width;
            [FieldOffset(8)]
            public int Height;
            [FieldOffset(12)]
            public ushort Planes;
            [FieldOffset(14)]
            public ushort BitCount;
            [FieldOffset(16)]
            public uint Compression;
            [FieldOffset(20)]
            public uint SizeImage;
            [FieldOffset(24)]
            public int XPelsPerMeter;
            [FieldOffset(28)]
            public int YPelsPerMeter;
            [FieldOffset(32)]
            public uint ClrUsed;
            [FieldOffset(36)]
            public uint ClrImportant;
            [FieldOffset(40)]
            public uint RedMask;
            [FieldOffset(44)]
            public uint GreenMask;
            [FieldOffset(48)]
            public uint BlueMask;
            [FieldOffset(52)]
            public uint AlphaMask;
            [FieldOffset(56)]
            public uint CSType;
            [FieldOffset(60)]
            public CIEXYZTRIPLE Endpoints;
            [FieldOffset(96)]
            public uint GammaRed;
            [FieldOffset(100)]
            public uint GammaGreen;
            [FieldOffset(104)]
            public uint GammaBlue;
            [FieldOffset(108)]
            public uint Intent;
            [FieldOffset(112)]
            public uint ProfileData;
            [FieldOffset(116)]
            public uint ProfileSize;
            [FieldOffset(120)]
            public uint Reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal struct BITMAPINFO
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;

            // do the easy stuff ...
            public BITMAPINFO(int width, int height, short bpp)
            {
                this.biSize = Marshal.SizeOf(typeof(BITMAPINFO));
                this.biWidth = width;
                this.biHeight = height;
                this.biPlanes = 1;
                this.biBitCount = bpp;
                this.biCompression = 0;
                this.biSizeImage = 0;
                this.biXPelsPerMeter = 0;
                this.biYPelsPerMeter = 0;
                this.biClrUsed = 0;
                this.biClrImportant = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ICONINFO
        {
            public bool IsIcon;
            public int xHotspot;
            public int yHotspot;
            public BitmapHandle MaskBitmap;
            public BitmapHandle ColorBitmap;
        };

        [DllImport("user32.dll")]
        private static extern IconHandle CreateIconIndirect([In] ref ICONINFO iconInfo);

        [DllImport("gdi32.dll")]
        internal static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("gdi32.dll", EntryPoint = "CreateBitmap", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr CreateBitmap(int width, int height, int planes, int bitsPerPixel, IntPtr bits);

        internal const int DIB_RGB_COLORS = 0;

        //[DllImport("user32.dll")]
        //public static extern int LoadCursor(int hInstance, string lpCursorName);


        #endregion

        private IconHandle _iconHandle;
        private Cursor _ghostCursor;

        public Cursor Cursor
        {
            get { return _ghostCursor; }
        }

        #region Mouse Cursor
        [DllImport("user32.dll", EntryPoint = "GetCursorInfo")]
        public static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll", EntryPoint = "CopyIcon")]
        public static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport("user32.dll", EntryPoint = "GetIconInfo")]
        public static extern bool GetIconInfo(IntPtr hIcon, out aICONINFO piconinfo);

        [StructLayout(LayoutKind.Sequential)]
        public struct CURSORINFO
        {
            public Int32 cbSize;        // Specifies the size, in bytes, of the structure. 
            public Int32 flags;         // Specifies the cursor state. This parameter can be one of the following values:
            public IntPtr hCursor;          // Handle to the cursor. 
            public pPOINT ptScreenPos;       // A POINT structure that receives the screen coordinates of the cursor. 
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct pPOINT
        {
            public Int32 x;
            public Int32 y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct aICONINFO
        {
            public bool fIcon;         // Specifies whether this structure defines an icon or a cursor. A value of TRUE specifies 
            public Int32 xHotspot;     // Specifies the x-coordinate of a cursor's hot spot. If this structure defines an icon, the hot 
            public Int32 yHotspot;     // Specifies the y-coordinate of the cursor's hot spot. If this structure defines an icon, the hot 
            public IntPtr hbmMask;     // (HBITMAP) Specifies the icon bitmask bitmap. If this structure defines a black and white icon, 
            public IntPtr hbmColor;    // (HBITMAP) Handle to the icon color bitmap. This member can be optional if this 
        }

        public const Int32 CURSOR_SHOWING = 0x00000001;

        /*
        static System.Drawing.Bitmap CaptureCursor() //ref int x, ref int y)
        {
            System.Drawing.Bitmap bmp;
            IntPtr hicon;
            CURSORINFO ci = new CURSORINFO();
            aICONINFO icInfo;
            ci.cbSize = Marshal.SizeOf(ci);
            if (GetCursorInfo(out ci))
            {
                if (ci.flags == CURSOR_SHOWING)
                {
                    hicon = CopyIcon(ci.hCursor);
                    if (GetIconInfo(hicon, out icInfo))
                    {                        
                        //x = ci.ptScreenPos.x - ((int)icInfo.xHotspot);
                        //y = ci.ptScreenPos.y - ((int)icInfo.yHotspot);
                        System.Drawing.Icon ic = System.Drawing.Icon.FromHandle(hicon);                        
                        bmp = ic.ToBitmap();
                        DestroyIcon(hicon);
                        return bmp;
                    }
                }
            }
            return null;
        }*/

        static BitmapSource CaptureCursor() //ref int x, ref int y)
        {
            IntPtr hicon;
            CURSORINFO ci = new CURSORINFO();
            aICONINFO icInfo;
            ci.cbSize = Marshal.SizeOf(ci);
            if (GetCursorInfo(out ci))
            {
                if (ci.flags == CURSOR_SHOWING)
                {
                    hicon = CopyIcon(ci.hCursor);
                    if (GetIconInfo(hicon, out icInfo))
                    {
                        System.Drawing.Icon ic = System.Drawing.Icon.FromHandle(hicon);
                        System.Drawing.Bitmap bmp = ic.ToBitmap();

                        BitmapSource retVal=System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        DestroyIcon(hicon);
                        return retVal;
                    }
                }
            }
            return null;
        }
        
        #endregion

        /*
        internal GhostCursor(Visual visual, Cursor cur,  DragDropEffects dragDropEffects)
        {            
            //CursorInteropHelper.Create(_iconHandle);
        }
        */

        public static BitmapSource CreateBitmapSourceFromBitmap(System.Drawing.Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        public GhostCursor(Visual visual)
        {
            BitmapSource renderBitmap = CaptureScreen(visual);

            BitmapSource actualCursor = CaptureCursor();
            byte[] pixels = GetBitmapPixels(actualCursor, (int)actualCursor.Width, (int)actualCursor.Height);            
            WriteableBitmap wrbmp = new WriteableBitmap(renderBitmap);
            wrbmp.WritePixels(new Int32Rect(0, 0, (int)actualCursor.Width, (int)actualCursor.Height), pixels, actualCursor.PixelWidth * actualCursor.Format.BitsPerPixel / 8, 0);

            int width = renderBitmap.PixelWidth;
            int height = renderBitmap.PixelHeight;
            int stride = width * 4;

            // unfortunately, this byte array will get placed on the large object heap more than likely ... 
            pixels = GetBitmapPixels(wrbmp, width, height);
           
            // -height specifies top-down bitmap
            BITMAPV5HEADER bInfo = new BITMAPV5HEADER(width, -height, 32);
            IntPtr ppvBits = IntPtr.Zero;
            BitmapHandle dibSectionHandle = null;            

            try
            {
                dibSectionHandle = new BitmapHandle(CreateDIBSection(IntPtr.Zero, ref bInfo, DIB_RGB_COLORS, out ppvBits, IntPtr.Zero, 0));

                // copy the pixels into the DIB section now ...
                Marshal.Copy(pixels, 0, ppvBits, pixels.Length);

                
                if (!dibSectionHandle.IsInvalid && ppvBits != IntPtr.Zero)
                {
                    CreateCursor(width, height, dibSectionHandle);
                }
            }
            finally
            {
                if (dibSectionHandle != null)
                {
                    dibSectionHandle.Dispose();
                }
            }
        }

        private void CreateCursor(int width, int height, BitmapHandle dibSectionHandle)
        {
            BitmapHandle monoBitmapHandle = null;
            try
            {
                monoBitmapHandle = new BitmapHandle(CreateBitmap(width, height, 1, 1, IntPtr.Zero));

                ICONINFO icon = new ICONINFO();
                icon.IsIcon = false;
                icon.xHotspot = 0;
                icon.yHotspot = 0;
                icon.ColorBitmap = dibSectionHandle;
                icon.MaskBitmap = monoBitmapHandle;

                _iconHandle = CreateIconIndirect(ref icon);
                if (!_iconHandle.IsInvalid)
                {
                    _ghostCursor = CursorInteropHelper.Create(_iconHandle);
                }

            }
            finally
            {
                // destroy the temporary mono bitmap now ...
                if (monoBitmapHandle != null)
                {
                    monoBitmapHandle.Dispose();
                }
            }
        }

        private static RenderTargetBitmap GetRenderTargetBitmap(Visual visual)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap(200, 200, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            return bitmap;
        }

        private BitmapSource CaptureScreen(Visual target)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(800, 600, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(target);
                LinearGradientBrush opacityMask = new LinearGradientBrush(Color.FromArgb(255, 1, 1, 1), Color.FromArgb(0, 1, 1, 1), 30);
                ctx.PushOpacityMask(opacityMask);
                ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                ctx.Pop();
            }
            renderBitmap.Render(dv);

            return renderBitmap;
        }


        private static byte[] GetBitmapPixels(BitmapSource renderBitmap, int width, int height)
        {
            // The stride is the width of a single row of pixels (a scan line), rounded up to a four-byte boundary. 
            // The stride is always greater than or equal to the actual pixel width. If the stride is positive, 
            // the bitmap is top-down. If the stride is negative, the bitmap is bottom-up.
            int stride = width * 4;
            FormatConvertedBitmap bitmap = new FormatConvertedBitmap();
            bitmap.BeginInit();
            bitmap.DestinationFormat = PixelFormats.Bgra32;
            bitmap.Source = renderBitmap;
            bitmap.EndInit();

            byte[] pixels = new byte[stride * height];
            bitmap.CopyPixels(Int32Rect.Empty, pixels, stride, 0);
            return pixels;
        }

        /*
         * Code zum überlagern von 2 Cursor:
         * http://blog.m-ri.de/index.php/2008/12/07/aus-zwei-mach-eins-wie-man-zwei-cursor-kombinieren-kann/
         * 
         * HICON CombineIcons(HICON hIcon1, HICON hIcon2)
{
 // Remember that HCURSOR and HICON are identical!
 // hIcon1 is overlayed by hIcon2.
 // hIcon2 isn't adjusted in size or position.
 // It just overlays hIcon1
 // Get bitmaps of icon 1
 ICONINFO iconInfo;
 ::ZeroMemory(&iconInfo,sizeof(iconInfo));
 if (!GetIconInfo(hIcon1,&iconInfo))
  return NULL;
 
 // Attach the bitmaps to get them automatically freed
 // upon error.
 CBitmap bitmap, mask;
 bitmap.Attach(iconInfo.hbmColor);
 mask.Attach(iconInfo.hbmMask);
 
 // Get size and width
 BITMAP bm;
 if (bitmap.m_hObject)
  bitmap.GetObject(sizeof(bm),&bm);
 else
  mask.GetObject(sizeof(bm),&bm);
 
 // Get the color depth from the icon and create an image list
 // Remember we need a
 UINT flags = 0;
 switch (bm.bmBitsPixel)
 {
 case 4:  flags = ILC_COLOR4;  break;
 case 8:  flags = ILC_COLOR8;  break;
 case 16: flags = ILC_COLOR16; break;
 case 24: flags = ILC_COLOR24; break;
 case 32: flags = ILC_COLOR32; break;
 default: flags = ILC_COLOR4;  break;  
 }
 CImageList il;
 // be ware that the monochrom cursor bitmap is twice the height
 if (!il.Create(bm.bmWidth,
      bm.bmHeight/(iconInfo.hbmColor!=NULL ? 1 : 2),
      ILC_MASK|flags,2,2))
  return NULL;
 
 // Load the both icons into the image list
 il.Add(hIcon1);
 il.Add(hIcon2);
 
 // Define the second icon as an overlay image
 il.SetOverlayImage(1,1);
 
 // Get a new icon with icon 2 overlayed
 HICON hCombined = ImageList_GetIcon(il.m_hImageList,0,
              ILD_NORMAL|INDEXTOOVERLAYMASK(1));
 if (!hCombined)
  return NULL;
 
 // Need the icon infos for this new icon
 ICONINFO iconInfoCombined;
 ::ZeroMemory(&iconInfoCombined,sizeof(iconInfo));
 if (!GetIconInfo(hCombined,&iconInfoCombined))
  return NULL;
 
 // Destroy the combined icon, we just have the bitmap and the mask
 ::DestroyIcon(hCombined);
 
 // Get the bitmaps into objects to get them automatically freed
 CBitmap bitmapCombined, maskCombined;
 bitmapCombined.Attach(iconInfo.hbmColor);
 maskCombined.Attach(iconInfo.hbmMask);
 
 // Get the hotspotinto and cursor data from
 // the ICONINFO of hCursor1
 iconInfoCombined.fIcon = iconInfo.fIcon;
 iconInfoCombined.xHotspot = iconInfo.xHotspot;
 iconInfoCombined.yHotspot = iconInfo.yHotspot;
 
 // OK we have can create a new Cursor out of the target
 // Don't forget to use DestroyIcon for the new Cursor/Icon
 return ::CreateIconIndirect(&iconInfoCombined);
}
         * */
    }
}