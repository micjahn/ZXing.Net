namespace WindowsFormsDemo
{
   partial class ExtendedResultForm
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
         this.propGridResult = new System.Windows.Forms.PropertyGrid();
         this.btnClose = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // propGridResult
         // 
         this.propGridResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.propGridResult.Location = new System.Drawing.Point(12, 12);
         this.propGridResult.Name = "propGridResult";
         this.propGridResult.Size = new System.Drawing.Size(260, 202);
         this.propGridResult.TabIndex = 0;
         // 
         // btnClose
         // 
         this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnClose.Location = new System.Drawing.Point(197, 227);
         this.btnClose.Name = "btnClose";
         this.btnClose.Size = new System.Drawing.Size(75, 23);
         this.btnClose.TabIndex = 1;
         this.btnClose.Text = "Close";
         this.btnClose.UseVisualStyleBackColor = true;
         // 
         // ExtendedResultForm
         // 
         this.AcceptButton = this.btnClose;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(284, 262);
         this.Controls.Add(this.btnClose);
         this.Controls.Add(this.propGridResult);
         this.Name = "ExtendedResultForm";
         this.Text = "ExtendedResultForm";
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.PropertyGrid propGridResult;
      private System.Windows.Forms.Button btnClose;
   }
}