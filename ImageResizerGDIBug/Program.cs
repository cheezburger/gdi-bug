using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using ImageResizer;
using ImageResizer.Plugins.AnimatedGifs;
using ImageResizer.Plugins.PrettyGifs;
using NUnit.Framework;

namespace ImageResizerGDIBug
{
    public class Program
    {
        [Test]
        public void TestBrokenTransformOfAnimiatedGif()
        {
            using (var imageStream = new MemoryStream(ReadBytes("ImageResizerGDIBug.poison.gif")))
            using (var bmp = new Bitmap(imageStream))
            using (var outputStream = new MemoryStream())
            {

                /*
                 * The exception occurs in GDI+ via System.Drawing.
                 * The error occurs for this "poison" gif on frame 2 (time dimension).
                 * The AnimatedGifs ImageResizer package fails on this exact same line
                 * I've commented out the ImageResizer bit using AnimatedGifs library 
                 * for clarity.
                 */
                var numFrames = bmp.GetFrameCount(FrameDimension.Time);
                Console.WriteLine(" number of time frames: {0}", numFrames);
                for (var frameNo = 0; frameNo < numFrames; frameNo++)
                {
                    Console.WriteLine("     select active frame number: {0}", frameNo);
                    bmp.SelectActiveFrame(FrameDimension.Time, frameNo);
                }

                /*
                 * Here is the essential code that is failing all over the place.
                 * The Build method uses the AnimatedGifs class and fails in 
                 * the method WriteAnimatedGif
                 */
/*
                var builderConfig = new ImageResizer.Configuration.Config();
                new PrettyGifs().Install(builderConfig);
                new AnimatedGifs().Install(builderConfig);

                var imageBuilder = new ImageBuilder(
                    builderConfig.Plugins.ImageBuilderExtensions,
                    builderConfig.Plugins,
                    builderConfig.Pipeline,
                    builderConfig.Pipeline);
                imageBuilder.Build(bmp, outputStream, new ResizeSettings
                                                          {
                                                              MaxWidth = 500
                                                          }, false);
*/
            }
        }

        public static byte[] ReadBytes(string embeddedResourcePath)
        {
            byte[] bytes;
            using (var stream = OpenRead(embeddedResourcePath))
            using (var reader = new BinaryReader(stream))
                bytes = reader.ReadBytes((int)stream.Length);

            return bytes;
        }

        public static Stream OpenRead(string embeddedResourcePath)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResourcePath);
        }

        static void Main()
        {
            try
            {
                new Program().TestBrokenTransformOfAnimiatedGif();
                Console.WriteLine("pass");
            }
            catch (Exception e)
            {
                Console.WriteLine("fail");
                Console.WriteLine();
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }

}
