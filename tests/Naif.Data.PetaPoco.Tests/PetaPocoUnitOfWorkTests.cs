//******************************************
//  Copyright (C) 2012-2013 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included License.txt file)        *
//                                         *
// *****************************************

using System;
using NUnit.Framework;
using PetaPoco;

namespace Naif.Data.PetaPoco.Tests
{
    [TestFixture]
    public class PetaPocoUnitOfWorkTests
    {
        private const string ConnectionStringName = "PetaPoco";

        [Test]
        public void PetaPocoUnitOfWork_Constructor_Throws_On_Null_ConnectionString()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentException>(() => new PetaPocoUnitOfWork(null));
        }

        [Test]
        public void PetaPocoUnitOfWork_Constructor_Throws_On_Empty_ConnectionString()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentException>(() => new PetaPocoUnitOfWork(String.Empty));
        }

        [Test]
        public void PetaPocoUnitOfWork_Constructor_Initialises_Database_Field()
        {
            //Arrange, Act
            var context = new PetaPocoUnitOfWork(ConnectionStringName);

            //Assert
            Assert.IsInstanceOf<Database>(context.Database);
        }

        [Test]
        public void PetaPocoUnitOfWork_Constructor_Initialises_Mapper_Field()
        {
            //Arrange, Act
            var context = new PetaPocoUnitOfWork(ConnectionStringName);

            //Assert
            Assert.IsInstanceOf<IMapper>(context.Mapper);
        }

        [Test]
        public void PetaPocoUnitOfWork_SupportsLinq_Property_Returns_False()
        {
            //Arrange, Act
            var context = new PetaPocoUnitOfWork(ConnectionStringName);

            //Assert
            Assert.IsFalse(context.SupportsLinq);
        }
    }
}
