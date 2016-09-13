namespace WindowsFormsDemo
{
   partial class DecodingOptionsForm
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
         this.chkAutoRotate = new System.Windows.Forms.CheckBox();
         this.chkTryHarder = new System.Windows.Forms.CheckBox();
         this.chkTryInverted = new System.Windows.Forms.CheckBox();
         this.btnOk = new System.Windows.Forms.Button();
         this.chkPureBarcode = new System.Windows.Forms.CheckBox();
         this.chkMultipleDecode = new System.Windows.Forms.CheckBox();
         this.chkMultipleDecodeOnlyQR = new System.Windows.Forms.CheckBox();
         this.dataGridViewBarcodeFormats = new System.Windows.Forms.DataGridView();
         this.ColumnSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
         this.ColumnBarcodeFormat = new System.Windows.Forms.DataGridViewTextBoxColumn();
         ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBarcodeFormats)).BeginInit();
         this.SuspendLayout();
         // 
         // chkAutoRotate
         // 
         this.chkAutoRotate.AutoSize = true;
         this.chkAutoRotate.Location = new System.Drawing.Point(22, 22);
         this.chkAutoRotate.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.chkAutoRotate.Name = "chkAutoRotate";
         this.chkAutoRotate.Size = new System.Drawing.Size(204, 29);
         this.chkAutoRotate.TabIndex = 0;
         this.chkAutoRotate.Text = "enable Auto Rotate";
         this.chkAutoRotate.UseVisualStyleBackColor = true;
         // 
         // chkTryHarder
         // 
         this.chkTryHarder.AutoSize = true;
         this.chkTryHarder.Location = new System.Drawing.Point(22, 65);
         this.chkTryHarder.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.chkTryHarder.Name = "chkTryHarder";
         this.chkTryHarder.Size = new System.Drawing.Size(195, 29);
         this.chkTryHarder.TabIndex = 1;
         this.chkTryHarder.Text = "enable Try Harder";
         this.chkTryHarder.UseVisualStyleBackColor = true;
         // 
         // chkTryInverted
         // 
         this.chkTryInverted.AutoSize = true;
         this.chkTryInverted.Location = new System.Drawing.Point(22, 107);
         this.chkTryInverted.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.chkTryInverted.Name = "chkTryInverted";
         this.chkTryInverted.Size = new System.Drawing.Size(206, 29);
         this.chkTryInverted.TabIndex = 2;
         this.chkTryInverted.Text = "enable Try Inverted";
         this.chkTryInverted.UseVisualStyleBackColor = true;
         // 
         // btnOk
         // 
         this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnOk.Location = new System.Drawing.Point(766, 294);
         this.btnOk.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.btnOk.Name = "btnOk";
         this.btnOk.Size = new System.Drawing.Size(205, 42);
         this.btnOk.TabIndex = 6;
         this.btnOk.Text = "Ok";
         this.btnOk.UseVisualStyleBackColor = true;
         this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
         // 
         // chkPureBarcode
         // 
         this.chkPureBarcode.AutoSize = true;
         this.chkPureBarcode.Location = new System.Drawing.Point(22, 150);
         this.chkPureBarcode.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.chkPureBarcode.Name = "chkPureBarcode";
         this.chkPureBarcode.Size = new System.Drawing.Size(252, 29);
         this.chkPureBarcode.TabIndex = 3;
         this.chkPureBarcode.Text = "image has Pure Barcode";
         this.chkPureBarcode.UseVisualStyleBackColor = true;
         // 
         // chkMultipleDecode
         // 
         this.chkMultipleDecode.AutoSize = true;
         this.chkMultipleDecode.Location = new System.Drawing.Point(22, 192);
         this.chkMultipleDecode.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.chkMultipleDecode.Name = "chkMultipleDecode";
         this.chkMultipleDecode.Size = new System.Drawing.Size(245, 29);
         this.chkMultipleDecode.TabIndex = 4;
         this.chkMultipleDecode.Text = "try to find multiple codes";
         this.chkMultipleDecode.UseVisualStyleBackColor = true;
         // 
         // chkMultipleDecodeOnlyQR
         // 
         this.chkMultipleDecodeOnlyQR.AutoSize = true;
         this.chkMultipleDecodeOnlyQR.Location = new System.Drawing.Point(55, 234);
         this.chkMultipleDecodeOnlyQR.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.chkMultipleDecodeOnlyQR.Name = "chkMultipleDecodeOnlyQR";
         this.chkMultipleDecodeOnlyQR.Size = new System.Drawing.Size(298, 29);
         this.chkMultipleDecodeOnlyQR.TabIndex = 5;
         this.chkMultipleDecodeOnlyQR.Text = "only QR Codes (special case)";
         this.chkMultipleDecodeOnlyQR.UseVisualStyleBackColor = true;
         // 
         // dataGridViewBarcodeFormats
         // 
         this.dataGridViewBarcodeFormats.AllowUserToAddRows = false;
         this.dataGridViewBarcodeFormats.AllowUserToDeleteRows = false;
         this.dataGridViewBarcodeFormats.AllowUserToResizeColumns = false;
         this.dataGridViewBarcodeFormats.AllowUserToResizeRows = false;
         this.dataGridViewBarcodeFormats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.dataGridViewBarcodeFormats.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
         this.dataGridViewBarcodeFormats.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnSelected,
            this.ColumnBarcodeFormat});
         this.dataGridViewBarcodeFormats.Location = new System.Drawing.Point(369, 22);
         this.dataGridViewBarcodeFormats.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.dataGridViewBarcodeFormats.Name = "dataGridViewBarcodeFormats";
         this.dataGridViewBarcodeFormats.Size = new System.Drawing.Size(607, 260);
         this.dataGridViewBarcodeFormats.TabIndex = 7;
         // 
         // ColumnSelected
         // 
         this.ColumnSelected.Frozen = true;
         this.ColumnSelected.HeaderText = "Selected";
         this.ColumnSelected.Name = "ColumnSelected";
         this.ColumnSelected.Width = 60;
         // 
         // ColumnBarcodeFormat
         // 
         this.ColumnBarcodeFormat.Frozen = true;
         this.ColumnBarcodeFormat.HeaderText = "Barcode format";
         this.ColumnBarcodeFormat.Name = "ColumnBarcodeFormat";
         this.ColumnBarcodeFormat.ReadOnly = true;
         this.ColumnBarcodeFormat.Resizable = System.Windows.Forms.DataGridViewTriState.True;
         this.ColumnBarcodeFormat.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
         this.ColumnBarcodeFormat.Width = 300;
         // 
         // DecodingOptionsForm
         // 
         this.AcceptButton = this.btnOk;
         this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.AutoSize = true;
         this.ClientSize = new System.Drawing.Size(986, 351);
         this.Controls.Add(this.dataGridViewBarcodeFormats);
         this.Controls.Add(this.chkMultipleDecodeOnlyQR);
         this.Controls.Add(this.chkMultipleDecode);
         this.Controls.Add(this.chkPureBarcode);
         this.Controls.Add(this.btnOk);
         this.Controls.Add(this.chkTryInverted);
         this.Controls.Add(this.chkTryHarder);
         this.Controls.Add(this.chkAutoRotate);
         this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.MinimumSize = new System.Drawing.Size(1010, 356);
         this.Name = "DecodingOptionsForm";
         this.Text = "Options";
         ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBarcodeFormats)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.CheckBox chkAutoRotate;
      private System.Windows.Forms.CheckBox chkTryHarder;
      private System.Windows.Forms.CheckBox chkTryInverted;
      private System.Windows.Forms.Button btnOk;
      private System.Windows.Forms.CheckBox chkPureBarcode;
      private System.Windows.Forms.CheckBox chkMultipleDecode;
      private System.Windows.Forms.CheckBox chkMultipleDecodeOnlyQR;
      private System.Windows.Forms.DataGridView dataGridViewBarcodeFormats;
      private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnSelected;
      private System.Windows.Forms.DataGridViewTextBoxColumn ColumnBarcodeFormat;
   }
}