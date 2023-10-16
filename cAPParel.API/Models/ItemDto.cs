﻿using cAPParel.API.Entities;
using System.ComponentModel.DataAnnotations;

namespace cAPParel.API.Models
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public double Price { get; set; }
        public string Color { get; set; }
        public ItemType Type { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }

        public ICollection<Image> Images { get; set; } = new List<Image>();
        public ICollection<FileData> OtherData { get; set; } = new List<FileData>();
    }
}
