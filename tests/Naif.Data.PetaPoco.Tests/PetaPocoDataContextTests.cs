﻿//******************************************
//  Copyright (C) 2012-2013 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included License.txt file)        *
//                                         *
// *****************************************

using System;
using Moq;
using NUnit.Framework;
using Naif.Core.Data;
using Naif.TestUtilities;
using Naif.TestUtilities.Models;
using PetaPoco;
using Naif.Core.Caching;

namespace Naif.Data.PetaPoco.Tests
{
    [TestFixture]
    public class PetaPocoDataContextTests
    {
        private const string ConnectionStringName = "PetaPoco";

        #region Constructor Tests

        [Test]
        public void PetaPocoDataContext_Constructor_Throws_On_Null_ICacheProvider()
        {
            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => new PetaPocoDataContext(ConnectionStringName, null));
        }

        [Test]
        public void PetaPocoDataContext_Constructor_Throws_On_Null_ConnectionString()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();

            //Act, Assert
            Assert.Throws<ArgumentException>(() => new PetaPocoDataContext(null, mockCache.Object));
        }

        [Test]
        public void PetaPocoDataContext_Constructor_Throws_On_Empty_ConnectionString()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();

            //Act, Assert
            Assert.Throws<ArgumentException>(() => new PetaPocoDataContext(String.Empty, mockCache.Object));
        }

        [Test]
        public void PetaPocoDataContext_Constructor_Initialises_Database_Field()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();

            //Act
            var context = new PetaPocoDataContext(ConnectionStringName, mockCache.Object);

            //Assert
            Assert.IsInstanceOf<Database>(Util.GetPrivateMember<PetaPocoDataContext, Database>(context, "_database"));
        }

        #endregion

        #region GetRepository Tests

        [Test]
        public void PetaPocoDataContext_GetRepository_Returns_Repository()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>(); 
            var context = new PetaPocoDataContext(ConnectionStringName, mockCache.Object);

            //Act
            var repo = context.GetRepository<Dog>();

            //Assert
            Assert.IsInstanceOf<IRepository<Dog>>(repo);
        }

        #endregion
    }
}
