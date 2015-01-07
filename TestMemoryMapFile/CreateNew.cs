using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;
using System.Security.AccessControl;

namespace TestMemoryMapFile
{
    public class CreateNew
    {
        //public MemoryMappedFile 
        public void Init()
        {
            var security = new MemoryMappedFileSecurity();
            security.AddAccessRule(new System.Security.AccessControl.AccessRule<MemoryMappedFileRights>(new SecurityIdentifier(WellKnownSidType.WorldSid,null), MemoryMappedFileRights.FullControl,System.Security.AccessControl.AccessControlType.Allow));
            var  mmf = MemoryMappedFile.CreateNew("MYTestMap", 10000,MemoryMappedFileAccess.ReadWrite,MemoryMappedFileOptions.DelayAllocatePages,security, HandleInheritability.Inheritable);
            //using (MemoryMappedFile mmf = MemoryMappedFile.CreateNew("MYTestMap", 10000,MemoryMappedFileAccess.ReadWrite,MemoryMappedFileOptions.DelayAllocatePages,security,HandleInheritability.Inheritable))
            //MemoryMappedFileSecurity mc = mmf.GetAccessControl();
            //foreach (AccessRule<MemoryMappedFileRights> item in mc.GetAccessRules(true,true,typeof(System.Security.Principal.NTAccount)))
            //{
            //    Console.WriteLine("user:"+item.IdentityReference);
            //    Console.WriteLine("right:" + item.Rights);
            //}

            using (MemoryMappedViewStream stream = mmf.CreateViewStream())
            {
                
                if (stream.CanWrite)
                {
                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write("test data");
                    //BinaryFormatter bf = new BinaryFormatter();
                    //bf.Serialize(stream, "testData");}
                }
            }
            using (MemoryMappedViewStream stream = mmf.CreateViewStream())
            {
                if (stream.CanRead)
                {
                    BinaryReader reader = new BinaryReader(stream);
                    Console.WriteLine("output:" + reader.ReadString());
                    //BinaryFormatter bf = new BinaryFormatter();
                    //Console.WriteLine( bf.Deserialize(stream));
                }
            }
        }
    }
}
