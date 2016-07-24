using System;
using System.IO;
using System.Text;

namespace Paymentech.Core
{
	/// <summary>
	/// 
	/// </summary>
    public interface IFiler
    {
        /// <summary>
        /// Reads all the text in a text file and returns it to the caller.
        /// </summary>
        /// <param name="path">The path to the file being read.</param>
        /// <returns>The contents of the file.</returns>
        string ReadAllText(string path);
        /// <summary>
        /// Determines if the specified file exists.
        /// </summary>
        /// <param name="path">The path to the file to test.</param>
        /// <returns>Returns true if the specified file exists, false if it does not.</returns>
        bool Exists(string path);
        /// <summary>
        /// Determines if the specified directory exists.
        /// </summary>
        /// <param name="path">The path to the directory to test.</param>
        /// <returns>Returns true if the specified directory exists, false if it does not.</returns>
        bool DirectoryExists(string path);
    }
}
