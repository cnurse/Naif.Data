//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
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
using NPoco;
using NUnit.Framework;

namespace Naif.Data.NPoco.Tests
{
    [TestFixture]
    public class NPocoRepositoryTests
    {
        private const string ConnectionStringName = "NPoco";
        private readonly string[] _dogAges = TestConstants.NPOCO_DogAges.Split(',');
        private readonly string[] _dogNames = TestConstants.NPOCO_DogNames.Split(',');

        private NPocoUnitOfWork _nPocoUnitOfWork;

        private Mock<ICacheProvider> _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = new Mock<ICacheProvider>();
            _nPocoUnitOfWork = CreateNPocoUnitOfWork();
        }

        [TearDown]
        public void TearDown()
        {
            DataUtil.DeleteDatabase(TestConstants.NPOCO_DatabaseName);
        }

        [Test]
        public void NPocoRepository_Constructor_Throws_On_Null_ICacheProvider()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentNullException>(() => new NPocoRepository<Dog>(_nPocoUnitOfWork, null));
        }

        [Test]
        public void NPocoRepository_Constructor_Throws_On_Null_UnitOfWork()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => new NPocoRepository<Dog>(null, mockCache.Object));
        }

        [Test]
        public void NPocoRepository_Add_Inserts_Item_Into_DataBase()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.NPOCO_RecordCount);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);
            var dog = new Dog
                            {
                                Age = TestConstants.NPOCO_InsertDogAge,
                                Name = TestConstants.NPOCO_InsertDogName
                            };

            //Act
            repository.Add(dog);

            //Assert
            int actualCount = DataUtil.GetRecordCount(TestConstants.NPOCO_DatabaseName, TestConstants.NPOCO_TableName);
            Assert.AreEqual(TestConstants.NPOCO_RecordCount + 1, actualCount);
        }

        [Test]
        public void NPocoRepository_Add_Inserts_Item_Into_DataBase_With_Correct_ID()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.NPOCO_RecordCount);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);
            var dog = new Dog
            {
                Age = TestConstants.NPOCO_InsertDogAge,
                Name = TestConstants.NPOCO_InsertDogName
            };

            //Act
            repository.Add(dog);

            //Assert
            int newId = DataUtil.GetLastAddedRecordID(TestConstants.NPOCO_DatabaseName,
                TestConstants.NPOCO_TableName, "ID");
            Assert.AreEqual(TestConstants.NPOCO_RecordCount + 1, newId);
        }

        [Test]
        public void NPocoRepository_Add_Inserts_Item_Into_DataBase_With_Correct_ColumnValues()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.NPOCO_RecordCount);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);
            var dog = new Dog
            {
                Age = TestConstants.NPOCO_InsertDogAge,
                Name = TestConstants.NPOCO_InsertDogName
            };

            //Act
            repository.Add(dog);

            //Assert
            DataTable table = DataUtil.GetTable(TestConstants.NPOCO_DatabaseName, TestConstants.NPOCO_TableName);
            DataRow row = table.Rows[table.Rows.Count - 1];

            Assert.AreEqual(TestConstants.NPOCO_InsertDogAge, row["Age"]);
            Assert.AreEqual(TestConstants.NPOCO_InsertDogName, row["Name"]);
        }

        [Test]
        public void NPocoRepository_Delete_Deletes_Item_From_DataBase()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.NPOCO_RecordCount);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);
            var dog = new Dog
            {
                ID = TestConstants.NPOCO_DeleteDogId,
                Age = TestConstants.NPOCO_DeleteDogAge,
                Name = TestConstants.NPOCO_DeleteDogName
            };

            //Act
            repository.Delete(dog);

            //Assert
            int actualCount = DataUtil.GetRecordCount(TestConstants.NPOCO_DatabaseName,
                TestConstants.NPOCO_TableName);
            Assert.AreEqual(TestConstants.NPOCO_RecordCount - 1, actualCount);
        }

        [Test]
        public void NPocoRepository_Delete_Deletes_Item_From_DataBase_With_Correct_ID()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.NPOCO_RecordCount);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);
            var dog = new Dog
            {
                ID = TestConstants.NPOCO_DeleteDogId,
                Age = TestConstants.NPOCO_DeleteDogAge,
                Name = TestConstants.NPOCO_DeleteDogName
            };

            //Act
            repository.Delete(dog);

            //Assert
            DataTable table = DataUtil.GetTable(TestConstants.NPOCO_DatabaseName, TestConstants.NPOCO_TableName);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsFalse((int)row["ID"] == TestConstants.NPOCO_DeleteDogId);
            }
        }

        [Test]
        public void NPocoRepository_Delete_Does_Nothing_With_Invalid_ID()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.NPOCO_RecordCount);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);
            var dog = new Dog
            {
                ID = TestConstants.NPOCO_InvalidDogId,
                Age = TestConstants.NPOCO_DeleteDogAge,
                Name = TestConstants.NPOCO_DeleteDogName
            };

            //Act
            repository.Delete(dog);

            //Assert
            //Assert
            int actualCount = DataUtil.GetRecordCount(TestConstants.NPOCO_DatabaseName,
                TestConstants.NPOCO_TableName);
            Assert.AreEqual(TestConstants.NPOCO_RecordCount, actualCount);
        }

        [Test]
        [TestCase(1, "WHERE ID < @0", 2)]
        [TestCase(4, "WHERE Age <= @0", 5)]
        [TestCase(2, "WHERE Name LIKE @0", "S%")]
        public void NPocoRepository_Find_Returns_Correct_Rows(int count, string sqlCondition, object arg)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PETAPOCO_RecordCount);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);

            //Act
            IEnumerable<Dog> dogs = repository.Find(sqlCondition, arg);

            //Assert
            Assert.AreEqual(count, dogs.Count());
        }


        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        public void NPocoRepository_GetAll_Returns_All_Rows(int count)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(count);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);

            //Act
            IEnumerable<Dog> dogs = repository.GetAll();

            //Assert
            Assert.AreEqual(count, dogs.Count());
        }

        [Test]
        public void NPocoRepository_GetAll_Returns_List_Of_Models()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);

            //Act
            var dogs = repository.GetAll().ToList();

            //Assert
            for (int i = 0; i < dogs.Count(); i++)
            {
                Assert.IsInstanceOf<Dog>(dogs[i]);
            }
        }

        [Test]
        public void NPocoRepository_GetAll_Returns_Models_With_Correct_Properties()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);

            //Act
            var dogs = repository.GetAll();

            //Assert
            var dog = dogs.First();
            Assert.AreEqual(_dogAges[0], dog.Age.ToString());
            Assert.AreEqual(_dogNames[0], dog.Name);
        }

        [Test]
        public void NPocoRepository_GetById_Returns_Instance_Of_Model_If_Valid_Id()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);

            //Act
            var dog = repository.GetById(TestConstants.NPOCO_ValidDogId);

            //Assert
            Assert.IsInstanceOf<Dog>(dog);
        }

        [Test]
        public void NPocoRepository_GetById_Returns_Null_If_InValid_Id()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);

            //Act
            var dog = repository.GetById(TestConstants.NPOCO_InvalidDogId);

            //Assert
            Assert.IsNull(dog);
        }

        [Test]
        public void NPocoRepository_GetById_Returns_Model_With_Correct_Properties()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);

            //Act
            var dog = repository.GetById(TestConstants.NPOCO_ValidDogId);

            //Assert
            Assert.AreEqual(TestConstants.NPOCO_ValidDogAge, dog.Age);
            Assert.AreEqual(TestConstants.NPOCO_ValidDogName, dog.Name);
        }

        [Test]
        [TestCase("Spot", 2)]
        [TestCase("Buddy", 1)]
        public void NPocoRepository_GetByProperty_Returns_List_Of_Models_If_Valid_Property(string dogName, int count)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);

            //Act
            var dogs = repository.GetByProperty("Name", dogName);

            //Assert
            Assert.IsInstanceOf<IEnumerable<Dog>>(dogs);
            Assert.AreEqual(count, dogs.Count());
        }

        [Test]
        public void NPocoRepository_GetByProperty_Returns_Instance_Of_Model_If_Valid_Property()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);
            var dogName = _dogNames[2];

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);

            //Act
            var dog = repository.GetByProperty("Name", dogName).FirstOrDefault();

            //Assert
            Assert.IsInstanceOf<Dog>(dog);
        }

        [Test]
        public void NPocoRepository_GetByProperty_Returns_Empty_List_If_InValid_Proeprty()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);
            const string dogName = "Invalid";

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);

            //Act
            var dogs = repository.GetByProperty("Name", dogName);

            //Assert
            Assert.IsInstanceOf<IEnumerable<Dog>>(dogs);
            Assert.AreEqual(0, dogs.Count());
        }

        [Test]
        [TestCase("Spot")]
        [TestCase("Buddy")]
        public void NPocoRepository_GetByProperty_Returns_Models_With_Correct_Properties(string dogName)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);

            //Act
            var dogs = repository.GetByProperty("Name", dogName);

            //Assert
            foreach (Dog dog in dogs)
            {
                Assert.AreEqual(dogName, dog.Name);
            }
        }

        [Test]
        [TestCase(TestConstants.PAGE_First, TestConstants.PAGE_RecordCount)]
        [TestCase(TestConstants.PAGE_Second, TestConstants.PAGE_RecordCount)]
        [TestCase(TestConstants.PAGE_Last, TestConstants.PAGE_RecordCount)]
        public void NPocoRepository_GetPage_Returns_Page_Of_Rows(int pageIndex, int pageSize)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PAGE_TotalCount);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);

            //Act
            var dogs = repository.GetPage(pageIndex, pageSize);

            //Assert
            Assert.AreEqual(pageSize, dogs.PageSize);
        }

        [Test]
        public void NPocoRepository_GetPage_Returns_List_Of_Models()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PAGE_TotalCount);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);

            //Act
            var dogs = repository.GetPage(TestConstants.PAGE_First, TestConstants.PAGE_RecordCount);

            //Assert
            for (int i = 0; i < dogs.Count(); i++)
            {
                Assert.IsInstanceOf<Dog>(dogs[i]);
            }
        }

        [Test]
        public void NPocoRepository_GetPage_Returns_Models_With_Correct_Properties()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PAGE_TotalCount);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);

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
        public void NPocoRepository_GetPage_Returns_Correct_Page(int pageIndex, int pageSize, int firstId)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PAGE_TotalCount);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);

            //Act
            var dogs = repository.GetPage(pageIndex, pageSize);

            //Assert
            var dog = dogs.First();
            Assert.AreEqual(firstId, dog.ID);
        }

        [Test]
        public void NPocoRepository_Update_Updates_Item_In_DataBase()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.NPOCO_RecordCount);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);
            var dog = new Dog
            {
                ID = TestConstants.NPOCO_UpdateDogId,
                Age = TestConstants.NPOCO_UpdateDogAge,
                Name = TestConstants.NPOCO_UpdateDogName
            };

            //Act
            repository.Update(dog);

            //Assert
            int actualCount = DataUtil.GetRecordCount(TestConstants.NPOCO_DatabaseName,
                TestConstants.NPOCO_TableName);
            Assert.AreEqual(TestConstants.NPOCO_RecordCount, actualCount);
        }

        [Test]
        public void NPocoRepository_Update_Updates_Item_With_Correct_ID()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.NPOCO_RecordCount);

            var repository = new NPocoRepository<Dog>(_nPocoUnitOfWork, mockCache.Object);
            var dog = new Dog
            {
                ID = TestConstants.NPOCO_UpdateDogId,
                Age = TestConstants.NPOCO_UpdateDogAge,
                Name = TestConstants.NPOCO_UpdateDogName
            };

            //Act
            repository.Update(dog);

            //Assert
            DataTable table = DataUtil.GetTable(TestConstants.NPOCO_DatabaseName, TestConstants.NPOCO_TableName);
            foreach (DataRow row in table.Rows)
            {
                if ((int)row["ID"] == TestConstants.NPOCO_UpdateDogId)
                {
                    Assert.AreEqual(TestConstants.NPOCO_UpdateDogAge, row["Age"]);
                    Assert.AreEqual(TestConstants.NPOCO_UpdateDogName, row["Name"]);
                }
            }
        }

        private NPocoUnitOfWork CreateNPocoUnitOfWork()
        {
            return new NPocoUnitOfWork(ConnectionStringName, _cache.Object);
        }

        private void SetUpDatabase(int count)
        {
            DataUtil.CreateDatabase(TestConstants.NPOCO_DatabaseName);
            DataUtil.ExecuteNonQuery(TestConstants.NPOCO_DatabaseName, TestConstants.NPOCO_CreateTableSql);
            for (int i = 0; i < count; i++)
            {
                var name = (i < _dogNames.Length) ? _dogNames[i] : String.Format("Name_{0}", i);
                var age = (i < _dogNames.Length) ? _dogAges[i] : i.ToString();

                DataUtil.ExecuteNonQuery(TestConstants.NPOCO_DatabaseName, String.Format(TestConstants.PETAPOCO_InsertRow, name, age));
            }
        }
    }
}
