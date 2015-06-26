﻿//******************************************
//  Copyright (C) 2012-2013 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included License.txt file)        *
//                                         *
// *****************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Moq;
using Naif.Core.Caching;
using Naif.TestUtilities;
using Naif.TestUtilities.Models;
using NUnit.Framework;

namespace Naif.Data.EntityFramework.Tests
{
    [TestFixture]
    public class EFRepositoryTests
    {
        private const string ConnectionStringName = "EntityFramework";
        private readonly string[] _dogAges = TestConstants.EF_DogAges.Split(',');
        private readonly string[] _dogNames = TestConstants.EF_DogNames.Split(',');

        private NaifDbContext _dbContext;

        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
            DataUtil.DeleteDatabase(TestConstants.EF_DatabaseName);
        }

        [Test]
        public void EFRepository_Constructor_Throws_On_Null_ICacheProvider()
        {
            //Arrange
            var db = new NaifDbContext(ConnectionStringName, null);

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => new EFRepository<Dog>(db, null));
        }

        [Test]
        public void EFRepository_Constructor_Throws_On_Null_DbContext()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => new EFRepository<Dog>(null, mockCache.Object));
        }

        [Test]
        public void EFRepository_Add_Inserts_Item_Into_DataBase()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.EF_RecordCount);

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);
            var dog = new Dog
            {
                Age = TestConstants.EF_InsertDogAge,
                Name = TestConstants.EF_InsertDogName
            };

            //Act
            repository.Add(dog);

            //Assert
            int actualCount = DataUtil.GetRecordCount(TestConstants.EF_DatabaseName,
                TestConstants.EF_TableName);
            Assert.AreEqual(TestConstants.EF_RecordCount + 1, actualCount);
        }

        [Test]
        public void EFRepository_Add_Inserts_Item_Into_DataBase_With_Correct_ID()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.EF_RecordCount);

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);
            var dog = new Dog
            {
                Age = TestConstants.EF_InsertDogAge,
                Name = TestConstants.EF_InsertDogName
            };

            //Act
            repository.Add(dog);

            //Assert
            int newId = DataUtil.GetLastAddedRecordID(TestConstants.EF_DatabaseName,
                TestConstants.EF_TableName, "ID");
            Assert.AreEqual(TestConstants.EF_RecordCount + 1, newId);
        }

        [Test]
        public void EFRepository_Add_Inserts_Item_Into_DataBase_With_Correct_ColumnValues()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.EF_RecordCount);

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);
            var dog = new Dog
            {
                Age = TestConstants.EF_InsertDogAge,
                Name = TestConstants.EF_InsertDogName
            };

            //Act
            repository.Add(dog);

            //Assert
            DataTable table = DataUtil.GetTable(TestConstants.EF_DatabaseName, TestConstants.EF_TableName);
            DataRow row = table.Rows[table.Rows.Count - 1];

            Assert.AreEqual(TestConstants.EF_InsertDogAge, row["Age"]);
            Assert.AreEqual(TestConstants.EF_InsertDogName, row["Name"]);
        }

        [Test]
        public void EFRepository_Delete_Deletes_Item_From_DataBase()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.EF_RecordCount);

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);
            var dog = new Dog
            {
                ID = TestConstants.EF_DeleteDogId,
                Age = TestConstants.EF_DeleteDogAge,
                Name = TestConstants.EF_DeleteDogName
            };

            //Act
            repository.Delete(dog);

            //Assert
            int actualCount = DataUtil.GetRecordCount(TestConstants.EF_DatabaseName,
                TestConstants.EF_TableName);
            Assert.AreEqual(TestConstants.EF_RecordCount - 1, actualCount);
        }

        [Test]
        public void EFRepository_Delete_Deletes_Item_From_DataBase_With_Correct_ID()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.EF_RecordCount);

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);
            var dog = new Dog
            {
                ID = TestConstants.EF_DeleteDogId,
                Age = TestConstants.EF_DeleteDogAge,
                Name = TestConstants.EF_DeleteDogName
            };

            //Act
            repository.Delete(dog);

            //Assert
            DataTable table = DataUtil.GetTable(TestConstants.EF_DatabaseName, TestConstants.EF_TableName);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsFalse((int)row["ID"] == TestConstants.EF_DeleteDogId);
            }
        }

        [Test]
        public void EFRepository_Delete_Does_Nothing_With_Invalid_ID()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.EF_RecordCount);

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);
            var dog = new Dog
                            {
                                ID = TestConstants.EF_InvalidDogId,
                                Age = TestConstants.EF_DeleteDogAge,
                                Name = TestConstants.EF_DeleteDogName
                            };

            //Act
            repository.Delete(dog);

            //Assert
            //Assert
            int actualCount = DataUtil.GetRecordCount(TestConstants.EF_DatabaseName,
                TestConstants.EF_TableName);
            Assert.AreEqual(TestConstants.EF_RecordCount, actualCount);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        public void EFRepository_GetAll_Returns_All_Rows(int count)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(count);

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);

            //Act
            IEnumerable<Dog> dogs = repository.GetAll();

            //Assert
            Assert.AreEqual(count, dogs.Count());
        }

        [Test]
        public void EFRepository_GetAll_Returns_List_Of_Models()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);

            //Act
            var dogs = repository.GetAll().ToList();

            //Assert
            for (int i = 0; i < dogs.Count(); i++)
            {
                Assert.IsInstanceOf<Dog>(dogs[i]);
            }
        }

        [Test]
        public void EFRepository_GetAll_Returns_Models_With_Correct_Properties()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);

            //Act
            var dogs = repository.GetAll();

            //Assert
            var dog = dogs.First();
            Assert.AreEqual(_dogAges[0], dog.Age.ToString());
            Assert.AreEqual(_dogNames[0], dog.Name);
        }

        [Test]
        public void EFRepository_GetById_Returns_Instance_Of_Model_If_Valid_Id()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);

            //Act
            var dog = repository.GetById(TestConstants.EF_ValidDogId);

            //Assert
            Assert.IsInstanceOf<Dog>(dog);
        }

        [Test]
        public void EFRepository_GetById_Returns_Null_If_InValid_Id()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);

            //Act
            var dog = repository.GetById(TestConstants.EF_InvalidDogId);

            //Assert
            Assert.IsNull(dog);
        }

        [Test]
        public void EFRepository_GetById_Returns_Model_With_Correct_Properties()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);

            //Act
            var dog = repository.GetById(TestConstants.EF_ValidDogId);

            //Assert
            Assert.AreEqual(TestConstants.EF_ValidDogAge, dog.Age);
            Assert.AreEqual(TestConstants.EF_ValidDogName, dog.Name);
        }

        [Test]
        [TestCase("Spot", 2)]
        [TestCase("Buddy", 1)]
        public void EFRepository_GetByProperty_Returns_List_Of_Models_If_Valid_Property(string dogName, int count)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);

            //Act
            var dogs = repository.GetByProperty("Name", dogName);

            //Assert
            Assert.IsInstanceOf<IEnumerable<Dog>>(dogs);
            Assert.AreEqual(count, dogs.Count());
        }

        [Test]
        public void EFRepository_GetByProperty_Returns_Instance_Of_Model_If_Valid_Property()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);
            var dogName = _dogNames[2];

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);

            //Act
            var dog = repository.GetByProperty("Name", dogName).FirstOrDefault();

            //Assert
            Assert.IsInstanceOf<Dog>(dog);
        }

        [Test]
        public void EFRepository_GetByProperty_Returns_Empty_List_If_InValid_Proeprty()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);
            const string dogName = "Invalid";

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);

            //Act
            var dogs = repository.GetByProperty("Name", dogName);

            //Assert
            Assert.IsInstanceOf<IEnumerable<Dog>>(dogs);
            Assert.AreEqual(0, dogs.Count());
        }

        [Test]
        [TestCase("Spot")]
        [TestCase("Buddy")]
        public void EFRepository_GetByProperty_Returns_Models_With_Correct_Properties(string dogName)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);

            //Act
            var dogs = repository.GetByProperty("Name", dogName);

            //Assert
            foreach (Dog dog in dogs)
            {
                Assert.AreEqual(dogName, dog.Name);
            }
        }

        [Test]
        public void EFRepository_Update_Updates_Item_In_DataBase()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.EF_RecordCount);

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);
            var dog = new Dog
            {
                ID = TestConstants.EF_UpdateDogId,
                Age = TestConstants.EF_UpdateDogAge,
                Name = TestConstants.EF_UpdateDogName
            };

            //Act
            repository.Update(dog);

            //Assert
            int actualCount = DataUtil.GetRecordCount(TestConstants.EF_DatabaseName,
                TestConstants.EF_TableName);
            Assert.AreEqual(TestConstants.EF_RecordCount, actualCount);
        }

        [Test]
        public void EFRepository_Update_Updates_Item_With_Correct_ID()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.EF_RecordCount);

            var repository = new EFRepository<Dog>(_dbContext, mockCache.Object);
            var dog = new Dog
            {
                ID = TestConstants.EF_UpdateDogId,
                Age = TestConstants.EF_UpdateDogAge,
                Name = TestConstants.EF_UpdateDogName
            };

            //Act
            repository.Update(dog);

            //Assert
            DataTable table = DataUtil.GetTable(TestConstants.EF_DatabaseName, TestConstants.EF_TableName);
            foreach (DataRow row in table.Rows)
            {
                if ((int)row["ID"] == TestConstants.EF_UpdateDogId)
                {
                    Assert.AreEqual(TestConstants.EF_UpdateDogAge, row["Age"]);
                    Assert.AreEqual(TestConstants.EF_UpdateDogName, row["Name"]);
                }
            }
        }

        private NaifDbContext CreateEFDatabase()
        {
            return new NaifDbContext(ConnectionStringName, (modelBuilder) => modelBuilder.Entity<Dog>().ToTable("Dogs"));
        }

        private void SetUpDatabase(int count)
        {
            DataUtil.CreateDatabase(TestConstants.EF_DatabaseName);
            DataUtil.ExecuteNonQuery(TestConstants.EF_DatabaseName, TestConstants.EF_CreateTableSql);
            for (int i = 0; i < count; i++)
            {
                DataUtil.ExecuteNonQuery(TestConstants.EF_DatabaseName, String.Format(TestConstants.EF_InsertRow, _dogNames[i], _dogAges[i]));
            }

            _dbContext = CreateEFDatabase();
        }
    }
}