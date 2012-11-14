namespace DotNetSiemensPLCToolBoxLibrary.DBF.Index {
    public class BTree
    {
        private BTree objLesserTree = null;
        private BTree objGreaterTree = null;
        private BTree objAboveTree = null;
        private NodeKey objKey;
        private int intWhere;

        public BTree(NodeKey inkey, int inWhere, BTree TopTree)
        {
            objKey = inkey;
            intWhere = inWhere;

            if (TopTree != null)
            {
                this.objAboveTree = TopTree.findPosition(this.objKey);
                if (this.objAboveTree.Key.CompareKey(inkey) > 0)
                {
                    this.objAboveTree.SetLesser(this);
                }
                else
                {
                    this.objAboveTree.SetGreater(this);
                }
            }


        }

        public void SetGreater(BTree GreaterTree)
        {
            this.objGreaterTree = GreaterTree;
        }

        public void SetLesser(BTree LesserTree)
        {
            this.objLesserTree = LesserTree;
        }

        /// <summary>
        /// Find te position of a Key within the BinaryTree
        /// </summary>
        /// <param name="KeyToFind">The key to find</param>
        /// <returns>The BinaryTree Node that contains the Key</returns>
        private BTree findPosition(NodeKey KeyToFind)
        {
            if (objKey.CompareKey(KeyToFind) > 0)
            {
                if (objLesserTree == null)
                {
                    return this;
                }
                else
                {
                    return (objLesserTree.findPosition(KeyToFind));
                }
            }
            else
            {
                if (objGreaterTree == null) return this;
                return (objGreaterTree.findPosition(KeyToFind));
            }
        }

        private BTree goingUp(NodeKey inKey)
        {
            if (objKey.CompareKey(inKey) <= 0)
            {
                if (objAboveTree == null)
                {
                    return null;
                }
                else
                {
                    return objAboveTree.goingUp(objKey);
                }
            }
            return this;
        }

        #region SET / GET

        public int Where
        {
            get { return this.intWhere; }
        }

        /// <summary>
        /// The NodeKey of this BinaryTree
        /// </summary>
        public NodeKey Key
        {
            get { return this.objKey; }
        }

        /// <summary>
        /// Get the least tree (furthest away from root)
        /// </summary>
        public BTree Least
        {
            get
            {
                BTree returnTree = this;
                if (objLesserTree != null)
                {
                    returnTree = objLesserTree.Least;
                }
                return returnTree;
            }
        }

        /// <summary>
        /// Get the highest tree (closest to the root or root)
        /// </summary>
        public BTree Next
        {
            get
            {
                BTree returnTree = this;
                if (objGreaterTree != null)
                {
                    returnTree = objGreaterTree.Next;
                }
                return returnTree;
            }
        }

        #endregion
    }
}
