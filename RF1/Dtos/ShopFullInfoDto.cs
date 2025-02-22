﻿using RF1.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace RF1.Dtos
{
    public class ShopFullInfoDto
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Rating { get; set; }
        public List<OrderFarmDto> Orders { get; set; }
    }

    public class OrderFarmDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public double? ShopPrice { get; set; }
        public bool SoldOut { get; set; }
        public string Status { get; set; }

        [Required]
        public int? ShopId { get; set; }

        [Required]
        public int ProductId {  get; set; }
        public ShortProductDto Product { get; set; }

        public ShortFarmDto ShortFarm { get; set; }
    }

    public class ShortFarmDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
    }
}
