using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CMHL.Lawer
{
    public class AppHome
    {
        public static string BaseDirectory
        {
            get
            {
                return System.IO.Directory.GetParent(
                   System.IO.Directory.GetParent(
                   System.AppDomain.CurrentDomain.BaseDirectory).FullName).FullName + "\\";
            }
        }

        public static string Config
        {
            get
            {
                string confCenter = BaseDirectory + @"config\";
                if (Directory.Exists(confCenter) == false)
                {
                    Directory.CreateDirectory(confCenter);
                }
                return confCenter;
            }
        }

        public static string Service
        {
            get
            {
                string serverInterface = BaseDirectory + @"service\";

                return serverInterface;
            }
        }

        public static string Buffer
        {
            get
            {
                string strBufFile = BaseDirectory + @"buffer\";
                if (Directory.Exists(strBufFile) == false)
                {
                    Directory.CreateDirectory(strBufFile);
                }
                return strBufFile;
            }
        }

        public static string Upload
        {
            get
            {
                string path = Buffer + "upload\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        public static string Download
        {
            get
            {
                string path = Buffer + "download\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        public static string Log
        {
            get
            {
                string path = BaseDirectory + "log\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        public static string MapJson
        {
            get
            {
                string path = Config + "mapjson\\";
                return path;
            }
        }
    }
}
