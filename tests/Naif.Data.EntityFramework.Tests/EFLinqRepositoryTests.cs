//******************************************
//  Copyright (C) 2012-2013 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included License.txt file)        *
//                                         *
// *****************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Moq;
using Naif.Core.Caching;
using Naif.TestUtilities;
using Naif.TestUtilities.Models;
using NUnit.Framework;

namespace Naif.Data.EntityFramework.Tests
{
    [TestFixture]
    public class EFLinqRepositoryTests
    {
        private const string ConnectionStringName = "EntityFramework";
        private readonly string[] _dogAges = TestConstants.EF_DogAges.Split(',');
        private readonly string[] _dogNames = TestConstants.EF_DogNames.Split(',');

        private Mock<ICacheProvider> _cache;

        private EFUnitOfWork _efUnitOfWork;

        [SetUp]
        public void SetUp()
        {
            _cache = new Mock<ICacheProvider>();
        }

        [TearDown]
        public void TearDown()
        {
            DataUtil.DeleteDatabase(TestConstants.EF_DatabaseName);
        }

        [Test]
        public void EFLinqRepository_Constructor_Throws_On_Null_DbContext()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentNullException>(() => new EFLinqRepository<Dog>(null, _cache.Object));
        }

        [Test]
        public void EFLinqRepository_Add_Inserts_Item_Into_DataBase()
        {
            //Arrange
            SetUpDatabase(TestConstants.EF_RecordCount);

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);
            var dog = new Dog
                            {
                                Age = TestConstants.EF_InsertDogAge,
                                Name = TestConstants.EF_InsertDogName
                            };

            //Act
            repository.Add(dog);
            _efUnitOfWork.Commit();

