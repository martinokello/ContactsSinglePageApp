using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using SinglePageApp.Models;
using SinglePageApp.XMLManipulatorEngine;

namespace ContactsSinglePageApp.Controllers
{
    public class ContactsController : ApiController
    {
        // GET api/values
        private readonly XMLManipulator XMLServices = new XMLManipulator();
        [HttpGet]
        public ContactDetail GetContact(string contactId)
        {
            var physicalDataSourcePath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["virtualPath"]);
            return XMLServices.GetContact(contactId, physicalDataSourcePath);
        }

        // GET api/
        [HttpGet]
        public ContactDetail[] GetContacts()
        {
            var physicalDataSourcePath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["virtualPath"]);

            return XMLServices.GetContacts(physicalDataSourcePath);
        }

        // POST api/values
        [HttpPost]
        public void AddContact()
        {
            var physicalDataSourcePath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["virtualPath"]);
            var physicalAvatarPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["avatarPath"]);
            var contact = new ContactDetail();
            ReadFileData(contact, physicalAvatarPath);

            XMLServices.AddContact(contact, physicalDataSourcePath);
        }

        // PUT api/values/5
        [HttpPut]
        public IHttpActionResult UpdateContact()
        {
            var physicalDataSourcePath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["virtualPath"]);
            var physicalAvatarPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["avatarPath"]);
            var contact = new ContactDetail();
            ReadFileData(contact, physicalAvatarPath);

            var result = XMLServices.UpdateContact(contact, physicalDataSourcePath);
            if (result) return Ok();
            return NotFound();
        }

        [HttpPost]
        public IHttpActionResult UpdateProfilePicture()
        {
            var physicalDataSourcePath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["virtualPath"]);
            var physicalAvatarPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["avatarPath"]);
            var contact = new ContactDetail();
            ReadAvatarFileData(contact, physicalAvatarPath);

            var result = XMLServices.UpdateProfileContact(contact, physicalDataSourcePath);
            if (result) return Ok();
            return NotFound();
        }
        [HttpPost]
        public IHttpActionResult PostAndSyncData(ContactDetail[] data)
        {
            try
            {
                var physicalDataSourcePath =
                    HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["virtualPath"]);
                var physicalAvatarPath =
                    HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["avatarPath"]);
                foreach (var contact in data)
                {
                    ReadFileData(contact, physicalAvatarPath);
                    var result = XMLServices.UpdateContact(contact, physicalDataSourcePath);
                }
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }
        private void ReadFileData(ContactDetail contact, string physicalAvatarPath)
        {
            var httpRequest = HttpContext.Current.Request;
            var avatarUrl = string.Empty;
            // Check if files are available
            if (httpRequest.Files.Count > 0)
            {
                var files = new List<string>();

                // interate the files and save on the server
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var filePath = physicalAvatarPath+postedFile.FileName;
                    postedFile.SaveAs(filePath);

                    contact.Avatar = "/Avatars/" + postedFile.FileName;
                    files.Add(filePath);
                }
            }
            foreach (var key in httpRequest.Form.AllKeys)
            {
                foreach (var val in httpRequest.Form.GetValues(key))
                {
                    if (key.Equals("ContactId", StringComparison.OrdinalIgnoreCase))
                    {
                        contact.ContactId = val;
                    }
                    if (key.Equals("MobileNumber", StringComparison.OrdinalIgnoreCase))
                    {
                        contact.MobileNumber = val;
                    }
                    if (key.Equals("HomeNumber", StringComparison.OrdinalIgnoreCase))
                    {
                        contact.HomeNumber = val;
                    }
                    if (key.Equals("ContactName", StringComparison.OrdinalIgnoreCase))
                    {
                        contact.Name = val;
                    }
                    if (key.Equals("EmailAddress", StringComparison.OrdinalIgnoreCase))
                    {
                        contact.EmailAddress = val;
                    }
                }
            }
        }
        private void ReadAvatarFileData(ContactDetail contact, string physicalAvatarPath)
        {
            var httpRequest = HttpContext.Current.Request;
            var avatarUrl = string.Empty;
            // Check if files are available
            if (httpRequest.Files.Count > 0)
            {
                var files = new List<string>();

                // interate the files and save on the server
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var filePath = physicalAvatarPath + postedFile.FileName;
                    postedFile.SaveAs(filePath);

                    contact.Avatar = "/Avatars/" + postedFile.FileName;
                    files.Add(filePath);
                }
            }
            foreach (var key in httpRequest.Form.AllKeys)
            {
                foreach (var val in httpRequest.Form.GetValues(key))
                {
                    if (key.Equals("ContactId", StringComparison.OrdinalIgnoreCase))
                    {
                        contact.ContactId = val;
                    }

                }
            }
        }
    }
}
