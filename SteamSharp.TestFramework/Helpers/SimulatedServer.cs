﻿using System;
using System.Net;
using System.Threading;

namespace SteamSharp.TestFramework.Helpers {

	public class SimulatedServer : IDisposable {

		private readonly HttpListener _listener;
		private readonly Action<HttpListenerContext> _handler;
		private Thread _processor;

		public static SimulatedServer Create( string url, Action<HttpListenerContext> handler, AuthenticationSchemes authenticationSchemes = AuthenticationSchemes.Anonymous ) {

			var listener = new HttpListener {
				Prefixes = { url },
				AuthenticationSchemes = authenticationSchemes
			};

			var server = new SimulatedServer( listener, handler );
			server.Start();
			return server;

		}

		public SimulatedServer( HttpListener listener, Action<HttpListenerContext> handler ) {
			_listener = listener;
			_handler = handler;
		}

		public void Start() {

			if( !_listener.IsListening ) {

				_listener.Start();

				_processor = new Thread( () => {

					var context = _listener.GetContext();
					_handler( context );
					context.Response.Close();

				} ) { Name = "SimulatedWebServer" };

				_processor.Start();

			}

		}

		public void Dispose() {
			_processor.Abort();
			_listener.Stop();
			_listener.Close();
		}

	}

}
