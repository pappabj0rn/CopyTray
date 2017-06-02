using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CopyTray
{
    public partial class Form1 : Form
    {
        private IDictionary<string, string> _files;

        public Form1()
        {
            InitializeComponent();
            LoadFiles();
            AddFilesToContextMenu();
        }

        //https://stackoverflow.com/questions/357076/best-way-to-hide-a-window-from-the-alt-tab-program-switcher
        protected override CreateParams CreateParams
        {
            get
            {
                var Params = base.CreateParams;
                Params.ExStyle |= 0x80;
                return Params;
            }
        }

        private void LoadFiles()
        {
            _files = new Dictionary<string, string>();

            var currentDirectoryInfo = new DirectoryInfo(Application.StartupPath);
            var txtInfos = currentDirectoryInfo.EnumerateFiles("*.txt").ToList();
            foreach (var txtInfo in txtInfos)
            {
                using (var fileStream = File.OpenText(txtInfo.FullName))
                {
                    _files.Add(txtInfo.Name, fileStream.ReadToEnd());
                }
            }
        }

        private void AddFilesToContextMenu()
        {
            var itemOffset = contextMenu.Items.Count;

            foreach (var file in _files)
            {
                var i = contextMenu.Items.Count - itemOffset;
                contextMenu.Items.Insert(i,new ToolStripMenuItem(file.Key,null,OnFileMenuItemClicked));
            }
        }

        private void OnFileMenuItemClicked(object sender, EventArgs eventArgs)
        {
            var menuItem = (ToolStripMenuItem) sender;
            Clipboard.SetText(_files[menuItem.Text]);
        }

        private void trayIcon_Click(object sender, EventArgs e)
        {
            var args = (MouseEventArgs)e;
            if (args.Button != MouseButtons.Left) return;

            Clipboard.SetText(_files.First().Value);
        }

        private void OnExitMenuItemClicked(object sender, EventArgs e)
        {
            Close();
        }
    }
}