namespace Shopping_Online.Models
{
    public class StatisticalModel {
        public int Id { get; set; }
        public int Quantity { get; set; }   // so luong ban
        public int Sold { get; set; }       // so luong don hang
        public int Revenue { get; set; }    // Doanh thu
        public int Profit { get; set; }     // Loi nhuan
        public DateTime DateCreated { get; set; }
    }
    
}