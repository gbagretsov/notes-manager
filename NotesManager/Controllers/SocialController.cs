using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using NotesManager.Models;

namespace NotesManager.Controllers
{
    public class SocialController : Controller
    {
        public ActionResult UserProfile(int? id)
        {            
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            UsersNotesContext uc = new UsersNotesContext();
            if (id == null || uc.Users.Find(id) == null || uc.Users.Find(id).Nickname == User.Identity.Name)
                return RedirectToAction("Personal", "Notes");
            else
            {
                int linkType;
                int curUserId = uc.Users.Where(u => u.Nickname == User.Identity.Name).FirstOrDefault().Id;
                if (uc.UserRelations.Count(u => (u.User1Id == curUserId && u.User2Id == (int)id) ||
                        (u.User2Id == curUserId && u.User1Id == (int)id)) == 0)
                    linkType = 0;
                else
                {
                    UsersRelation ur = uc.UserRelations.Where(u => (u.User1Id == curUserId && u.User2Id == (int)id) ||
                        (u.User2Id == curUserId && u.User1Id == (int)id)).Single();
                    if (ur.Friends == true)
                        linkType = 3;
                    else if (ur.User1Id == curUserId)
                        linkType = 1;
                    else
                        linkType = 2;
                }

                UsersNotesContext nc = new UsersNotesContext();
                List<Note> notesList = new List<Note>();
                foreach (Note n in nc.Notes.Where(n => n.CreatorId == (int)id && n.Access == "Public").ToList())
                    notesList.Add(n);
                if (linkType == 3)
                    foreach (Note n in nc.Notes.Where(n => n.CreatorId == (int)id && n.Access == "Friends").ToList())
                        notesList.Add(n);

                ViewBag.LinkType = linkType;
                ViewBag.NotesList = notesList;
                return View(uc.Users.Find(id));
            }
        }

        public ActionResult SendRequest(int? id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            UsersNotesContext uc = new UsersNotesContext();
            User user = uc.Users.Find(id);
            int curUserId = uc.Users.Where(u => u.Nickname == User.Identity.Name).FirstOrDefault().Id;
            if (id == null || user == null || user.Nickname == User.Identity.Name)
                return RedirectToAction("Personal", "Notes");
            else
            {
                UsersRelation ur = new UsersRelation
                {
                    User1Id = curUserId,
                    User2Id = (int)id,
                    Friends = false
                };
                if (uc.UserRelations.Count(u => (u.User1Id == curUserId && u.User2Id == (int)id) ||
                    (u.User2Id == curUserId && u.User1Id == (int)id)) == 0)
                {
                    uc.UserRelations.Add(ur);
                    uc.SaveChanges();
                }
                return RedirectToAction("UserProfile", "Social", new { id = id });
            }                
        }

        public ActionResult ConfirmRequest(int? id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            UsersNotesContext uc = new UsersNotesContext();
            User user = uc.Users.Find(id);
            int curUserId = uc.Users.Where(u => u.Nickname == User.Identity.Name).FirstOrDefault().Id;
            if (id == null || user == null || user.Nickname == User.Identity.Name)
                return RedirectToAction("Personal", "Notes");
            else
            {
                try
                {
                    UsersRelation ur = uc.UserRelations.Where(u => (u.Friends == false && u.User2Id == curUserId && u.User1Id == (int)id)).Single();
                    ur.Friends = true;
                    uc.Entry(ur).State = EntityState.Modified;
                    uc.SaveChanges();
                }
                catch (Exception) { return RedirectToAction("UserProfile", "Social", new { id = id }); }
                return RedirectToAction("UserProfile", "Social", new { id = id });
            }
        }

        public ActionResult CancelRequest(int? id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            UsersNotesContext uc = new UsersNotesContext();
            User user = uc.Users.Find(id);
            int curUserId = uc.Users.Where(u => u.Nickname == User.Identity.Name).FirstOrDefault().Id;
            if (id == null || user == null || user.Nickname == User.Identity.Name)
                return RedirectToAction("Personal", "Notes");
            else
            {
                try
                {
                    UsersRelation ur = uc.UserRelations.Where(u => (u.Friends == false && u.User1Id == curUserId && u.User2Id == (int)id)).Single();
                    uc.UserRelations.Remove(ur);
                    uc.SaveChanges();
                }
                catch (Exception) { return RedirectToAction("UserProfile", "Social", new { id = id }); }
            }
            return RedirectToAction("UserProfile", "Social", new { id = id });
        }

        public ActionResult RejectRequest(int? id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            UsersNotesContext uc = new UsersNotesContext();
            User user = uc.Users.Find(id);
            int curUserId = uc.Users.Where(u => u.Nickname == User.Identity.Name).FirstOrDefault().Id;
            if (id == null || user == null || user.Nickname == User.Identity.Name)
                return RedirectToAction("Personal", "Notes");
            else
            {
                try 
                {
                    UsersRelation ur = uc.UserRelations.Where(u => (u.Friends == false && u.User2Id == curUserId && u.User1Id == (int)id)).Single();
                    uc.UserRelations.Remove(ur);
                    uc.SaveChanges();
                }
                catch (Exception) { return RedirectToAction("UserProfile", "Social", new { id = id }); }                
            }
            return RedirectToAction("UserProfile", "Social", new { id = id });
        }

        public ActionResult RemoveFromFriends(int? id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            UsersNotesContext uc = new UsersNotesContext();
            User user = uc.Users.Find(id);
            int curUserId = uc.Users.Where(u => u.Nickname == User.Identity.Name).FirstOrDefault().Id;
            if (id == null || user == null || user.Nickname == User.Identity.Name)
                return RedirectToAction("Personal", "Notes");
            else
            {
                try
                {
                    UsersRelation ur = uc.UserRelations.Where(u => u.Friends == true && 
                        (u.User1Id == curUserId && u.User2Id == (int)id) ||
                        (u.User2Id == curUserId && u.User1Id == (int)id)).Single();
                    uc.UserRelations.Remove(ur);
                    uc.SaveChanges();
                }
                catch (Exception) { return RedirectToAction("UserProfile", "Social", new { id = id }); }
                
            }
            return RedirectToAction("UserProfile", "Social", new { id = id });
        }

        [HttpGet]
        public ActionResult FindUser()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            else
                return View();
        }

        [HttpPost]
        public ActionResult FindUser(string userNickName)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            User user = null;
            using (UsersNotesContext uc = new UsersNotesContext())
                user = uc.Users.Where(u => u.Nickname == userNickName).FirstOrDefault();
            if (user != null)
                return RedirectToAction("UserProfile", "Social", new { id = user.Id });
            else
            {
                ModelState.AddModelError("", "Пользователь с таким именем не найден");
                return View();
            }
        }

    }
}