using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NotesManager.Models
{
    public class Note
    {
        public int Id { get; set; }
        public int CreatorId{ get; set; }

        [Display (Name = "Текст заметки")]
        public string Content { get; set; }

        [Display (Name = "Дата события")]
        public DateTime EventDate { get; set; }
        public bool NeedToNotify { get; set; }

        [Display (Name = "Кто может видеть эту записку")]
        public string Access { get; set; }
    }
}