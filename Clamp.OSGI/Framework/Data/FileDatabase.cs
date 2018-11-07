using Clamp.OSGI.Framework.Data.Description;
using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    internal class FileDatabase
    {
        private Stream updatingLock;

        private bool inTransaction;
        private string rootDirectory;
        private Hashtable foldersToUpdate;
        private Hashtable deletedFiles;
        private Hashtable deletedDirs;
        private IDisposable transactionLock;
        private bool ignoreDesc;

        public string DatabaseLockFile
        {
            get { return Path.Combine(rootDirectory, "fdb-lock"); }
        }

        public bool IgnoreDescriptionData
        {
            get { return ignoreDesc; }
            set { ignoreDesc = value; }
        }

        internal string UpdateDatabaseLockFile
        {
            get { return Path.Combine(rootDirectory, "fdb-update-lock"); }
        }


        #region public mehtod

        public FileDatabase(string rootDirectory)
        {
            this.rootDirectory = rootDirectory;
        }

        public string[] GetDirectories(string dir)
        {
            return Directory.GetDirectories(dir);
        }



        public bool DirExists(string dir)
        {
            return Directory.Exists(dir);
        }

        public void CreateDir(string dir)
        {
            Directory.CreateDirectory(dir);
        }

        public object ReadObject(string file)
        {
            //using (Stream s = OpenRead(file))
            //{
            //    BinaryXmlReader reader = new BinaryXmlReader(s, typeMap);
            //    return reader.ReadValue("data");
            //}

            return null;
        }

        public IDisposable LockRead()
        {
            return FileLock(FileAccess.Read, -1);
        }

        public IDisposable LockWrite()
        {
            return FileLock(FileAccess.Write, -1);
        }
        public void WriteObject(string file, object obj)
        {
            //using (Stream s = Create(file))
            //{
            //    BinaryXmlWriter writer = new BinaryXmlWriter(s, typeMap);
            //    writer.WriteValue("data", obj);
            //}
        }


        public void WriteSharedObject(string objectId, string targetFile, PersistentObject obj)
        {
            WriteSharedObject(null, null, null, objectId, targetFile, obj);
        }

        public string WriteSharedObject(string directory, string sharedFileName, string extension, string objectId, string readFileName, PersistentObject obj)
        {
            string file = readFileName;

            if (file == null)
            {
                int count = 1;
                string name = GetFileKey(directory, sharedFileName, objectId);
                file = Path.Combine(directory, name + extension);

                while (Exists(file))
                {
                    count++;
                    file = Path.Combine(directory, name + "_" + count + extension);
                }
            }

            //TODO 增加数据库里
            //using (Stream s = Create(file))
            //{
            //    BinaryXmlWriter writer = new BinaryXmlWriter(s, typeMap);
            //    writer.WriteBeginElement("File");
            //    writer.WriteValue("id", objectId);
            //    writer.WriteValue("data", obj);
            //    writer.WriteEndElement();
            //}


            return file;
        }

        public bool BeginTransaction()
        {
            if (inTransaction)
                throw new InvalidOperationException("Already in a transaction");

            transactionLock = LockWrite();
            try
            {
                updatingLock = new FileStream(UpdateDatabaseLockFile, System.IO.FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                // The database is already being updated. Can't do anything for now.
                return false;
            }
            finally
            {
                transactionLock.Dispose();
            }

            // Delete .new files that could have been left by an aborted database update

            transactionLock = LockRead();
            CleanDirectory(rootDirectory);

            inTransaction = true;
            foldersToUpdate = new Hashtable();
            deletedFiles = new Hashtable();
            deletedDirs = new Hashtable();
            return true;
        }

        public void CommitTransaction()
        {
            if (!inTransaction)
                return;

            try
            {
                transactionLock.Dispose();
                transactionLock = LockWrite();
                foreach (string dir in foldersToUpdate.Keys)
                {
                    foreach (string file in Directory.GetFiles(dir, "*.new"))
                    {
                        string dst = file.Substring(0, file.Length - 4);
                        File.Delete(dst);
                        File.Move(file, dst);
                    }
                }
                foreach (string file in deletedFiles.Keys)
                    File.Delete(file);
                foreach (string dir in deletedDirs.Keys)
                    Directory.Delete(dir, true);
            }
            finally
            {
                transactionLock.Dispose();
                EndTransaction();
            }
        }


        public void RollbackTransaction()
        {
            if (!inTransaction)
                return;

            try
            {
                // There is no need for write lock since existing files won't be updated.

                foreach (string dir in foldersToUpdate.Keys)
                {
                    foreach (string file in Directory.GetFiles(dir, "*.new"))
                        File.Delete(file);
                }
            }
            finally
            {
                transactionLock.Dispose();
                EndTransaction();
            }
        }
        public object ReadSharedObject(string fullFileName)
        {
            object result;
            OpenFileForPath(fullFileName, null, false, out result);
            return result;
        }

        public object ReadSharedObject(string directory, string sharedFileName, string extension, string objectId, out string fileName)
        {
            return ReadSharedObject(directory, sharedFileName, extension, objectId, false, out fileName);
        }

        public string[] GetDirectoryFiles(string dir, string pattern)
        {
            if (pattern == null || pattern.Length == 0 || pattern.EndsWith("*"))
                throw new NotSupportedException();

            if (inTransaction)
            {
                Hashtable files = new Hashtable();
                foreach (string f in Directory.GetFiles(dir, pattern))
                {
                    if (!deletedFiles.Contains(f))
                        files[f] = f;
                }
                foreach (string f in Directory.GetFiles(dir, pattern + ".new"))
                {
                    string ofile = f.Substring(0, f.Length - 4);
                    files[ofile] = ofile;
                }
                string[] res = new string[files.Count];
                int n = 0;
                foreach (string s in files.Keys)
                    res[n++] = s;
                return res;
            }
            else
                return Directory.GetFiles(dir, pattern);
        }

        public bool Exists(string fileName)
        {
            if (inTransaction)
            {
                if (deletedFiles.Contains(fileName))
                    return false;
                if (File.Exists(fileName + ".new"))
                    return true;
            }
            return File.Exists(fileName);
        }

        public void Delete(string fileName)
        {
            if (inTransaction)
            {
                if (deletedFiles.Contains(fileName))
                    return;
                if (File.Exists(fileName + ".new"))
                    File.Delete(fileName + ".new");
                if (File.Exists(fileName))
                    deletedFiles[fileName] = null;
            }
            else
            {
                File.Delete(fileName);
            }
        }

        public void Rename(string fileName, string newName)
        {
            if (inTransaction)
            {
                deletedFiles.Remove(newName);
                deletedDirs.Remove(Path.GetDirectoryName(newName));
                foldersToUpdate[Path.GetDirectoryName(newName)] = null;
                string s = File.Exists(fileName + ".new") ? fileName + ".new" : fileName;
                File.Copy(s, newName + ".new");
                Delete(fileName);
            }
            else
                File.Move(fileName, newName);
        }

        public bool DirectoryIsEmpty(string dir)
        {
            foreach (string f in Directory.GetFiles(dir))
            {
                if (!inTransaction || !deletedFiles.Contains(f))
                    return false;
            }
            return true;
        }

        public void DeleteDir(string dirName)
        {
            if (inTransaction)
            {
                if (deletedDirs.Contains(dirName))
                    return;
                if (Directory.Exists(dirName + ".new"))
                    Directory.Delete(dirName + ".new", true);
                if (Directory.Exists(dirName))
                    deletedDirs[dirName] = null;
            }
            else
            {
                Directory.Delete(dirName, true);
            }
        }

        #endregion

        #region private Method

        private void EndTransaction()
        {
            inTransaction = false;
            deletedFiles = null;
            foldersToUpdate = null;
            updatingLock.Close();
            updatingLock = null;
            transactionLock = null;
        }

        private void CleanDirectory(string dir)
        {
            foreach (string file in Directory.GetFiles(dir, "*.new"))
                File.Delete(file);

            foreach (string sdir in Directory.GetDirectories(dir))
                CleanDirectory(sdir);
        }


        private object ReadSharedObject(string directory, string sharedFileName, string extension, string objectId, bool checkOnly, out string fileName)
        {
            string name = GetFileKey(directory, sharedFileName, objectId);
            string file = Path.Combine(directory, name + extension);

            object result;

            if (OpenFileForPath(file, objectId, checkOnly, out result))
            {
                fileName = file;
                return result;
            }

            foreach (string f in GetDirectoryFiles(directory, name + "*" + extension))
            {
                if (f != file && OpenFileForPath(f, objectId, checkOnly, out result))
                {
                    fileName = f;
                    return result;
                }
            }

            fileName = null;

            return null;
        }

        private bool OpenFileForPath(string f, string objectId, bool checkOnly, out object result)
        {
            result = null;

            if (!Exists(f))
            {
                return false;
            }

            //using (Stream s = OpenRead(f))
            //{
            //    BinaryXmlReader reader = new BinaryXmlReader(s, typeMap);
            //    reader.ReadBeginElement();
            //    string id = reader.ReadStringValue("id");
            //    if (objectId == null || objectId == id)
            //    {
            //        if (!checkOnly)
            //            result = reader.ReadValue("data");
            //        return true;
            //    }
            //}
            //using (LiteDatabase liteDatabase = new LiteDatabase(f))
            //{
            //    if (objectId == null || objectId == id)
            //    {
            //        if (!checkOnly)
            //            result = reader.ReadValue("data");
            //        return true;
            //    }
            //}

            return false;
        }


        private IDisposable FileLock(FileAccess access, int timeout)
        {
            DateTime tim = DateTime.Now;
            DateTime wt = tim;

            FileShare share = access == FileAccess.Read ? FileShare.Read : FileShare.None;

            string path = Path.GetDirectoryName(DatabaseLockFile);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            do
            {
                try
                {
                    return new FileStream(DatabaseLockFile, System.IO.FileMode.OpenOrCreate, access, share);
                }
                catch (IOException)
                {
                    if ((DateTime.Now - wt).TotalSeconds >= 4)
                    {
                        Console.WriteLine("正在等待 " + access + " 组件数据库被锁");
                        wt = DateTime.Now;
                    }

                }
                System.Threading.Thread.Sleep(100);
            }
            while (timeout <= 0 || (DateTime.Now - tim).TotalMilliseconds < timeout);

            throw new Exception("锁超时了");
        }

        private string GetFileKey(string directory, string sharedFileName, string objectId)
        {
            int avlen = System.Math.Min(System.Math.Max(240 - directory.Length, 10), 130);
            string name = sharedFileName + "_" + Util.GetStringHashCode(objectId).ToString("x");
            if (name.Length > avlen)
                return name.Substring(name.Length - avlen);
            else
                return name;
        }

        #endregion








    }
}
