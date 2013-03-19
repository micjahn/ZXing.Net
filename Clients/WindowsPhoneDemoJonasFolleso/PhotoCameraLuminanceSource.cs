using System;
using ZXing;


namespace ScannerDemo
{
    public class PhotoCameraLuminanceSource : LuminanceSource
    {
        public byte[] PreviewBufferY { get; private set; }

        public PhotoCameraLuminanceSource(int width, int height) : base(width, height)
        {        
            PreviewBufferY = new byte[width * height];
        }

        public override byte[] Matrix
        {
            get { return PreviewBufferY; }
        }

        public override byte[] getRow(int y, byte[] row)
        {
            if (row == null || row.Length < Width)
            {
                row = new byte[Width];
            }

            for (int i = 0; i < Height; i++)
                row[i] = PreviewBufferY[i * Width + y];

            return row;
        }
    }
}
