using Gma.System.MouseKeyHook;
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
        public MainForm()
        {
            InitializeComponent();
            // ������������ NotifyIcon
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.Application; // �������� �� ���� ������
            notifyIcon.Visible = true;
            notifyIcon.MouseClick += NotifyIcon_MouseClick;

            //�������� ������� ������� ��������
            _screenshotCounter = int.Parse(_iniFile.Read("Config", "CurrentNum"));
            // ������� ���������� ������
            // ������������� ����������� ����
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyDown += GlobalHook_KeyDown;

            // �������� ������������ ����
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("�������", null, Open_Click);
            contextMenu.Items.Add("�����", null, Exit_Click);
            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {
            // ��������� ��������� "Alt + NumPad1"
            if (e.Alt && e.KeyCode == Keys.NumPad1)
            {
                // ������� �����, ���� ��� �� ����������
                if (!Directory.Exists(SaveDirField.Text))
                {
                    Directory.CreateDirectory(SaveDirField.Text);
                }
                _screenshot.TakeScreenshot(SaveDirField.Text, GetName());
            }
        }

        //�������� ��� ������ ���������
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

        // ���������� ����� �� ������
        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Open_Click(sender, e); // ��������� ����� �� ������ �����
            }
        }

        private void Open_Click(object sender, EventArgs e)
        {
            Visible = true;
            WindowState = FormWindowState.Normal; // ������������ ��������� ����
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false; // ������ ������
            _globalHook.Dispose();
            Application.Exit(); // ��������� ����������
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
            Visible = false; // ������ �����
            ShowInTaskbar = false; // ������ �� ������ �����
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                // ������ ����� ��� ������������
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
