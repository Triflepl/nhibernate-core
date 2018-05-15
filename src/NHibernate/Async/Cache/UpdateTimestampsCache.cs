﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using NHibernate.Cfg;

namespace NHibernate.Cache
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class UpdateTimestampsCache
	{
		private readonly NHibernate.Util.AsyncLock _preInvalidate = new NHibernate.Util.AsyncLock();
		private readonly NHibernate.Util.AsyncLock _invalidate = new NHibernate.Util.AsyncLock();
		private readonly NHibernate.Util.AsyncLock _isUpToDate = new NHibernate.Util.AsyncLock();

		public virtual Task ClearAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return updateTimestamps.ClearAsync(cancellationToken);
		}

		//Since v5.1
		[Obsolete("Please use PreInvalidate(IReadOnlyCollection<string>) instead.")]
		public Task PreInvalidateAsync(object[] spaces, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				//Only for backwards compatibility.
				return PreInvalidateAsync(spaces.OfType<string>().ToList(), cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[MethodImpl()]
		public virtual async Task PreInvalidateAsync(IReadOnlyCollection<string> spaces, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			using (await _preInvalidate.LockAsync())
			{
				//TODO: to handle concurrent writes correctly, this should return a Lock to the client
				long ts = updateTimestamps.NextTimestamp() + updateTimestamps.Timeout;
				foreach (var space in spaces)
				{
					await (updateTimestamps.PutAsync(space, ts, cancellationToken)).ConfigureAwait(false);
				}

				//TODO: return new Lock(ts);
			}

			//TODO: return new Lock(ts);
		}

		//Since v5.1
		[Obsolete("Please use PreInvalidate(IReadOnlyCollection<string>) instead.")]
		public Task InvalidateAsync(object[] spaces, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				//Only for backwards compatibility.
				return InvalidateAsync(spaces.OfType<string>().ToList(), cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		[MethodImpl()]
		public virtual async Task InvalidateAsync(IReadOnlyCollection<string> spaces, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			using (await _invalidate.LockAsync())
			{
				//TODO: to handle concurrent writes correctly, the client should pass in a Lock
				long ts = updateTimestamps.NextTimestamp();
				//TODO: if lock.getTimestamp().equals(ts)
				foreach (var space in spaces)
				{
					log.Debug("Invalidating space [{0}]", space);
					await (updateTimestamps.PutAsync(space, ts, cancellationToken)).ConfigureAwait(false);
				}
			}
		}

		[MethodImpl()]
		public virtual async Task<bool> IsUpToDateAsync(ISet<string> spaces, long timestamp /* H2.1 has Long here */, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			using (await _isUpToDate.LockAsync())
			{
				foreach (string space in spaces)
				{
					object lastUpdate = await (updateTimestamps.GetAsync(space, cancellationToken)).ConfigureAwait(false);
					if (lastUpdate == null)
					{
						//the last update timestamp was lost from the cache
						//(or there were no updates since startup!)

						//NOTE: commented out, since users found the "safe" behavior
						//      counter-intuitive when testing, and we couldn't deal
						//      with all the forum posts :-(
						//updateTimestamps.put( space, new Long( updateTimestamps.nextTimestamp() ) );
						//result = false; // safer

						//OR: put a timestamp there, to avoid subsequent expensive
						//    lookups to a distributed cache - this is no good, since
						//    it is non-threadsafe (could hammer effect of an actual
						//    invalidation), and because this is not the way our
						//    preferred distributed caches work (they work by
						//    replication)
						//updateTimestamps.put( space, new Long(Long.MIN_VALUE) );
					}
					else
					{
						if ((long) lastUpdate >= timestamp)
						{
							return false;
						}
					}
				}
				return true;
			}
		}
	}
}
