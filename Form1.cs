using Gma.System.MouseKeyHook;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PoScSe
{
    public partial class MainForm : Form
    {
        private NotifyIcon notifyIcon;
        private int _screenshotCounter = 0;
        private Screenshot _screenshot = new Screenshot();
        private IKeyboardMouseEvents _globalHook;
        private IniFile _iniFile = new IniFile();
        private bool _pause = false;
        private string _screenKey;
        private string _pauseKey;

        public MainForm()
        {
            InitializeComponent();
            // Конфигурация NotifyIcon
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.Application; // Замените на свою иконку
            notifyIcon.Visible = true;
            notifyIcon.MouseClick += NotifyIcon_MouseClick;

            //Получили текущий счетчик названий
            _screenshotCounter = int.Parse(_iniFile.Read("Config", "CurrentNum"));
            _screenKey = _iniFile.Read("Config", "Screen");
            _pauseKey = _iniFile.Read("Config", "Pause");
            // Создаем комбинацию клавиш
            // Инициализация глобального хука
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyDown += GlobalHook_KeyDown;

            // Создание контекстного меню
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Открыть", null, Open_Click);
            contextMenu.Items.Add("Выход", null, Exit_Click);
            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P && e.Alt)
            {
                _pause = !_pause;
            }

            // Проверяем сочетание "Alt + NumPad1"
            if (!_pause && e.Alt && e.KeyCode == Keys.NumPad1)
            {
                // Создаем папку, если она не существует
                if (!Directory.Exists(SaveDirField.Text))
                {
                    Directory.CreateDirectory(SaveDirField.Text);
                }
                var prefix = _iniFile.Read("Config", "Prefix");
                _screenshot.TakeScreenshot(SaveDirField.Text, prefix, GetName(), false);
            }

            if (!_pause && e.KeyCode == Keys.G)
            {
                var prefix = _iniFile.Read("Config", "Prefix");
                _screenshot.TakeScreenshot(SaveDirField.Text, prefix, "image");
            }
        }

        //Получаем имя нового скриншота
        private string GetName()
        {
            if (int.Parse(_iniFile.Read("Config", "SaveType")) == 1)
            {
                return ChangeCurrentNum();
            }
            else
            {
                return DateTime.Now.ToString();
            }
        }

        // Обработчик клика на иконку
        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Open_Click(sender, e); // Открывает форму по левому клику
            }
        }

        private void Open_Click(object sender, EventArgs e)
        {
            Visible = true;
            WindowState = FormWindowState.Normal; // Восстановить состояние окна
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false; // Скрыть иконку
            _globalHook.Dispose();
            Application.Exit(); // Завершить приложение
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _globalHook.Dispose();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SaveDirField.Text = _iniFile.Read("Config", "Dir");
            var radBut = _iniFile.Read("Config", "SaveType");
            if (radBut == "1")
            {
                NumButton.Checked = true;
            }
            else
            {
                DateButton.Checked = true;
            }

            SaveConfig.Enabled = false;
            Visible = false; // Скрыть форму
            ShowInTaskbar = false; // Скрыть из панели задач
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                // Скрыть форму при сворачивании
                this.Hide();
            }
        }

        private void SaveConfig_Click(object sender, EventArgs e)
        {
            _iniFile.Write("Config", "Dir", SaveDirField.Text);

            SaveConfig.Enabled = false;
        }

        private string ChangeCurrentNum()
        {
            _screenshotCounter++;
            _iniFile.Write("Config", "CurrentNum", _screenshotCounter.ToString());
            return _screenshotCounter.ToString();
        }

        private void NumButton_MouseCaptureChanged(object sender, EventArgs e)
        {
            SaveConfig.Enabled = true;
        }

        private void DateButton_MouseCaptureChanged(object sender, EventArgs e)
        {
            SaveConfig.Enabled = true;
        }

        private void SaveDirField_TextChanged(object sender, EventArgs e)
        {
            SaveConfig.Enabled = true;
        }
    }
}
