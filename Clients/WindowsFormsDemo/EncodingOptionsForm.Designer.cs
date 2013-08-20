namespace WindowsFormsDemo
{
   partial class EncodingOptionsForm
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
         this.btnUse = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.propOptions = new System.Windows.Forms.PropertyGrid();
         this.cmbRenderer = new System.Windows.Forms.ComboBox();
         this.SuspendLayout();
         // 
         // btnUse
         // 
         this.btnUse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnUse.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnUse.Location = new System.Drawing.Point(199, 247);
         this.btnUse.Name = "btnUse";
         this.btnUse.Size = new System.Drawing.Size(107, 23);
         this.btnUse.TabIndex = 0;
         this.btnUse.Text = "Use";
         this.btnUse.UseVisualStyleBackColor = true;
         // 
         // btnCancel
         // 
         this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(12, 247);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(107, 23);
         this.btnCancel.TabIndex = 1;
         this.btnCancel.Text = "Cancel";
         this.btnCancel.UseVisualStyleBackColor = true;
         // 
         // propOptions
         // 
         this.propOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.propOptions.Location = new System.Drawing.Point(12, 43);
         this.propOptions.Name = "propOptions";
         this.propOptions.Size = new System.Drawing.Size(294, 198);
         this.propOptions.TabIndex = 2;
         // 
         // cmbRenderer
         // 
         this.cmbRenderer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.cmbRenderer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cmbRenderer.FormattingEnabled = true;
         this.cmbRenderer.Location = new System.Drawing.Point(12, 16);
         this.cmbRenderer.Name = "cmbRenderer";
         this.cmbRenderer.Size = new System.Drawing.Size(294, 21);
         this.cmbRenderer.TabIndex = 3;
         // 
         // EncodingOptionsForm
         // 
         this.AcceptButton = this.btnUse;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.btnCancel;
         this.ClientSize = new System.Drawing.Size(318, 282);
         this.Controls.Add(this.cmbRenderer);
         this.Controls.Add(this.propOptions);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnUse);
         this.MinimumSize = new System.Drawing.Size(334, 320);
         this.Name = "EncodingOptionsForm";
         this.Text = "EncodingOptionsForm";
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Button btnUse;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.PropertyGrid propOptions;
      private System.Windows.Forms.ComboBox cmbRenderer;
   }
}