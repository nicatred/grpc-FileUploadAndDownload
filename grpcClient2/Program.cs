using Grpc.Net.Client;
using grpcFileServiceDownloadClinet;


var channel = GrpcChannel.ForAddress("https://localhost:7060");
var client = new FileService.FileServiceClient(channel);

string downloadPath = "C:\\Users\\Nicat\\source\\repos\\GrpcServer\\grpcClient2\\Downloads";

var fileInfo = new grpcFileServiceDownloadClinet.FileInfo
{
    FileExtension = ".mp4",
    FileName = "1"
};


FileStream fileStream = null;

var request = client.FileDownload(fileInfo);

CancellationTokenSource cancellationToken = new CancellationTokenSource();
int count = 0;
decimal chunkSize = 0;  

while (await request.ResponseStream.MoveNext(cancellationToken.Token))
{
    if (count == 0)
    {
        fileStream = new FileStream(@$"{downloadPath}\{request.ResponseStream.Current.FileInfo.FileName}{request.ResponseStream.Current.FileInfo.FileExtension}", FileMode.Create);
        fileStream.SetLength(request.ResponseStream.Current.FileSize);
    }

    var buffer = request.ResponseStream.Current.Buffer.ToByteArray();
    await fileStream.WriteAsync(buffer, 0, request.ResponseStream.Current.ReadedByte);

    Console.WriteLine($"{Math.Round(((chunkSize += request.ResponseStream.Current.ReadedByte) * 100) / request.ResponseStream.Current.FileSize)}%");
}