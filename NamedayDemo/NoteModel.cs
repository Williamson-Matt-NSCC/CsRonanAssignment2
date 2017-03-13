using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamedayDemo
{
    public class NoteModel
    {
        public int NoteNumber
        {
            get; set;
        }

        public string NoteBody
        {
            get; set;
        }

        //public IEnumerable<string> NoteName { get; set; }
        public string NoteName { get; set; }

        //public NoteModel(int noteNumber, IEnumerable<string> noteName, string noteBody)
        //{
        //    this.NoteNumber = noteNumber;
        //    this.NoteName = noteName;
        //    this.NoteBody = noteBody;
        //}

        public NoteModel(int noteNumber, string noteName, string noteBody)
        {
            this.NoteNumber = noteNumber;
            this.NoteName = noteName;
            this.NoteBody = noteBody;
        }

        public NoteModel()
        {

        }
        public string NoteNameAsString => string.Join(", ", NoteName);
    }
}
