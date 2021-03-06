﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;

namespace Mohmd.AspNetCore.PortableResolver
{
    public class ResolverContext
    {
        #region Properties

        /// <summary>
        /// Gets the singleton app engine used to access services.
        /// </summary>
        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                {
                    Create();
                }

                return Singleton<IEngine>.Instance;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a static instance of the app engine.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Create()
        {
            //create AppEngine as engine
            return Singleton<IEngine>.Instance ?? (Singleton<IEngine>.Instance = new ResolverEngine());
        }

        public static IEngine CreateNew()
        {
            var engine = new ResolverEngine();

            var instance = Singleton<IEngine>.Instance;

            if (instance is ResolverEngine rootEngine)
            {
                var scope = rootEngine?.ServiceProvider?.CreateScope();

                if (scope != null)
                {
                    engine.Configure(scope);
                }
            }

            return engine;
        }

        /// <summary>
        /// Sets the static engine instance to the supplied engine. Use this method to supply your own engine implementation.
        /// </summary>
        /// <param name="engine">The engine to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }

        #endregion
    }
}
