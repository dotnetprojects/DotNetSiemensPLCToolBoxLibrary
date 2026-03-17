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

        private void CmdAttachV21_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Result = "21";
            this.Close();
        }

        private void CmdAttachV20_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Result = "20";
            this.Close();
        }

        private void CmdAttachV19_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Result = "19";
            this.Close();
        }

        private void CmdAttachV18_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Result = "18";
            this.Close();
        }
        private void CmdAttachV17_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Result = "17";
            this.Close();
        }
        private void CmdAttachV16_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Result = "16";
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
