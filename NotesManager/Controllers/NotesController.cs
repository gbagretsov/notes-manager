﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NotesManager.Models;
using System.Data.Entity;

namespace NotesManager.Controllers
{
    public class NotesController : Controller
    {
        public ActionResult Personal()
        {
            if (User.Identity.IsAuthenticated)
            {
                UsersNotesContext uc = new UsersNotesContext();  
                if (uc.Users.Count(u => u.Nickname == User.Identity.Name) == 0)
                    return RedirectToAction("Logoff", "Account");

                int id = uc.Users.Where(u => u.Nickname == User.Identity.Name).Single().Id;
                List<Note> notes = uc.Notes.Where(n => n.CreatorId == id).ToList();
                ViewBag.NotesList = notes;
                
                // Сначала добавляем входящие заявки
                List<User> incomingList = new List<User>();
                foreach (UsersRelation ur in uc.UserRelations.Where(u => u.User2Id == id && !u.Friends).ToList())
                    incomingList.Add(uc.Users.Find(ur.User1Id));

                // Затем подтверджённых друзей
                List<User> friendsList = new List<User>();
                foreach (UsersRelation ur in uc.UserRelations.Where(u => u.Friends).ToList())
                    if (ur.User1Id == id)
                        friendsList.Add(uc.Users.Find(ur.User2Id));
                    else if (ur.User2Id == id)
                        friendsList.Add(uc.Users.Find(ur.User1Id));
                
                // Затем исходящие заявки
                List<User> requestsList = new List<User>();
                foreach (UsersRelation ur in uc.UserRelations.Where(u => u.User1Id == id && !u.Friends).ToList())
                    requestsList.Add(uc.Users.Find(ur.User2Id));

                ViewBag.IncomingList = incomingList;
                ViewBag.FriendsList = friendsList;
                ViewBag.RequestsList = requestsList;

                return View();
            }
            else
                return RedirectToAction("Index", "Home");
        }

        public ActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
                return View();
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Note model)
        {
            if (User.Identity.IsAuthenticated)
            {
                UsersNotesContext uc = new UsersNotesContext();

                uc.Notes.Add(new Note
                {
                    CreatorId = uc.Users.Where(u => u.Nickname == User.Identity.Name).Single().Id,
                    Content = model.Content,
                    Access = model.Access,
                    EventDate = model.EventDate.Date >= DateTime.Today.Date ? model.EventDate : new DateTime(1901, 1, 1),
                    NeedToNotify = model.EventDate.Date >= DateTime.Today.Date
                });
                uc.SaveChanges();
                return RedirectToAction("Personal", "Notes");
            }
            else
                return RedirectToAction("Index", "Home");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null || !User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            UsersNotesContext uc = new UsersNotesContext();
            int userId = uc.Users.Where(u => u.Nickname == User.Identity.Name).Single().Id;
            Note note = uc.Notes.Find(id);
            if (note != null && note.CreatorId == userId)
                return View(note);
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Edit(Note note)
        {
            if (note.EventDate.Date < DateTime.Today.Date)
                note.EventDate = new DateTime(1901, 1, 1);
            note.NeedToNotify = note.EventDate.Date >= DateTime.Today.Date;
            UsersNotesContext nc = new UsersNotesContext();
            nc.Entry(note).State = EntityState.Modified;
            nc.SaveChanges();
            return RedirectToAction("Personal");
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null || !User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            UsersNotesContext nc = new UsersNotesContext();
            int userId = nc.Users.Where(u => u.Nickname == User.Identity.Name).Single().Id;
            Note note = nc.Notes.Find(id);
            if (note != null && note.CreatorId == userId)
                return View(note);
            else
                return RedirectToAction("Index", "Home");
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
                return RedirectToAction("Index", "Home");
            UsersNotesContext uc = new UsersNotesContext();
            Note note = uc.Notes.Find(id);
            if (User.Identity.IsAuthenticated)
            {
                if (uc.Users.Where(u => u.Id == note.CreatorId).Single().Nickname == User.Identity.Name)
                {
                    uc.Notes.Remove(note);
                    uc.SaveChanges();
                }
                return RedirectToAction("Personal");
            }
            else
                return RedirectToAction("Index", "Home");
        }

    }
}