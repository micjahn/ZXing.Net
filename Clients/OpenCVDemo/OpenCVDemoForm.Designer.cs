namespace OpenCVDemo
{
   partial class OpenCVDemoForm
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
         this.btnClose = new System.Windows.Forms.Button();
         this.labDuration = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.label4 = new System.Windows.Forms.Label();
         this.txtResult = new System.Windows.Forms.TextBox();
         this.txtResultBarcodeType = new System.Windows.Forms.TextBox();
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         this.captureButton = new System.Windows.Forms.Button();
         this.btnOpenFile = new System.Windows.Forms.Button();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.SuspendLayout();
         // 
         // btnClose
         // 
         this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnClose.Location = new System.Drawing.Point(382, 228);
         this.btnClose.Name = "btnClose";
         this.btnClose.Size = new System.Drawing.Size(107, 23);
         this.btnClose.TabIndex = 26;
         this.btnClose.Text = "Close";
         this.btnClose.UseVisualStyleBackColor = true;
         this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
         // 
         // labDuration
         // 
         this.labDuration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.labDuration.Location = new System.Drawing.Point(371, 50);
         this.labDuration.Name = "labDuration";
         this.labDuration.Size = new System.Drawing.Size(118, 23);
         this.labDuration.TabIndex = 25;
         // 
         // label3
         // 
         this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(258, 99);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(44, 13);
         this.label3.TabIndex = 24;
         this.label3.Text = "Content";
         // 
         // label4
         // 
         this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(258, 60);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(31, 13);
         this.label4.TabIndex = 23;
         this.label4.Text = "Type";
         // 
         // txtResult
         // 
         this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtResult.Location = new System.Drawing.Point(258, 115);
         this.txtResult.Multiline = true;
         this.txtResult.Name = "txtResult";
         this.txtResult.ReadOnly = true;
         this.txtResult.Size = new System.Drawing.Size(231, 107);
         this.txtResult.TabIndex = 22;
         // 
         // txtResultBarcodeType
         // 
         this.txtResultBarcodeType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.txtResultBarcodeType.Location = new System.Drawing.Point(258, 76);
         this.txtResultBarcodeType.Name = "txtResultBarcodeType";
         this.txtResultBarcodeType.ReadOnly = true;
         this.txtResultBarcodeType.Size = new System.Drawing.Size(231, 20);
         this.txtResultBarcodeType.TabIndex = 21;
         // 
         // pictureBox1
         // 
         this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.pictureBox1.Location = new System.Drawing.Point(12, 12);
         this.pictureBox1.Name = "pictureBox1";
         this.pictureBox1.Size = new System.Drawing.Size(240, 210);
         this.pictureBox1.TabIndex = 20;
         this.pictureBox1.TabStop = false;
         // 
         // captureButton
         // 
         this.captureButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.captureButton.Location = new System.Drawing.Point(382, 11);
         this.captureButton.Name = "captureButton";
         this.captureButton.Size = new System.Drawing.Size(107, 23);
         this.captureButton.TabIndex = 19;
         this.captureButton.Text = "Start Capturing";
         this.captureButton.UseVisualStyleBackColor = true;
         this.captureButton.Click += new System.EventHandler(this.captureButton_Click);
         // 
         // btnOpenFile
         // 
         this.btnOpenFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnOpenFile.Location = new System.Drawing.Point(269, 11);
         this.btnOpenFile.Name = "btnOpenFile";
         this.btnOpenFile.Size = new System.Drawing.Size(107, 23);
         this.btnOpenFile.TabIndex = 27;
         this.btnOpenFile.Text = "Open File";
         this.btnOpenFile.UseVisualStyleBackColor = true;
         this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
         // 
         // Form1
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(497, 260);
         this.Controls.Add(this.btnOpenFile);
         this.Controls.Add(this.btnClose);
         this.Controls.Add(this.labDuration);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.txtResult);
         this.Controls.Add(this.txtResultBarcodeType);
         this.Controls.Add(this.pictureBox1);
         this.Controls.Add(this.captureButton);
         this.Name = "Form1";
         this.Text = "Form1";
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button btnClose;
      private System.Windows.Forms.Label labDuration;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.TextBox txtResult;
      private System.Windows.Forms.TextBox txtResultBarcodeType;
      private System.Windows.Forms.PictureBox pictureBox1;
      private System.Windows.Forms.Button captureButton;
      private System.Windows.Forms.Button btnOpenFile;

   }
}

