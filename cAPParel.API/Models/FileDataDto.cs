using cAPParel.API.Entities;

namespace cAPParel.API.Models
{
    public class FileDataDto
    {
        public int Id { get; set; }
        public byte[] Data { get; set; }
        public string Description { get; set; }
        public DataType Type { get; set; }
    }
}
