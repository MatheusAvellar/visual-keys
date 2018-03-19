using System;
using System.Windows.Forms;

namespace VisualKeys {
    public partial class VisualKeyWindow : Form {
        private const int WM_NCHITTEST = 0x84;
        private const int HT_CAPTION = 0x2;
        
        public VisualKeyWindow() {
            InitializeComponent();

            if(IsKeyLocked(Keys.NumLock))
                numLockOn.ForeColor = System.Drawing.Color.CornflowerBlue;
            else
                numLockOn.ForeColor = System.Drawing.Color.LightGray;

            if(IsKeyLocked(Keys.CapsLock))
                capsLockOn.ForeColor = System.Drawing.Color.CornflowerBlue;
            else
                capsLockOn.ForeColor = System.Drawing.Color.LightGray;

            if(IsKeyLocked(Keys.Scroll))
                scrollLockOn.ForeColor = System.Drawing.Color.CornflowerBlue;
            else
                scrollLockOn.ForeColor = System.Drawing.Color.LightGray;
        }

        protected override void WndProc(ref Message m) {
            base.WndProc(ref m);
            if(m.Msg == WM_NCHITTEST)
                m.Result = (IntPtr)(HT_CAPTION);
        }
    }
}
