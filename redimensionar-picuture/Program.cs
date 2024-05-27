using System.Drawing;
using System.Drawing.Imaging;

public class InformacoesFoto
{
    public int? TamanhoMaximoBytes { get; set; }
    public short Largura { get; set; } = 0;
    public short Altura { get; set; } = 0;
    public short LarguraMinima { get; set; } = 0;
    public short AlturaMinima { get; set; } = 0;
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Hello, World!");

        var informacoesFoto = new InformacoesFoto()
        {
            TamanhoMaximoBytes = 100 * 1024, //Multiplicamos 100 KB por 1024 para converter para bytes.
            Largura = 600,
            Altura = 2400,
            LarguraMinima = 800,
            AlturaMinima = 550
        };

        var bytesFoto = CarregarFoto(informacoesFoto);

        Console.WriteLine($"Tamanho da foto em Bytes: {bytesFoto?.Length}");
        Console.WriteLine($"Tamanho da foto em Kbs: {bytesFoto?.Length / 1024.0:F2}");
        Console.WriteLine($"Tamanho da foto em Mb: {bytesFoto?.Length / (1024.0 * 1024.0):F2}");
    }

    public static byte[]? CarregarFoto(InformacoesFoto dadosFace)
    {
        var bytesFotoOriginal = File.ReadAllBytes("IMAGEM.jpg");

        Console.WriteLine($"Tamanho original da foto em Bytes: {bytesFotoOriginal.Length}");
        Console.WriteLine($"Tamanho original da foto em Kbs: {bytesFotoOriginal.Length / 1024.0:F2}");
        Console.WriteLine($"Tamanho original da foto em Mb: {bytesFotoOriginal.Length / (1024.0 * 1024.0):F2}\r\n");

        if (dadosFace.TamanhoMaximoBytes != null)
        {
            var bytesNovaFoto = bytesFotoOriginal;
            var tamanhoNovaFoto = bytesFotoOriginal.Length;
            var larguraNovaFoto = dadosFace.Largura;
            var alturaNovaFoto = dadosFace.Altura;

            while (tamanhoNovaFoto > dadosFace.TamanhoMaximoBytes)
            {
                if (larguraNovaFoto < dadosFace.LarguraMinima || alturaNovaFoto < dadosFace.AlturaMinima)
                {
                    bytesNovaFoto = RedimensionarImagem(bytesFotoOriginal, dadosFace.LarguraMinima, dadosFace.AlturaMinima);

                    // Se ainda estiver acima do tamanho máximo, retornar null
                    if (bytesNovaFoto.Length > dadosFace.TamanhoMaximoBytes)
                    {
                        return null;
                    }

                    break;
                }

                bytesNovaFoto = RedimensionarImagem(bytesFotoOriginal, larguraNovaFoto, alturaNovaFoto);

                larguraNovaFoto = (short)(larguraNovaFoto * 0.9);
                alturaNovaFoto = (short)(alturaNovaFoto * 0.9);
                tamanhoNovaFoto = bytesNovaFoto.Length;
            }

            return bytesNovaFoto;
        }

        return RedimensionarImagem(bytesFotoOriginal, dadosFace.Largura, dadosFace.Altura);
    }

    public static byte[] RedimensionarImagem(byte[] imagemBytes, int largura, int altura)
    {
        if (imagemBytes.Length == 0)
        {
            return null;
        }

        var bitmapImagem = RedimensionarImagem(ArrayBytesToImage(imagemBytes), new Size(largura, altura));

        using (var stream = new MemoryStream())
        {
            bitmapImagem.Save(stream, ImageFormat.Jpeg);
            return stream.ToArray();
        }
    }

    public static Bitmap RedimensionarImagem(System.Drawing.Image imagem, Size tamanho)
    {
        var novaImagem = new Bitmap(tamanho.Width, tamanho.Height);
        using (var graphics = Graphics.FromImage(novaImagem))
        {
            graphics.DrawImage(imagem, 0, 0, novaImagem.Width, novaImagem.Height);
        }

        return novaImagem;
    }

    public static Image ArrayBytesToImage(byte[] base64array)
    {
        using MemoryStream ms = new(base64array);
        return Image.FromStream(ms, true);
    }
}
