using System;
using System.Collections.Generic;

namespace Message
{
    /// <summary>
    /// Base interface for Signals
    /// </summary>
    public interface ISignal
    {
        string Hash { get; }
    }

    /// <summary>
    /// Interface for SignalHub to enable dependency injection
    /// </summary>
    public interface ISignalHub
    {
        /// <summary>
/// Gets the signal instance for the requested type from the hub, creating and caching one if necessary.
/// </summary>
/// <typeparam name="SType">The concrete signal type to retrieve.</typeparam>
/// <returns>The signal instance of the requested type; newly created and stored if it did not already exist.</returns>
SType Get<SType>() where SType : ISignal, new();
        /// <summary>
/// Registers a parameterless listener for the signal identified by the given hash.
/// </summary>
/// <param name="signalHash">The signal's Hash value used to locate the signal.</param>
/// <param name="handler">The callback to invoke when the signal is dispatched.</param>
/// <remarks>If no signal with the specified hash exists or the located signal does not accept a parameterless listener, this method has no effect.</remarks>
void AddListenerToHash(string signalHash, Action handler);
        /// <summary>
/// Unregisters a zero-argument listener from the signal identified by the specified hash.
/// </summary>
/// <param name="signalHash">The hash identifier of the signal (commonly the signal type's full name).</param>
/// <param name="handler">The previously registered listener delegate to remove.</param>
void RemoveListenerFromHash(string signalHash, Action handler);
    }

    /// <summary>
    /// A hub for Signals you can implement in your classes
    /// </summary>
    public class SignalHub : ISignalHub
    {
        private Dictionary<Type, ISignal> signals = new Dictionary<Type, ISignal>();

        /// <summary>
        /// Getter for a signal of a given type
        /// </summary>
        /// <typeparam name="SType">Type of signal</typeparam>
        /// <returns>The proper signal binding</returns>
        public SType Get<SType>() where SType : ISignal, new()
        {
            Type signalType = typeof(SType);
            ISignal signal;

            if (signals.TryGetValue(signalType, out signal))
            {
                return (SType)signal;
            }

            return (SType)Bind(signalType);
        }

        /// <summary>
        /// Manually provide a SignalHash and bind it to a given listener
        /// (you most likely want to use an AddListener, unless you know exactly
        /// what you are doing)
        /// </summary>
        /// <param name="signalHash">Unique hash for signal</param>
        /// <param name="handler">Callback for signal listener</param>
        public void AddListenerToHash(string signalHash, Action handler)
        {
            ISignal signal = GetSignalByHash(signalHash);
            if (signal != null && signal is ASignal)
            {
                (signal as ASignal).AddListener(handler);
            }
        }

        /// <summary>
        /// Manually provide a SignalHash and unbind it from a given listener
        /// (you most likely want to use a RemoveListener, unless you know exactly
        /// what you are doing)
        /// </summary>
        /// <param name="signalHash">Unique hash for signal</param>
        /// <param name="handler">Callback for signal listener</param>
        public void RemoveListenerFromHash(string signalHash, Action handler)
        {
            ISignal signal = GetSignalByHash(signalHash);
            if (signal != null && signal is ASignal)
            {
                (signal as ASignal).RemoveListener(handler);
            }
        }

        private ISignal Bind(Type signalType)
        {
            ISignal signal;
            if (signals.TryGetValue(signalType, out signal))
            {
                UnityEngine.Debug.LogError(string.Format("Signal already registered for type {0}",
                    signalType.ToString()));
                return signal;
            }

            signal = (ISignal)Activator.CreateInstance(signalType);
            signals.Add(signalType, signal);
            return signal;
        }

        private ISignal Bind<T>() where T : ISignal, new()
        {
            return Bind(typeof(T));
        }

        private ISignal GetSignalByHash(string signalHash)
        {
            foreach (ISignal signal in signals.Values)
            {
                if (signal.Hash == signalHash)
                {
                    return signal;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Abstract class for Signals, provides hash by type functionality
    /// </summary>
    public abstract class ABaseSignal : ISignal
    {
        protected string _hash;

        /// <summary>
        /// Unique id for this signal
        /// </summary>
        public string Hash
        {
            get
            {
                if (string.IsNullOrEmpty(_hash))
                {
                    _hash = this.GetType().ToString();
                }

                return _hash;
            }
        }
    }

    /// <summary>
    /// Strongly typed messages with no parameters
    /// </summary>
    public abstract class ASignal : ABaseSignal
    {
        private Action callback;

        /// <summary>
        /// Adds a listener to this Signal
        /// </summary>
        /// <param name="handler">Method to be called when signal is fired</param>
        public void AddListener(Action handler)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(
                handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                    inherit: false).Length == 0,
                "Adding anonymous delegates as Signal callbacks is not supported (you wouldn't be able to unregister them later).");
#endif
            callback += handler;
        }

        /// <summary>
        /// Removes a listener from this Signal
        /// </summary>
        /// <param name="handler">Method to be unregistered from signal</param>
        public void RemoveListener(Action handler)
        {
            callback -= handler;
        }

        /// <summary>
        /// Dispatch this signal
        /// </summary>
        public void Dispatch()
        {
            if (callback != null)
            {
                callback();
            }
        }
    }

    /// <summary>
    /// Strongly typed messages with 1 parameter
    /// </summary>
    /// <typeparam name="T">Parameter type</typeparam>
    public abstract class ASignal<T> : ABaseSignal
    {
        private Action<T> callback;

        /// <summary>
        /// Adds a listener to this Signal
        /// </summary>
        /// <param name="handler">Method to be called when signal is fired</param>
        public void AddListener(Action<T> handler)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(
                handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                    inherit: false).Length == 0,
                "Adding anonymous delegates as Signal callbacks is not supported (you wouldn't be able to unregister them later).");
#endif
            callback += handler;
        }

