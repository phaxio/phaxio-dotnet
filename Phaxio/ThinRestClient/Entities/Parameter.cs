namespace Phaxio.ThinRestClient
{
    public class Parameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string ContentType { get; set; }
    }

    public class FileParameter
    {
        public string Name { get; set; }
        public byte[] Bytes { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}