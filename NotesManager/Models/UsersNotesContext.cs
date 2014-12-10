using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace NotesManager.Models
{
    public class UsersNotesContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UsersRelation> UserRelations { get; set; }
        public DbSet<Note> Notes { get; set; }
    }
}