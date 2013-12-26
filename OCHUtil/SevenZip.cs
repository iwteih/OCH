using IOCH;
using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OCHUtil
{
    public class SevenZip
    {
        public static string Compress(string input)
        {
            byte[] compressByte = null;

            SevenZipCompressor compressor = new SevenZipCompressor();
            compressor.CompressionMethod = CompressionMethod.Ppmd;
            compressor.CompressionLevel = CompressionLevel.High;

            using (MemoryStream msIn = GetUTF8MemeoryStream(input))
            {
                using (MemoryStream msOut = new MemoryStream())
                {
                    compressor.CompressStream(msIn, msOut);
                    msOut.Position = 0;
                    compressByte = new byte[msOut.Length];
                    msOut.Read(compressByte, 0, compressByte.Length);
                }
            }

            if (compressByte != null)
            {
                string output = Convert.ToBase64String(compressByte, Base64FormattingOptions.None);

                return output;
            }

            return string.Empty;
        }

        public static string Decompress(string input)
        {
            byte[] inputbytes = Convert.FromBase64String(input);
            byte[] decompressBuffer = null;

            using (MemoryStream msIn = new MemoryStream())
            {
                msIn.Write(inputbytes, 0, inputbytes.Length);
                msIn.Position = 0;

                using (SevenZipExtractor extractor = new SevenZipExtractor(msIn))
                {
                    using (MemoryStream msOut = new MemoryStream())
                    {
                        extractor.ExtractFile(0, msOut);
                        msOut.Position = 0; 
                        decompressBuffer = new byte[msOut.Length];
                        msOut.Read(decompressBuffer, 0, decompressBuffer.Length);
                    }
                }
            }

            if (decompressBuffer != null)
            {
                return Encoding.UTF8.GetString(decompressBuffer);
            }

            return string.Empty;
        }

        private static MemoryStream GetUTF8MemeoryStream(string input)
        {
            MemoryStream ms = new MemoryStream();
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            ms.Write(bytes, 0, bytes.Length);

            return ms;
        }

    }
}
