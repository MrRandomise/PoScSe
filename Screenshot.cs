using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PoScSe
{
    public class Screenshot
    {

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern void ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }


        public void TakeScreenshot(string saveDirectory, string prefix, string name)
        {
            IntPtr hWnd = GetForegroundWindow();
            RECT rect;
            GetWindowRect(hWnd, out rect);

            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            using (Bitmap bitmap = new Bitmap(width, height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    IntPtr hdc = GetDC(hWnd);
                    graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
                    ReleaseDC(hWnd, hdc);
                }
                string fileName = Path.Combine(saveDirectory, $"{prefix}-screenshot_{name}.png");
                bitmap.Save(fileName, ImageFormat.Png);
                Console.WriteLine($"Скриншот сохранен: {fileName}");
            }
        }

    }
}
