using System.Transactions;
using Raven.Database.Indexing;
using Xunit;
using System.Linq;

namespace Raven.Client.Tests.Bugs
{
	public class AsyncCommit : LocalClientTest
	{
		[Fact]
		public void DtcCommitWillGiveOldResult()
		{
			using(var documentStore = NewDocumentStore())
			{
				using(var s = documentStore.OpenSession())
				{
					s.Store(new AccurateCount.User{ Name = "Ayende"});	
					s.SaveChanges();
				}

				using (var s = documentStore.OpenSession())
				using (var scope = new TransactionScope())
				{
					var user = s.Load<AccurateCount.User>("users/1");
					user.Name = "Rahien";
					s.SaveChanges();
					scope.Complete();
				}


				using (var s = documentStore.OpenSession())
				{
					var user = s.Load<AccurateCount.User>("users/1");
					Assert.Equal("Ayende", user.Name);
				}
			}
		}

        [Fact]
        public void DtcCommitWillGiveOldResultWhenQuerying()
        {
            using (var documentStore = NewDocumentStore())
            {
                documentStore.DatabaseCommands.PutIndex("test",
                                                        new IndexDefinition
                                                        {
                                                            Map = "from doc in docs select new { doc.Name }"
                                                        });

                using (var s = documentStore.OpenSession())
                {
                    s.Store(new AccurateCount.User { Name = "Ayende" });
                    s.SaveChanges();

                    s.LuceneQuery<AccurateCount.User>("test")
                        .WaitForNonStaleResults()
                        .FirstOrDefault();
                }

                using (var s = documentStore.OpenSession())
                using (var scope = new TransactionScope())
                {
                    var user = s.Load<AccurateCount.User>("users/1");
                    user.Name = "Rahien";
                    s.SaveChanges();
                    scope.Complete();
                }


                using (var s = documentStore.OpenSession())
                {
                    var user = s.LuceneQuery<AccurateCount.User>("test")
                        .FirstOrDefault();
                    Assert.Equal("Ayende", user.Name);
                }
            }
        }

		[Fact]
		public void DtcCommitWillGiveNewResultIfNonAuthoritiveIsSetToFalse()
		{
			using (var documentStore = NewDocumentStore())
			{
				using (var s = documentStore.OpenSession())
				{
					s.Store(new AccurateCount.User { Name = "Ayende" });
					s.SaveChanges();
				}

				using (var s = documentStore.OpenSession())
				using (var scope = new TransactionScope())
				{
					var user = s.Load<AccurateCount.User>("users/1");
					user.Name = "Rahien";
					s.SaveChanges();
					scope.Complete();
				}

				using (var s = documentStore.OpenSession())
				{
					s.AllowNonAuthoritiveInformation = false;
					var user = s.Load<AccurateCount.User>("users/1");
					Assert.Equal("Rahien", user.Name);
				}
			}
		}

        [Fact]
        public void DtcCommitWillGiveNewResultIfNonAuthoritiveIsSetToFalseWhenQuerying()
        {
            using (var documentStore = NewDocumentStore())
            {
                documentStore.DatabaseCommands.PutIndex("test",
                                                        new IndexDefinition
                                                        {
                                                            Map = "from doc in docs select new { doc.Name }"
                                                        });

                using (var s = documentStore.OpenSession())
                {
                    s.Store(new AccurateCount.User { Name = "Ayende" });
                    s.SaveChanges();

                    s.LuceneQuery<AccurateCount.User>("test")
                        .WaitForNonStaleResults()
                        .FirstOrDefault();
                }

                using (var s = documentStore.OpenSession())
                using (var scope = new TransactionScope())
                {
                    var user = s.Load<AccurateCount.User>("users/1");
                    user.Name = "Rahien";
                    s.SaveChanges();
                    scope.Complete();
                }


                using (var s = documentStore.OpenSession())
                {
                    s.AllowNonAuthoritiveInformation = false;
                    var user = s.LuceneQuery<AccurateCount.User>("test")
                        .FirstOrDefault();
                    Assert.Equal("Rahien", user.Name);
                }
            }
        }
	}
}