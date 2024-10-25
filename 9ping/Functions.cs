using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Net;

namespace ninePing
{
    class Functions
    {
        public static void CheckDirectory(string folder)
        {
            string path = GlobalConfig.Path.AppPath + folder;
            try
            {
                if (Directory.Exists(path))
                {
                    string[] subdirectoryEntries = Directory.GetDirectories(path); ;
                    return;
                }
                //MessageBox.Show(folder + " , " + path);
                Directory.CreateDirectory(path);

            }
            catch (Exception err)
            {
                MessageBox.Show("The process failed: " + err.ToString());
            }
            finally { }
        }


        public static void DelDirectory(string folder)
        {
            string path = GlobalConfig.Path.ProfilesPath + folder;
            try
            {
                //MessageBox.Show(folder + " , " + path);
                Directory.Delete(path,true);
                //MessageBox.Show(folder + " Deleted");
            }
            catch (Exception err)
            {
                MessageBox.Show("The process failed: "+ err.ToString());
            }
            finally { }
        }
        public static void RenameDirectory(string folderSRC, string folderDST)
        {
            string pathSRC = GlobalConfig.Path.AppPath + folderSRC;
            string pathDST = GlobalConfig.Path.AppPath + folderDST;

            try
            {
                
               // MessageBox.Show(pathSRC +" --> " +pathDST);
                Directory.Move(pathSRC, pathDST);
            }
            catch (Exception err)
            {
                MessageBox.Show("The process failed: " + err.ToString());
            }
            finally { }
        }


        public static string CompareVersions(string LatestVersion)
        {
            if (LatestVersion.Length > 0)
            {
                string verCurrentStr = GetVersion();
                if (verCurrentStr == null || LatestVersion == null)
                    return "unknown";
                Version verCurrent = new Version(verCurrentStr);
                Version verNew = new Version(LatestVersion);

                if (verNew.CompareTo(verCurrent) > 0)
                    return "upgrade";
                else
                    return "OK";
            }
            else
            {
                return "unknown";

            }
        }
        public static string GetVersion()
        {
            string[] ProductVersion = Application.ProductVersion.Split('.');

            return ProductVersion[0] + "." + ProductVersion[1] + "." + ProductVersion[2];
        }
        public static string GetVersionRevision()
        {
            string[] ProductVersion = Application.ProductVersion.Split('.');

            return ProductVersion[3];
        }
    }
}
