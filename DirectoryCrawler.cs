using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MediumTestCleaner
{
    public static class DirectoryCrawler
    {
        public static List<DirectoryInfo> FindAllTestFolders(List<DirectoryInfo> folderList)
        {
            return folderList.Where(f => f.Name.Contains("Test")).ToList();
        }

        public static IEnumerable<FileInfo> GetFileTypeCs(DirectoryInfo folder)
        {
            return folder.GetFiles().Where(f => f.Name.Contains(".cs"));
        }
    }
}
