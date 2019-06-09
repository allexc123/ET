﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ETModel
{
	public abstract class NetworkComponent : Component
	{
		public AppType AppType;
		
		protected AService Service;

		private readonly Dictionary<long, Session> sessions = new Dictionary<long, Session>();

		public IMessagePacker MessagePacker { get; set; }

		public IMessageDispatcher MessageDispatcher { get; set; }

		public void Awake(NetworkProtocol protocol)
		{
			switch (protocol)
			{
				
				case NetworkProtocol.TCP:
					this.Service = new TService() { Parent = this };
					break;
				
			}
		}

		public void Awake(NetworkProtocol protocol, string address)
		{
			try
			{
				IPEndPoint ipEndPoint;
				switch (protocol)
				{
					
					case NetworkProtocol.TCP:
						ipEndPoint = NetworkHelper.ToIPEndPoint(address);
						this.Service = new TService(ipEndPoint, this.OnAccept) { Parent = this };
						break;
					
				}
			}
			catch (Exception e)
			{
				throw new Exception($"NetworkComponent Awake Error {address}", e);
			}
		}

		public int Count
		{
			get { return this.sessions.Count; }
		}

		public void OnAccept(AChannel channel)
		{
			Session session = ComponentFactory.CreateWithParent<Session, AChannel>(this, channel);
			this.sessions.Add(session.Id, session);
			session.Start();
		}

		public virtual void Remove(long id)
		{
			Session session;
			if (!this.sessions.TryGetValue(id, out session))
			{
				return;
			}
			this.sessions.Remove(id);
			session.Dispose();
		}

		public Session Get(long id)
		{
			Session session;
			this.sessions.TryGetValue(id, out session);
			return session;
		}

		/// <summary>
		/// 创建一个新Session
		/// </summary>
		public Session Create(IPEndPoint ipEndPoint)
		{
			AChannel channel = this.Service.ConnectChannel(ipEndPoint);
			Session session = ComponentFactory.CreateWithParent<Session, AChannel>(this, channel);
			this.sessions.Add(session.Id, session);
			session.Start();
			return session;
		}
		
		/// <summary>
		/// 创建一个新Session
		/// </summary>
		public Session Create(string address)
		{
			AChannel channel = this.Service.ConnectChannel(address);
			Session session = ComponentFactory.CreateWithParent<Session, AChannel>(this, channel);
			this.sessions.Add(session.Id, session);
			session.Start();
			return session;
		}

		public void Update()
		{
			if (this.Service == null)
			{
				return;
			}
			this.Service.Update();
		}

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();

			foreach (Session session in this.sessions.Values.ToArray())
			{
				session.Dispose();
			}

			this.Service.Dispose();
		}
	}
}