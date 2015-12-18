//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using Moq;
using Naif.Core.Caching;
using Naif.TestUtilities;
using Naif.TestUtilities.Models;
using NUnit.Framework;

namespace Naif.Data.EntityFramework.Tests
{
    [TestFixture]
    public class EFUnitOfWorkTests
    {
        private const string ConnectionStringName = "NaifDbContext";

        private Mock<ICacheProvider> _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = new Mock<ICacheProvider>();
        }

        [Test]
        public void EFUnitOfWork_Constructor_Throws_On_Null_Cache()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentNullException>(() => new EFUnitOfWork(ConnectionStringName, null, null));
        }


        [Test]
        public void EFUnitOfWork_Constructor_Throws_On_Null_ConnectionString()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentException>(() => new EFUnitOfWork(null, null, _cache.Object));
        }

        [Test]
        public void EFUnitOfWork_Constructor_Throws_On_Empty_ConnectionString()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentException>(() => new EFUnitOfWork(String.Empty, null, _cache.Object));
        }

        [Test]
        public void EFUnitOfWork_Constructor_Initialises_Database_Field()
        {
            //Arrange, Act
            var context = new EFUnitOfWork(ConnectionStringName, null, _cache.Object);

            //Assert
            Assert.IsInstanceOf<NaifDbContext>(Util.GetPrivateField<EFUnitOfWork, NaifDbContext>(context, "_dbContext"));
        }

        [Test]
        public void EFUnitOfWork_Constructor_Overload_Throws_On_Null_DbContext()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentNullException>(() => new EFUnitOfWork(null, _cache.Object));
        }

        [Test]
        public void EFUnitOfWork_GetRepository_Returns_Repository()
        {
            //Arrange, Act
            var context = new EFUnitOfWork(ConnectionStringName, null, _cache.Object);

            //Act
            var rep = context.GetRepository<Dog>();

            //Assert
            Assert.IsInstanceOf<IRepository<Dog>>(rep);
        }

        [Test]
        public void EFUnitOfWork_SupportsLinq_Property_Returns_True()
        {
            //Arrange
            var context = new EFUnitOfWork(ConnectionStringName, null, _cache.Object);

            //Assert
            Assert.IsTrue(context.SupportsLinq);
        }
    }
}
