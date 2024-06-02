// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2024 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Vincent Wing (vwing@uci.edu)
// Contributors:
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;


namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Utility class for unzipping zip files
    /// </summary>
    public static class Unzip
    {
        // Call this method with the path to the zip file and the output folder.
        public static string UnzipFile(string zipFilePath, string outputFolderPath)
        {
            // Ensure the output directory exists
            if (!Directory.Exists(outputFolderPath))
                Directory.CreateDirectory(outputFolderPath);

            // Initialize the Zip input stream
            using (FileStream fileStream = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read))
            using (ZipInputStream zipInputStream = new ZipInputStream(fileStream))
            {
                ZipEntry entry;
                while ((entry = zipInputStream.GetNextEntry()) != null)
                {
                    // Create directory for the entry if it does not exist
                    string directoryName = Path.GetDirectoryName(entry.Name);
                    string fileName = Path.GetFileName(entry.Name);
                    string directoryPath = Path.Combine(outputFolderPath, directoryName);

                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    // If the entry is a file, extract it
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        string filePath = Path.Combine(directoryPath, fileName);
                        using (FileStream streamWriter = File.Create(filePath))
                        {
                            int size = 2048;
                            byte[] buffer = new byte[size];

                            try
                            {
                                while ((size = zipInputStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    streamWriter.Write(buffer, 0, size);
                                }
                            }
                            catch
                            {
                                Debug.LogWarning("Couldn't extract " + filePath + " from zip archive");
                            }
                        }
                    }
                }
            }

            return outputFolderPath;
        }
    }
}