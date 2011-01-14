using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5;

namespace DotNetSiemensPLCToolBoxLibrary.PLCs.S5.MC5
{
    public static class MC5toComment
    {
        public static S5CommentBlock GetCommentBlock(ProjectBlockInfo blkInfo, byte[] commentBlock)
        {
            S5CommentBlock retVal = new S5CommentBlock();            

            if (commentBlock != null)
            {                
                int nr = 28;
                int hdlen = 0x7f & commentBlock[nr];

                retVal.Name = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(commentBlock, nr + 1, hdlen);
           
                nr += hdlen + 1;
                int last = 0;
                retVal.CommentLines = "";
                while (nr + 3 < commentBlock.Length)
                {
                    int zeile = commentBlock[nr];
                    int len = 0x7f & commentBlock[nr + 2];
                    string cmt = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(commentBlock, nr + 3, len);

                    retVal.CommentLines += "".PadLeft((zeile - last), '\n').Replace("\n", "\r\n");
                    retVal.CommentLines += cmt;
                    nr += len + 3;
                }

            }
            return retVal;
        }
    }
}
