using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesManager.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string Notapassword { get; set; }
        public string Email { get; set; }
        public bool Notify { get; set; }
    }
}