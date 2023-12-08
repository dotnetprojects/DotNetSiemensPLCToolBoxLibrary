namespace DotNetSiemensPLCToolBoxLibrary.Communication.FetchWrite
{
    public enum OperationCode : byte
    {
        Write = 3,
        WriteAnswer = 4,
        Fetch = 5,
        FetchAnswer = 6,
    }
}