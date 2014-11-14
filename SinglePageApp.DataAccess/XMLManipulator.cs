using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Security.Policy;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Net;
using SinglePageApp.Interfaces;
using SinglePageApp.Models;

namespace SinglePageApp.XMLManipulatorEngine
{
    public class XMLManipulator : IXmlServices
    {
        private static readonly object lockObject = new object();

        public ContactDetail GetContact(string contactId, string urlDataStoreXml)
        {
            lock (lockObject)
            {
                XDocument doc = LoadXDocument(urlDataStoreXml);
                try
                {

                    var c =
                        doc.Descendants("Contact").SingleOrDefault(p => p.Attribute("contactId").Value == contactId);

                    if (c == null) return null;

                    return new ContactDetail
                    {
                        Name =
                            c.Descendants("Name").SingleOrDefault() != null
                                ? c.Descendants("Name").SingleOrDefault().Value
                                : "",

                        ContactId = c.Attribute("contactId") != null ? c.Attribute("contactId").Value : "",
                        MobileNumber =
                            c.Descendants("MobileNumber").SingleOrDefault() != null
                                ? c.Descendants("MobileNumber").SingleOrDefault().Value
                                : "",
                        HomeNumber =
                            c.Descendants("HomeNumber").SingleOrDefault() != null
                                ? c.Descendants("HomeNumber").SingleOrDefault().Value
                                : "",
                        Avatar =
                            c.Descendants("Avatar").SingleOrDefault() != null
                                ? c.Descendants("Avatar").SingleOrDefault().Value
                                : "",
                        EmailAddress =
                            c.Descendants("EmailAddress").SingleOrDefault() != null
                                ? c.Descendants("EmailAddress").SingleOrDefault().Value
                                : ""

                    };

                }
                catch (Exception e)
                {
                    return null;
                }
            }

        }
        public ContactDetail[] GetContacts(string urlDataStoreXml)
        {
            try
            {
                lock (lockObject)
                {
                    var results = new List<ContactDetail>();
                    XDocument doc = LoadXDocument(urlDataStoreXml);

                    var contacts = doc.Descendants("Contact");

                    foreach (var c in contacts)
                    {
                        results.Add(new ContactDetail
                        {
                            Name =c.Descendants("Name").SingleOrDefault() != null? c.Descendants("Name").SingleOrDefault().Value:"",
                            ContactId = c.Attribute("contactId") != null ? c.Attribute("contactId").Value : "",
                            MobileNumber = c.Descendants("MobileNumber").SingleOrDefault() != null ? c.Descendants("MobileNumber").SingleOrDefault().Value : "",
                            HomeNumber = c.Descendants("HomeNumber").SingleOrDefault()!= null? c.Descendants("HomeNumber").SingleOrDefault().Value:"",
                            Avatar = c.Descendants("Avatar").SingleOrDefault() != null?c.Descendants("Avatar").SingleOrDefault().Value:"",
                            EmailAddress = c.Descendants("EmailAddress").SingleOrDefault()!= null?c.Descendants("EmailAddress").SingleOrDefault().Value:""
                        });
                    }

                    return results.ToArray();
                }
            }
            catch (Exception e)
            {
                return null;
            }

        }


        public bool UpdateProfileContact(ContactDetail contact, string urlDataStoreXml)
        {
            lock (lockObject)
            {
                XDocument doc = LoadXDocument(urlDataStoreXml);
                try
                {
                    if (doc == null)
                    {
                        return false;
                    }
                    var existingContact = doc.Root.Descendants("Contact").SingleOrDefault(p => p.Attribute("contactId").Value == contact.ContactId);

                    if (existingContact != null && !string.IsNullOrEmpty(contact.Avatar))
                    {
                        existingContact.Descendants("Avatar").SingleOrDefault().SetValue(contact.Avatar);
                    }
                    SaveDocument(doc, urlDataStoreXml);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

        }
        public bool UpdateContact(ContactDetail contact, string urlDataStoreXml)
        {
            lock (lockObject)
            {
                XDocument doc = LoadXDocument(urlDataStoreXml);
                try
                {
                    if (doc == null)
                    {
                        return false;
                    }
                    var existingContact = doc.Root.Descendants("Contact").SingleOrDefault(p => p.Attribute("contactId").Value == contact.ContactId);

                    if (existingContact != null && !string.IsNullOrEmpty(contact.Name))
                    {
                        existingContact.Descendants("Name").SingleOrDefault().SetValue(contact.Name);
                    }
                    if (existingContact != null && !string.IsNullOrEmpty(contact.MobileNumber))
                    {
                        existingContact.Descendants("MobileNumber").SingleOrDefault().SetValue(contact.MobileNumber);
                    }
                    if (existingContact != null && !string.IsNullOrEmpty(contact.HomeNumber))
                    {
                        existingContact.Descendants("HomeNumber").SingleOrDefault().SetValue(contact.HomeNumber);
                    }

                    if (existingContact != null && !string.IsNullOrEmpty(contact.EmailAddress))
                    {
                        existingContact.Descendants("EmailAddress").SingleOrDefault().SetValue(contact.EmailAddress);
                    }

                    if (existingContact != null && !string.IsNullOrEmpty(contact.Avatar))
                    {
                        existingContact.Descendants("Avatar").SingleOrDefault().SetValue(contact.Avatar);
                    }
                    if (existingContact == null)
                    {
                        AddContact(contact,urlDataStoreXml);
                    }
                    SaveDocument(doc, urlDataStoreXml);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

        }
        public void AddContact(ContactDetail contact, string urlDataStoreXml)
        {
            lock (lockObject)
            {
                XDocument doc = LoadXDocument(urlDataStoreXml);
                if (doc == null)
                {
                    doc = new XDocument();
                    doc.Add(new XElement("Contacts"));
                }

                var contacts = doc.Root.Descendants("Contacts");
                var cont = new XElement("Contact");
                var id = Guid.NewGuid();
                cont.SetAttributeValue("contactId",id);
                if (!string.IsNullOrEmpty(contact.Name))
                {
                    var name = new XElement("Name", contact.Name);
                    cont.Add(name);
                }
                if (!string.IsNullOrEmpty(contact.MobileNumber))
                {
                    var mobNumber = new XElement("MobileNumber", contact.MobileNumber);
                    cont.Add(mobNumber);
                }
                if (!string.IsNullOrEmpty(contact.HomeNumber))
                {
                    var homeNumber = new XElement("HomeNumber", contact.HomeNumber);
                    cont.Add(homeNumber);
                }

                if (!string.IsNullOrEmpty(contact.EmailAddress))
                {
                    var email = new XElement("EmailAddress", contact.EmailAddress);
                    cont.Add(email);
                }

                var avatar = new XElement("Avatar", contact.Avatar);
                cont.Add(avatar);

                cont.Add(contact);
                doc.Root.Add(cont);
                SaveDocument(doc, urlDataStoreXml);
            }
        }

        private XDocument LoadXDocument(string urlDataStoreXml)
        {
            XDocument doc = null;
            var fileInfo = new FileInfo(urlDataStoreXml);
            if (fileInfo.Exists)
            {
                doc = XDocument.Load(urlDataStoreXml);
            }
            return doc;
        }
        private void SaveDocument(XDocument doc, string path)
        {
            var fileInfo = new FileInfo(path);

            var stream = fileInfo.CreateText();
            var writer = new XmlTextWriter(stream);
            doc.WriteTo(writer);

            writer.Flush();
            writer.Close();
            stream.Close();
        }
    }
}
