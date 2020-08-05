using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using FolderManager.Models;
using System.Data.Entity.Core;

namespace FolderManager.Controllers {
    public class FolderController : Controller {

        /// <summary>
        /// Path where my folders JSON is stored.
        /// </summary>
        private string root_path = HttpRuntime.AppDomainAppPath + "root_folder\\";

        /// <summary>
        /// Create my index page.
        /// </summary>
        /// <returns>Returns a view for my index page.</returns>
        public ActionResult Index() {
            return View();
        }

        /// <summary>
        /// Get all folders in database
        /// </summary>
        /// <returns>JSON with the operation status, a message informing what happened and a list of all folder structured as a TreeView node.</returns>
        public ActionResult GetFolders() {
            List<TreeViewNode> nodes = new List<TreeViewNode>();

            try {
                using (var db = new FolderDBEntities()) {
                    var folders = (from f in db.Folders select f).ToList();
                    foreach (var folder in folders) {
                        TreeViewNode t = new TreeViewNode();
                        t.id = folder.Id.ToString();
                        t.parent = folder.parent != null ? folder.parent.ToString() : "#";
                        t.text = folder.name;
                        t.icon = "fa fa-folder";
                        nodes.Add(t);
                    }
                }
            } catch (EntityException ex) {
                return Json(new {
                    status = false,
                    msg = "Connection failed. Something went bad when trying to connect to database. Please try again later."
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new {
                status = true,
                response = nodes,
                msg = "Data is now available."
            }, JsonRequestBehavior.AllowGet); 
        }

        /// <summary>
        /// Finds if there's any folder which parent no longer exists.
        /// </summary>
        /// <returns>List of all the folder with no parent.</returns>
        public List<int> FindFoldersWithNoParent() {
            List<int> toBeRemoved = new List<int>();

            try {
                using (var db = new FolderDBEntities()) {

                    var folders = (from f in db.Folders select f).ToList();

                    if (folders.Count > 0) {
                        for (int i = 0; i < folders.Count; i++) {
                            bool hasDad = false;
                            if (folders[i].parent == null)
                                continue;
                            for (int j = 0; j < folders.Count; j++) {
                                if (folders[i].parent == folders[j].Id) {
                                    hasDad = true;
                                    break;
                                }
                            }
                            if (!hasDad) {
                                toBeRemoved.Add(folders[i].Id);
                            }
                        }
                    }
                }
            } catch (EntityException ex) {
                return null;
            }
            return toBeRemoved;
        }

        /// <summary>
        /// Removes a folder and all the folder under it.
        /// </summary>
        /// <param name="id">Folder ID that will be removed.</param>
        /// <returns>True or false whether or not the operation went fine.</returns>
        public Boolean RemoveFolder(string id) {

            try {
                decimal folderID = Convert.ToDecimal(id);
                using (var db = new FolderDBEntities()) {

                    var folder = (from f in db.Folders
                                  where (f.Id == folderID)
                                  select f).SingleOrDefault();

                    if (folder != null) {
                        db.Folders.Remove(folder);
                        db.SaveChanges();

                        List<int> toBeRemoved = FindFoldersWithNoParent();
                        while (toBeRemoved.Count > 0) {
                            foreach (int idTBR in toBeRemoved) {
                                var tbr = (from f in db.Folders
                                                  where (f.Id == idTBR)
                                                  select f).SingleOrDefault();
                                db.Folders.Remove(tbr);
                            }
                            db.SaveChanges();
                            toBeRemoved = FindFoldersWithNoParent();
                        }
                    }
                }
            } catch (EntityException ex) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Update the name of a folder.
        /// </summary>
        /// <param name="id">ID folder that will be edited.</param>
        /// <param name="newName">New name that the folder will receive.</param>
        /// <returns>True or false whether or not the operation went fine.</returns>
        public Boolean UpdateFolder(string id, string newName) {

            try {
                decimal folderID = Convert.ToDecimal(id);
                using (var db = new FolderDBEntities()) {

                    var folder = (from f in db.Folders
                                  where (f.Id == folderID)
                                  select f).SingleOrDefault();

                    if (folder != null) {
                        folder.name = newName;
                        db.SaveChanges();
                    }
                }
            } catch (EntityException ex) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Create a new folder.
        /// </summary>
        /// <param name="parent">ID of the new folder's parent.</param>
        /// <param name="name">Name of the nem folder</param>
        /// <returns>True or false whether or not the operation went fine.</returns>
        public Boolean AddFolder(string parent, string name) {

            try {
                using (var db = new FolderDBEntities()) {

                    var folders = (from f in db.Folders
                                select f).OrderBy(c => c.Id).ToList();

                    int id = folders[folders.Count - 1].Id + 1;

                    int? parentID = Convert.ToInt32(parent);


                    var folder = new Folder();
                    folder.Id = id;
                    folder.name = name;
                    folder.parent = parentID;

                    db.Folders.Add(folder);
                    db.SaveChanges();
                    
                }
            } catch (EntityException ex) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Perfomes sequential actions between Create, Update and Remove.
        /// </summary>
        /// <param name="actionJSON">JSON with all the actions all the actions to be executed.</param>
        /// <returns>JSON with the operation status, a message informing what happened.</returns>
        public ActionResult PerformeActions(string actionJSON) {
            dynamic actions = JsonConvert.DeserializeObject(actionJSON);
            foreach (var a in actions) {
                string typeAction = a.action;
                
                switch (typeAction) {
                    case "add":
                        string parent = a.parent;
                        string nameAdd = a.name;
                        AddFolder(parent, nameAdd);
                        break;

                    case "edit":
                        string idEdit = a.id;
                        string nameEdited = a.name;
                        UpdateFolder(idEdit, nameEdited);
                        break;

                    case "del":
                        string idDel = a.id;
                        RemoveFolder(idDel);
                        break;
                    default:
                        break;
                }
            }
            return Json(new {
                status = true,
                msg = "All actions were perfomed successfully."
            }, JsonRequestBehavior.AllowGet);
        }

    }
}