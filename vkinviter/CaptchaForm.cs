using System;
using System.Drawing;
using System.Windows.Forms;

namespace vkinviter
{
    public partial class CaptchaForm : Form
    {
        public CaptchaForm()
        {
            InitializeComponent();
        }
        public CaptchaForm(Bitmap picture)
            : this()
        {
            pictureCaptcha.Image = picture;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            CaptchaContainer.Key = captchaInput.Text;
            Close();
        }

        private void captchaInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CaptchaContainer.Key = captchaInput.Text;
                Close();
            }
        }
    }
}
