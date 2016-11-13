using System.Drawing;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders
{
    public static class ProjectFolderExtensions
    {
        public static Image FolderClosedImage(this ProjectFolder projectFolder)
        {
            System.Reflection.Assembly thisExe;
            thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream file = projectFolder.GetType().Assembly.GetManifestResourceStream(projectFolder.FolderClosedImageName);
            if (file == null)
                return null;
            return Image.FromStream(file);
        }

        public static Image FolderOpenedImage(this ProjectFolder projectFolder)
        {
            System.Reflection.Assembly thisExe;
            thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream file = projectFolder.GetType().Assembly.GetManifestResourceStream(projectFolder.FolderOpenedImageName);
            if (file == null)
                return null;
            return Image.FromStream(file);
        }
    }
}
