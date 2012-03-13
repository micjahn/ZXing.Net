using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightDemo
{
   public partial class App : Application
   {

      public App()
      {
         this.Startup += this.Application_Startup;
         this.Exit += this.Application_Exit;
         this.UnhandledException += this.Application_UnhandledException;

         InitializeComponent();
      }

      private void Application_Startup(object sender, StartupEventArgs e)
      {
         this.RootVisual = new MainPage();
      }

      private void Application_Exit(object sender, EventArgs e)
      {

      }

      private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
      {
         // Wenn die Anwendung außerhalb des Debuggers ausgeführt wird, melden Sie die Ausnahme mithilfe
         // des Ausnahmemechanismus des Browsers. In IE wird hier ein gelbes Warnsymbol in der 
         // Statusleiste angezeigt, Firefox zeigt einen Skriptfehler an.
         if (!System.Diagnostics.Debugger.IsAttached)
         {

            // Hinweis: So kann die Anwendung weiterhin ausgeführt werden, nachdem eine Ausnahme ausgelöst, aber nicht
            // behandelt wurde. 
            // Bei Produktionsanwendungen sollte diese Fehlerbehandlung durch eine Anwendung ersetzt werden, die 
            // den Fehler der Website meldet und die Anwendung beendet.
            e.Handled = true;
            Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
         }
      }

      private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
      {
         try
         {
            string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
            errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

            System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
         }
         catch (Exception)
         {
         }
      }
   }
}
