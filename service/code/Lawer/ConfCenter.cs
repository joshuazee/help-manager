using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CMHL.Lawer
{
    public class ConfCenter
    {
        private static XmlDocument appDoc = LoadApp();

        private static XmlDocument LoadApp()
        {
            XmlDocument appDoc = new XmlDocument();
            try
            {
                appDoc.Load(AppHome.Config + "config.xml");
            }
            catch
            {
                return null;
            }

            return appDoc;
        }

        internal static string GetStringInstance(XmlDocument doc, string selectPattern)
        {
            string retVal = string.Empty;
            if (doc == null)
            {
                ReLoad();
            }
            XmlNode sne = doc.SelectSingleNode(selectPattern);
            if (sne != null)
            {
                return sne.InnerXml;
            }
            return string.Empty;
        }

        public static void ReLoad()
        {
            appDoc = LoadApp();
        }

        public static string Connection
        {
            get
            {
                return GetStringInstance(appDoc, @"/config/connection");
            }
        }

        public static int ImportantUserRoleLevel
        {
            get
            {
                string t = GetStringInstance(appDoc, @"/config/ImportantUserRoleLevel");
                if(!string.IsNullOrEmpty(t))
                {
                    return Convert.ToInt32(t);
                }
                else
                {
                    return 0;
                }
            }
        }

        public static int AdministratorUserRoleLevel
        {
            get
            {
                string t = GetStringInstance(appDoc, @"/config/AdministratorUserRoleLevel");
                if (!string.IsNullOrEmpty(t))
                {
                    return Convert.ToInt32(t);
                }
                else
                {
                    return 0;
                }
            }
        }

        public static int LoginAdminRoleLevel
        {
            get
            {
                string t = GetStringInstance(appDoc, @"/config/LoginAdminRoleLevel");
                if (!string.IsNullOrEmpty(t))
                {
                    return Convert.ToInt32(t);
                }
                else
                {
                    return 0;
                }
            }
        }

        public static string MailAppKey
        {
            get
            {
                return GetStringInstance(appDoc, @"/config/MailAppKey");
            }
        }

        public static string MailAppSecret
        {
            get
            {
                return GetStringInstance(appDoc, @"/config/MailAppSecret");
            }
        }

        public static string MailTemplateID1
        {
            get
            {
                return GetStringInstance(appDoc, @"/config/MailTemplateID1");
            }
        }

        public static string VideoAppKey
        {
            get
            {
                return GetStringInstance(appDoc, @"/config/VideoAppKey");
            }
        }

        public static string VideoAppSecret
        {
            get
            {
                return GetStringInstance(appDoc, @"/config/VideoAppSecret");
            }
        }

        public static string VideoBucket
        {
            get
            {
                return GetStringInstance(appDoc, @"/config/VideoBucket");
            }
        }

        public static string AppDownloadPath
        {
            get
            {
                return GetStringInstance(appDoc, @"/config/AppDownloadPath");
            }
        }
    }
}
