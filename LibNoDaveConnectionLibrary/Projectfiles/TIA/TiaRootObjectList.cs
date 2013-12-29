using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA
{
    public class TiaRootObjectList
    {
        private TiaFileObject obj;

        public List<TiaRootObjectEntry> TiaRootObjectEntrys { get; set; }
        public TiaRootObjectList(TiaFileObject obj)
        {
            this.obj = obj;

            TiaRootObjectEntrys = new List<TiaRootObjectEntry>();

            var b = new BinaryReader(new MemoryStream(obj.Data));
            var bytesize = b.ReadInt32();

            var cnt = b.ReadInt32();
            for (int i = 0; i < cnt; i++)
            {
                TiaRootObjectEntrys.Add(new TiaRootObjectEntry() {ObjectId = b.ReadTiaObjectId(), Name = b.ReadString()});                
            }
        }
    }
}
