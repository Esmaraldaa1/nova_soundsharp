namespace project
{
    public class MP3Player
    {
        public int id;
        public string make;
        public string model;
        public int mbsize;
        public double price;
        public int stock;
        public MP3Player(int id, string make, string model, int mbsize, double price, int stock)
        {
            this.id = id;
            this.make = make;
            this.model = model;
            this.mbsize = mbsize;
            this.price = price;
            this.stock = stock;
        }
    }
}