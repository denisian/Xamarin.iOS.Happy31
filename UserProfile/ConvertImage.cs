//
//  ConvertImage.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System;
using System.Drawing;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace Happy31
{
    /// <summary>
    /// Converting and resizing users' avatar
    /// </summary>
    public static class ConvertImage
    {
        public async static Task<byte[]> ConvertImageToBinary(UIImage image)
        {
            if (image == null)
                return null;

            byte[] imageBytes = null;
            imageBytes = await Task.Run(() =>
            {
                NSData imageAsPng = image.AsJPEG(compressionQuality: 0.1f); //max value is 1.0 and minimum is 0.0
                imageBytes = new byte[imageAsPng.Length];
                System.Runtime.InteropServices.Marshal.Copy(imageAsPng.Bytes, imageBytes, 0, Convert.ToInt32(imageAsPng.Length));
                return imageBytes;
            });
            return imageBytes;
        }

        public async static Task<UIImage> ConvertBinaryToImage(byte[] imageBytes)
        {
            if (imageBytes == null)
                return null;

            var image = new UIImage();
            image = await Task.Run(() =>
            {
                NSData data = NSData.FromArray(imageBytes);
                return new UIImage(data);
            });
            return image;
        }

        // resize the image to be contained within a maximum width and height, keeping aspect ratio
        public static UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
        {
            var sourceSize = sourceImage.Size;
            var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
            if (maxResizeFactor > 1) return sourceImage;
            var width = maxResizeFactor * sourceSize.Width;
            var height = maxResizeFactor * sourceSize.Height;
            UIGraphics.BeginImageContext(new SizeF((float)width, (float)height));
            sourceImage.Draw(new RectangleF(0, 0, (float)width, (float)height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }
    }
}