﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class OneToOneType : EntityType, IAssociationType
	{

		public override Task NullSafeSetAsync(DbCommand st, object value, int index, bool[] settable, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				NullSafeSet(st, value, index, settable, session);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override Task NullSafeSetAsync(DbCommand cmd, object value, int index, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				NullSafeSet(cmd, value, index, session);
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override Task<bool> IsDirtyAsync(object old, object current, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<bool>(cancellationToken);
			}
			try
			{
				return Task.FromResult<bool>(IsDirty(old, current, session));
			}
			catch (Exception ex)
			{
				return Task.FromException<bool>(ex);
			}
		}

		public override Task<bool> IsDirtyAsync(object old, object current, bool[] checkable, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<bool>(cancellationToken);
			}
			try
			{
				return Task.FromResult<bool>(IsDirty(old, current, checkable, session));
			}
			catch (Exception ex)
			{
				return Task.FromException<bool>(ex);
			}
		}

		public override Task<bool> IsModifiedAsync(object old, object current, bool[] checkable, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<bool>(cancellationToken);
			}
			try
			{
				return Task.FromResult<bool>(IsModified(old, current, checkable, session));
			}
			catch (Exception ex)
			{
				return Task.FromException<bool>(ex);
			}
		}

		public override async Task<object> HydrateAsync(DbDataReader rs, string[] names, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			IType type = GetIdentifierOrUniqueKeyType(session.Factory);
			object identifier = session.GetContextEntityIdentifier(owner);

			//This ugly mess is only used when mapping one-to-one entities with component ID types
			EmbeddedComponentType componentType = type as EmbeddedComponentType;
			if (componentType != null)
			{
				EmbeddedComponentType ownerIdType = session.GetEntityPersister(null, owner).IdentifierType as EmbeddedComponentType;
				if (ownerIdType != null)
				{
					object[] values = await (ownerIdType.GetPropertyValuesAsync(identifier, session, cancellationToken)).ConfigureAwait(false);
					object id = await (componentType.ResolveIdentifierAsync(values, session, null, cancellationToken)).ConfigureAwait(false);
					IEntityPersister persister = session.Factory.GetEntityPersister(type.ReturnedClass.FullName);
					var key = session.GenerateEntityKey(id, persister);
					return session.PersistenceContext.GetEntity(key);
				}
			}
			return identifier;
		}

		public override Task<object> DisassembleAsync(object value, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				return Task.FromResult<object>(Disassemble(value, session, owner));
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override Task<object> AssembleAsync(object cached, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				//this should be a call to resolve(), not resolveIdentifier(), 
				//'cos it might be a property-ref, and we did not cache the
				//referenced value
				return ResolveIdentifierAsync(session.GetContextEntityIdentifier(owner), session, owner, cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}
	}
}
