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

        public string removeTicket(int value){
            if(AvaliableTickets <= 0){
                return "Ingresso esgotado! Escolha outro filme";
            }else{
            AvaliableTickets -= value;
                return $"Ingresso do filme {Name} comprado com sucesso! Se divirta";
            }
        }
    }
}