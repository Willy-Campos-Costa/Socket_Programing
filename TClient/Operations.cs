using System;
using System.Net;
using System.Net.Sockets;

namespace TClient
{
    public class Operations
    {
        public async Task Start(){
            try{
            string ip = "172.18.52.12" ; int port = 8000;
            TcpClient client = new TcpClient();
            await client.ConnectAsync(IPAddress.Parse(ip),port);
            await Communication(client);
            }catch(Exception ex){
                System.Console.WriteLine("Problema ao comunicar com o servidor: "+ex.Message);
            }
        }
        public async Task Communication(TcpClient client){
            Console.Clear();
            using (var stream = client.GetStream())
            using (var write = new StreamWriter(stream))
            using (var read = new StreamReader(stream))
            {
                //Aguardando a resposta inicial do servidor - MENU
                Console.WriteLine(await read.ReadLineAsync());                
                Console.WriteLine(await read.ReadLineAsync());
                
                //Entra no método de digitar opção e envia p/ servidor
                string value = SelectOption();                 
                System.Console.WriteLine("\nvalor: " + value);
                await write.WriteLineAsync(value);
                await write.FlushAsync();
            }
        }
        public string SelectOption(){
            while(true){
                char op = Console.ReadKey().KeyChar;
                if(op == '1' || op == '2'){
                    return op.ToString();
                }else{
                    System.Console.WriteLine(" <-- Valor invalido");
                }
            }
        }
    }
}