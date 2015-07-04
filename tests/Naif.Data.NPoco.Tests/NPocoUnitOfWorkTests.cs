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
using NPoco;
using NUnit.Framework;

namespace Naif.Data.NPoco.Tests
{
    [TestFixture]
    public class NPocoUnitOfWorkTests
    {
        private const string ConnectionStringName = "NPoco";

        private Mock<ICacheProvider> _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = new Mock<ICacheProvider>();
        }

        [Test]
        public void NPocoUnitOfWork_Constructor_Throws_On_Null_Cache()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentNullException>(() => new NPocoUnitOfWork(ConnectionStringName, null));
        }

        [Test]
        public void NPocoUnitOfWork_Constructor_Throws_On_Null_ConnectionString()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentException>(() => new NPocoUnitOfWork(null, _cache.Object));
        }

        [Test]
        public void NPocoUnitOfWork_Constructor_Throws_On_Empty_ConnectionString()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentException>(() => new NPocoUnitOfWork(String.Empty, _cache.Object));
        }

        [Test]
        public void NPocoUnitOfWork_Constructor_Initialises_Database_Field()
        {
            //Arrange

            //Act
            var context = new NPocoUnitOfWork(ConnectionStringName, _cache.Object);

            //Assert
            Assert.IsInstanceOf<Database>(context.Database);
        }

        [Test]
        public void NPocoUnitOfWork_GetRepository_Returns_Repository()
        {
            //Arrange, Act
            var context = new NPocoUnitOfWork(ConnectionStringName, _cache.Object);

            //Act
            var rep = context.GetRepository<Dog>();

            //Assert
            Assert.IsInstanceOf<IRepository<Dog>>(rep);
        }
    }
}
