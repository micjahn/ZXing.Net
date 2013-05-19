using WindowsPhone8Demo.ViewModels;
using ZXing;

namespace WindowsPhone8Demo.Models
{
    public static class BarcodeResult
    {
        public static void AddToResultCollection(Result result, MainViewModel viewModel)
        {
            viewModel.Results.Add(result);
        }
    }
}
