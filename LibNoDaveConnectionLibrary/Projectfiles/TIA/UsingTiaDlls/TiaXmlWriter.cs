using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.UsingTiaDlls
{
   
    internal class TiaXmlWriter : XmlWriter
    {
        internal enum WriteStepType
        {
            TiaObjectsRoot,
            TiaObjects,
        }

        internal WriteStepType WriteStep { get; set; }

        private TiaObjectStructure _tiaObjectStructure;

        private TiaObject _currentObject = null;
        private TiaStorrageObject _currenTiaStorrageObject = null;

        private string currentElement = null;
        private string currentAttribute = null;
        private string currentString = null;

        private Dictionary<string, string> attributeHolder = new Dictionary<string, string>();

        public TiaXmlWriter(TiaObjectStructure tiaObjectStructure)
        {
            this._tiaObjectStructure = new TiaObjectStructure();
            this.WriteStep = WriteStepType.TiaObjectsRoot;
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            parseCurrentElementObject();

            attributeHolder.Clear();
            currentElement = localName;

            if (localName == "StorageObject")
            {
                WriteStep = WriteStepType.TiaObjects;
            }
        }


        private void parseCurrentElementObject()
        {
            if (currentElement == "StorageObject")
            {
                _currenTiaStorrageObject = TiaStorrageObject.CreateStorrageObject(_tiaObjectStructure,
                    int.Parse(attributeHolder["id"]), int.Parse(attributeHolder["instId"]), int.Parse(attributeHolder["clusterId"]));
            }
        }

        public override void WriteEndElement()
        {
            currentElement = null;

           
        }

        public override void WriteFullEndElement()
        {

        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            currentAttribute = localName;
        }

        public override void WriteEndAttribute()
        {
            attributeHolder.Add(currentAttribute, currentString);

            currentAttribute = null;
            currentString = null;         
        }
        
       

        public override void WriteString(string text)
        {
            currentString = text;

            if (WriteStep == WriteStepType.TiaObjectsRoot)
            {
                if (currentElement == "objects" && currentAttribute == "lastInstanceId")
                    _tiaObjectStructure.lastInstanceId = int.Parse(text);
                else if (currentElement == "objects" && currentAttribute == "objectCount")
                    _tiaObjectStructure.objectCount = int.Parse(text);
            }
        }

       

        public override void WriteRaw(char[] buffer, int index, int count)
        {

        }

        #region not used

        public override void WriteStartDocument()
        {

        }

        public override void WriteStartDocument(bool standalone)
        {

        }

        public override void WriteEndDocument()
        {

        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {

        }

        public override void WriteCData(string text)
        {

        }

        public override void WriteComment(string text)
        {

        }

        public override void WriteProcessingInstruction(string name, string text)
        {

        }

        public override void WriteEntityRef(string name)
        {

        }

        public override void WriteCharEntity(char ch)
        {

        }

        public override void WriteWhitespace(string ws)
        {

        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {

        }

        public override void WriteChars(char[] buffer, int index, int count)
        {

        }
        
        public override void WriteRaw(string data)
        {

        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {

        }

        public override void Close()
        {

        }

        public override void Flush()
        {

        }

        public override string LookupPrefix(string ns)
        {
            return "";
        }

        public override WriteState WriteState
        {
            get { return WriteState.Start; }
        }

        #endregion
    }
}
