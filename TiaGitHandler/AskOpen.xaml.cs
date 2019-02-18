namespace TiaGitHandler
{
    /// <summary>
    /// Interaktionslogik für AskOpen.xaml
    /// </summary>
    public partial class AskOpen
    {
        public object Result;

        public AskOpen()
        {
            InitializeComponent();
        }

        private void CmdOpen_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Result = false;
            this.Close();
        }

        private void CmdAttachV151_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Result = "15.1";
            this.Close();
        }

        private void CmdAttachV14SP1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Result = "14SP1";
            this.Close();
        }
    }
}