        /// <summary>
        /// Removes a listener from this Signal
        /// </summary>
        /// <param name="handler">Method to be unregistered from signal</param>
        public void RemoveListener(Action<T> handler)
        {
            callback -= handler;
        }

        /// <summary>
        /// Dispatch this signal with 1 parameter
        /// </summary>
        public void Dispatch(T arg1)
        {
            if (callback != null)
            {
                callback(arg1);
            }
        }
    }

    /// <summary>
    /// Strongly typed messages with 2 parameters
    /// </summary>
    /// <typeparam name="T">First parameter type</typeparam>
    /// <typeparam name="U">Second parameter type</typeparam>
    public abstract class ASignal<T, U> : ABaseSignal
    {
        private Action<T, U> callback;

        /// <summary>
        /// Adds a listener to this Signal
        /// </summary>
        /// <param name="handler">Method to be called when signal is fired</param>
        public void AddListener(Action<T, U> handler)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(
                handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                    inherit: false).Length == 0,
                "Adding anonymous delegates as Signal callbacks is not supported (you wouldn't be able to unregister them later).");
#endif
            callback += handler;
        }

        /// <summary>
        /// Removes a listener from this Signal
        /// </summary>
        /// <param name="handler">Method to be unregistered from signal</param>
        public void RemoveListener(Action<T, U> handler)
        {
            callback -= handler;
        }

        /// <summary>
        /// Dispatch this signal
        /// </summary>
        public void Dispatch(T arg1, U arg2)
        {
            if (callback != null)
            {
                callback(arg1, arg2);
            }
        }
    }

    /// <summary>
    /// Strongly typed messages with 3 parameter
    /// </summary>
    /// <typeparam name="T">First parameter type</typeparam>
    /// <typeparam name="U">Second parameter type</typeparam>
    /// <typeparam name="V">Third parameter type</typeparam>
    public abstract class ASignal<T, U, V> : ABaseSignal
    {
        private Action<T, U, V> callback;

        /// <summary>
        /// Adds a listener to this Signal
        /// </summary>
        /// <param name="handler">Method to be called when signal is fired</param>
        public void AddListener(Action<T, U, V> handler)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(
                handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                    inherit: false).Length == 0,
                "Adding anonymous delegates as Signal callbacks is not supported (you wouldn't be able to unregister them later).");
#endif
            callback += handler;
        }

        /// <summary>
        /// Removes a listener from this Signal
        /// </summary>
        /// <param name="handler">Method to be unregistered from signal</param>
        public void RemoveListener(Action<T, U, V> handler)
        {
            callback -= handler;
        }

        /// <summary>
        /// Dispatch this signal
        /// </summary>
        public void Dispatch(T arg1, U arg2, V arg3)
        {
            if (callback != null)
            {
                callback(arg1, arg2, arg3);
            }
        }
    }

    /// <summary>
    /// Strongly typed messages with 4 parameter
    /// </summary>
    /// <typeparam name="T">First parameter type</typeparam>
    /// <typeparam name="U">Second parameter type</typeparam>
    /// <typeparam name="V">Third parameter type</typeparam>
    /// <typeparam name="W">Fourth parameter type</typeparam>
    public abstract class ASignal<T, U, V, W> : ABaseSignal
    {
        private Action<T, U, V, W> callback;

        /// <summary>
        /// Adds a listener to this Signal
        /// </summary>
        /// <param name="handler">Method to be called when signal is fired</param>
        public void AddListener(Action<T, U, V, W> handler)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(
                handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                    inherit: false).Length == 0,
                "Adding anonymous delegates as Signal callbacks is not supported (you wouldn't be able to unregister them later).");
#endif
            callback += handler;
        }

        /// <summary>
        /// Removes a listener from this Signal
        /// </summary>
        /// <param name="handler">Method to be unregistered from signal</param>
        public void RemoveListener(Action<T, U, V, W> handler)
        {
            callback -= handler;
        }

        /// <summary>
        /// Dispatch this signal
        /// </summary>
        public void Dispatch(T arg1, U arg2, V arg3, W arg4)
        {
            if (callback != null)
            {
                callback(arg1, arg2, arg3, arg4);
            }
        }
    }
}