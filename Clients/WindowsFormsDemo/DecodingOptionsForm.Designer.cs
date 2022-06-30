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
            this.btnOk = new System.Windows.Forms.Button();
            this.chkMultipleDecode = new System.Windows.Forms.CheckBox();
            this.chkMultipleDecodeOnlyQR = new System.Windows.Forms.CheckBox();
            this.dataGridViewBarcodeFormats = new System.Windows.Forms.DataGridView();
            this.ColumnSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnBarcodeFormat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkUseGlobalHistogramBinarizer = new System.Windows.Forms.CheckBox();
            this.propOptions = new System.Windows.Forms.PropertyGrid();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBarcodeFormats)).BeginInit();
            this.SuspendLayout();
            // 
            // chkAutoRotate
            // 
            this.chkAutoRotate.AutoSize = true;
            this.chkAutoRotate.Location = new System.Drawing.Point(354, 12);
            this.chkAutoRotate.Name = "chkAutoRotate";
            this.chkAutoRotate.Size = new System.Drawing.Size(118, 17);
            this.chkAutoRotate.TabIndex = 0;
            this.chkAutoRotate.Text = "enable Auto Rotate";
            this.chkAutoRotate.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(555, 477);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(112, 23);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // chkMultipleDecode
            // 
            this.chkMultipleDecode.AutoSize = true;
            this.chkMultipleDecode.Location = new System.Drawing.Point(354, 35);
            this.chkMultipleDecode.Name = "chkMultipleDecode";
            this.chkMultipleDecode.Size = new System.Drawing.Size(139, 17);
            this.chkMultipleDecode.TabIndex = 4;
            this.chkMultipleDecode.Text = "try to find multiple codes";
            this.chkMultipleDecode.UseVisualStyleBackColor = true;
            // 
            // chkMultipleDecodeOnlyQR
            // 
            this.chkMultipleDecodeOnlyQR.AutoSize = true;
            this.chkMultipleDecodeOnlyQR.Location = new System.Drawing.Point(383, 58);
            this.chkMultipleDecodeOnlyQR.Name = "chkMultipleDecodeOnlyQR";
            this.chkMultipleDecodeOnlyQR.Size = new System.Drawing.Size(165, 17);
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
            this.dataGridViewBarcodeFormats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridViewBarcodeFormats.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewBarcodeFormats.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnSelected,
            this.ColumnBarcodeFormat});
            this.dataGridViewBarcodeFormats.Location = new System.Drawing.Point(12, 12);
            this.dataGridViewBarcodeFormats.Name = "dataGridViewBarcodeFormats";
            this.dataGridViewBarcodeFormats.Size = new System.Drawing.Size(331, 459);
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
            // chkUseGlobalHistogramBinarizer
            // 
            this.chkUseGlobalHistogramBinarizer.AutoSize = true;
            this.chkUseGlobalHistogramBinarizer.Location = new System.Drawing.Point(354, 81);
            this.chkUseGlobalHistogramBinarizer.Name = "chkUseGlobalHistogramBinarizer";
            this.chkUseGlobalHistogramBinarizer.Size = new System.Drawing.Size(163, 17);
            this.chkUseGlobalHistogramBinarizer.TabIndex = 11;
            this.chkUseGlobalHistogramBinarizer.Text = "use GlobalHistogramBinarizer";
            this.chkUseGlobalHistogramBinarizer.UseVisualStyleBackColor = true;
            // 
            // propOptions
            // 
            this.propOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propOptions.Location = new System.Drawing.Point(354, 104);
            this.propOptions.Name = "propOptions";
            this.propOptions.Size = new System.Drawing.Size(313, 367);
            this.propOptions.TabIndex = 12;
            // 
            // DecodingOptionsForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(676, 508);
            this.Controls.Add(this.propOptions);
            this.Controls.Add(this.chkUseGlobalHistogramBinarizer);
            this.Controls.Add(this.dataGridViewBarcodeFormats);
            this.Controls.Add(this.chkMultipleDecodeOnlyQR);
            this.Controls.Add(this.chkMultipleDecode);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.chkAutoRotate);
            this.MinimumSize = new System.Drawing.Size(558, 211);
            this.Name = "DecodingOptionsForm";
            this.Text = "Options";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBarcodeFormats)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkAutoRotate;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.CheckBox chkMultipleDecode;
        private System.Windows.Forms.CheckBox chkMultipleDecodeOnlyQR;
        private System.Windows.Forms.DataGridView dataGridViewBarcodeFormats;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnSelected;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnBarcodeFormat;
        private System.Windows.Forms.CheckBox chkUseGlobalHistogramBinarizer;
        private System.Windows.Forms.PropertyGrid propOptions;
    }
}