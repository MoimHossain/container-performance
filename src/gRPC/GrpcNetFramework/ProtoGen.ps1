

$nugetDir = "C:\Users\Moim_\.nuget\packages\grpc.tools\1.22.0\tools\windows_x64"

$protocolExe = "$nugetDir\protoc.exe"
$grpcExe = "$nugetDir\grpc_csharp_plugin.exe"

& $protocolExe --proto_path="./protos" --grpc_out="./grpc" --csharp_out="./grpc" --csharp_opt=file_extension=.g.cs Account.proto --plugin=protoc-gen-grpc=$grpcExe