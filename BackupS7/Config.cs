namespace BackupS7
{
    public class Config
    {
        public string Ip { get; set; }

        public int Rack { get; set; }

        public int Slot { get; set; }

        public string[] BackupBlocks { get; set; }

        public BackupType BackupType { get; set; }
    }
}
