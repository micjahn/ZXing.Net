namespace ServiceDemo
{
   partial class BarcodeScannerService
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

      #region Vom Komponenten-Designer generierter Code

      /// <summary> 
      /// Erforderliche Methode für die Designerunterstützung. 
      /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
      /// </summary>
      private void InitializeComponent()
      {
         this.fileWatcher = new System.IO.FileSystemWatcher();
         ((System.ComponentModel.ISupportInitialize)(this.fileWatcher)).BeginInit();
         // 
         // fileWatcher
         // 
         this.fileWatcher.EnableRaisingEvents = true;
         this.fileWatcher.Created += new System.IO.FileSystemEventHandler(this.fileWatcher_Created);
         // 
         // BarcodeScannerService
         // 
         this.ServiceName = "BarcodeScannerService";
         ((System.ComponentModel.ISupportInitialize)(this.fileWatcher)).EndInit();

      }

      #endregion

      private System.IO.FileSystemWatcher fileWatcher;
   }
}
