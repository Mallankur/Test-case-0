using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadAPIClient
{
    public  class ASCIIConverter
    {
        static void Askiee(string[] args)
        {

            string filePath = @"E:\Probe 1_raman spectra new_20240628-141400\raman spectra new_20240628-141529_58668.spc";
            byte[] fileData = File.ReadAllBytes(filePath);


            string hexString = BitConverter.ToString(fileData).Replace("-", "");
            Console.WriteLine("Hexadecimal Representation:");
            Console.WriteLine(hexString);


            string asciiString = Encoding.ASCII.GetString(fileData);
            Console.WriteLine("\nASCII Representation:");
            Console.WriteLine(asciiString);


            string binaryToCharString = ConvertBinaryToChar(fileData);
            Console.WriteLine("\nBinary to Character Representation:");
            Console.WriteLine(binaryToCharString);
        }


        static string ConvertBinaryToChar(byte[] data)
        {
            StringBuilder binaryStringBuilder = new StringBuilder();
            foreach (byte b in data)
            {

                string binaryString = Convert.ToString(b, 2).PadLeft(8, '0');
                binaryStringBuilder.Append(binaryString + " ");
            }


            return binaryStringBuilder.ToString();
        }

    }
}
