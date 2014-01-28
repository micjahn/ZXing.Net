using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WindowsPhone8Demo.ViewModels;

namespace WindowsPhone8Demo.Views
{
    public partial class CaptureView : PhoneApplicationPage
    {
        public CaptureView()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
           ((CaptureViewModel)DataContext).Orientation = this.Orientation;
        }
    }
}