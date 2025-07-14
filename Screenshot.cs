using System.Drawing.Imaging;
using System.Runtime.InteropServices;
//using Telegram.Bot;
//using Telegram.Bot.Types.InputFiles;
//using TL;

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

        //private readonly TelegramBotClient _botClient = new TelegramBotClient("7302927750:AAHRDYuEMbxMrMxfiatlwYoWtq9hVRYLXTI");
        //private readonly long _chatId = 1302058677;
        public UserTelegram _userTelegram = new UserTelegram();

        public Screenshot()
        {
            TakeUser();
        }

        public async void TakeScreenshot(string saveDirectory, string prefix, string name, bool bot = true)
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
                //var file = new InputOnlineFile(stream, "screenshot.png");
                if (bot)
                {
                    using var ms = new MemoryStream();
                    bitmap.Save(ms, ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);
                    await _userTelegram.SendMessage(ms);
                    Console.WriteLine("Скриншот отправлен в Telegram.");
                }
                else
                {
                    string fileName = Path.Combine(saveDirectory, $"{prefix}-screenshot_{name}.png");
                    bitmap.Save(fileName, ImageFormat.Png);
                    Console.WriteLine($"Скриншот сохранен: {fileName}");
                }
            }
            //await _botClient.SendPhotoAsync(_chatId, file);
        }

        private async void TakeUser()
        {
            await _userTelegram.TakeUserTelegram();
        }
    }
}
