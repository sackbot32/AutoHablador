using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FFMpegCore.Pipes;
using UnityEngine;

public class ByteFrame : IVideoFrame, IDisposable
{
    public int Width { get; }

    public int Height { get; }

    public string Format { get; }

    public ByteFrame(byte[] source,TextureFormat format,int width, int height)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Debug.Log("EL FORMATO ES " + format.ToString());
        //Format = format.ToString();
        Format = "argb";
        //Format = "bgra";
        Width = width;
        Height = height;
    }

    public byte[] Source;
    public void Dispose()
    {
        Debug.Log("DISPOSING?");
    }

    public void Serialize(Stream pipe)
    {
        var data = Source;
        pipe.Write(data, 0, data.Length);
    }

    public async Task SerializeAsync(Stream pipe, CancellationToken token)
    {
        var data = Source;
        await pipe.WriteAsync(data, 0, data.Length, token).ConfigureAwait(true);
    }

    
}
