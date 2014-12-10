using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesManager.Models
{
    public class UsersRelation
    {
        public int Id { get; set; }
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public bool Friends { get; set; }
    }
}