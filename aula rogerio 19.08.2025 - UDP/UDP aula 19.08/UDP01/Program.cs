using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    private static UdpClient? _udp;

    public static async Task Main(string[] args)
    {
        int portaLocal = 5000;
        string ipRemoto = "127.0.0.1";
        int portaRemota = 5001;

        if (args.Length > 0 && int.TryParse(args[0], out int tmp) && tmp > 0 && tmp <= 65535) portaLocal = tmp;
        if (args.Length > 1) ipRemoto = args[1];
        if (args.Length > 2 && int.TryParse(args[2], out tmp) && tmp > 0 && tmp <= 65535) portaRemota = tmp;

        Console.WriteLine($"[P2P-UDP] Porta local: {portaLocal} | Par remoto: {ipRemoto}:{portaRemota}");

        try
        {
            IPEndPoint epLocal = new IPEndPoint(IPAddress.Any, portaLocal);
            _udp = new UdpClient(epLocal);

            IPEndPoint[] epRemoto = new IPEndPoint[27];
            for (int i=0;i<epRemoto.Length;i++)
                epRemoto[i] = new IPEndPoint(IPAddress.Parse(ipRemoto+"."+(i+2)), portaRemota);

            IPEndPoint localEscolhido = (IPEndPoint)_udp.Client.LocalEndPoint!;
            Console.WriteLine("[P2P-UDP] Ouvindo em " + localEscolhido.Address + ":" + localEscolhido.Port);
            Console.WriteLine("Digite mensagens e pressione Enter. Ctrl+C para sair.\n");

            Console.CancelKeyPress += OnCancelKeyPress;

            Task receiveTask = ReceiveLoopAsync(_udp);
            Task sendTask = SendLoopAsync(_udp, epRemoto);

            await Task.WhenAll(receiveTask, sendTask);
        }
        catch (SocketException ex)
        {
            Console.WriteLine("[ERRO] Socket: " + ex.Message);
        }
        finally
        {
            if (_udp != null)
            {
                try { _udp.Close(); } catch { }
            }
            Console.WriteLine("[P2P-UDP] Encerrado.");
        }
    }

    private static async Task ReceiveLoopAsync(UdpClient udp)
    {
        while (true)
        {
            UdpReceiveResult result;
            try
            {
                result = await udp.ReceiveAsync();
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (SocketException ex)
            {
                Console.WriteLine("[RECV-ERRO] " + ex.Message);
                continue;
            }

            IPEndPoint remoto = result.RemoteEndPoint;
            string msg = Encoding.UTF8.GetString(result.Buffer);
            string ts = DateTime.Now.ToString("HH:mm:ss");
            Console.WriteLine($"[{ts}] << {remoto.Address}:{remoto.Port} : {msg}");
        }
    }

    private static async Task SendLoopAsync(UdpClient udp, IPEndPoint[] endpointDestino)
    {
        while (true)
        {
            string? linha = Console.ReadLine();
            if (linha == null) break;

            byte[] dados = Encoding.UTF8.GetBytes(linha);
            try
            {
                foreach (IPEndPoint endpoint in endpointDestino)
                {
                    await udp.SendAsync(dados, dados.Length, endpoint);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("[SEND-ERRO] " + ex.Message);
            }
        }
    }

    private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        if (_udp != null)
        {
            try { _udp.Close(); } catch { }
        }
        Environment.Exit(0);
    }
}
