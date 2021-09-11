using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MediumTestCleaner
{
    internal class Program
    {
        private const string Path = "C:\\EBSCOIS\\ordermanagement.academic.ebsconet\\src\\";

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            ReplaceMethods(GetAllTestFiles(Path), "Medium");
            ReplaceMethods(GetAllTestFiles(Path), "ViewTest");
        }

        private static void ReplaceMethods(List<FileInfo> fileList, string methodTypeToRemove)
        {
            
            Console.WriteLine("Number of test files being analyzed: " + fileList.Count);
            var countOfMethodsDeleted = 0;
            var countOfFilesDeleted = 0;

            foreach (var fileInfo in fileList)
            {
                var fileString = OpenFile_CopyContents_Close(fileInfo);
                var allSectionsToDelete = GetsListOfFileSectionsForDelete(methodTypeToRemove, fileString);

                if (allSectionsToDelete.Count > 0)
                {
                    countOfMethodsDeleted += allSectionsToDelete.Count;
                    var newFile = DeleteTheseSections(allSectionsToDelete, fileString);
                    if (fileString != newFile)
                    {
                        if (!newFile.Contains("TestMethod"))
                        {
                            fileInfo.Delete();
                            countOfFilesDeleted++;
                        }
                        else
                        {
                            File.WriteAllText(fileInfo.FullName, newFile);
                        }
                    }
                }
            }

            Console.WriteLine($"Number of {methodTypeToRemove} test methods found removed: {countOfMethodsDeleted}");
            Console.WriteLine($"Number of {methodTypeToRemove} files deleted: {countOfFilesDeleted}");
        }

        private static List<FileSection> GetsListOfFileSectionsForDelete(string methodTypeToRemove, string fileString)
        {
            var tracker = new FileParserTracker();
            var allSectionsToDelete = new List<FileSection>();

            while (!tracker.atTheEnd)
            {
                for (var i = 0; i < fileString.Length; i++)
                {
                    if (!tracker.methodStarted)
                    {
                        if (CheckForMethodStart(fileString, i))
                        {
                            tracker.methodStartIndex = i;
                            tracker.methodStarted = true;
                        }
                    }
                    else
                    {
                        if (fileString[i] == '{')
                        {
                            tracker.openbraceCount++;
                        }
                        if (fileString[i] == '}')
                        {
                            tracker.closebraceCount++;
                        }

                        if (tracker.openbraceCount != 0 && tracker.openbraceCount == tracker.closebraceCount)
                        {
                            var isRemoveType = fileString[tracker.methodStartIndex..i].Contains($"TestCategory(\"{methodTypeToRemove}\")");

                            if (isRemoveType)
                            {
                                allSectionsToDelete.Add(new FileSection
                                {
                                    MethodStart = tracker.methodStartIndex - 1,
                                    MethodEnd = i + 1
                                });
                            }

                            tracker.MethodReset();
                        }
                    }

                }
                tracker.atTheEnd = true;
            }
            return allSectionsToDelete;
        }

        private static bool CheckForMethodStart(string fileString, int index)
        {
            var next11OrEnd = index < fileString.Length - 11 ? index + 11 : fileString.Length - 1;
            var every11Chars = fileString[index..next11OrEnd];
            return every11Chars.Contains("TestMethod");
        }

        private static string OpenFile_CopyContents_Close(FileInfo fileInfo)
        {
            var file = new StreamReader(fileInfo.FullName);
            var fileString = file.ReadToEnd();
            file.Close();

            return fileString;
        }

        private static string DeleteTheseSections(List<FileSection> allSectionsToDelete, string fileString)
        {
            var offSet = 0;

            foreach (var section in allSectionsToDelete)
            {
                fileString = fileString[0..(section.MethodStart - offSet)] + fileString[(section.MethodEnd - offSet)..(fileString.Length)];
                offSet += section.TotalLength;
            }

            return fileString;
        }

        private static List<FileInfo> GetAllTestFiles(string testFilesPath)
        {
            var startingDirectory = new DirectoryInfo(testFilesPath);
            var testFolders = DirectoryCrawler.FindAllTestFolders(startingDirectory.GetDirectories().ToList());
            return RecursiveCheck(testFolders);
        }

        private static List<FileInfo> RecursiveCheck(List<DirectoryInfo> directories)
        {
            var fileList = new List<FileInfo>();
            var testDirectories = new List<DirectoryInfo>();
            foreach (var folder in directories)
            {
                fileList.AddRange(DirectoryCrawler.GetFileTypeCs(folder));
                testDirectories.AddRange(folder.GetDirectories().ToList());
            }
            if (testDirectories.Count == 0)
            {
                return fileList;
            }
            else
            {
                fileList.AddRange(RecursiveCheck(testDirectories));
                return fileList;
            }
        }
    }
}
