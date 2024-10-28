using System.Net.WebSockets;
using System.Net;
using System.Diagnostics;

namespace OpenGLWinForms;

internal static class StreamingDriver
{
    private static readonly AutoResetEvent resetEvent = new(false);
    private static byte[] data = [];

    public static async Task StartWebSocketServer(CancellationToken token)
    {
        HttpListener listener = new();
        listener.Prefixes.Add("http://localhost:5000/");
        listener.Start();
        Console.WriteLine("WebSocket server started.");
        try
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "Stream.html",
                UseShellExecute = true
            });
        } catch (Exception e)
        {
            Console.WriteLine("Can't start browser: " + e.Message);
        }

        while (!token.IsCancellationRequested)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            Console.WriteLine("WebSocket Context created.");
            if (context.Request.IsWebSocketRequest)
            {
                HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
                while (webSocketContext.WebSocket.State == WebSocketState.Open && resetEvent.WaitOne() && !token.IsCancellationRequested)
                {
                    // After each frame is rendered, send it to the client
                    await webSocketContext.WebSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, token);
                }
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    public static void SendPixelData(byte[] pixelData)
    {
        data = pixelData;
        resetEvent.Set();
    }
}
