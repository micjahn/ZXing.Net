namespace EmguCVDemo
{
   partial class EmguCVDemoForm
   {
      /// <summary>
      /// Erforderliche Designervariable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Verwendete Ressourcen bereinigen.
      /// </summary>
      /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Vom Windows Form-Designer generierter Code

      /// <summary>
      /// Erforderliche Methode für die Designerunterstützung.
      /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
      /// </summary>
      private void InitializeComponent()
      {
         this.captureButton = new System.Windows.Forms.Button();
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         this.label3 = new System.Windows.Forms.Label();
         this.label4 = new System.Windows.Forms.Label();
         this.txtContentWebCam = new System.Windows.Forms.TextBox();
         this.txtTypeWebCam = new System.Windows.Forms.TextBox();
         this.labDuration = new System.Windows.Forms.Label();
         this.btnClose = new System.Windows.Forms.Button();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.SuspendLayout();
         // 
         // captureButton
         // 
         this.captureButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.captureButton.Location = new System.Drawing.Point(383, 12);
         this.captureButton.Name = "captureButton";
         this.captureButton.Size = new System.Drawing.Size(107, 23);
         this.captureButton.TabIndex = 3;
         this.captureButton.Text = "Start Capturing";
         this.captureButton.UseVisualStyleBackColor = true;
         this.captureButton.Click += new System.EventHandler(this.captureButton_Click);
         // 
         // pictureBox1
         // 
         this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.pictureBox1.Location = new System.Drawing.Point(13, 13);
         this.pictureBox1.Name = "pictureBox1";
         this.pictureBox1.Size = new System.Drawing.Size(240, 210);
         this.pictureBox1.TabIndex = 4;
         this.pictureBox1.TabStop = false;
         // 
         // label3
         // 
         this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(259, 100);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(44, 13);
         this.label3.TabIndex = 16;
         this.label3.Text = "Content";
         // 
         // label4
         // 
         this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(259, 61);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(31, 13);
         this.label4.TabIndex = 15;
         this.label4.Text = "Type";
         // 
         // txtContentWebCam
         // 
         this.txtContentWebCam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtContentWebCam.Location = new System.Drawing.Point(259, 116);
         this.txtContentWebCam.Multiline = true;
         this.txtContentWebCam.Name = "txtContentWebCam";
         this.txtContentWebCam.ReadOnly = true;
         this.txtContentWebCam.Size = new System.Drawing.Size(231, 107);
         this.txtContentWebCam.TabIndex = 14;
         // 
         // txtTypeWebCam
         // 
         this.txtTypeWebCam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.txtTypeWebCam.Location = new System.Drawing.Point(259, 77);
         this.txtTypeWebCam.Name = "txtTypeWebCam";
         this.txtTypeWebCam.ReadOnly = true;
         this.txtTypeWebCam.Size = new System.Drawing.Size(231, 20);
         this.txtTypeWebCam.TabIndex = 13;
         // 
         // labDuration
         // 
         this.labDuration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.labDuration.Location = new System.Drawing.Point(372, 51);
         this.labDuration.Name = "labDuration";
         this.labDuration.Size = new System.Drawing.Size(118, 23);
         this.labDuration.TabIndex = 17;
         // 
         // btnClose
         // 
         this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnClose.Location = new System.Drawing.Point(383, 229);
         this.btnClose.Name = "btnClose";
         this.btnClose.Size = new System.Drawing.Size(107, 23);
         this.btnClose.TabIndex = 18;
         this.btnClose.Text = "Close";
         this.btnClose.UseVisualStyleBackColor = true;
         this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
         // 
         // Form1
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(498, 262);
         this.Controls.Add(this.btnClose);
         this.Controls.Add(this.labDuration);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.txtContentWebCam);
         this.Controls.Add(this.txtTypeWebCam);
         this.Controls.Add(this.pictureBox1);
         this.Controls.Add(this.captureButton);
         this.Name = "Form1";
         this.Text = "Form1";
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button captureButton;
      private System.Windows.Forms.PictureBox pictureBox1;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.TextBox txtContentWebCam;
      private System.Windows.Forms.TextBox txtTypeWebCam;
      private System.Windows.Forms.Label labDuration;
      private System.Windows.Forms.Button btnClose;
   }
}

