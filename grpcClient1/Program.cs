using Google.Protobuf;
using Grpc.Net.Client;
using grpcFileServiceClinet;


try
{
    var channel = GrpcChannel.ForAddress("https://localhost:7060");

    var client = new FileService.FileServiceClient(channel);

    string file = "C:\\Users\\Nicat\\source\\repos\\GrpcServer\\grpcClient1\\Files\\1.mp4";

    using FileStream fileStream = new FileStream(file, FileMode.Open);

    var content = new BytesContent
    {
        FileSize = fileStream.Length,
        ReadedByte = 0,
        FileInfo = new grpcFileServiceClinet.FileInfo
        {
            FileName = Path.GetFileNameWithoutExtension(fileStream.Name),
            FileExtension = Path.GetExtension(fileStream.Name)
        },
    };

    var upload = client.FileUpload();
    byte[] buffer = new byte[2048];
 
    while ((content.ReadedByte = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
    {
        content.Buffer = ByteString.CopyFrom(buffer);
        await upload.RequestStream.WriteAsync(content);
    }
    await upload.RequestStream.CompleteAsync();
    fileStream.Close();
}
catch (Exception ex)
{

    throw;
}
