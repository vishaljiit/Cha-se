using System;
using System.IO;
using System.Text;

namespace Paymentech.Core
{
    /// <summary>
    /// Class that wraps all file I/O for use in unit testing.
    /// </summary>
    public class Filer : IFiler
    {
		/// <summary>
		/// Default constructor.
		/// </summary>
        public Filer()
        {
        }

        /// <summary>
        /// Reads all the text in a text file and returns it to the caller.
        /// </summary>
        /// <param name="path">The path to the file being read.</param>
        /// <returns>The contents of the file.</returns>
        public string ReadAllText(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Determines if the specified file exists.
        /// </summary>
        /// <param name="path">The path to the file to test.</param>
        /// <returns>Returns true if the specified file exists, false if it does not.</returns>
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Determines if the specified directory exists.
        /// </summary>
        /// <param name="path">The path to the directory to test.</param>
        /// <returns>Returns true if the specified directory exists, false if it does not.</returns>
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
    }
}
