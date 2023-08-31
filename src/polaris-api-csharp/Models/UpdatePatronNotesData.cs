using System;
using System.Collections.Generic;
using System.Text;

namespace Clc.Polaris.Models
{
    public class UpdatePatronNotesData
    {
        public string BlockingNote { get; set; }
        public int? BlockingNoteMode { get; set; }
        public string NonBlockingNote { get; set; }
        public int? NonBlockingNoteMode { get; set; }
    }
}
