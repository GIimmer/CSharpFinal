using System;
using Microsoft.EntityFrameworkCore;

namespace CSharpFinal.Models{
    public class Bidder : DbContext{
        public int BidderId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public double user_bid { get; set; }
    }
}