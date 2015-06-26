//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.Data.Entity;
using Moq;
using Naif.Core.Caching;
using Naif.TestUtilities;
using Naif.TestUtilities.Models;
using NUnit.Framework;

namespace Naif.Data.EntityFramework.Tests
{
    [TestFixture]
    public class EFDataContextTests
    {
        private const string ConnectionStringName = "NaifDbContext";

        [Test]
        public void EFDataContext_Constructor_Throws_On_Null_ICacheProvider()
        {
            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => new EFDataContext(ConnectionStringName, null, null));
        }

        [Test]
        public void EFDataContext_Constructor_Throws_On_Null_ConnectionString()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();

            //Act, Assert
            Assert.Throws<ArgumentException>(() => new EFDataContext(null, mockCache.Object, null));
        }

        [Test]
        public void EFDataContext_Constructor_Throws_On_Empty_ConnectionString()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();

            //Act, Assert
            Assert.Throws<ArgumentException>(() => new EFDataContext(String.Empty, mockCache.Object, null));
        }

        [Test]
        public void EFDataContext_Constructor_Initialises_Database_Field()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();

            //Act
            var context = new EFDataContext(ConnectionStringName, mockCache.Object, null);

            //Assert
            Assert.IsInstanceOf<NaifDbContext>(Util.GetPrivateField<EFDataContext, NaifDbContext>(context, "_context"));
        }

        [Test]
        public void EFDataContex_GetRepository_Returns_Repository()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            var context = new EFDataContext(ConnectionStringName, mockCache.Object, null);

            //Act
            var repo = context.GetRepository<Dog>();

            //Assert
            Assert.IsInstanceOf<IRepository<Dog>>(repo);
        }

    }
}
