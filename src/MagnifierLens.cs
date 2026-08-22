using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MagnifierApp
{
    public class MagnifierLens : Form
    {
        private AppSettings _settings;
        private Timer _renderTimer;
        private bool _isRunning;
        private int _lastX = -9999;
        private int _lastY = -9999;

        // Lens dimension constants matching Firefox extension
        private const int CIRCLE_SMALL = 150;
        private const int CIRCLE_MEDIUM = 250;
        private const int CIRCLE_LARGE = 350;

        private readonly Size RECT_SMALL = new Size(240, 80);
        private readonly Size RECT_MEDIUM = new Size(380, 127);
        private readonly Size RECT_LARGE = new Size(500, 167);

        // Preallocated render resources for 0-allocation 60 FPS loop
        private Bitmap _renderBmp;
        private Graphics _renderGraphics;
        private Bitmap _screenBmp;
        private Graphics _screenGraphics;
        private int _allocatedW;
        private int _allocatedH;
        private int _allocatedSrcW;
        private int _allocatedSrcH;

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public MagnifierLens(AppSettings settings)
        {
            _settings = settings;

            // Form properties for transparent overlay
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            DoubleBuffered = true;

            // Initialize size
            Size sz = GetCurrentLensSize();
            Size = sz;
            Location = new Point(-2000, -2000);

            // Timer for 60 FPS update loop (~16ms)
            _renderTimer = new Timer();
            _renderTimer.Interval = 16;
            _renderTimer.Tick += RenderTimer_Tick;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= NativeMethods.WS_EX_LAYERED;
                cp.ExStyle |= NativeMethods.WS_EX_TRANSPARENT;
                cp.ExStyle |= NativeMethods.WS_EX_TOOLWINDOW;
                cp.ExStyle |= NativeMethods.WS_EX_TOPMOST;
                cp.ExStyle |= NativeMethods.WS_EX_NOACTIVATE;
                return cp;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try
            {
                // Exclude this window from screen capture / BitBlt to completely eliminate recursion & flicker!
                NativeMethods.SetWindowDisplayAffinity(Handle, NativeMethods.WDA_EXCLUDEFROMCAPTURE);
            }
            catch { }
        }

        public void Start()
        {
            _isRunning = true;
            _settings.Enabled = true;
            _lastX = -9999;
            _lastY = -9999;

            Size sz = GetCurrentLensSize();
            Size = sz;

            Show();
            _renderTimer.Start();
            UpdateFrame();
        }

        public void Stop()
        {
            _isRunning = false;
            _settings.Enabled = false;
            _renderTimer.Stop();
            Hide();
        }

        public void Toggle()
        {
            if (_isRunning)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }

        public void UpdateSettings(AppSettings settings)
        {
            _settings = settings;
            if (_isRunning)
            {
                FreeBuffers();
                Size sz = GetCurrentLensSize();
                Size = sz;
                UpdateFrame();
            }
        }

        private Size GetCurrentLensSize()
        {
            if (_settings.LensShape == "rect")
            {
                if (_settings.LensSize == "small") return RECT_SMALL;
                if (_settings.LensSize == "large") return RECT_LARGE;
                return RECT_MEDIUM;
            }
            else
            {
                if (_settings.LensSize == "small") return new Size(CIRCLE_SMALL, CIRCLE_SMALL);
                if (_settings.LensSize == "large") return new Size(CIRCLE_LARGE, CIRCLE_LARGE);
                return new Size(CIRCLE_MEDIUM, CIRCLE_MEDIUM);
            }
        }

        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            if (!_isRunning) return;
            UpdateFrame();
        }

        private void EnsureBuffers(int lw, int lh, int srcW, int srcH)
        {
            if (_renderBmp == null || _allocatedW != lw || _allocatedH != lh)
            {
                if (_renderGraphics != null) { _renderGraphics.Dispose(); _renderGraphics = null; }
                if (_renderBmp != null) { _renderBmp.Dispose(); _renderBmp = null; }

                _renderBmp = new Bitmap(lw, lh, PixelFormat.Format32bppArgb);
                _renderGraphics = Graphics.FromImage(_renderBmp);
                _renderGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                _renderGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                _renderGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                _allocatedW = lw;
                _allocatedH = lh;
            }

            if (_screenBmp == null || _allocatedSrcW != srcW || _allocatedSrcH != srcH)
            {
                if (_screenGraphics != null) { _screenGraphics.Dispose(); _screenGraphics = null; }
                if (_screenBmp != null) { _screenBmp.Dispose(); _screenBmp = null; }

                _screenBmp = new Bitmap(srcW, srcH, PixelFormat.Format32bppRgb);
                _screenGraphics = Graphics.FromImage(_screenBmp);
                _allocatedSrcW = srcW;
                _allocatedSrcH = srcH;
            }
        }

        private void FreeBuffers()
        {
            if (_renderGraphics != null) { _renderGraphics.Dispose(); _renderGraphics = null; }
            if (_renderBmp != null) { _renderBmp.Dispose(); _renderBmp = null; }
            if (_screenGraphics != null) { _screenGraphics.Dispose(); _screenGraphics = null; }
            if (_screenBmp != null) { _screenBmp.Dispose(); _screenBmp = null; }
            _allocatedW = 0;
            _allocatedH = 0;
            _allocatedSrcW = 0;
            _allocatedSrcH = 0;
        }

        private void UpdateFrame()
        {
            NativeMethods.POINT pt;
            if (!NativeMethods.GetCursorPos(out pt)) return;

            Size lensSize = GetCurrentLensSize();
            int lw = lensSize.Width;
            int lh = lensSize.Height;
            int gap = 14;

            // Compute lens top-left relative to cursor
            int lx, ly;
            switch (_settings.LensPosition)
            {
                case "left":
                    lx = pt.X - lw - gap;
                    ly = pt.Y - lh / 2;
                    break;
                case "right":
                    lx = pt.X + gap;
                    ly = pt.Y - lh / 2;
                    break;
                case "up":
                    lx = pt.X - lw / 2;
                    ly = pt.Y - lh - gap;
                    break;
                case "down":
                    lx = pt.X - lw / 2;
                    ly = pt.Y + gap;
                    break;
                default: // "center"
                    lx = pt.X - lw / 2;
                    ly = pt.Y - lh / 2;
                    break;
            }

            // Virtual screen bounds clamping
            Rectangle virtualScreen = SystemInformation.VirtualScreen;
            lx = Math.Max(virtualScreen.Left + 2, Math.Min(lx, virtualScreen.Right - lw - 2));
            ly = Math.Max(virtualScreen.Top + 2, Math.Min(ly, virtualScreen.Bottom - lh - 2));

            // Compute source area around cursor
            float zoom = Math.Max(1.0f, _settings.Zoom);
            int srcW = (int)Math.Max(2, lw / zoom);
            int srcH = (int)Math.Max(2, lh / zoom);
            int srcX = pt.X - srcW / 2;
            int srcY = pt.Y - srcH / 2;

            try
            {
                EnsureBuffers(lw, lh, srcW, srcH);

                // Capture screen snippet into reusable buffer
                _screenGraphics.CopyFromScreen(srcX, srcY, 0, 0, new Size(srcW, srcH), CopyPixelOperation.SourceCopy);

                // Clear render surface
                _renderGraphics.Clear(Color.Transparent);

                int margin = 4;
                Rectangle innerRect = new Rectangle(margin, margin, lw - margin * 2, lh - margin * 2);

                using (GraphicsPath path = new GraphicsPath())
                {
                    if (_settings.LensShape == "circle")
                    {
                        path.AddEllipse(innerRect);
                    }
                    else
                    {
                        AddRoundedRect(path, innerRect, 10);
                    }

                    // Draw zoomed screen image clipped to shape
                    using (Region clipRegion = new Region(path))
                    {
                        _renderGraphics.Clip = clipRegion;
                        _renderGraphics.DrawImage(_screenBmp, innerRect, new Rectangle(0, 0, srcW, srcH), GraphicsUnit.Pixel);
                        _renderGraphics.ResetClip();
                    }

                    // Draw crosshairs
                    using (Pen crossPen = new Pen(Color.FromArgb(90, 80, 140, 255), 1.2f))
                    {
                        using (Region clipRegion = new Region(path))
                        {
                            _renderGraphics.Clip = clipRegion;
                            _renderGraphics.DrawLine(crossPen, 0, lh / 2, lw, lh / 2);
                            _renderGraphics.DrawLine(crossPen, lw / 2, 0, lw / 2, lh);
                            _renderGraphics.ResetClip();
                        }
                    }

                    // Draw outer glow and border ring (green theme #3cdc64)
                    using (Pen glowPen = new Pen(Color.FromArgb(160, 40, 200, 80), 4.5f))
                    {
                        _renderGraphics.DrawPath(glowPen, path);
                    }
                    using (Pen borderPen = new Pen(Color.FromArgb(245, 60, 220, 100), 2.5f))
                    {
                        _renderGraphics.DrawPath(borderPen, path);
                    }
                    // Inner specular highlight
                    using (Pen innerPen = new Pen(Color.FromArgb(100, 255, 255, 255), 1.0f))
                    {
                        Rectangle specularRect = new Rectangle(margin + 1, margin + 1, lw - (margin + 1) * 2, lh - (margin + 1) * 2);
                        using (GraphicsPath innerPath = new GraphicsPath())
                        {
                            if (_settings.LensShape == "circle")
                                innerPath.AddEllipse(specularRect);
                            else
                                AddRoundedRect(innerPath, specularRect, 8);
                            _renderGraphics.DrawPath(innerPen, innerPath);
                        }
                    }
                }

                // Update layered window with per-pixel alpha bitmap
                SetBitmap(_renderBmp, lx, ly);
                _lastX = pt.X;
                _lastY = pt.Y;
            }
            catch { }
        }

        private void SetBitmap(Bitmap bitmap, int x, int y)
        {
            IntPtr screenDc = NativeMethods.GetDC(IntPtr.Zero);
            IntPtr memDc = NativeMethods.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;

            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                oldBitmap = NativeMethods.SelectObject(memDc, hBitmap);

                Size size = bitmap.Size;
                NativeMethods.POINT pointSource = new NativeMethods.POINT { X = 0, Y = 0 };
                NativeMethods.POINT topPos = new NativeMethods.POINT { X = x, Y = y };

                NativeMethods.BLENDFUNCTION blend = new NativeMethods.BLENDFUNCTION();
                blend.BlendOp = NativeMethods.AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = 255;
                blend.AlphaFormat = NativeMethods.AC_SRC_ALPHA;

                NativeMethods.UpdateLayeredWindow(
                    Handle,
                    screenDc,
                    ref topPos,
                    ref size,
                    memDc,
                    ref pointSource,
                    0,
                    ref blend,
                    NativeMethods.ULW_ALPHA
                );
            }
            finally
            {
                NativeMethods.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    NativeMethods.SelectObject(memDc, oldBitmap);
                    NativeMethods.DeleteObject(hBitmap);
                }
                NativeMethods.DeleteDC(memDc);
            }
        }

        private static void AddRoundedRect(GraphicsPath path, Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_renderTimer != null)
                {
                    _renderTimer.Stop();
                    _renderTimer.Dispose();
                    _renderTimer = null;
                }
                FreeBuffers();
            }
            base.Dispose(disposing);
        }
    }
}
