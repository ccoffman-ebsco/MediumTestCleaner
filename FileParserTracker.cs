using System;

namespace MediumTestCleaner
{
    public class FileParserTracker
    {

        public FileParserTracker()
        {
            openbraceCount = 0;
            closebraceCount = 0;
            methodStartIndex = -1;
            methodStarted = false;
            atTheEnd = false;
        }

        public int openbraceCount { get; set; }
        public int closebraceCount { get; set; }
        public int methodStartIndex { get; set; }
        public bool methodStarted { get; set; }
        public bool atTheEnd { get; set; }

        internal void MethodReset()
        {           
           openbraceCount = 0;
           closebraceCount = 0;
           methodStartIndex = -1;
           methodStarted = false;
        }
    }
}