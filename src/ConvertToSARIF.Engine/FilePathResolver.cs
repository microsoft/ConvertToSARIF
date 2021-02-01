using ConvertToSARIF.Engine.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConvertToSARIF.Engine
{
    public class FilePathResolver
    {
        public virtual string Resolver(string filePath, string currentPath)
        {
            if (string.IsNullOrEmpty(Path.GetFileName(filePath)))
            {
                throw new FileNotFoundException(Resources.FileNotProvided);
            }

            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(currentPath ?? string.Empty, filePath);
            }

            return filePath;
        }
    }
}
