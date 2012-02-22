using System;
using System.Windows.Forms;

namespace WindowsFormsDemo
{
   static class Program
   {
      /// <summary>
      /// Der Haupteinstiegspunkt für die Anwendung.
      /// </summary>
      [STAThread]
      static void Main()
      {
         AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
         Application.ThreadException += Application_ThreadException;

         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.Run(new WindowsFormsDemoForm());
      }

      static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
      {
         MessageBox.Show(e.Exception.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
      {
         MessageBox.Show(e.ExceptionObject.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
   }
}
