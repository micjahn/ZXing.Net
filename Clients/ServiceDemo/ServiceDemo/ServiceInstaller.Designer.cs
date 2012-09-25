namespace ServiceDemo
{
   partial class ServiceInstaller
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
         this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
         this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
         // 
         // serviceInstaller1
         // 
         this.serviceInstaller1.DelayedAutoStart = true;
         this.serviceInstaller1.Description = "Demo Service for a file based BarcodeScanner";
         this.serviceInstaller1.DisplayName = "BarcodeScanner";
         this.serviceInstaller1.ServiceName = "BarcodeScanner";
         // 
         // serviceProcessInstaller1
         // 
         this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
         this.serviceProcessInstaller1.Password = null;
         this.serviceProcessInstaller1.Username = null;
         // 
         // ServiceInstaller
         // 
         this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceInstaller1, this.serviceProcessInstaller1});

      }

      #endregion

      private System.ServiceProcess.ServiceInstaller serviceInstaller1;
      private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
   }
}