            //Assert
            int actualCount = DataUtil.GetRecordCount(TestConstants.EF_DatabaseName, TestConstants.EF_TableName);
            Assert.AreEqual(TestConstants.EF_RecordCount + 1, actualCount);
        }

        [Test]
        public void EFLinqRepository_Add_Inserts_Item_Into_DataBase_With_Correct_ID()
        {
            //Arrange
            SetUpDatabase(TestConstants.EF_RecordCount);

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);
            var dog = new Dog
                            {
                                Age = TestConstants.EF_InsertDogAge,
                                Name = TestConstants.EF_InsertDogName
                            };

            //Act
            repository.Add(dog);
            _efUnitOfWork.Commit();

            //Assert
            int newId = DataUtil.GetLastAddedRecordID(TestConstants.EF_DatabaseName, TestConstants.EF_TableName, "ID");
            Assert.AreEqual(TestConstants.EF_RecordCount + 1, newId);
        }

        [Test]
        public void EFLinqRepository_Add_Inserts_Item_Into_DataBase_With_Correct_ColumnValues()
        {
            //Arrange
            SetUpDatabase(TestConstants.EF_RecordCount);

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);
            var dog = new Dog
                            {
                                Age = TestConstants.EF_InsertDogAge,
                                Name = TestConstants.EF_InsertDogName
                            };

            //Act
            repository.Add(dog);
            _efUnitOfWork.Commit();

            //Assert
            DataTable table = DataUtil.GetTable(TestConstants.EF_DatabaseName, TestConstants.EF_TableName);
            DataRow row = table.Rows[table.Rows.Count - 1];

            Assert.AreEqual(TestConstants.EF_InsertDogAge, row["Age"]);
            Assert.AreEqual(TestConstants.EF_InsertDogName, row["Name"]);
        }

        [Test]
        public void EFLinqRepository_Delete_Deletes_Item_From_DataBase()
        {
            //Arrange
            SetUpDatabase(TestConstants.EF_RecordCount);

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);
            var dog = new Dog
                            {
                                ID = TestConstants.EF_DeleteDogId,
                                Age = TestConstants.EF_DeleteDogAge,
                                Name = TestConstants.EF_DeleteDogName
                            };

            //Act
            repository.Delete(dog);
            _efUnitOfWork.Commit();

            //Assert
            int actualCount = DataUtil.GetRecordCount(TestConstants.EF_DatabaseName, TestConstants.EF_TableName);
            Assert.AreEqual(TestConstants.EF_RecordCount - 1, actualCount);
        }

        [Test]
        public void EFLinqRepository_Delete_Deletes_Item_From_DataBase_With_Correct_ID()
        {
            //Arrange
            SetUpDatabase(TestConstants.EF_RecordCount);

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);
            var dog = new Dog
            {
                ID = TestConstants.EF_DeleteDogId,
                Age = TestConstants.EF_DeleteDogAge,
                Name = TestConstants.EF_DeleteDogName
            };

            //Act
            repository.Delete(dog);
            _efUnitOfWork.Commit();

            //Assert
            DataTable table = DataUtil.GetTable(TestConstants.EF_DatabaseName, TestConstants.EF_TableName);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsFalse((int)row["ID"] == TestConstants.EF_DeleteDogId);
            }
        }

        [Test]
        public void EFLinqRepository_Delete_Throws_With_Invalid_ID()
        {
            //Arrange
            SetUpDatabase(TestConstants.EF_RecordCount);

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);
            var dog = new Dog
                            {
                                ID = TestConstants.EF_InvalidDogId,
                                Age = TestConstants.EF_DeleteDogAge,
                                Name = TestConstants.EF_DeleteDogName
                            };

            //Act
            var success = false;
            try
            {
                repository.Delete(dog);
                _efUnitOfWork.Commit();
                success = true;
            }
            catch (Exception)
            {
                
            }

            //Assert
            Assert.IsFalse(success);
        }

        [Test]
        [TestCase("Spot", 2)]
        [TestCase("Buddy", 1)]
        public void EFLinqRepository_Find_Returns_List_Of_Models_If_Valid_Property(string dogName, int count)
        {
            //Arrange
            SetUpDatabase(5);

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);

            //Act
            var dogs = repository.Find((d) => d.Name == dogName);

            //Assert
            Assert.IsInstanceOf<IEnumerable<Dog>>(dogs);
            Assert.AreEqual(count, dogs.Count());
        }

        [Test]
        public void EFLinqRepository_Find_Returns_Instance_Of_Model_If_Valid_Property()
        {
            //Arrange
            SetUpDatabase(5);
            var dogName = _dogNames[2];

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);

            //Act
            var dog = repository.Find((d) => d.Name == dogName).FirstOrDefault();

            //Assert
            Assert.IsInstanceOf<Dog>(dog);
        }

        [Test]
        public void EFLinqRepository_Find_Returns_Empty_List_If_InValid_Proeprty()
        {
            //Arrange
            SetUpDatabase(5);
            const string dogName = "Invalid";

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);

            //Act
            var dogs = repository.Find((d) => d.Name == dogName);

            //Assert
            Assert.IsInstanceOf<IEnumerable<Dog>>(dogs);
            Assert.AreEqual(0, dogs.Count());
        }

        [Test]
        [TestCase("Spot")]
        [TestCase("Buddy")]
        public void EFLinqRepository_Find_Returns_Models_With_Correct_Properties(string dogName)
        {
            //Arrange
            SetUpDatabase(5);

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);

            //Act
            var dogs = repository.Find((d) => d.Name == dogName);

            //Assert
            foreach (Dog dog in dogs)
            {
                Assert.AreEqual(dogName, dog.Name);
            }
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        public void EFLinqRepository_GetAll_Returns_All_Rows(int count)
        {
            //Arrange
            SetUpDatabase(count);

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);

            //Act
            IEnumerable<Dog> dogs = repository.GetAll();

            //Assert
            Assert.AreEqual(count, dogs.Count());
        }

        [Test]
        public void EFLinqRepository_GetAll_Returns_List_Of_Models()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);

            //Act
            var dogs = repository.GetAll().ToList();

            //Assert
            for (int i = 0; i < dogs.Count(); i++)
            {
                Assert.IsInstanceOf<Dog>(dogs[i]);
            }
        }

        [Test]
        public void EFLinqRepository_GetAll_Returns_Models_With_Correct_Properties()
        {
            //Arrange
            SetUpDatabase(5);

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);

            //Act
            var dogs = repository.GetAll();

            //Assert
            var dog = dogs.First();
            Assert.AreEqual(_dogAges[0], dog.Age.ToString());
            Assert.AreEqual(_dogNames[0], dog.Name);
        }

        [Test]
        [TestCase(TestConstants.PAGE_First, TestConstants.PAGE_RecordCount)]
        [TestCase(TestConstants.PAGE_Second, TestConstants.PAGE_RecordCount)]
        [TestCase(TestConstants.PAGE_Last, TestConstants.PAGE_RecordCount)]
        public void EFLinqRepository_GetPage_Returns_Page_Of_Rows(int pageIndex, int pageSize)
        {
            //Arrange
            SetUpDatabase(TestConstants.PAGE_TotalCount);

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);

            //Act
            var dogs = repository.GetPage(pageIndex, pageSize);

            //Assert
            Assert.AreEqual(pageSize, dogs.PageSize);
        }

        [Test]
        public void EFLinqRepository_GetPage_Returns_List_Of_Models()
        {
            //Arrange
            SetUpDatabase(TestConstants.PAGE_TotalCount);

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);

            //Act
            var dogs = repository.GetPage(TestConstants.PAGE_First, TestConstants.PAGE_RecordCount);

            //Assert
            for (int i = 0; i < dogs.Count(); i++)
            {
                Assert.IsInstanceOf<Dog>(dogs[i]);
            }
        }

        [Test]
        public void EFLinqRepository_GetPage_Returns_Models_With_Correct_Properties()
        {
            //Arrange
            SetUpDatabase(TestConstants.PAGE_TotalCount);

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);

            //Act
            var dogs = repository.GetPage(TestConstants.PAGE_First, TestConstants.PAGE_RecordCount);

            //Assert
            var dog = dogs.First();
            Assert.AreEqual(_dogAges[0], dog.Age.ToString());
            Assert.AreEqual(_dogNames[0], dog.Name);
        }

        [Test]
        [TestCase(TestConstants.PAGE_First, TestConstants.PAGE_RecordCount, 1)]
        [TestCase(TestConstants.PAGE_Second, TestConstants.PAGE_RecordCount, 6)]
        [TestCase(2, 4, 9)]
        public void EFLinqRepository_GetPage_Returns_Correct_Page(int pageIndex, int pageSize, int firstId)
        {
            //Arrange
            SetUpDatabase(TestConstants.PAGE_TotalCount);

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);

            //Act
            var dogs = repository.GetPage(pageIndex, pageSize);

            //Assert
            var dog = dogs.First();
            Assert.AreEqual(firstId, dog.ID);
        }


        //[Test]
        //public void EFLinqRepository_GetSingle_Returns_Instance_Of_Model_If_Valid_Id()
        //{
        //    //Arrange
        //    SetUpDatabase(5);

        //    var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);

        //    //Act
        //    var dog = repository.GetSingle((d) => d.ID == TestConstants.EF_ValidDogId);

        //    //Assert
        //    Assert.IsInstanceOf<Dog>(dog);
        //}

        //[Test]
        //public void EFLinqRepository_GetSingle_Returns_Null_If_InValid_Id()
        //{
        //    //Arrange
        //    var mockCache = new Mock<ICacheProvider>();
        //    SetUpDatabase(5);

        //    var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);

        //    //Act
        //    var dog = repository.GetSingle((d) => d.ID == TestConstants.EF_InvalidDogId);

        //    //Assert
        //    Assert.IsNull(dog);
        //}

        //[Test]
        //public void EFLinqRepository_GetSingle_Returns_Model_With_Correct_Properties()
        //{
        //    //Arrange
        //    var mockCache = new Mock<ICacheProvider>();
        //    SetUpDatabase(5);

        //    var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);

        //    //Act
        //    var dog = repository.GetSingle((d) => d.ID == TestConstants.EF_ValidDogId);

        //    //Assert
        //    Assert.AreEqual(TestConstants.EF_ValidDogAge, dog.Age);
        //    Assert.AreEqual(TestConstants.EF_ValidDogName, dog.Name);
        //}

        [Test]
        public void EFLinqRepository_Update_Updates_Item_In_DataBase()
        {
            //Arrange
            SetUpDatabase(TestConstants.EF_RecordCount);

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);
            var dog = new Dog
                            {
                                ID = TestConstants.EF_UpdateDogId,
                                Age = TestConstants.EF_UpdateDogAge,
                                Name = TestConstants.EF_UpdateDogName
                            };

            //Act
            repository.Update(dog);
            _efUnitOfWork.Commit();

            //Assert
            int actualCount = DataUtil.GetRecordCount(TestConstants.EF_DatabaseName, TestConstants.EF_TableName);
            Assert.AreEqual(TestConstants.EF_RecordCount, actualCount);
        }

        [Test]
        public void EFLinqRepository_Update_Updates_Item_With_Correct_ID()
        {
            //Arrange
            SetUpDatabase(TestConstants.EF_RecordCount);

            var repository = new EFLinqRepository<Dog>(_efUnitOfWork, _cache.Object);
            var dog = new Dog
                            {
                                ID = TestConstants.EF_UpdateDogId,
                                Age = TestConstants.EF_UpdateDogAge,
                                Name = TestConstants.EF_UpdateDogName
                            };

            //Act
            repository.Update(dog);
            _efUnitOfWork.Commit();

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

        private EFUnitOfWork CreateEFUnitOfWork()
        {
            return new EFUnitOfWork(ConnectionStringName, (modelBuilder) => modelBuilder.Entity<Dog>().ToTable("Dogs"), _cache.Object);
        }

        private void SetUpDatabase(int count)
        {
            DataUtil.CreateDatabase(TestConstants.EF_DatabaseName);
            DataUtil.ExecuteNonQuery(TestConstants.EF_DatabaseName, TestConstants.EF_CreateTableSql);
            for (int i = 0; i < count; i++)
            {
                var name = (i < _dogNames.Length) ? _dogNames[i] : String.Format("Name_{0}", i);
                var age = (i < _dogNames.Length) ? _dogAges[i] : i.ToString();

                DataUtil.ExecuteNonQuery(TestConstants.EF_DatabaseName, String.Format(TestConstants.EF_InsertRow, name, age));
            }

            _efUnitOfWork = CreateEFUnitOfWork();
        }
    }
}
