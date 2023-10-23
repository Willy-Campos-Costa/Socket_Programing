namespace TServidor
{
    public class Movies
    {
        public string Name { get; private set; }
        public int AvaliableTickets { get; private set; }
        public double Price { get; private set; }

        public Movies(string name,int tikets,double price)
        {
            Name = name;
            AvaliableTickets = tikets;
            Price = price;
        }

        public void removeTicket(int value){
            AvaliableTickets -= value;
        }
    }
}