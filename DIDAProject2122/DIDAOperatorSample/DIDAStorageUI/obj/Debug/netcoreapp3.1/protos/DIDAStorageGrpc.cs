// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: protos/DIDAStorage.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

/// <summary>
/// this service specifies how to access the storage 
/// </summary>
public static partial class DIDAStorageService
{
  static readonly string __ServiceName = "DIDAStorageService";

  [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
  static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
  {
    #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
    if (message is global::Google.Protobuf.IBufferMessage)
    {
      context.SetPayloadLength(message.CalculateSize());
      global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
      context.Complete();
      return;
    }
    #endif
    context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
  }

  [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
  static class __Helper_MessageCache<T>
  {
    public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
  }

  [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
  static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
  {
    #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
    if (__Helper_MessageCache<T>.IsBufferMessage)
    {
      return parser.ParseFrom(context.PayloadAsReadOnlySequence());
    }
    #endif
    return parser.ParseFrom(context.PayloadAsNewBuffer());
  }

  [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
  static readonly grpc::Marshaller<global::DIDAReadRequest> __Marshaller_DIDAReadRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::DIDAReadRequest.Parser));
  [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
  static readonly grpc::Marshaller<global::DIDARecordReply> __Marshaller_DIDARecordReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::DIDARecordReply.Parser));
  [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
  static readonly grpc::Marshaller<global::DIDAWriteRequest> __Marshaller_DIDAWriteRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::DIDAWriteRequest.Parser));
  [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
  static readonly grpc::Marshaller<global::DIDAVersion> __Marshaller_DIDAVersion = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::DIDAVersion.Parser));
  [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
  static readonly grpc::Marshaller<global::DIDAUpdateIfRequest> __Marshaller_DIDAUpdateIfRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::DIDAUpdateIfRequest.Parser));

  [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
  static readonly grpc::Method<global::DIDAReadRequest, global::DIDARecordReply> __Method_read = new grpc::Method<global::DIDAReadRequest, global::DIDARecordReply>(
      grpc::MethodType.Unary,
      __ServiceName,
      "read",
      __Marshaller_DIDAReadRequest,
      __Marshaller_DIDARecordReply);

  [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
  static readonly grpc::Method<global::DIDAWriteRequest, global::DIDAVersion> __Method_write = new grpc::Method<global::DIDAWriteRequest, global::DIDAVersion>(
      grpc::MethodType.Unary,
      __ServiceName,
      "write",
      __Marshaller_DIDAWriteRequest,
      __Marshaller_DIDAVersion);

  [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
  static readonly grpc::Method<global::DIDAUpdateIfRequest, global::DIDAVersion> __Method_updateIfValueIs = new grpc::Method<global::DIDAUpdateIfRequest, global::DIDAVersion>(
      grpc::MethodType.Unary,
      __ServiceName,
      "updateIfValueIs",
      __Marshaller_DIDAUpdateIfRequest,
      __Marshaller_DIDAVersion);

  /// <summary>Service descriptor</summary>
  public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
  {
    get { return global::DIDAStorageReflection.Descriptor.Services[0]; }
  }

  /// <summary>Base class for server-side implementations of DIDAStorageService</summary>
  [grpc::BindServiceMethod(typeof(DIDAStorageService), "BindService")]
  public abstract partial class DIDAStorageServiceBase
  {
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public virtual global::System.Threading.Tasks.Task<global::DIDARecordReply> read(global::DIDAReadRequest request, grpc::ServerCallContext context)
    {
      throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public virtual global::System.Threading.Tasks.Task<global::DIDAVersion> write(global::DIDAWriteRequest request, grpc::ServerCallContext context)
    {
      throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public virtual global::System.Threading.Tasks.Task<global::DIDAVersion> updateIfValueIs(global::DIDAUpdateIfRequest request, grpc::ServerCallContext context)
    {
      throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
    }

  }

  /// <summary>Creates service definition that can be registered with a server</summary>
  /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
  [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
  public static grpc::ServerServiceDefinition BindService(DIDAStorageServiceBase serviceImpl)
  {
    return grpc::ServerServiceDefinition.CreateBuilder()
        .AddMethod(__Method_read, serviceImpl.read)
        .AddMethod(__Method_write, serviceImpl.write)
        .AddMethod(__Method_updateIfValueIs, serviceImpl.updateIfValueIs).Build();
  }

  /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
  /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
  /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
  /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
  [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
  public static void BindService(grpc::ServiceBinderBase serviceBinder, DIDAStorageServiceBase serviceImpl)
  {
    serviceBinder.AddMethod(__Method_read, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::DIDAReadRequest, global::DIDARecordReply>(serviceImpl.read));
    serviceBinder.AddMethod(__Method_write, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::DIDAWriteRequest, global::DIDAVersion>(serviceImpl.write));
    serviceBinder.AddMethod(__Method_updateIfValueIs, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::DIDAUpdateIfRequest, global::DIDAVersion>(serviceImpl.updateIfValueIs));
  }

}
#endregion
