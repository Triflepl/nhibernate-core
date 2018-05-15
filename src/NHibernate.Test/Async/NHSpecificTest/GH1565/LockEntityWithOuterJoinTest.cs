﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1565
{
	using System.Threading.Tasks;
	[TestFixture]
	public class LockEntityWithOuterJoinTestAsync : BugTestCase
	{
		[Test]
		public async Task LockWithOuterJoin_ShouldBePossibleAsync()
		{
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					var entity = await (session.GetAsync<MainEntity>(id, LockMode.Upgrade));
					Assert.That(entity.Id, Is.EqualTo(id));
					await (transaction.CommitAsync());
				}
			}
		}

		private int id;
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.FlushMode = FlushMode.Auto;
					var entity = new MainEntity();
					session.Save(entity);
					transaction.Commit();
					id = entity.Id;
				}
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (var session = OpenSession())
			{
				session.CreateSQLQuery("delete from MainEntity").ExecuteUpdate();
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return TestDialect.SupportsEmptyInsertsOrHasNonIdentityNativeGenerator;
		}
	}
}
