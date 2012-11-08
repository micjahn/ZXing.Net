namespace WindowsCEDemoForm
{
   partial class WindowsCEDemoForm
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

      private void InitializeComponent()
      {
         this.labDuration = new System.Windows.Forms.Label();
         this.btnSelectBarcodeImageFileForDecoding = new System.Windows.Forms.Button();
         this.txtContent = new System.Windows.Forms.TextBox();
         this.labContent = new System.Windows.Forms.Label();
         this.txtType = new System.Windows.Forms.TextBox();
         this.labBarcodeType = new System.Windows.Forms.Label();
         this.picBarcode = new System.Windows.Forms.PictureBox();
         this.btnStartDecoding = new System.Windows.Forms.Button();
         this.txtBarcodeImageFile = new System.Windows.Forms.TextBox();
         this.labBarcodeImageFile = new System.Windows.Forms.Label();
         this.btnStartEncoding = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // labDuration
         // 
         this.labDuration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.labDuration.Location = new System.Drawing.Point(132, 52);
         this.labDuration.Name = "labDuration";
         this.labDuration.Size = new System.Drawing.Size(106, 22);
         // 
         // btnSelectBarcodeImageFileForDecoding
         // 
         this.btnSelectBarcodeImageFileForDecoding.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnSelectBarcodeImageFileForDecoding.Location = new System.Drawing.Point(265, 23);
         this.btnSelectBarcodeImageFileForDecoding.Name = "btnSelectBarcodeImageFileForDecoding";
         this.btnSelectBarcodeImageFileForDecoding.Size = new System.Drawing.Size(35, 21);
         this.btnSelectBarcodeImageFileForDecoding.TabIndex = 18;
         this.btnSelectBarcodeImageFileForDecoding.Text = "...";
         this.btnSelectBarcodeImageFileForDecoding.Click += new System.EventHandler(this.btnSelectBarcodeImageFileForDecoding_Click);
         // 
         // txtContent
         // 
         this.txtContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtContent.Location = new System.Drawing.Point(132, 137);
         this.txtContent.Multiline = true;
         this.txtContent.Name = "txtContent";
         this.txtContent.Size = new System.Drawing.Size(168, 46);
         this.txtContent.TabIndex = 17;
         // 
         // labContent
         // 
         this.labContent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.labContent.Location = new System.Drawing.Point(132, 120);
         this.labContent.Name = "labContent";
         this.labContent.Size = new System.Drawing.Size(168, 14);
         this.labContent.Text = "Content";
         // 
         // txtType
         // 
         this.txtType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtType.Location = new System.Drawing.Point(132, 94);
         this.txtType.Name = "txtType";
         this.txtType.Size = new System.Drawing.Size(168, 23);
         this.txtType.TabIndex = 16;
         // 
         // labBarcodeType
         // 
         this.labBarcodeType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.labBarcodeType.Location = new System.Drawing.Point(132, 74);
         this.labBarcodeType.Name = "labBarcodeType";
         this.labBarcodeType.Size = new System.Drawing.Size(168, 17);
         this.labBarcodeType.Text = "Barcode Type";
         // 
         // picBarcode
         // 
         this.picBarcode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)));
         this.picBarcode.Location = new System.Drawing.Point(3, 52);
         this.picBarcode.Name = "picBarcode";
         this.picBarcode.Size = new System.Drawing.Size(123, 130);
         // 
         // btnStartDecoding
         // 
         this.btnStartDecoding.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnStartDecoding.Location = new System.Drawing.Point(244, 50);
         this.btnStartDecoding.Name = "btnStartDecoding";
         this.btnStartDecoding.Size = new System.Drawing.Size(56, 21);
         this.btnStartDecoding.TabIndex = 15;
         this.btnStartDecoding.Text = "Decode";
         this.btnStartDecoding.Click += new System.EventHandler(this.btnStartDecoding_Click);
         // 
         // txtBarcodeImageFile
         // 
         this.txtBarcodeImageFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtBarcodeImageFile.Location = new System.Drawing.Point(0, 23);
         this.txtBarcodeImageFile.Name = "txtBarcodeImageFile";
         this.txtBarcodeImageFile.Size = new System.Drawing.Size(257, 23);
         this.txtBarcodeImageFile.TabIndex = 14;
         this.txtBarcodeImageFile.TextChanged += new System.EventHandler(this.txtBarcodeImageFile_TextChanged);
         // 
         // labBarcodeImageFile
         // 
         this.labBarcodeImageFile.Location = new System.Drawing.Point(0, 0);
         this.labBarcodeImageFile.Name = "labBarcodeImageFile";
         this.labBarcodeImageFile.Size = new System.Drawing.Size(151, 20);
         this.labBarcodeImageFile.Text = "Barcode Image File";
         // 
         // btnStartEncoding
         // 
         this.btnStartEncoding.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnStartEncoding.Location = new System.Drawing.Point(182, 50);
         this.btnStartEncoding.Name = "btnStartEncoding";
         this.btnStartEncoding.Size = new System.Drawing.Size(56, 21);
         this.btnStartEncoding.TabIndex = 23;
         this.btnStartEncoding.Text = "Encode";
         this.btnStartEncoding.Click += new System.EventHandler(this.btnStartEncoding_Click);
         // 
         // WindowsCEDemoForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
         this.AutoScroll = true;
         this.ClientSize = new System.Drawing.Size(318, 194);
         this.Controls.Add(this.btnStartEncoding);
         this.Controls.Add(this.labDuration);
         this.Controls.Add(this.btnSelectBarcodeImageFileForDecoding);
         this.Controls.Add(this.txtContent);
         this.Controls.Add(this.labContent);
         this.Controls.Add(this.txtType);
         this.Controls.Add(this.labBarcodeType);
         this.Controls.Add(this.picBarcode);
         this.Controls.Add(this.btnStartDecoding);
         this.Controls.Add(this.txtBarcodeImageFile);
         this.Controls.Add(this.labBarcodeImageFile);
         this.Name = "WindowsCEDemoForm";
         this.Text = "WindowsCEDemo";
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Label labDuration;
      private System.Windows.Forms.Button btnSelectBarcodeImageFileForDecoding;
      private System.Windows.Forms.TextBox txtContent;
      private System.Windows.Forms.Label labContent;
      private System.Windows.Forms.TextBox txtType;
      private System.Windows.Forms.Label labBarcodeType;
      private System.Windows.Forms.PictureBox picBarcode;
      private System.Windows.Forms.Button btnStartDecoding;
      private System.Windows.Forms.TextBox txtBarcodeImageFile;
      private System.Windows.Forms.Label labBarcodeImageFile;
      private System.Windows.Forms.Button btnStartEncoding;
   }
}