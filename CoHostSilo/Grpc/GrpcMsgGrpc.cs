// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: GrpcMsg.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace CoHost {
  public static partial class CoHost
  {
    static readonly string __ServiceName = "CoHost.CoHost";

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

    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

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

    static readonly grpc::Marshaller<global::Echo.EchoRequest> __Marshaller_echo_EchoRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Echo.EchoRequest.Parser));
    static readonly grpc::Marshaller<global::Echo.EchoReply> __Marshaller_echo_EchoReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Echo.EchoReply.Parser));
    static readonly grpc::Marshaller<global::Google.Protobuf.WellKnownTypes.Empty> __Marshaller_google_protobuf_Empty = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Google.Protobuf.WellKnownTypes.Empty.Parser));

    static readonly grpc::Method<global::Echo.EchoRequest, global::Echo.EchoReply> __Method_Echo = new grpc::Method<global::Echo.EchoRequest, global::Echo.EchoReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Echo",
        __Marshaller_echo_EchoRequest,
        __Marshaller_echo_EchoReply);

    static readonly grpc::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::Google.Protobuf.WellKnownTypes.Empty> __Method_PerTest = new grpc::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::Google.Protobuf.WellKnownTypes.Empty>(
        grpc::MethodType.Unary,
        __ServiceName,
        "PerTest",
        __Marshaller_google_protobuf_Empty,
        __Marshaller_google_protobuf_Empty);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::CoHost.GrpcMsgReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of CoHost</summary>
    [grpc::BindServiceMethod(typeof(CoHost), "BindService")]
    public abstract partial class CoHostBase
    {
      public virtual global::System.Threading.Tasks.Task<global::Echo.EchoReply> Echo(global::Echo.EchoRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::Google.Protobuf.WellKnownTypes.Empty> PerTest(global::Google.Protobuf.WellKnownTypes.Empty request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for CoHost</summary>
    public partial class CoHostClient : grpc::ClientBase<CoHostClient>
    {
      /// <summary>Creates a new client for CoHost</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public CoHostClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for CoHost that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public CoHostClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected CoHostClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected CoHostClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual global::Echo.EchoReply Echo(global::Echo.EchoRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return Echo(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::Echo.EchoReply Echo(global::Echo.EchoRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_Echo, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::Echo.EchoReply> EchoAsync(global::Echo.EchoRequest request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return EchoAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::Echo.EchoReply> EchoAsync(global::Echo.EchoRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_Echo, null, options, request);
      }
      public virtual global::Google.Protobuf.WellKnownTypes.Empty PerTest(global::Google.Protobuf.WellKnownTypes.Empty request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return PerTest(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::Google.Protobuf.WellKnownTypes.Empty PerTest(global::Google.Protobuf.WellKnownTypes.Empty request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_PerTest, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::Google.Protobuf.WellKnownTypes.Empty> PerTestAsync(global::Google.Protobuf.WellKnownTypes.Empty request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return PerTestAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::Google.Protobuf.WellKnownTypes.Empty> PerTestAsync(global::Google.Protobuf.WellKnownTypes.Empty request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_PerTest, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override CoHostClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new CoHostClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(CoHostBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_Echo, serviceImpl.Echo)
          .AddMethod(__Method_PerTest, serviceImpl.PerTest).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static void BindService(grpc::ServiceBinderBase serviceBinder, CoHostBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_Echo, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Echo.EchoRequest, global::Echo.EchoReply>(serviceImpl.Echo));
      serviceBinder.AddMethod(__Method_PerTest, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Google.Protobuf.WellKnownTypes.Empty, global::Google.Protobuf.WellKnownTypes.Empty>(serviceImpl.PerTest));
    }

  }
}
#endregion
