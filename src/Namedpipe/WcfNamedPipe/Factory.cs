

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WcfNamedPipe
{
    /// <summary>
    /// Encapsulates WCF service hosting and client proxy creation details and provides 
    /// simple API to host and consume services.
    /// </summary>
    public abstract class WcfService
    {
        #region Factory instances

        private readonly static Lazy<WcfService> _Tcp = new Lazy<WcfService>(() => new TcpServiceFactory());
        private readonly static Lazy<WcfService> _Http = new Lazy<WcfService>(() => new HttpServiceFactory());

        /// <summary>
        /// Gets the default Factory (based on NET TCP binding)
        /// </summary>
        public static WcfService DefaultFactory
        {
            get
            {
                return _Tcp.Value;
            }
        }

        /// <summary>
        /// Gets the TCP Factory (based on NET TCP binding)
        /// </summary>
        public static WcfService TcpFactory
        {
            get
            {
                return _Tcp.Value;
            }
        }

        /// <summary>
        /// Gets the HTTP Factory (based on HTTP binding)
        /// </summary>
        public static WcfService HttpFactory
        {
            get
            {
                return _Http.Value;
            }
        }

        #endregion

        #region Hosting Methods

        /// <summary>
        /// Creates and possible hosts a WCF service 
        /// </summary>
        /// <param name="serviceImplTypes">The service implementation types</param>
        /// <param name="serviceNameResolver">The name resolver</param>
        /// <param name="contractTypeResolver">The contract interface resolver</param>
        /// <param name="serviceBaseNS">Ther service namespace</param>
        /// <param name="port">The port</param>
        /// <param name="errorHandler">The error handler method</param>
        /// <param name="errorLogger">The error logger</param>
        /// <param name="warningLogger">warning message logger</param>
        /// <param name="infoLogger">Information logger</param>
        /// <param name="autoHost">Hosts the services or not</param>
        /// <returns></returns>
        public List<ServiceHost> CreateServers(
            List<Type> serviceImplTypes,
            Func<Type, string> serviceNameResolver,
            Func<Type, Type> contractTypeResolver,
            string serviceBaseNS,
            int port,
            Action<object, Exception> errorHandler,
            Action<string> errorLogger = null,
            Action<string> warningLogger = null,
            Action<string> infoLogger = null,
            bool autoHost = true)
        {
            var hosts = new List<ServiceHost>();

            hosts.AddRange(CreateServersCore(
                serviceImplTypes,
                serviceNameResolver,
                contractTypeResolver,
                serviceBaseNS,
                port,
                errorHandler,
                errorLogger,
                warningLogger,
                infoLogger,
                autoHost));

            return hosts;
        }

        // Creates the services
        protected virtual IEnumerable<ServiceHost> CreateServersCore(
            List<Type> serviceImplTypes,
            Func<Type, string> serviceNameResolver,
            Func<Type, Type> contractTypeResolver,
            string serviceBaseNS,
            int port,
            Action<object, Exception> errorHandler,
            Action<string> errorLogger = null,
            Action<string> warningLogger = null,
            Action<string> infoLogger = null,
            bool autoHost = true)
        {
            var eh = new ServiceErrorHandler(errorHandler);

            foreach (var serviceImplType in serviceImplTypes)
            {
                var contractType = contractTypeResolver(serviceImplType);
                var serviceName = serviceNameResolver(serviceImplType);

                var host = HostService(
                    serviceImplType,
                    serviceName,
                    contractType,
                    serviceBaseNS,
                    port,
                    eh,
                    errorHandler,
                    errorLogger,
                    warningLogger,
                    infoLogger,
                    autoHost);

                yield return host;
            }
        }

        // Hosts service
        protected virtual ServiceHost HostService(
            Type serviceImplType,
            string serviceName,
            Type contractType,
            string serviceBaseNS,
            int port,
            ServiceErrorHandler eh,
            Action<object, Exception> errorHandler,
            Action<string> errorLogger = null,
            Action<string> warningLogger = null,
            Action<string> infoLogger = null,
            bool autoHost = true)
        {
            var uri = CreateEndpointUri(serviceName, serviceBaseNS, Environment.MachineName, port);

            var host = CreateHost(serviceImplType, uri, eh);

            var binding = CreateBinding();

            host.AddServiceEndpoint(contractType, binding, uri);


            if (autoHost)
            {
                try
                {
                    host.Open();

                    host.Faulted += (sender, e) =>
                    {
                        if (errorHandler != null) errorHandler(host, null);
                    };
                }
                catch (AddressAccessDeniedException aade)
                {
                    var msgText = string.Format("{1}\r\n{0} ",
                        aade.Message,
                        "The process doesn't have privilege to register WCF service namespaces.");

                    errorLogger(msgText);
                    Console.WriteLine(msgText);
                    throw;
                }
                catch (Exception ex)
                {
                    var msgText = "Failed to host service " + ": " + host.Description.Name;
                    msgText += Environment.NewLine + ex.Message;

                    errorLogger(msgText);
                    Console.WriteLine(msgText);
                    throw;
                }
            }

            return host;
        }

        // Creates the host
        protected virtual ServiceHost CreateHost(
            Type serviceImplType,
            Uri uri,
            ServiceErrorHandler eh)
        {
            return new ServiceHost(serviceImplType, uri);
        }

        #endregion

        #region Client Proxy generation methods

        /// <summary>
        ///     Generates a service client contract (a proxy object) that can be used to invoke service methods on remote machine
        /// </summary>
        /// <typeparam name="TServiceContract">The contract interface</typeparam>
        /// <param name="machineName">The service name where the service is located</param>
        /// <param name="port">The port number</param>
        /// <param name="serviceNameResolver">The service name resolver delegate</param>
        /// <param name="serviceBaseNS">The service namespace</param>
        /// <param name="upnName">UPN - optional</param>
        /// <param name="password">Password</param>
        /// <returns>A client proxy instance</returns>
        public ClientProxy<TServiceContract> CreateChannel<TServiceContract>(
            string machineName,
            int port,
            Func<Type, string> serviceNameResolver,
            string serviceBaseNS,
            string upnName = null,
            string password = null) where TServiceContract : class
        {
            var binding = CreateBinding();

            var serviceUrl = CreateEndpointUri(serviceNameResolver(typeof(TServiceContract)), serviceBaseNS, machineName, port);

            var endpointAddress = default(EndpointAddress);

            endpointAddress = new EndpointAddress(serviceUrl);

            var channelFactory = default(ChannelFactory<TServiceContract>);

            channelFactory =
                    new ChannelFactory<TServiceContract>(binding, endpointAddress);

            // Extract Domain & UserName part form the upnName.
            if (!String.IsNullOrEmpty(upnName) && !String.IsNullOrEmpty(password))
            {
                var userName = upnName;
                var domain = string.Empty;
                var index = userName.IndexOf('@');
                if (index != -1)
                {
                    domain = userName.Substring(index + 1, userName.Length - index - 1);
                    userName = userName.Substring(0, index);
                }
                channelFactory.Credentials.Windows.ClientCredential.UserName = userName;
                channelFactory.Credentials.Windows.ClientCredential.Domain = domain;
                channelFactory.Credentials.Windows.ClientCredential.Password = password;
            }

            var channel = channelFactory.CreateChannel(endpointAddress);
            return new ClientProxy<TServiceContract>(channel);
        }


        #endregion

        #region Common Service methods

        /// <summary>
        /// Creates a binding object
        /// </summary>
        /// <returns>Binding object</returns>
        protected abstract Binding CreateBinding();

        /// <summary>
        ///     Creates an endpoint URI
        /// </summary>
        /// <param name="serviceName">The name of the service</param>
        /// <param name="serviceBaseNS">The namespace where the service belongs</param>
        /// <param name="serverName">The server name where the service is located</param>
        /// <param name="port">The port number</param>
        /// <returns>The URI of the service</returns>
        protected virtual Uri CreateEndpointUri(
            string serviceName, string serviceBaseNS, string serverName, int port)
        {
            var url = string.Format("{0}://{1}:{2}/{3}/{4}",
                GetProtocolString(), serverName, port, serviceBaseNS, serviceName);
            return new Uri(url);
        }

        /// <summary>
        ///     Gets the protocol string
        /// </summary>
        /// <returns>Protocol string</returns>
        protected abstract string GetProtocolString();

        #endregion

        #region Error handler class

        // Service error handler
        public sealed class ServiceErrorHandler : IErrorHandler
        {
            private Action<object, Exception> _errorHandler;


            public ServiceErrorHandler()
                : this(null)
            {
            }


            public ServiceErrorHandler(Action<object, Exception> errorHandler)
            {
                this._errorHandler = errorHandler;
            }

            bool IErrorHandler.HandleError(Exception error)
            {
                Debug.WriteLine("ERROR: " + error);
                if (_errorHandler != null)
                {
                    _errorHandler(this, error);
                }
                return false;
            }

            void IErrorHandler.ProvideFault(
                Exception error,
                MessageVersion version,
                ref Message fault)
            {
                // Do not promote exceptions:
                // The exception the client gets will be determined by the fault 
                // contract (if any) and the exception type being thrown.
            }
        }

        #endregion

        #region Client Proxy classes

        /// <summary>
        /// Encapsulates a client channel and provides the disposing ability for the client channel
        /// </summary>
        public class ClientProxy : IDisposable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ClientProxy"/> class.
            /// </summary>
            /// <param name="serviceClient">The service client to proxy.</param>
            public ClientProxy(object serviceClient)
            {
                if (serviceClient == null)
                {
                    throw new ArgumentNullException("serviceClient");
                }
                Client = serviceClient;
                OCScope = new OperationContextScope((IContextChannel)serviceClient);
            }

            /// <summary>
            /// Gets the service client.
            /// </summary>
            public object Client { get; private set; }

            private OperationContextScope OCScope = null;

            public void SetHeader(string name, string value)
            {
                // ToDo Expand this implementation later
            }

            #region IDisposable Members

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                var proxy = Client as ICommunicationObject;
                if (proxy == null)
                {
                    return;
                }
                try
                {
                    if (OCScope != null)
                    {
                        OCScope.Dispose();
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    OCScope = null;
                }

                try
                {
                    proxy.Close();
                }
                catch (CommunicationException)
                {
                }
                catch (TimeoutException)
                {
                }
                finally
                {
                    proxy.Abort();
                    Client = null;
                }
            }

            #endregion
        }

        /// <summary>
        /// Encapsulates a client channel (Typed) and provides the disposing ability for the client channel
        /// </summary>
        public class ClientProxy<TServiceContract> : ClientProxy where TServiceContract : class
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ClientProxy{TServiceContract}"/> class.
            /// </summary>
            /// <param name="serviceClient">The service client to wrap.</param>
            public ClientProxy(TServiceContract serviceClient)
                : base(serviceClient)
            {
            }

            /// <summary>
            /// Gets the service client.
            /// </summary>
            public new TServiceContract Client
            {
                get { return (TServiceContract)base.Client; }
            }
        }

        #endregion

        #region Factory classes

        /// <summary>
        ///     Service Factory that provides services with netTcpBinding
        /// </summary>
        public sealed class TcpServiceFactory : WcfService
        {
            /// <summary>
            /// Creates a NET TCP binding object
            /// </summary>
            /// <returns>The NET TCP binding</returns>
            protected override Binding CreateBinding()
            {
                var netTcpBinding = new NetTcpBinding();
                netTcpBinding.ReceiveTimeout = TimeSpan.MaxValue;
                netTcpBinding.SendTimeout = TimeSpan.MaxValue;
                netTcpBinding.MaxReceivedMessageSize = Int32.MaxValue;
                netTcpBinding.MaxBufferSize = Int32.MaxValue;
                netTcpBinding.MaxBufferPoolSize = Int32.MaxValue;
                netTcpBinding.MaxConnections = 100;

                netTcpBinding.ReaderQuotas.MaxDepth = 64;
                netTcpBinding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
                netTcpBinding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
                netTcpBinding.ReaderQuotas.MaxBytesPerRead = 4096;
                netTcpBinding.ReaderQuotas.MaxNameTableCharCount = 16384;
                netTcpBinding.Security.Mode = SecurityMode.None;

                return netTcpBinding;
            }

            /// <summary>
            /// Gets the protocol string for TCP
            /// </summary>
            /// <returns>The protocol string</returns>
            protected override string GetProtocolString()
            {
                return "net.tcp";
            }
        }


        /// <summary>
        ///     Service Factory that provides services with wsHTTPBinding
        /// </summary>
        public sealed class HttpServiceFactory : WcfService
        {
            /// <summary>
            ///     Creates the HOST object
            /// </summary>
            /// <param name="serviceImplType">The service type that has to be hosted</param>
            /// <param name="uri">The URI of the service</param>
            /// <param name="eh">The error handler object</param>
            /// <returns>The service host instance</returns>
            protected override ServiceHost CreateHost(Type serviceImplType, Uri uri, WcfService.ServiceErrorHandler eh)
            {
                return new HttpServiceHost(serviceImplType, uri);
            }

            /// <summary>
            /// Creates the HTTP binding
            /// </summary>
            /// <returns>The binding object</returns>
            protected override Binding CreateBinding()
            {
                var hasCallback = false;
                var disableSecurity = true;

                if (hasCallback)
                {
                    // Expand this implementation later
                    var httpBinding = new WSDualHttpBinding();
                    // http://consultingblogs.emc.com/pauloreichert/archive/2007/02/22/WCF-Reliable-Sessions-Puzzle.aspx
                    httpBinding.ReceiveTimeout = TimeSpan.MaxValue;
                    httpBinding.MaxReceivedMessageSize = Int32.MaxValue;
                    httpBinding.MaxBufferPoolSize = Int32.MaxValue;

                    httpBinding.ReaderQuotas.MaxDepth = 64;
                    httpBinding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
                    httpBinding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
                    httpBinding.ReaderQuotas.MaxBytesPerRead = 4096;
                    httpBinding.ReaderQuotas.MaxNameTableCharCount = 16384;
                    // http://consultingblogs.emc.com/pauloreichert/archive/2007/02/22/WCF-Reliable-Sessions-Puzzle.aspx
                    httpBinding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
                    httpBinding.SendTimeout = TimeSpan.FromMinutes(20);
                    httpBinding.Security.Mode = WSDualHttpSecurityMode.None;

                    return httpBinding;
                }
                else
                {
                    var httpBinding = new WSHttpBinding();

                    httpBinding.ReceiveTimeout = TimeSpan.MaxValue;
                    httpBinding.SendTimeout = TimeSpan.MaxValue;
                    httpBinding.MaxReceivedMessageSize = Int32.MaxValue;
                    httpBinding.MaxBufferPoolSize = Int32.MaxValue;

                    httpBinding.ReaderQuotas.MaxDepth = 64;
                    httpBinding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
                    httpBinding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
                    httpBinding.ReaderQuotas.MaxBytesPerRead = 4096;
                    httpBinding.ReaderQuotas.MaxNameTableCharCount = 16384;


                    if (disableSecurity)
                    {
                        httpBinding.Security.Mode = SecurityMode.None;
                    }

                    return httpBinding;
                }
            }

            /// <summary>
            /// Gets the HTTP protocol string. (think about SSL later here)
            /// </summary>
            /// <returns>Protocol string</returns>
            protected override string GetProtocolString()
            {
                return "http";
            }



            /// <summary>
            /// HTTP service Host that adds more error handling capabilities to the default ServiceHost of WCF
            /// </summary>
            public class HttpServiceHost : ServiceHost
            {
                public HttpServiceHost(Type serviceType)
                    : base(serviceType)
                {
                }

                public HttpServiceHost(Type serviceType, params string[] baseAddresses)
                    : base(serviceType, baseAddresses.Select(address => new Uri(address)).ToArray())
                {
                }

                public HttpServiceHost(Type serviceType, params Uri[] baseAddresses)
                    : base(serviceType, baseAddresses)
                {
                }

                public HttpServiceHost(object singletonInstance, params string[] baseAddresses)
                    : base(singletonInstance, baseAddresses.Select(address => new Uri(address)).ToArray())
                {
                }

                public HttpServiceHost(object singletonInstance)
                    : base(singletonInstance)
                {
                }

                public HttpServiceHost(object singletonInstance, params Uri[] baseAddresses)
                    : base(singletonInstance, baseAddresses)
                {
                }

                #region Private Classes

                private class ErrorHandlerBehavior : IServiceBehavior, IErrorHandler
                {
                    private IErrorHandler _errorHandler;

                    public ErrorHandlerBehavior(IErrorHandler errorHandler)
                    {
                        _errorHandler = errorHandler;
                    }

                    void IServiceBehavior.Validate(
                        ServiceDescription description,
                        ServiceHostBase host)
                    {
                    }

                    void IServiceBehavior.AddBindingParameters(
                        ServiceDescription description,
                        ServiceHostBase host,
                        Collection<ServiceEndpoint> endpoints,
                        BindingParameterCollection parameters)
                    {
                    }

                    void IServiceBehavior.ApplyDispatchBehavior(
                        ServiceDescription description,
                        ServiceHostBase host)
                    {
                        foreach (ChannelDispatcher dispatcher in host.ChannelDispatchers)
                        {
                            dispatcher.ErrorHandlers.Add(this);
                        }
                    }

                    bool IErrorHandler.HandleError(Exception error)
                    {
                        return _errorHandler.HandleError(error);
                    }

                    void IErrorHandler.ProvideFault(
                        Exception error,
                        MessageVersion version,
                        ref Message fault)
                    {
                        _errorHandler.ProvideFault(error, version, ref fault);
                    }
                }

                #endregion

                readonly List<IServiceBehavior> _errorHandlers = new List<IServiceBehavior>();

                /// <summary>
                /// Can only call before openning the host.
                /// </summary>
                public void EnableMetadataExchange(bool enableHttpGet = true)
                {
                    if (State == CommunicationState.Opened)
                    {
                        throw new InvalidOperationException("Host is already opened");
                    }

                    var metadataBehavior = Description.Behaviors.Find<ServiceMetadataBehavior>();
                    if (metadataBehavior == null)
                    {
                        metadataBehavior = new ServiceMetadataBehavior();
                        Description.Behaviors.Add(metadataBehavior);
                    }

                    metadataBehavior.HttpGetEnabled = enableHttpGet;
                }

                /// <summary>
                /// Configures service host to perform custom error processing.
                /// </summary>
                /// <remarks>
                /// Can be called only before openning the host.
                /// </remarks>
                public void AddErrorHandler(IErrorHandler errorHandler)
                {
                    if (errorHandler == null)
                    {
                        throw new ArgumentNullException("errorHandler");
                    }
                    if (State == CommunicationState.Opened)
                    {
                        throw new InvalidOperationException("Host is already opened");
                    }
                    var errorHandlerBehavior = new ErrorHandlerBehavior(errorHandler);
                    _errorHandlers.Add(errorHandlerBehavior);
                }

                protected override void OnOpening()
                {
#if !NET35
                    ResolverInstaller.AddSharedTypeResolver(this);
#endif

                    foreach (IServiceBehavior behavior in _errorHandlers)
                    {
                        Description.Behaviors.Add(behavior);
                    }
                    base.OnOpening();
                }
            }


#if !NET35


            /// <summary>
            /// Provides utility methods to install the DataContract type resolver on the endpoint.        
            /// </summary>
            public static class ResolverInstaller
            {
                [MethodImpl(MethodImplOptions.NoInlining)]
                public static void AddSharedTypeResolver(ServiceHost host)
                {
                    Debug.Assert(host.State != CommunicationState.Opened);

                    foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
                    {
                        AddSharedTypeResolver(endpoint);
                    }
                }

                [MethodImpl(MethodImplOptions.NoInlining)]
                public static void AddSharedTypeResolver<T>(ClientBase<T> proxy) where T : class
                {
                    Debug.Assert(proxy.State != CommunicationState.Opened);
                    AddSharedTypeResolver(proxy.Endpoint);
                }

                [MethodImpl(MethodImplOptions.NoInlining)]
                public static void AddSharedTypeResolver<T>(ChannelFactory<T> factory) where T : class
                {
                    Debug.Assert(factory.State != CommunicationState.Opened);
                    AddSharedTypeResolver(factory.Endpoint);
                }

                [MethodImpl(MethodImplOptions.NoInlining)]
                public static void AddSharedTypeResolver(ChannelFactory factory)
                {
                    Debug.Assert(factory.State != CommunicationState.Opened);
                    AddSharedTypeResolver(factory.Endpoint);
                }

                private static void AddSharedTypeResolver(ServiceEndpoint endpoint)
                {
                    foreach (var operation in endpoint.Contract.Operations)
                    {
                        var behavior = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                        behavior.DataContractResolver = new SharedTypeResolver();
                    }
                }
            }


            /// <summary>
            /// Provides a mechanism for dynamically mapping types to and from 
            /// type representations during serialization and deserialization of data contract instances.        
            /// </summary>
            public sealed class SharedTypeResolver : DataContractResolver
            {
                public override bool TryResolveType(
                    Type dataContractType,
                    Type declaredType,
                    DataContractResolver knownTypeResolver,
                    out XmlDictionaryString typeName,
                    out XmlDictionaryString typeNamespace)
                {
                    if (!knownTypeResolver.TryResolveType(dataContractType, declaredType, null, out typeName, out typeNamespace))
                    {
                        var dictionary = new XmlDictionary();
                        typeName = dictionary.Add(dataContractType.FullName);
                        typeNamespace = dictionary.Add(dataContractType.Assembly.FullName);
                    }
                    return true;
                }

                public override Type ResolveName(
                    string typeName,
                    string typeNamespace,
                    Type declaredType,
                    DataContractResolver knownTypeResolver)
                {
                    return knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null) ??
                           Type.GetType(typeName + ", " + typeNamespace);
                }
            }
#endif
        }

        #endregion
    }
}
