using System.Linq;
using Gravity.Manager.Domain;
using Gravity.Manager.Domain.Audits;
using Gravity.Manager.Domain.Aws;
using Gravity.Runtime.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;

namespace Gravity.Manager.Data.EF.Tests
{
    public class GravityManagerDbContextTests
    {
        [Test]
        public void DbContext_Insert_CreatesAudit()
        {
            var (audits, dm) = GetUnitsOfWork();

            var acc = new AwsAccount {Name = "foo"};

            dm.AwsAccounts.Insert(acc, true);

            var audit = audits.Audit.GetAll().Last();
            
            Assert.AreEqual(FixedDateTimeProvider.DateTime, audit.Date);
            Assert.AreEqual(DefaultUserProvider.UserId, audit.UserId);
            Assert.AreEqual(typeof(AwsAccount).Name, audit.EntityName);
            Assert.IsNull(audit.OldValue);
            Assert.AreEqual($"\"Id\":{acc.Id},\"Name\":\"foo\"", audit.NewValue);
        }
        
        [Test]
        public void DbContext_Update_CreatesAudit()
        {
            var (audits, dm) = GetUnitsOfWork();

            var ses = new DiscoverySession
            {
                AwsAccount = new AwsAccount {Name = "foo"},
                RunDate = FixedDateTimeProvider.DateTime
            };
            dm.DiscoverySessions.Insert(ses, true);
            
            ses.RunDate = ses.RunDate.AddDays(1);
            dm.DiscoverySessions.Update(ses, true);

            var audit = audits.Audit.GetAll().Last();
            
            Assert.AreEqual(FixedDateTimeProvider.DateTime, audit.Date);
            Assert.AreEqual(DefaultUserProvider.UserId, audit.UserId);
            Assert.AreEqual(typeof(DiscoverySession).Name, audit.EntityName);
            Assert.AreEqual($"\"Id\":{ses.Id},\"AwsAccountId\":{ses.AwsAccountId},\"RunDate\":\"2001-01-01T00:00:00\"", audit.OldValue);
            Assert.AreEqual($"\"Id\":{ses.Id},\"AwsAccountId\":{ses.AwsAccountId},\"RunDate\":\"2001-01-02T00:00:00\"", audit.NewValue);
        }
        
        [Test]
        public void DbContext_Delete_CreatesAudit()
        {
            var (audits, dm) = GetUnitsOfWork();

            var acc = new AwsAccount {Name = "foo"};
            dm.AwsAccounts.Insert(acc, true);
            dm.AwsAccounts.Delete(acc, true);

            var audit = audits.Audit.GetAll().Last();
            
            Assert.AreEqual(FixedDateTimeProvider.DateTime, audit.Date);
            Assert.AreEqual(DefaultUserProvider.UserId, audit.UserId);
            Assert.AreEqual(typeof(AwsAccount).Name, audit.EntityName);
            Assert.IsNull(audit.NewValue);
            Assert.AreEqual($"\"Id\":{acc.Id},\"Name\":\"foo\"", audit.OldValue);
        }

        [Test]
        public void AuditRepository_GetPage_PaginatesData()
        {
            const int count = 10;  
            // count of audits, there are two by default for user and org.

            var (auditsUow, dmUow) = GetUnitsOfWork();
            
            var auditRepository     = auditsUow.Audit;
            var awsAccountRepository = dmUow.AwsAccounts;
            
            for (var i = 0; i < count - 2; i++)
            {
                awsAccountRepository.Insert(new AwsAccount {Name = i.ToString()}, true);
            }

            awsAccountRepository.Commit();

            var audits = auditRepository.GetAll();
            
            Assert.AreEqual(count, audits.Count());

            var page = auditRepository.GetPageAsync<object, object>(0, 3).Result;
            Assert.AreEqual(count, page.Count);
            Assert.AreEqual(3, page.PageSize);
            Assert.AreEqual(3, page.Data.Count);
            Assert.AreEqual(0, page.PageIndex);
            Assert.AreEqual(4, page.TotalPages);
            Assert.IsFalse(page.HasPrevious);
            Assert.IsTrue(page.HasNext);

            page = auditRepository.GetPageAsync<object, object>(3, 3).Result;
            Assert.AreEqual(count, page.Count);
            Assert.AreEqual(1, page.Data.Count);
            Assert.AreEqual(3, page.PageSize);
            Assert.AreEqual(3, page.PageIndex);
            Assert.AreEqual(4, page.TotalPages);
            Assert.IsTrue(page.HasPrevious);
            Assert.IsFalse(page.HasNext);
        }

        private static (IAuditUnitOfWork audit, IDiscoveryUnitOfWork dm) GetUnitsOfWork()
        {
            var ctx = GetTestDbContext();

            return (new AuditUnitOfWork(ctx), new DiscoveryUnitOfWork(ctx));
        }
        
        public static GravityManagerDbContext GetTestDbContext()
        {
            // Unique db name for each test to avoid side effects.
            var dbName = TestContext.CurrentContext.Test.FullName;

            var options = new DbContextOptionsBuilder<GravityManagerDbContext>()
                .UseInMemoryDatabase(dbName)
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var userProvider = new DefaultUserProvider();
            var dbContext = new GravityManagerDbContext(options, userProvider, new JsonNetSerializer(),
                new FixedDateTimeProvider());

            dbContext.Users.Add(userProvider.GetOperatingUser());
            
            dbContext.SaveChanges();

            return dbContext;
        }
    }
}