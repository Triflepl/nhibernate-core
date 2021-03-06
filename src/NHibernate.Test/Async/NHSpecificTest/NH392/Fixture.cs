﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH392
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH392"; }
		}

		[Test]
		public async Task UnsavedMinusOneNoNullReferenceExceptionAsync()
		{
			UnsavedValueMinusOne uvmo = new UnsavedValueMinusOne();
			uvmo.Name = "TEST";
			uvmo.UpdateTimestamp = DateTime.Now;

			Assert.AreEqual(-1, uvmo.Id);

			using (ISession s = OpenSession())
			{
				ITransaction tran = s.BeginTransaction();
				try
				{
					await (s.SaveOrUpdateAsync(uvmo));
					await (tran.CommitAsync());
				}
				catch
				{
					await (tran.RollbackAsync());
				}
			}
		}

		protected override void OnTearDown()
		{
			using (ISession s = Sfi.OpenSession())
			{
				// s.Delete("from UnsavedValueMinusOne") loads then delete entities one by one, checking the version.
				// This fails with ODBC & Sql Server 2008+, see NH-1756 test case for more details.
				// Use an in-db query instead.
				s.CreateQuery("delete from UnsavedValueMinusOne").ExecuteUpdate();
			}
		}
	}
}
