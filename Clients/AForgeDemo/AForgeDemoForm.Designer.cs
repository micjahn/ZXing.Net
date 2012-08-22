namespace AForgeDemo
{
   partial class AForgeDemoForm
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
         this.label1 = new System.Windows.Forms.Label();
         this.cmbDevice = new System.Windows.Forms.ComboBox();
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         this.label2 = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.txtBarcodeFormat = new System.Windows.Forms.TextBox();
         this.txtContent = new System.Windows.Forms.TextBox();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(13, 13);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(41, 13);
         this.label1.TabIndex = 0;
         this.label1.Text = "Device";
         // 
         // cmbDevice
         // 
         this.cmbDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cmbDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cmbDevice.FormattingEnabled = true;
         this.cmbDevice.Location = new System.Drawing.Point(100, 10);
         this.cmbDevice.Name = "cmbDevice";
         this.cmbDevice.Size = new System.Drawing.Size(270, 21);
         this.cmbDevice.TabIndex = 1;
         this.cmbDevice.SelectedIndexChanged += new System.EventHandler(this.cmbDevice_SelectedIndexChanged);
         // 
         // pictureBox1
         // 
         this.pictureBox1.Location = new System.Drawing.Point(12, 147);
         this.pictureBox1.Name = "pictureBox1";
         this.pictureBox1.Size = new System.Drawing.Size(358, 213);
         this.pictureBox1.TabIndex = 2;
         this.pictureBox1.TabStop = false;
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(12, 40);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(82, 13);
         this.label2.TabIndex = 3;
         this.label2.Text = "Barcode Format";
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(13, 66);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(44, 13);
         this.label3.TabIndex = 4;
         this.label3.Text = "Content";
         // 
         // txtBarcodeFormat
         // 
         this.txtBarcodeFormat.Location = new System.Drawing.Point(100, 37);
         this.txtBarcodeFormat.Name = "txtBarcodeFormat";
         this.txtBarcodeFormat.ReadOnly = true;
         this.txtBarcodeFormat.Size = new System.Drawing.Size(270, 20);
         this.txtBarcodeFormat.TabIndex = 5;
         // 
         // txtContent
         // 
         this.txtContent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtContent.Location = new System.Drawing.Point(100, 63);
         this.txtContent.Multiline = true;
         this.txtContent.Name = "txtContent";
         this.txtContent.ReadOnly = true;
         this.txtContent.Size = new System.Drawing.Size(270, 78);
         this.txtContent.TabIndex = 6;
         // 
         // AForgeDemoForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(382, 372);
         this.Controls.Add(this.txtContent);
         this.Controls.Add(this.txtBarcodeFormat);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.pictureBox1);
         this.Controls.Add(this.cmbDevice);
         this.Controls.Add(this.label1);
         this.Name = "AForgeDemoForm";
         this.Text = "AForgeDemo";
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.ComboBox cmbDevice;
      private System.Windows.Forms.PictureBox pictureBox1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.TextBox txtBarcodeFormat;
      private System.Windows.Forms.TextBox txtContent;
   }
}

