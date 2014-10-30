using SinglePageApp.Models;

namespace SinglePageApp.Interfaces
{
    public interface IXmlServices
    {
        ContactDetail GetContact(string contactId, string dataStoreXml);
        ContactDetail[] GetContacts(string dataStoreXml);
        void AddContact(ContactDetail contact, string urlDataStoreXml);
        bool UpdateContact(ContactDetail contact, string urlDataStoreXml);
    }
}
