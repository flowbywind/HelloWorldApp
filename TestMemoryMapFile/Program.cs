using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestMemoryMapFile
{
    class Program
    {
        static void Main(string[] args)
        {
            //Main2(args);
            CreateNew create = new CreateNew();
            create.Init();
            Console.Read();
            Program p = new Program();
            p.Save();
            Console.WriteLine(p.GetRealTimeInfo());
            Console.Read();

        }

        #region function 1
        private string MemoryMappedFileName
        {
            get
            {
                string memoryMappedFileName;
                memoryMappedFileName = @"Global\AutoJobServiceSharedMappedFile"; //edit by gwt add Global\
                return memoryMappedFileName;
            }
        }
        private MemoryMappedFile _mappedFile;
        public MemoryMappedFile MemoryMappedFile
        {

            get
            {
                try
                {
                    //return _mappedFile ?? (_mappedFile = MemoryMappedFile.CreateOrOpen(MemoryMappedFileName, 100000));
                    //解决由于windows服务创建的内存映射文件 非服务进程不能访问 加入security
                    var security = new MemoryMappedFileSecurity();
                    security.AddAccessRule(new AccessRule<MemoryMappedFileRights>("everyone", MemoryMappedFileRights.FullControl, AccessControlType.Allow));
                    return _mappedFile ?? (_mappedFile = MemoryMappedFile.CreateOrOpen(MemoryMappedFileName, 100000, MemoryMappedFileAccess.ReadWrite,
                                     MemoryMappedFileOptions.DelayAllocatePages, security,
                                     HandleInheritability.Inheritable));
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public void Save()
        {
            try
            {
                lock (this)
                {
                    //var security = new MemoryMappedFileSecurity();
                    //security.AddAccessRule(new AccessRule<MemoryMappedFileRights>("Everyone", MemoryMappedFileRights.FullControl, AccessControlType.Allow));
                    using (var memoryMappedFile = MemoryMappedFile.CreateNew(MemoryMappedFileName, 100000))
                    {
                        using (MemoryMappedViewStream stream = memoryMappedFile.CreateViewStream())
                        {

                            if (stream.CanWrite)
                            {
                                BinaryFormatter formatter = new BinaryFormatter();
                                //string info = "test data";
                                //string info = GetInfo();
                                //LogManagement.Instance.AddSysEntry(SeverityLevels.Info, "记录保存内存信息 by gwt", info);
                                formatter.Serialize(stream, "test info ser");
                                //LogManagement.Instance.AddSysEntry(SeverityLevels.Info, "内存信息序列化成功","");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public string GetRealTimeInfo()
        {
            string memoryMappedFileName;
                memoryMappedFileName = @"Global\AutoJobServiceSharedMappedFile"; //edit by gwt add Global\
            using (var mmf = MemoryMappedFile.OpenExisting(MemoryMappedFileName)) { 
            using (MemoryMappedViewStream stream = mmf.CreateViewStream())
            {
                if (stream.CanRead)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    string str = (string)formatter.Deserialize(stream);
                    return str;
                }
                return "";
            }
            }
        }
        #endregion

        static void Main2(string[] args)
        {
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateNew("testmap", 10000))
            {
                bool mutexCreated;
                Mutex mutex = new Mutex(true, "testmapmutex", out mutexCreated);
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    //BinaryWriter writer = new BinaryWriter(stream);
                    //writer.Write(1);
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, "test hello");
                }
                mutex.ReleaseMutex();

                mutex.WaitOne();
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    //BinaryReader reader = new BinaryReader(stream);
                    //Console.WriteLine("Process A says: {0}", reader.ReadBoolean());
                    //Console.WriteLine("Process B says: {0}", reader.ReadBoolean());
                    //Console.WriteLine("Process C says: {0}", reader.ReadBoolean());
                    BinaryFormatter formatter = new BinaryFormatter();
                    string str = (string)formatter.Deserialize(stream);
                    Console.WriteLine(str);
                }
                mutex.ReleaseMutex();
            }
        }
    }
}
