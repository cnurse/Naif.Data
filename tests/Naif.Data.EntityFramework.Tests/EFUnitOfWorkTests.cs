//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using Naif.TestUtilities;
using NUnit.Framework;

namespace Naif.Data.EntityFramework.Tests
{
    [TestFixture]
    public class EFUnitOfWorkTests
    {
        private const string ConnectionStringName = "NaifDbContext";

        [Test]
        public void EFUnitOfWork_Constructor_Throws_On_Null_ConnectionString()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentException>(() => new EFUnitOfWork(null, null));
        }

        [Test]
        public void EFUnitOfWork_Constructor_Throws_On_Empty_ConnectionString()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentException>(() => new EFUnitOfWork(String.Empty, null));
        }

        [Test]
        public void EFUnitOfWork_Constructor_Initialises_Database_Field()
        {
            //Arrange, Act
            var context = new EFUnitOfWork(ConnectionStringName, null);

            //Assert
            Assert.IsInstanceOf<NaifDbContext>(Util.GetPrivateField<EFUnitOfWork, NaifDbContext>(context, "_dbContext"));
        }

        [Test]
        public void EFUnitOfWork_Constructor_Overload_Throws_On_Null_DbContext()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentNullException>(() => new EFUnitOfWork(null));
        }

        [Test]
        public void EFUnitOfWork_SupportsLinq_Property_Returns_True()
        {
            //Arrange
            var context = new EFUnitOfWork(ConnectionStringName, null);

            //Assert
            Assert.IsTrue(context.SupportsLinq);
        }
    }
}
