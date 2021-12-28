using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestlessDb.Common.Types
{
    public enum Disposition { EMBEDDED, STANDALONE }
    public class FormatterInfo
    {
        public string Name {  get; set; }   
        public string Description { get; set; }
        public string OutputFormat { get; set; }
        public string FileExtension { get; set; }
        public string ContentType { get; set; } 
        public Disposition Disposition { get; set; }    
    }
}
