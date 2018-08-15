using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CSharpFinal.Models{
    public class Product : DbContext{
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public double CurrentBid { get; set; }
        public DateTime EndDate { get; set; }
        public List<Bidder> Bidders_On { get; set; }
        public int SellerId { get; set; }
        public User Seller { get; set; }
        public Product()
        {
            Bidders_On = new List<Bidder>();
        }
    }
}