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
using Naif.Data.NPoco;
using Naif.TestUtilities;
using Naif.TestUtilities.Models;
using NPoco;
using NUnit.Framework;

namespace Naif.Data.NPoco.Tests
{
    [TestFixture]
    public class NPocoDataContextTests
    {
        private const string ConnectionStringName = "PetaPoco";

        [Test]
        public void NPocoDataContext_Constructor_Throws_On_Null_ICacheProvider()
        {
            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => new NPocoDataContext(ConnectionStringName, null));
        }

        [Test]
        public void NPocoDataContext_Constructor_Throws_On_Null_ConnectionString()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();

            //Act, Assert
            Assert.Throws<ArgumentException>(() => new NPocoDataContext(null, mockCache.Object));
        }

        [Test]
        public void NPocoDataContext_Constructor_Throws_On_Empty_ConnectionString()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();

            //Act, Assert
            Assert.Throws<ArgumentException>(() => new NPocoDataContext(String.Empty, mockCache.Object));
        }

        [Test]
        public void NPocoDataContext_Constructor_Initialises_Database_Field()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();

            //Act
            var context = new NPocoDataContext(ConnectionStringName, mockCache.Object);

            //Assert
            Assert.IsInstanceOf<Database>(Util.GetPrivateMember<NPocoDataContext, Database>(context, "_database"));
        }

        [Test]
        public void NPocoDataContext_GetRepository_Returns_Repository()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>(); 
            var context = new NPocoDataContext(ConnectionStringName, mockCache.Object);

            //Act
            var repo = context.GetRepository<Dog>();

            //Assert
            Assert.IsInstanceOf<IRepository<Dog>>(repo);
        }
    }
}
