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
         this.btnScreenCapture = new System.Windows.Forms.Button();
         this.btnExtendedResult = new System.Windows.Forms.Button();
         this.btnDecodingOptions = new System.Windows.Forms.Button();
         this.labDuration = new System.Windows.Forms.Label();
         this.labBarcodeText = new System.Windows.Forms.Label();
         this.labType = new System.Windows.Forms.Label();
         this.txtContent = new System.Windows.Forms.TextBox();
         this.txtType = new System.Windows.Forms.TextBox();
         this.btnStartDecoding = new System.Windows.Forms.Button();
         this.picBarcode = new System.Windows.Forms.PictureBox();
         this.btnSelectBarcodeImageFileForDecoding = new System.Windows.Forms.Button();
         this.tabPageEncoder = new System.Windows.Forms.TabPage();
         this.chkImageScaling = new System.Windows.Forms.CheckBox();
         this.btnEncodeOptions = new System.Windows.Forms.Button();
         this.btnEncodeDecode = new System.Windows.Forms.Button();
         this.btnEncoderSave = new System.Windows.Forms.Button();
         this.btnEncode = new System.Windows.Forms.Button();
         this.txtEncoderContent = new System.Windows.Forms.TextBox();
         this.labEncoderContent = new System.Windows.Forms.Label();
         this.labEncoderType = new System.Windows.Forms.Label();
         this.cmbEncoderType = new System.Windows.Forms.ComboBox();
         this.picEncodedBarCode = new System.Windows.Forms.PictureBox();
         this.tabPageWebCam = new System.Windows.Forms.TabPage();
         this.btnDecodeWebCam = new System.Windows.Forms.Button();
         this.label1 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.txtContentWebCam = new System.Windows.Forms.TextBox();
         this.txtTypeWebCam = new System.Windows.Forms.TextBox();
         this.picWebCam = new System.Windows.Forms.PictureBox();
         this.btnDecodingFilter = new System.Windows.Forms.Button();
         this.chkScaleDecodingImage = new System.Windows.Forms.CheckBox();
         this.tabCtrlMain.SuspendLayout();
         this.tabPageDecoder.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.picBarcode)).BeginInit();
         this.tabPageEncoder.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.picEncodedBarCode)).BeginInit();
         this.tabPageWebCam.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.picWebCam)).BeginInit();
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
         this.tabCtrlMain.Controls.Add(this.tabPageWebCam);
         this.tabCtrlMain.Location = new System.Drawing.Point(12, 12);
         this.tabCtrlMain.Name = "tabCtrlMain";
         this.tabCtrlMain.SelectedIndex = 0;
         this.tabCtrlMain.Size = new System.Drawing.Size(474, 283);
         this.tabCtrlMain.TabIndex = 3;
         // 
         // tabPageDecoder
         // 
         this.tabPageDecoder.Controls.Add(this.btnDecodingFilter);
         this.tabPageDecoder.Controls.Add(this.chkScaleDecodingImage);
         this.tabPageDecoder.Controls.Add(this.btnScreenCapture);
         this.tabPageDecoder.Controls.Add(this.btnExtendedResult);
         this.tabPageDecoder.Controls.Add(this.btnDecodingOptions);
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
         // btnScreenCapture
         // 
         this.btnScreenCapture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnScreenCapture.Location = new System.Drawing.Point(353, 3);
         this.btnScreenCapture.Name = "btnScreenCapture";
         this.btnScreenCapture.Size = new System.Drawing.Size(107, 23);
         this.btnScreenCapture.TabIndex = 13;
         this.btnScreenCapture.Text = "Screen Capture";
         this.btnScreenCapture.UseVisualStyleBackColor = true;
         this.btnScreenCapture.Click += new System.EventHandler(this.btnScreenCapture_Click);
         // 
         // btnExtendedResult
         // 
         this.btnExtendedResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnExtendedResult.Location = new System.Drawing.Point(272, 79);
         this.btnExtendedResult.Name = "btnExtendedResult";
         this.btnExtendedResult.Size = new System.Drawing.Size(75, 23);
         this.btnExtendedResult.TabIndex = 12;
         this.btnExtendedResult.Text = "Result";
         this.btnExtendedResult.UseVisualStyleBackColor = true;
         this.btnExtendedResult.Visible = false;
         this.btnExtendedResult.Click += new System.EventHandler(this.btnExtendedResult_Click);
         // 
         // btnDecodingOptions
         // 
         this.btnDecodingOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnDecodingOptions.Location = new System.Drawing.Point(353, 79);
         this.btnDecodingOptions.Name = "btnDecodingOptions";
         this.btnDecodingOptions.Size = new System.Drawing.Size(107, 23);
         this.btnDecodingOptions.TabIndex = 11;
         this.btnDecodingOptions.Text = "Options";
         this.btnDecodingOptions.UseVisualStyleBackColor = true;
         this.btnDecodingOptions.Click += new System.EventHandler(this.btnDecodingOptions_Click);
         // 
         // labDuration
         // 
         this.labDuration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.labDuration.Location = new System.Drawing.Point(229, 60);
         this.labDuration.Name = "labDuration";
         this.labDuration.Size = new System.Drawing.Size(118, 23);
         this.labDuration.TabIndex = 10;
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
         this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
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
         this.picBarcode.Size = new System.Drawing.Size(214, 174);
         this.picBarcode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
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
         this.tabPageEncoder.Controls.Add(this.chkImageScaling);
         this.tabPageEncoder.Controls.Add(this.btnEncodeOptions);
         this.tabPageEncoder.Controls.Add(this.btnEncodeDecode);
         this.tabPageEncoder.Controls.Add(this.btnEncoderSave);
         this.tabPageEncoder.Controls.Add(this.btnEncode);
         this.tabPageEncoder.Controls.Add(this.txtEncoderContent);
         this.tabPageEncoder.Controls.Add(this.labEncoderContent);
         this.tabPageEncoder.Controls.Add(this.labEncoderType);
         this.tabPageEncoder.Controls.Add(this.cmbEncoderType);
         this.tabPageEncoder.Controls.Add(this.picEncodedBarCode);
         this.tabPageEncoder.Location = new System.Drawing.Point(4, 22);
         this.tabPageEncoder.Name = "tabPageEncoder";
         this.tabPageEncoder.Padding = new System.Windows.Forms.Padding(3);
         this.tabPageEncoder.Size = new System.Drawing.Size(466, 257);
         this.tabPageEncoder.TabIndex = 1;
         this.tabPageEncoder.Text = "Encoder";
         this.tabPageEncoder.UseVisualStyleBackColor = true;
         // 
         // chkImageScaling
         // 
         this.chkImageScaling.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.chkImageScaling.AutoSize = true;
         this.chkImageScaling.Checked = true;
         this.chkImageScaling.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkImageScaling.Location = new System.Drawing.Point(7, 200);
         this.chkImageScaling.Name = "chkImageScaling";
         this.chkImageScaling.Size = new System.Drawing.Size(100, 17);
         this.chkImageScaling.TabIndex = 18;
         this.chkImageScaling.Text = "scale the image";
         this.chkImageScaling.UseVisualStyleBackColor = true;
         this.chkImageScaling.CheckedChanged += new System.EventHandler(this.chkImageScaling_CheckedChanged);
         // 
         // btnEncodeOptions
         // 
         this.btnEncodeOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnEncodeOptions.Location = new System.Drawing.Point(127, 224);
         this.btnEncodeOptions.Name = "btnEncodeOptions";
         this.btnEncodeOptions.Size = new System.Drawing.Size(107, 23);
         this.btnEncodeOptions.TabIndex = 17;
         this.btnEncodeOptions.Text = "Options";
         this.btnEncodeOptions.UseVisualStyleBackColor = true;
         this.btnEncodeOptions.Click += new System.EventHandler(this.btnEncodeOptions_Click);
         // 
         // btnEncodeDecode
         // 
         this.btnEncodeDecode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.btnEncodeDecode.Location = new System.Drawing.Point(3, 224);
         this.btnEncodeDecode.Name = "btnEncodeDecode";
         this.btnEncodeDecode.Size = new System.Drawing.Size(107, 23);
         this.btnEncodeDecode.TabIndex = 16;
         this.btnEncodeDecode.Text = "Decode";
         this.btnEncodeDecode.UseVisualStyleBackColor = true;
         this.btnEncodeDecode.Click += new System.EventHandler(this.btnEncodeDecode_Click);
         // 
         // btnEncoderSave
         // 
         this.btnEncoderSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnEncoderSave.Location = new System.Drawing.Point(240, 224);
         this.btnEncoderSave.Name = "btnEncoderSave";
         this.btnEncoderSave.Size = new System.Drawing.Size(107, 23);
         this.btnEncoderSave.TabIndex = 15;
         this.btnEncoderSave.Text = "Save";
         this.btnEncoderSave.UseVisualStyleBackColor = true;
         this.btnEncoderSave.Click += new System.EventHandler(this.btnEncoderSave_Click);
         // 
         // btnEncode
         // 
         this.btnEncode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnEncode.Location = new System.Drawing.Point(353, 224);
         this.btnEncode.Name = "btnEncode";
         this.btnEncode.Size = new System.Drawing.Size(107, 23);
         this.btnEncode.TabIndex = 14;
         this.btnEncode.Text = "Encode";
         this.btnEncode.UseVisualStyleBackColor = true;
         this.btnEncode.Click += new System.EventHandler(this.btnEncode_Click);
         // 
         // txtEncoderContent
         // 
         this.txtEncoderContent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtEncoderContent.Location = new System.Drawing.Point(229, 63);
         this.txtEncoderContent.Multiline = true;
         this.txtEncoderContent.Name = "txtEncoderContent";
         this.txtEncoderContent.Size = new System.Drawing.Size(231, 155);
         this.txtEncoderContent.TabIndex = 13;
         // 
         // labEncoderContent
         // 
         this.labEncoderContent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.labEncoderContent.AutoSize = true;
         this.labEncoderContent.Location = new System.Drawing.Point(226, 47);
         this.labEncoderContent.Name = "labEncoderContent";
         this.labEncoderContent.Size = new System.Drawing.Size(44, 13);
         this.labEncoderContent.TabIndex = 12;
         this.labEncoderContent.Text = "Content";
         // 
         // labEncoderType
         // 
         this.labEncoderType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.labEncoderType.AutoSize = true;
         this.labEncoderType.Location = new System.Drawing.Point(226, 7);
         this.labEncoderType.Name = "labEncoderType";
         this.labEncoderType.Size = new System.Drawing.Size(31, 13);
         this.labEncoderType.TabIndex = 11;
         this.labEncoderType.Text = "Type";
         // 
         // cmbEncoderType
         // 
         this.cmbEncoderType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.cmbEncoderType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cmbEncoderType.FormattingEnabled = true;
         this.cmbEncoderType.Location = new System.Drawing.Point(229, 23);
         this.cmbEncoderType.Name = "cmbEncoderType";
         this.cmbEncoderType.Size = new System.Drawing.Size(231, 21);
         this.cmbEncoderType.TabIndex = 10;
         // 
         // picEncodedBarCode
         // 
         this.picEncodedBarCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.picEncodedBarCode.BackColor = System.Drawing.SystemColors.InactiveBorder;
         this.picEncodedBarCode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.picEncodedBarCode.Location = new System.Drawing.Point(3, 6);
         this.picEncodedBarCode.Name = "picEncodedBarCode";
         this.picEncodedBarCode.Size = new System.Drawing.Size(214, 188);
         this.picEncodedBarCode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
         this.picEncodedBarCode.TabIndex = 9;
         this.picEncodedBarCode.TabStop = false;
         // 
         // tabPageWebCam
         // 
         this.tabPageWebCam.Controls.Add(this.btnDecodeWebCam);
         this.tabPageWebCam.Controls.Add(this.label1);
         this.tabPageWebCam.Controls.Add(this.label2);
         this.tabPageWebCam.Controls.Add(this.txtContentWebCam);
         this.tabPageWebCam.Controls.Add(this.txtTypeWebCam);
         this.tabPageWebCam.Controls.Add(this.picWebCam);
         this.tabPageWebCam.Location = new System.Drawing.Point(4, 22);
         this.tabPageWebCam.Name = "tabPageWebCam";
         this.tabPageWebCam.Size = new System.Drawing.Size(466, 257);
         this.tabPageWebCam.TabIndex = 2;
         this.tabPageWebCam.Text = "WebCam";
         this.tabPageWebCam.UseVisualStyleBackColor = true;
         // 
         // btnDecodeWebCam
         // 
         this.btnDecodeWebCam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnDecodeWebCam.Location = new System.Drawing.Point(347, 57);
         this.btnDecodeWebCam.Name = "btnDecodeWebCam";
         this.btnDecodeWebCam.Size = new System.Drawing.Size(107, 23);
         this.btnDecodeWebCam.TabIndex = 13;
         this.btnDecodeWebCam.Text = "Decode";
         this.btnDecodeWebCam.UseVisualStyleBackColor = true;
         this.btnDecodeWebCam.Click += new System.EventHandler(this.btnDecodeWebCam_Click);
         // 
         // label1
         // 
         this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(223, 131);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(44, 13);
         this.label1.TabIndex = 12;
         this.label1.Text = "Content";
         // 
         // label2
         // 
         this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(223, 92);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(31, 13);
         this.label2.TabIndex = 11;
         this.label2.Text = "Type";
         // 
         // txtContentWebCam
         // 
         this.txtContentWebCam.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtContentWebCam.Location = new System.Drawing.Point(223, 147);
         this.txtContentWebCam.Multiline = true;
         this.txtContentWebCam.Name = "txtContentWebCam";
         this.txtContentWebCam.Size = new System.Drawing.Size(231, 107);
         this.txtContentWebCam.TabIndex = 10;
         // 
         // txtTypeWebCam
         // 
         this.txtTypeWebCam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.txtTypeWebCam.Location = new System.Drawing.Point(223, 108);
         this.txtTypeWebCam.Name = "txtTypeWebCam";
         this.txtTypeWebCam.Size = new System.Drawing.Size(231, 20);
         this.txtTypeWebCam.TabIndex = 9;
         // 
         // picWebCam
         // 
         this.picWebCam.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.picWebCam.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.picWebCam.Location = new System.Drawing.Point(3, 57);
         this.picWebCam.Name = "picWebCam";
         this.picWebCam.Size = new System.Drawing.Size(214, 197);
         this.picWebCam.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
         this.picWebCam.TabIndex = 8;
         this.picWebCam.TabStop = false;
         // 
         // btnDecodingFilter
         // 
         this.btnDecodingFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnDecodingFilter.Location = new System.Drawing.Point(148, 230);
         this.btnDecodingFilter.Name = "btnDecodingFilter";
         this.btnDecodingFilter.Size = new System.Drawing.Size(75, 23);
         this.btnDecodingFilter.TabIndex = 4;
         this.btnDecodingFilter.Text = "Filter";
         this.btnDecodingFilter.UseVisualStyleBackColor = true;
         this.btnDecodingFilter.Click += new System.EventHandler(this.button1_Click);
         // 
         // chkScaleDecodingImage
         // 
         this.chkScaleDecodingImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.chkScaleDecodingImage.AutoSize = true;
         this.chkScaleDecodingImage.Location = new System.Drawing.Point(9, 234);
         this.chkScaleDecodingImage.Name = "chkScaleDecodingImage";
         this.chkScaleDecodingImage.Size = new System.Drawing.Size(100, 17);
         this.chkScaleDecodingImage.TabIndex = 19;
         this.chkScaleDecodingImage.Text = "scale the image";
         this.chkScaleDecodingImage.UseVisualStyleBackColor = true;
         this.chkScaleDecodingImage.CheckedChanged += new System.EventHandler(this.chkScaleDecodingImage_CheckedChanged);
         // 
         // WindowsFormsDemoForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
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
         this.tabPageEncoder.ResumeLayout(false);
         this.tabPageEncoder.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.picEncodedBarCode)).EndInit();
         this.tabPageWebCam.ResumeLayout(false);
         this.tabPageWebCam.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.picWebCam)).EndInit();
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
      private System.Windows.Forms.TabPage tabPageWebCam;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox txtContentWebCam;
      private System.Windows.Forms.TextBox txtTypeWebCam;
      private System.Windows.Forms.PictureBox picWebCam;
      private System.Windows.Forms.Button btnDecodeWebCam;
      private System.Windows.Forms.Button btnEncode;
      private System.Windows.Forms.TextBox txtEncoderContent;
      private System.Windows.Forms.Label labEncoderContent;
      private System.Windows.Forms.Label labEncoderType;
      private System.Windows.Forms.ComboBox cmbEncoderType;
      private System.Windows.Forms.PictureBox picEncodedBarCode;
      private System.Windows.Forms.Button btnEncoderSave;
      private System.Windows.Forms.Button btnEncodeDecode;
      private System.Windows.Forms.Button btnEncodeOptions;
      private System.Windows.Forms.Button btnDecodingOptions;
      private System.Windows.Forms.Button btnExtendedResult;
      private System.Windows.Forms.Button btnScreenCapture;
      private System.Windows.Forms.CheckBox chkImageScaling;
      private System.Windows.Forms.Button btnDecodingFilter;
      private System.Windows.Forms.CheckBox chkScaleDecodingImage;
   }
}

