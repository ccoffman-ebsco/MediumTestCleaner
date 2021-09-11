using System;
using System.Collections.Generic;
using System.Text;

namespace MediumTestCleaner
{
    public class FileSection
    {
        public int MethodStart { get; set; }
        public int MethodEnd { get; set; }

        public int TotalLength => MethodEnd - MethodStart;
    }
}
