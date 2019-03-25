﻿namespace WindowsFormsDemo
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
            this.chkCode39ExtendedMode = new System.Windows.Forms.CheckBox();
            this.chkCode39ExtendedModeRelaxed = new System.Windows.Forms.CheckBox();
            this.chkCode39CheckDigit = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBarcodeFormats)).BeginInit();
            this.SuspendLayout();
            // 
            // chkAutoRotate
            // 
            this.chkAutoRotate.AutoSize = true;
            this.chkAutoRotate.Location = new System.Drawing.Point(12, 12);
            this.chkAutoRotate.Name = "chkAutoRotate";
            this.chkAutoRotate.Size = new System.Drawing.Size(118, 17);
            this.chkAutoRotate.TabIndex = 0;
            this.chkAutoRotate.Text = "enable Auto Rotate";
            this.chkAutoRotate.UseVisualStyleBackColor = true;
            // 
            // chkTryHarder
            // 
            this.chkTryHarder.AutoSize = true;
            this.chkTryHarder.Location = new System.Drawing.Point(12, 35);
            this.chkTryHarder.Name = "chkTryHarder";
            this.chkTryHarder.Size = new System.Drawing.Size(111, 17);
            this.chkTryHarder.TabIndex = 1;
            this.chkTryHarder.Text = "enable Try Harder";
            this.chkTryHarder.UseVisualStyleBackColor = true;
            // 
            // chkTryInverted
            // 
            this.chkTryInverted.AutoSize = true;
            this.chkTryInverted.Location = new System.Drawing.Point(12, 58);
            this.chkTryInverted.Name = "chkTryInverted";
            this.chkTryInverted.Size = new System.Drawing.Size(118, 17);
            this.chkTryInverted.TabIndex = 2;
            this.chkTryInverted.Text = "enable Try Inverted";
            this.chkTryInverted.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(418, 251);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(112, 23);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // chkPureBarcode
            // 
            this.chkPureBarcode.AutoSize = true;
            this.chkPureBarcode.Location = new System.Drawing.Point(12, 81);
            this.chkPureBarcode.Name = "chkPureBarcode";
            this.chkPureBarcode.Size = new System.Drawing.Size(142, 17);
            this.chkPureBarcode.TabIndex = 3;
            this.chkPureBarcode.Text = "image has Pure Barcode";
            this.chkPureBarcode.UseVisualStyleBackColor = true;
            // 
            // chkMultipleDecode
            // 
            this.chkMultipleDecode.AutoSize = true;
            this.chkMultipleDecode.Location = new System.Drawing.Point(12, 104);
            this.chkMultipleDecode.Name = "chkMultipleDecode";
            this.chkMultipleDecode.Size = new System.Drawing.Size(139, 17);
            this.chkMultipleDecode.TabIndex = 4;
            this.chkMultipleDecode.Text = "try to find multiple codes";
            this.chkMultipleDecode.UseVisualStyleBackColor = true;
            // 
            // chkMultipleDecodeOnlyQR
            // 
            this.chkMultipleDecodeOnlyQR.AutoSize = true;
            this.chkMultipleDecodeOnlyQR.Location = new System.Drawing.Point(30, 127);
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
            this.dataGridViewBarcodeFormats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
               | System.Windows.Forms.AnchorStyles.Left)
               | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewBarcodeFormats.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewBarcodeFormats.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnSelected,
            this.ColumnBarcodeFormat});
            this.dataGridViewBarcodeFormats.Location = new System.Drawing.Point(201, 12);
            this.dataGridViewBarcodeFormats.Name = "dataGridViewBarcodeFormats";
            this.dataGridViewBarcodeFormats.Size = new System.Drawing.Size(331, 233);
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
            // chkCode39ExtendedMode
            // 
            this.chkCode39ExtendedMode.AutoSize = true;
            this.chkCode39ExtendedMode.Location = new System.Drawing.Point(12, 181);
            this.chkCode39ExtendedMode.Name = "chkCode39ExtendedMode";
            this.chkCode39ExtendedMode.Size = new System.Drawing.Size(164, 17);
            this.chkCode39ExtendedMode.TabIndex = 8;
            this.chkCode39ExtendedMode.Text = "use Code 39 Extended Mode";
            this.chkCode39ExtendedMode.UseVisualStyleBackColor = true;
            // 
            // chkCode39ExtendedModeRelaxed
            // 
            this.chkCode39ExtendedModeRelaxed.AutoSize = true;
            this.chkCode39ExtendedModeRelaxed.Location = new System.Drawing.Point(30, 204);
            this.chkCode39ExtendedModeRelaxed.Name = "chkCode39ExtendedModeRelaxed";
            this.chkCode39ExtendedModeRelaxed.Size = new System.Drawing.Size(60, 17);
            this.chkCode39ExtendedModeRelaxed.TabIndex = 9;
            this.chkCode39ExtendedModeRelaxed.Text = "relaxed";
            this.chkCode39ExtendedModeRelaxed.UseVisualStyleBackColor = true;
            // 
            // chkCode39CheckDigit
            // 
            this.chkCode39CheckDigit.AutoSize = true;
            this.chkCode39CheckDigit.Location = new System.Drawing.Point(12, 227);
            this.chkCode39CheckDigit.Name = "chkCode39CheckDigit";
            this.chkCode39CheckDigit.Size = new System.Drawing.Size(160, 17);
            this.chkCode39CheckDigit.TabIndex = 10;
            this.chkCode39CheckDigit.Text = "assume Code 39 check digit";
            this.chkCode39CheckDigit.UseVisualStyleBackColor = true;
            // 
            // DecodingOptionsForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(542, 282);
            this.Controls.Add(this.chkCode39CheckDigit);
            this.Controls.Add(this.chkCode39ExtendedModeRelaxed);
            this.Controls.Add(this.chkCode39ExtendedMode);
            this.Controls.Add(this.dataGridViewBarcodeFormats);
            this.Controls.Add(this.chkMultipleDecodeOnlyQR);
            this.Controls.Add(this.chkMultipleDecode);
            this.Controls.Add(this.chkPureBarcode);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.chkTryInverted);
            this.Controls.Add(this.chkTryHarder);
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
        private System.Windows.Forms.CheckBox chkTryHarder;
        private System.Windows.Forms.CheckBox chkTryInverted;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.CheckBox chkPureBarcode;
        private System.Windows.Forms.CheckBox chkMultipleDecode;
        private System.Windows.Forms.CheckBox chkMultipleDecodeOnlyQR;
        private System.Windows.Forms.DataGridView dataGridViewBarcodeFormats;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnSelected;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnBarcodeFormat;
        private System.Windows.Forms.CheckBox chkCode39ExtendedMode;
        private System.Windows.Forms.CheckBox chkCode39ExtendedModeRelaxed;
        private System.Windows.Forms.CheckBox chkCode39CheckDigit;
    }
}