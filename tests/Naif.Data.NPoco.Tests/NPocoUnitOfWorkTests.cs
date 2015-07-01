//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using Naif.TestUtilities;
using NPoco;
using NUnit.Framework;

namespace Naif.Data.NPoco.Tests
{
    [TestFixture]
    public class NPocoUnitOfWorkTests
    {
        private const string ConnectionStringName = "NPoco";

        [Test]
        public void NPocoUnitOfWork_Constructor_Throws_On_Null_ConnectionString()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentException>(() => new NPocoUnitOfWork(null));
        }

        [Test]
        public void NPocoUnitOfWork_Constructor_Throws_On_Empty_ConnectionString()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentException>(() => new NPocoUnitOfWork(String.Empty));
        }

        [Test]
        public void NPocoUnitOfWork_Constructor_Initialises_Database_Field()
        {
            //Arrange

            //Act
            var context = new NPocoUnitOfWork(ConnectionStringName);

            //Assert
            Assert.IsInstanceOf<Database>(context.Database);
        }

        [Test]
        public void NPocoUnitOfWork_SupportsLinq_Property_Returns_False()
        {
            //Arrange
            var context = new NPocoUnitOfWork(ConnectionStringName);

            //ssert
            Assert.IsFalse(context.SupportsLinq);
        }
    }
}
