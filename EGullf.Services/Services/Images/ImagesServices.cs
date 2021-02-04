using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Services.Images
{
    public class ImagesServices
    {


        public Stream ResizeProfileImage(Stream ImgSource)
        {
            Stream ImgOutput = new MemoryStream();
            Bitmap ImgInput = new Bitmap(ImgSource);
            var ImgProcessed = ChangeImageSize(ImgInput, 300, 300);

            ImgInput.Dispose();          
            ImgProcessed.Save(ImgOutput, ImageFormat.Jpeg);
            ImgProcessed.Dispose();

            return ImgOutput;
        }


        public Bitmap ChangeImageSize(Bitmap imgSource, int Width, int Height)
        {
            int sourceWidth = imgSource.Width;
            int sourceHeight = imgSource.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgSource.HorizontalResolution,
                             imgSource.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.White);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgSource,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }



        //public static Bitmap ProcesarImagenEmpleado(Bitmap ImagenOriginal, int Medida)
        //{
        //    Bitmap ImagenProcesada = null;
        //    Bitmap BitMapOriginal = new Bitmap(ImagenOriginal);
        //    Bitmap BitMapNuevo = null;
        //    var TamanioSalida = Medida;

        //    try
        //    {
        //        //Procesamos la imagen 
        //        int NewHeight;
        //        int NewWidth;
        //        double NewRatio;

        //        NewHeight = TamanioSalida;
        //        NewRatio = (double)((double)TamanioSalida / (double)BitMapOriginal.Height);
        //        NewWidth = (int)(NewRatio * (double)BitMapOriginal.Width);

        //        BitMapNuevo = new Bitmap(NewWidth, NewHeight);
        //        BitMapNuevo = ResizeImage(BitMapOriginal, NewWidth, NewHeight);
        //        ImagenProcesada = BitMapNuevo;

        //        return ImagenProcesada;
        //    }
        //    catch
        //    {
        //        ImagenOriginal.Dispose();
        //        BitMapOriginal.Dispose();
        //        return null;
        //    }
        //    finally
        //    {
        //        ImagenOriginal.Dispose();
        //        BitMapOriginal.Dispose();
        //    }
        //}



        //private static Bitmap ResizeImage(Bitmap ImagenOriginal, int Width, int Height)
        //{
        //    Bitmap resizedImage = new Bitmap(Width, Height);
        //    try
        //    {
        //        using (Graphics gfx = Graphics.FromImage(resizedImage))
        //        {
        //            gfx.DrawImage(ImagenOriginal,
        //                          new System.Drawing.Rectangle(0, 0, Width, Height),
        //                          new System.Drawing.Rectangle(0, 0, ImagenOriginal.Width, ImagenOriginal.Height),
        //                          GraphicsUnit.Pixel
        //                          );
        //        }

        //        return resizedImage;
        //    }
        //    catch (Exception ex)
        //    {
        //        ImagenOriginal.Dispose();
        //        resizedImage.Dispose();
        //        throw ex;
        //    }
        //    finally
        //    {
        //        ImagenOriginal.Dispose();
        //    }
        //}





    }
}
