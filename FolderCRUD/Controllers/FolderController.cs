using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

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
        /// Get the JSON with the current folders structure
        /// </summary>
        /// <returns>JSON with the operation status, a message informing what happened and a JSON.</returns>
        public ActionResult Get() {
            try {
                string jsonFilePath = root_path + "folder_tree.json";
                if (System.IO.File.Exists(jsonFilePath)) {
                    string json = System.IO.File.ReadAllText(jsonFilePath);
                    return Json(new {
                        status = true,
                        msg = "Data is now available.",
                        response = json
                    }, JsonRequestBehavior.AllowGet);
                } else {
                    return Json(new {
                        status = false,
                        msg = "Data is now available.",
                        createNewJSON = true
                    }, JsonRequestBehavior.AllowGet);
                }
                
                
            } catch (UnauthorizedAccessException) {
                return Json(new {
                    status = false,
                    msg = "Folder cannot be created in these path due to accesse authorization."
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Persiste the folders structure into a JSON file.
        /// </summary>
        /// <param name="json">JSON file with the current.</param>
        /// <returns>JSON with the operation status and a message informing what happened.</returns>
        public ActionResult UpdateDB(string json) {

            string path = root_path + "folder_tree.json";
            try {
                System.IO.File.WriteAllText(path, json);
                return Json(new {
                    status = true,
                    msg = "Data has now been persisted."
                }, JsonRequestBehavior.AllowGet);
            } catch (UnauthorizedAccessException) {
                return Json(new {
                    status = false,
                    msg = "Folder cannot be created in these path due to accesse authorization."
                }, JsonRequestBehavior.AllowGet);
            }   
        }
    }
}