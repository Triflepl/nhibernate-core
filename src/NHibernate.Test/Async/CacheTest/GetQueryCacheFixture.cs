﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using NHibernate.Cache;
using NHibernate.Cfg;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.CacheTest
{
	using System.Threading.Tasks;
	[TestFixture]
	public class GetQueryCacheFixtureAsync : TestCase
	{
		protected override IList Mappings => new[] { "Simple.hbm.xml" };

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.UseQueryCache, "true");
			configuration.SetProperty(Environment.CacheProvider, typeof(LockedCacheProvider).AssemblyQualifiedName);
		}

		[Test]
		public async Task RetrievedQueryCacheMatchesGloballyStoredOneAsync()
		{
			var region = "RetrievedQueryCacheMatchesGloballyStoredOne";
			LockedCache.Semaphore = new SemaphoreSlim(0);
			LockedCache.CreationCount = 0;
			try
			{
				var failures = new ConcurrentBag<Exception>();
				var thread1 = new Thread(
					() =>
					{
						try
						{
							Sfi.GetQueryCache(region);
						}
						catch (Exception e)
						{
							failures.Add(e);
						}
					});
				var thread2 = new Thread(
					() =>
					{
						try
						{
							Sfi.GetQueryCache(region);
						}
						catch (Exception e)
						{
							failures.Add(e);
						}
					});
				thread1.Start();
				thread2.Start();

				// Give some time to threads for reaching the wait, having all of them ready to do most of their job concurrently.
				await (Task.Delay(100));
				// Let only one finish its job, it should be the one being stored in query cache dictionary.
				LockedCache.Semaphore.Release(1);
				// Give some time to released thread to finish its job.
				await (Task.Delay(100));
				// Release other thread
				LockedCache.Semaphore.Release(10);

				thread1.Join();
				thread2.Join();
				Assert.That(failures, Is.Empty, $"{failures.Count} thread(s) failed.");
			}
			finally
			{
				LockedCache.Semaphore.Dispose();
				LockedCache.Semaphore = null;
			}

			var queryCache = Sfi.GetQueryCache(region).Cache;
			var globalCache = Sfi.GetSecondLevelCacheRegion(region);
			Assert.That(globalCache, Is.SameAs(queryCache));
			Assert.That(LockedCache.CreationCount, Is.EqualTo(1));
		}
	}
}
