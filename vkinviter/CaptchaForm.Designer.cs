namespace vkinviter
{
    partial class CaptchaForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonOK = new System.Windows.Forms.Button();
            this.captchaInput = new System.Windows.Forms.TextBox();
            this.pictureCaptcha = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCaptcha)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(149, 190);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(88, 32);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // captchaInput
            // 
            this.captchaInput.Location = new System.Drawing.Point(13, 190);
            this.captchaInput.Name = "captchaInput";
            this.captchaInput.Size = new System.Drawing.Size(100, 26);
            this.captchaInput.TabIndex = 4;
            this.captchaInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.captchaInput_KeyDown);
            // 
            // pictureCaptcha
            // 
            this.pictureCaptcha.Location = new System.Drawing.Point(13, 28);
            this.pictureCaptcha.Name = "pictureCaptcha";
            this.pictureCaptcha.Size = new System.Drawing.Size(224, 122);
            this.pictureCaptcha.TabIndex = 3;
            this.pictureCaptcha.TabStop = false;
            // 
            // CaptchaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.captchaInput);
            this.Controls.Add(this.pictureCaptcha);
            this.Name = "CaptchaForm";
            this.Text = "Captcha";
            ((System.ComponentModel.ISupportInitialize)(this.pictureCaptcha)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox captchaInput;
        private System.Windows.Forms.PictureBox pictureCaptcha;
    }
}