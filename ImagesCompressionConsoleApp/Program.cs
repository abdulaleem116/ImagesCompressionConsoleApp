using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;

namespace ImagesCompressionConsoleApp
{
    class Program
    {
         /// <summary>
        /// inDirectory and outDirectory are placed in App.config
        /// </summary>
        static string inDirectory = ConfigurationManager.AppSettings.Get("inDirectory");
        string outDirectory = ConfigurationManager.AppSettings.Get("outDirectory");
        static string fileName = "";
        /// <summary>
        /// In main method, We are getting files from inDirectory 
        /// and applied filter in order to get only image type file.
        /// And checking, if their size is greater that 1MB then calling method
        /// named as "ReduceImageSize" for compressing processs
        /// </summary>
        /// <param name="args"></param>
        /// 

        static void Main(string[] args)
        {
            var inputDir = Directory.GetFiles(inDirectory, "*.*", SearchOption.AllDirectories);
            //Code using TPL
            Parallel.ForEach(inputDir, filename =>
            {
                doParralTask(filename);
            });
        }
        /// <summary>
        /// This is processing method for Compressing Image
        /// </summary>
        /// <param name="scaleFactor"></param>
        /// <param name="sourcePath"></param>
        /// <param name="fileName"></param>
        private void ReduceImageSize(double scaleFactor, Stream sourcePath, string fileName)
        {
            try
            {
                string targetPath = "";
                using (var image = System.Drawing.Image.FromStream(sourcePath))
                {
                    if (!Directory.Exists(outDirectory))
                    {
                        Directory.CreateDirectory(outDirectory);
                    }
                    string fName = fileName;
                    targetPath = outDirectory + fileName;
                    image.Save(targetPath, System.Drawing.Imaging.ImageFormat.Jpeg);

                    var newWidth = (int)(image.Width * scaleFactor);
                    var newHeight = (int)(image.Height * scaleFactor);
                    var thumbnailImg = new Bitmap(newWidth, newHeight);
                    var thumbGraph = Graphics.FromImage(thumbnailImg);
                    thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                    thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                    thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                    thumbGraph.DrawImage(image, imageRectangle);
                    image.Clone();
                    thumbnailImg.Save(targetPath, ImageFormat.Jpeg);
                    //Dispose BitMap
                    thumbnailImg.Dispose();
                    //Dispose Graphics
                    thumbGraph.Dispose();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Save Images when File size is less than 1MB.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="fileName"></param>
        private void SaveImageInOutDirectory(Stream sourcePath, string fileName)
        {
            try
            {
                string targetPath = "";
                using (var image = System.Drawing.Image.FromStream(sourcePath))
                {
                    if (!Directory.Exists(outDirectory))
                    {
                        Directory.CreateDirectory(outDirectory);
                    }
                    string fName = fileName;
                    targetPath = outDirectory + fileName;
                    image.Save(targetPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private static void doParralTask(string filename)
        {

            string fileName = Path.GetFileName(filename);
            if (Regex.IsMatch(filename, @".jpg|.png|.gif$"))
            {
                FileStream file = new FileStream(filename, FileMode.Open);
                var fileSize = file.Length / 1000000;
                if (fileSize > 1)
                {
                    Console.WriteLine(fileSize + " is greater than 1MB");
                    new Program().ReduceImageSize(0.1, file, fileName);
                }
                else
                {
                    // just save file in outDirectory only no need to send for Compressing process
                    Console.WriteLine("File is less than 1MB");
                    new Program().SaveImageInOutDirectory(file, fileName);

                }

            }
        }
    }
}
