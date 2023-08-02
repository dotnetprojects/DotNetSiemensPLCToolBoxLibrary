namespace DotNetSiemensPLCToolBoxLibrary.General
{
    public static class ByteArrayExtensions
    {
        public static int IndexOfBytes(this byte[] array, byte[] pattern, int startIndex = 0)
        {
            int i = startIndex;
            int endIndex = array.Length;
            int fidx = 0;

            while (i++ < endIndex - pattern.Length)
            {
                fidx = (array[i] == pattern[fidx]) ? ++fidx : 0;
                if (fidx == pattern.Length)
                {
                    return i - fidx + 1;
                }
            }
            return -1;
        }
    }
}
