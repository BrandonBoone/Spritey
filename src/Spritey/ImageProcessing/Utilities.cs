namespace Spritey.ImageProcessing
{
    using System;
    using System.IO;
    using System.Text;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;

    /// <summary>
    /// Various Utility methods to assist ImageProcessing
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Sets all Alpha 0 colors to transparent
        /// </summary>
        /// <param name="img">The bitmap to set transparency on</param>
        /// <param name="transparentColor">The new color to represent transparency</param>
        public static void ReplaceTransparentColor(this Image<Rgba32> img, Rgba32 transparentColor)
        {
            for (int y = 0; y < img.Height; ++y)
            {
                for (int x = 0; x < img.Width; ++x)
                {
                    Rgba32 color = img[x, y];
                    if (color.A == 0)
                    {
                        img[x, y] = transparentColor;
                    }
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="MemoryStream"/> to a base64 encoded string
        /// </summary>
        /// <param name="ms">The <see cref="MemoryStream"/> to convert</param>
        /// <returns>A base64 encoded string</returns>
        public static string ConvertToBase64(MemoryStream ms)
        {
            ms.Seek(0, SeekOrigin.Begin);
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Returns the byte count of a string
        /// </summary>
        /// <param name="str">the string to count</param>
        /// <returns>The byte count</returns>
        public static int GetByteCount(string str)
        {
            Encoding e = Encoding.UTF8;
            return e.GetByteCount(str);
        }

        /// <summary>
        /// Determines whether the specified img is animated.
        /// </summary>
        /// <param name="img">The img.</param>
        /// <returns>
        ///   <c>true</c> if the specified img is animated; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// We are only looking at the first frame dimension.
        /// </remarks>
        public static bool IsAnimated(this Image<Rgba32> img)
        {
            return img.Frames.Count > 1;
        }

        // https://stackoverflow.com/q/6495952/402706
        // Answered: [lincolnk](https://stackoverflow.com/a/6510156/402706)
        // Asked: [RHL](https://stackoverflow.com/q/6495952/402706)

        /// <summary>
        /// Returns a transparent background GIF image from the specified Bitmap.
        /// </summary>
        /// <param name="gifStream">The MemoryStream containing a GIF image to make transparent.</param>
        /// <param name="color">The Color to make transparent.</param>
        /// <returns>New MemoryStream containing a transparent background gif.</returns>
        public static MemoryStream MakeTransparentGif(MemoryStream gifStream, Rgba32 color)
        {
            // reference: https://www.w3.org/Graphics/GIF/spec-gif89a.txt
            // reference: http://www.onicos.com/staff/iz/formats/gif.html
            // reference: https://en.wikipedia.org/wiki/GIF#Image_coding
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
            byte R = color.R;
            byte G = color.G;
            byte B = color.B;
#pragma warning restore SA1312 // Variable names should begin with lower-case letter

            MemoryStream fout = new MemoryStream((int)gifStream.Length);
            int count = 0;
            byte[] buf = new byte[256];
            byte transparentIdx = 0;
            gifStream.Seek(0, SeekOrigin.Begin);

            // Byte 0-5 is header (Probably GIF89a or GIF87a)
            // byte 6-7 is screen width in pixels
            // byte 8-9 is screen height in pixels
            // byte 10 (A)
            //      bit 0:    Global Color Table Flag(GCTF)
            //      bit 1..3: Color Resolution
            //      bit 4:    Sort Flag to Global Color Table
            //      bit 5..7: Size of Global Color Table: 2 ^ (1 + n)
            // byte 11 (B) Background color
            // byte 12 (C) Default pixel Apsect ratio
            count = gifStream.Read(buf, 0, 13);

            // 71 == G, 73 == I, 70 === F
            if ((buf[0] != 71) || (buf[1] != 73) || (buf[2] != 70))
            {
                return null; // Not a GIF
            }

            fout.Write(buf, 0, 13);
            int colorTableSize = 0;

            // is GCT present? The highest bit means GTC is present (1000 0000)
            if ((buf[10] & 0x80) > 0)
            {
                // Get the size of the color table (first 3 bits == n; 2^(1+n)).
                // Bit shifting is a shortcut for power operation. Ex: 1 << 2 == 4, 2^2 == 4; 1 << 3 == 8, 2^3 == 8;
                colorTableSize = 1 << ((buf[10] & 7) + 1);
            }

            // Read the colors in the color table.
            for (var i = colorTableSize; i != 0; i--)
            {
                gifStream.Read(buf, 0, 3); // pull off three bytes for R, G, B

                // Does it match the color that's supposed to be transparent?
                if ((buf[0] == R) && (buf[1] == G) && (buf[2] == B))
                {
                    transparentIdx = (byte)(colorTableSize - i); // Find the index of the transparent color
                }

                fout.Write(buf, 0, 3);
            }

            bool gcePresent = false;
            while (true)
            {
                // Graphic Control Extension Block
                gifStream.Read(buf, 0, 1);
                fout.Write(buf, 0, 1);
                if (buf[0] != 0x21)
                {
                    break; // Extension Introducer (0x21)
                }

                gifStream.Read(buf, 0, 1);
                fout.Write(buf, 0, 1);
                gcePresent = buf[0] == 0xf9; // Graphic Control Label (0xf9)
                while (true)
                {
                    gifStream.Read(buf, 0, 1);
                    fout.Write(buf, 0, 1);
                    if (buf[0] == 0)
                    {
                        break; // block size is 0, no data?
                    }

                    // Block Size -Number of bytes in the block, after the Block
                    // Size field and up to but not including the Block Terminator.This
                    // field contains the fixed value 4.
                    count = buf[0];
                    if (gifStream.Read(buf, 0, count) != count)
                    {
                        return null;
                    }

                    if (gcePresent)
                    {
                        if (count == 4)
                        {
                            buf[0] |= 0x01; // Set the transparency flag to true.
                            buf[3] = transparentIdx; // set the transparency index previously found
                        }
                    }

                    fout.Write(buf, 0, count);
                }
            }

            // write out the remainder of the file.
            while (count > 0)
            {
                count = gifStream.Read(buf, 0, 1);
                fout.Write(buf, 0, 1);
            }

            fout.Seek(0, SeekOrigin.Begin);
            return fout;
        }
    }
}
