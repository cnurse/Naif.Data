//******************************************
//  Copyright (C) 2012-2013 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included License.txt file)        *
//                                         *
// *****************************************

using System;
using Moq;
using Naif.Core.Caching;
using Naif.TestUtilities.Models;
using NUnit.Framework;
using PetaPoco;

namespace Naif.Data.PetaPoco.Tests
{
    [TestFixture]
    public class PetaPocoUnitOfWorkTests
    {
        private const string ConnectionStringName = "PetaPoco";

        private Mock<ICacheProvider> _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = new Mock<ICacheProvider>();
        }

        [Test]
        public void PetaPocoUnitOfWork_Constructor_Throws_On_Null_Cache()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentNullException>(() => new PetaPocoUnitOfWork(ConnectionStringName, null));
        }

        [Test]
        public void PetaPocoUnitOfWork_Constructor_Throws_On_Null_ConnectionString()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentException>(() => new PetaPocoUnitOfWork(null, _cache.Object));
        }

        [Test]
        public void PetaPocoUnitOfWork_Constructor_Throws_On_Empty_ConnectionString()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentException>(() => new PetaPocoUnitOfWork(String.Empty, _cache.Object));
        }

        [Test]
        public void PetaPocoUnitOfWork_Constructor_Initialises_Database_Field()
        {
            //Arrange, Act
            var context = new PetaPocoUnitOfWork(ConnectionStringName, _cache.Object);

            //Assert
            Assert.IsInstanceOf<Database>(context.Database);
        }

        [Test]
        public void PetaPocoUnitOfWork_Constructor_Initialises_Mapper_Field()
        {
            //Arrange, Act
            var context = new PetaPocoUnitOfWork(ConnectionStringName, _cache.Object);

            //Assert
            Assert.IsInstanceOf<IMapper>(context.Mapper);
        }

        [Test]
        public void PetaPocoUnitOfWork_GetRepository_Returns_Repository()
        {
            //Arrange, Act
            var context = new PetaPocoUnitOfWork(ConnectionStringName, _cache.Object);

            //Act
            var rep = context.GetRepository<Dog>();

            //Assert
            Assert.IsInstanceOf<IRepository<Dog>>(rep);
        }

        [Test]
        public void PetaPocoUnitOfWork_GetLinqRepository_Throwsy()
        {
            //Arrange, Act
            var context = new PetaPocoUnitOfWork(ConnectionStringName, _cache.Object);

            //Act, Assert
            Assert.Throws<NotImplementedException>(() => context.GetLinqRepository<Dog>());
        }

        [Test]
        public void PetaPocoUnitOfWork_SupportsLinq_Property_Returns_False()
        {
            //Arrange, Act
            var context = new PetaPocoUnitOfWork(ConnectionStringName, _cache.Object);

            //Assert
            Assert.IsFalse(context.SupportsLinq);
        }
    }
}
