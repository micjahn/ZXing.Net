namespace WindowsFormsDemo
{
   partial class WindowsFormsDemoForm
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
         this.txtBarcodeImageFile = new System.Windows.Forms.TextBox();
         this.labBarcodeImageFile = new System.Windows.Forms.Label();
         this.tabCtrlMain = new System.Windows.Forms.TabControl();
         this.tabPageDecoder = new System.Windows.Forms.TabPage();
         this.labBarcodeText = new System.Windows.Forms.Label();
         this.labType = new System.Windows.Forms.Label();
         this.txtContent = new System.Windows.Forms.TextBox();
         this.txtType = new System.Windows.Forms.TextBox();
         this.btnStartDecoding = new System.Windows.Forms.Button();
         this.picBarcode = new System.Windows.Forms.PictureBox();
         this.btnSelectBarcodeImageFileForDecoding = new System.Windows.Forms.Button();
         this.tabPageEncoder = new System.Windows.Forms.TabPage();
         this.labDuration = new System.Windows.Forms.Label();
         this.tabCtrlMain.SuspendLayout();
         this.tabPageDecoder.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.picBarcode)).BeginInit();
         this.SuspendLayout();
         // 
         // btnClose
         // 
         this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnClose.Location = new System.Drawing.Point(379, 301);
         this.btnClose.Name = "btnClose";
         this.btnClose.Size = new System.Drawing.Size(107, 23);
         this.btnClose.TabIndex = 0;
         this.btnClose.Text = "&Close";
         this.btnClose.UseVisualStyleBackColor = true;
         this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
         // 
         // txtBarcodeImageFile
         // 
         this.txtBarcodeImageFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtBarcodeImageFile.Location = new System.Drawing.Point(9, 28);
         this.txtBarcodeImageFile.Name = "txtBarcodeImageFile";
         this.txtBarcodeImageFile.Size = new System.Drawing.Size(419, 20);
         this.txtBarcodeImageFile.TabIndex = 1;
         this.txtBarcodeImageFile.TextChanged += new System.EventHandler(this.txtBarcodeImageFile_TextChanged);
         // 
         // labBarcodeImageFile
         // 
         this.labBarcodeImageFile.AutoSize = true;
         this.labBarcodeImageFile.Location = new System.Drawing.Point(6, 12);
         this.labBarcodeImageFile.Name = "labBarcodeImageFile";
         this.labBarcodeImageFile.Size = new System.Drawing.Size(98, 13);
         this.labBarcodeImageFile.TabIndex = 2;
         this.labBarcodeImageFile.Text = "Barcode Image File";
         // 
         // tabCtrlMain
         // 
         this.tabCtrlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.tabCtrlMain.Controls.Add(this.tabPageDecoder);
         this.tabCtrlMain.Controls.Add(this.tabPageEncoder);
         this.tabCtrlMain.Location = new System.Drawing.Point(12, 12);
         this.tabCtrlMain.Name = "tabCtrlMain";
         this.tabCtrlMain.SelectedIndex = 0;
         this.tabCtrlMain.Size = new System.Drawing.Size(474, 283);
         this.tabCtrlMain.TabIndex = 3;
         // 
         // tabPageDecoder
         // 
         this.tabPageDecoder.Controls.Add(this.labDuration);
         this.tabPageDecoder.Controls.Add(this.labBarcodeText);
         this.tabPageDecoder.Controls.Add(this.labType);
         this.tabPageDecoder.Controls.Add(this.txtContent);
         this.tabPageDecoder.Controls.Add(this.txtType);
         this.tabPageDecoder.Controls.Add(this.btnStartDecoding);
         this.tabPageDecoder.Controls.Add(this.picBarcode);
         this.tabPageDecoder.Controls.Add(this.btnSelectBarcodeImageFileForDecoding);
         this.tabPageDecoder.Controls.Add(this.labBarcodeImageFile);
         this.tabPageDecoder.Controls.Add(this.txtBarcodeImageFile);
         this.tabPageDecoder.Location = new System.Drawing.Point(4, 22);
         this.tabPageDecoder.Name = "tabPageDecoder";
         this.tabPageDecoder.Padding = new System.Windows.Forms.Padding(3);
         this.tabPageDecoder.Size = new System.Drawing.Size(466, 257);
         this.tabPageDecoder.TabIndex = 0;
         this.tabPageDecoder.Text = "Decoder";
         this.tabPageDecoder.UseVisualStyleBackColor = true;
         // 
         // labBarcodeText
         // 
         this.labBarcodeText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.labBarcodeText.AutoSize = true;
         this.labBarcodeText.Location = new System.Drawing.Point(229, 128);
         this.labBarcodeText.Name = "labBarcodeText";
         this.labBarcodeText.Size = new System.Drawing.Size(44, 13);
         this.labBarcodeText.TabIndex = 9;
         this.labBarcodeText.Text = "Content";
         // 
         // labType
         // 
         this.labType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.labType.AutoSize = true;
         this.labType.Location = new System.Drawing.Point(229, 89);
         this.labType.Name = "labType";
         this.labType.Size = new System.Drawing.Size(31, 13);
         this.labType.TabIndex = 8;
         this.labType.Text = "Type";
         // 
         // txtContent
         // 
         this.txtContent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtContent.Location = new System.Drawing.Point(229, 144);
         this.txtContent.Multiline = true;
         this.txtContent.Name = "txtContent";
         this.txtContent.Size = new System.Drawing.Size(231, 107);
         this.txtContent.TabIndex = 7;
         // 
         // txtType
         // 
         this.txtType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.txtType.Location = new System.Drawing.Point(229, 105);
         this.txtType.Name = "txtType";
         this.txtType.Size = new System.Drawing.Size(231, 20);
         this.txtType.TabIndex = 6;
         // 
         // btnStartDecoding
         // 
         this.btnStartDecoding.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnStartDecoding.Location = new System.Drawing.Point(353, 55);
         this.btnStartDecoding.Name = "btnStartDecoding";
         this.btnStartDecoding.Size = new System.Drawing.Size(107, 23);
         this.btnStartDecoding.TabIndex = 5;
         this.btnStartDecoding.Text = "Decode";
         this.btnStartDecoding.UseVisualStyleBackColor = true;
         this.btnStartDecoding.Click += new System.EventHandler(this.btnStartDecoding_Click);
         // 
         // picBarcode
         // 
         this.picBarcode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.picBarcode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.picBarcode.Location = new System.Drawing.Point(9, 54);
         this.picBarcode.Name = "picBarcode";
         this.picBarcode.Size = new System.Drawing.Size(214, 197);
         this.picBarcode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
         this.picBarcode.TabIndex = 4;
         this.picBarcode.TabStop = false;
         // 
         // btnSelectBarcodeImageFileForDecoding
         // 
         this.btnSelectBarcodeImageFileForDecoding.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnSelectBarcodeImageFileForDecoding.Location = new System.Drawing.Point(434, 26);
         this.btnSelectBarcodeImageFileForDecoding.Name = "btnSelectBarcodeImageFileForDecoding";
         this.btnSelectBarcodeImageFileForDecoding.Size = new System.Drawing.Size(26, 23);
         this.btnSelectBarcodeImageFileForDecoding.TabIndex = 3;
         this.btnSelectBarcodeImageFileForDecoding.Text = "...";
         this.btnSelectBarcodeImageFileForDecoding.UseVisualStyleBackColor = true;
         this.btnSelectBarcodeImageFileForDecoding.Click += new System.EventHandler(this.btnSelectBarcodeImageFileForDecoding_Click);
         // 
         // tabPageEncoder
         // 
         this.tabPageEncoder.Location = new System.Drawing.Point(4, 22);
         this.tabPageEncoder.Name = "tabPageEncoder";
         this.tabPageEncoder.Padding = new System.Windows.Forms.Padding(3);
         this.tabPageEncoder.Size = new System.Drawing.Size(466, 257);
         this.tabPageEncoder.TabIndex = 1;
         this.tabPageEncoder.Text = "Encoder";
         this.tabPageEncoder.UseVisualStyleBackColor = true;
         // 
         // labDuration
         // 
         this.labDuration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.labDuration.Location = new System.Drawing.Point(229, 60);
         this.labDuration.Name = "labDuration";
         this.labDuration.Size = new System.Drawing.Size(118, 23);
         this.labDuration.TabIndex = 10;
         // 
         // WindowsFormsDemoForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.btnClose;
         this.ClientSize = new System.Drawing.Size(498, 336);
         this.Controls.Add(this.tabCtrlMain);
         this.Controls.Add(this.btnClose);
         this.MinimumSize = new System.Drawing.Size(514, 374);
         this.Name = "WindowsFormsDemoForm";
         this.Text = "WindowsFormsDemo";
         this.tabCtrlMain.ResumeLayout(false);
         this.tabPageDecoder.ResumeLayout(false);
         this.tabPageDecoder.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.picBarcode)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Button btnClose;
      private System.Windows.Forms.TextBox txtBarcodeImageFile;
      private System.Windows.Forms.Label labBarcodeImageFile;
      private System.Windows.Forms.TabControl tabCtrlMain;
      private System.Windows.Forms.TabPage tabPageDecoder;
      private System.Windows.Forms.TabPage tabPageEncoder;
      private System.Windows.Forms.Button btnSelectBarcodeImageFileForDecoding;
      private System.Windows.Forms.Button btnStartDecoding;
      private System.Windows.Forms.PictureBox picBarcode;
      private System.Windows.Forms.Label labType;
      private System.Windows.Forms.TextBox txtContent;
      private System.Windows.Forms.TextBox txtType;
      private System.Windows.Forms.Label labBarcodeText;
      private System.Windows.Forms.Label labDuration;
   }
}

