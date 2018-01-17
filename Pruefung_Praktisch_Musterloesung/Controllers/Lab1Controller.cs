using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Web.Mvc;
using System.Linq;

namespace Pruefung_Praktisch_Musterloesung.Controllers
{
    public class Lab1Controller : Controller
    {
        /**
         * 
         * 1. Cross Site Scripting, Path Traversal
		 *
		 * 2. localhost:50374/Lab1/index?type=../../Config.sys  -> Path Traversal
		 *	  localhost:50374/Lab1/index?type=<script type="text/javascript">alert("XSS");</script>  -> Cross Site Scripting
         * 
		 * 3. Mit Hilfe der ersten URL kann auf irgendeine Datei auf dem Computer zugegriffen werden, indem man mit den zwei Punkten eine Hierarchie züruck kommt im Filesystem.
		 * Dieses file wird dann auf der Seite geladen
		 *
		 * Mit Hilfe der zweiten URL kann man den Webserver javascript code ausführen lassen, da die Script Tags ohne weiteres in die Applikation gelangen.
         * */


        public ActionResult Index()
        {
            var type = Request.QueryString["type"];

            if (string.IsNullOrEmpty(type) || type.Contains("/") || type.Contains(".."))
            {
                type = "lions";                
            }

            var path = "~/Content/images/" + type;

            List<List<string>> fileUriList = new List<List<string>>();

            if (Directory.Exists(Server.MapPath(path)))
            {
                var scheme = Request.Url.Scheme; 
                var host = Request.Url.Host; 
                var port = Request.Url.Port;
                
                string[] fileEntries = Directory.GetFiles(Server.MapPath(path));
                foreach (var filepath in fileEntries)
                {
                    var filename = Path.GetFileName(filepath);
                    var imageuri = scheme + "://" + host + ":" + port + path.Replace("~", "") + "/" + filename;

                    var urilistelement = new List<string>();
                    urilistelement.Add(filename);
                    urilistelement.Add(imageuri);
                    urilistelement.Add(type);

                    fileUriList.Add(urilistelement);
                }
            }
            
            return View(fileUriList);
        }

        public ActionResult Detail()
        {
            var file = Request.QueryString["file"];
            var type = Request.QueryString["type"];

            if (string.IsNullOrEmpty(file) || file.Contains("/") || type.Contains(".."))
            {
                file = "Lion1.jpg";
            }
            if (string.IsNullOrEmpty(type) || type.Contains("/") || type.Contains(".."))
            {
                file = "lions";
            }

            var relpath = "~/Content/images/" + type + "/" + file;

            List<List<string>> fileUriItem = new List<List<string>>();
            var path = Server.MapPath(relpath);

            if (System.IO.File.Exists(path))
            {
                var scheme = Request.Url.Scheme;
                var host = Request.Url.Host;
                var port = Request.Url.Port;
                var absolutepath = Request.Url.AbsolutePath;

                var filename = Path.GetFileName(file);
                var imageuri = scheme + "://" + host + ":" + port + "/Content/images/" + type + "/" + filename;

                var urilistelement = new List<string>();
                urilistelement.Add(filename);
                urilistelement.Add(imageuri);
                urilistelement.Add(type);

                fileUriItem.Add(urilistelement);
            }
            
            return View(fileUriItem);
        }
    }
}