using System;
using System.IO;
using DotNetSiemensPLCToolBoxLibrary.DBF.Structures.MDX;

namespace DotNetSiemensPLCToolBoxLibrary.DBF.Index.MDX {
    /// <summary>
    /// A MDX-Structure (Part of a MDX-File)
    /// </summary>
    public class MDX : Index
    {
        private short shortTagPosition;
        private MDXTagHeader objTagHeader;
        private MDX4TagTableHeader objTagDescription;
        private MNode objNode;


        public MDX(short TagPosition)
        {
            this.shortTagPosition = TagPosition;
            this.objTagHeader = new MDXTagHeader();
            this.objTagTableHeader = new MDX4TagTableHeader();
        }

        /// <summary>
        /// Read the MDX subentry from the parent MDXFile structure
        /// </summary>
        /// <param name="Reader">A binary reader that contains the filestream of the MDX File</param>
        /// <param name="LengthOfTagTableHeader">The byte length of the TagTableHeader</param>
        /// <param name="StartPosition">The start position of the first tag table header</param>
        /// <returns>"True" if the operation was successfull</returns>
        public bool Read(BinaryReader Reader, byte LengthOfTagTableHeader, int StartPosition)
        {
            bool Successfull = true;

            //Read the TagTableHeader
            this.objTagDescription = MDX4TagTableHeader.Read(Reader, LengthOfTagTableHeader, StartPosition);

            // Read the TagHeader
            this.objTagHeader = MDXTagHeader.Read(Reader, this.objTagDescription.tagHeaderPageNumber*512);


            //Fill Index (base) with values from the Header
            base.shortKeysPerNode = this.objTagHeader.maxNumberOfKeysPage;
            base.shortKeyLength = this.objTagHeader.indexKeyLength;
            base.objKeyType = this.objTagHeader.keyType;
            base.unique_key = this.objTagHeader.uniqueFlag;



            int Index_record = (int) objTagHeader.pointerToRootPage;
            int reading = Index_record;


            //Read Nodes
            MNode llNode = null;
            while (reading > 0)
            {
                if (topNode == null)
                {
                    (objNode = new MNode(shortKeysPerNode, shortKeyLength, objKeyType, Index_record, false)).Read(Reader);

                }
                else
                {
                    llNode = new MNode(shortKeysPerNode, shortKeyLength, objKeyType, Index_record, false);
                    objNode.Read(Reader);
                    objNode.PreviousNode = llNode;
                    objNode = (MNode) llNode;
                } /* endif */
                workNode = objNode;
                objNode.Position = 0;
                objNode.Read(Reader);

                if (reading > 0)
                {
                    /* if reading is zero we're still reading Nodes, when < 0 then read
                         /* a leaf */
                    Index_record = objNode.LowerLevel;
                    if (Index_record == 0)
                    {
                        Index_record = objNode.KeyRecordNumber;
                        reading = 0; /* read a leaf so last time in loop */
                        objNode.Position = 0; //	/* so sequentially reads get first record */
                    } /* Index = 0 then it's a leaf pointer */
                } /* reading > 0 */
                if (topNode == null)
                    topNode = (MNode) objNode.Clone();

            }

            return Successfull;
        }


        public override int add_entry(NodeKey key, int recno)
        {
            throw new NotImplementedException();
        }

        internal MDX4TagTableHeader objTagTableHeader { get; set; }
    }
}
