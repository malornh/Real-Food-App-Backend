﻿using RF1.Models;
using RF1.Models.Enums;

namespace RF1.Dtos
{
    public class FarmFullInfoDto
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Rating { get; set; }
        public List<ProductDto> Products { get; set; }
    }
}
