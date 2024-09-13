using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

using System;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using File = System.IO.File;

namespace ReadAPIClient
{
    public  class Program
    {
        static void Main(string[] args)
        {

            string filePath = @"E:\Probe 1_raman spectra new_20240628-141400\raman spectra new_20240628-141529_58668.spc";
            byte[] fileData = File.ReadAllBytes(filePath); 
            string hexString = BitConverter.ToString(fileData).Replace("-", "");
            Console.WriteLine(hexString);          
            string asciiString = Encoding.ASCII.GetString(fileData);
            Console.WriteLine("\nASCII Representation:");
            Console.WriteLine(asciiString);
            Console.ReadLine(); 

          

            
        }
    }
}
