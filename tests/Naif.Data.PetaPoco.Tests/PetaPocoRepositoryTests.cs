//******************************************
//  Copyright (C) 2012-2013 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included License.txt file)        *
//                                         *
// *****************************************

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Naif.Core.Caching;
using PetaPoco;
using Naif.Data.PetaPoco;
using Naif.TestUtilities;
using Naif.TestUtilities.Models;
using Moq;
using System.Data;

namespace Naif.Data.PetaPoco.Tests
{
    [TestFixture]
    public class PetaPocoRepositoryTests
    {
        private const string ConnectionStringName = "PetaPoco";
        private readonly string[] _dogAges = TestConstants.PETAPOCO_DogAges.Split(',');
        private readonly string[] _dogNames = TestConstants.PETAPOCO_DogNames.Split(',');

        private PetaPocoUnitOfWork _petaPocoUnitOfWork;

        private Mock<ICacheProvider> _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = new Mock<ICacheProvider>();
            _petaPocoUnitOfWork = CreatePetaPocoUnitOfWork();
        }

        [TearDown]
        public void TearDown()
        {
            DataUtil.DeleteDatabase(TestConstants.PETAPOCO_DatabaseName);
        }

        [Test]
        public void PetaPocoRepository_Constructor_Throws_On_Null_ICacheProvider()
        {
            //Arrange, Act, Assert
            Assert.Throws<ArgumentNullException>(() => new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, null));
        }

        [Test]
        public void PetaPocoRepository_Constructor_Throws_On_Null_UnitOfWork()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => new PetaPocoRepository<Dog>(null, mockCache.Object));
        }

        [Test]
        public void PetaPocoRepository_Add_Inserts_Item_Into_DataBase()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PETAPOCO_RecordCount);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);
            var dog = new Dog
                            {
                                Age = TestConstants.PETAPOCO_InsertDogAge,
                                Name = TestConstants.PETAPOCO_InsertDogName
                            };

            //Act
            repository.Add(dog);

            //Assert
            int actualCount = DataUtil.GetRecordCount(TestConstants.PETAPOCO_DatabaseName,
                TestConstants.PETAPOCO_TableName);
            Assert.AreEqual(TestConstants.PETAPOCO_RecordCount + 1, actualCount);
        }

        [Test]
        public void PetaPocoRepository_Add_Inserts_Item_Into_DataBase_With_Correct_ID()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PETAPOCO_RecordCount);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);
            var dog = new Dog
                            {
                                Age = TestConstants.PETAPOCO_InsertDogAge,
                                Name = TestConstants.PETAPOCO_InsertDogName
                            };

            //Act
            repository.Add(dog);

            //Assert
            int newId = DataUtil.GetLastAddedRecordID(TestConstants.PETAPOCO_DatabaseName,
                TestConstants.PETAPOCO_TableName, "ID");
            Assert.AreEqual(TestConstants.PETAPOCO_RecordCount + 1, newId);
        }

        [Test]
        public void PetaPocoRepository_Add_Inserts_Item_Into_DataBase_With_Correct_ColumnValues()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PETAPOCO_RecordCount);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);
            var dog = new Dog
                            {
                                Age = TestConstants.PETAPOCO_InsertDogAge,
                                Name = TestConstants.PETAPOCO_InsertDogName
                            };

            //Act
            repository.Add(dog);

            //Assert
            DataTable table = DataUtil.GetTable(TestConstants.PETAPOCO_DatabaseName, TestConstants.PETAPOCO_TableName);
            DataRow row = table.Rows[table.Rows.Count - 1];

            Assert.AreEqual(TestConstants.PETAPOCO_InsertDogAge, row["Age"]);
            Assert.AreEqual(TestConstants.PETAPOCO_InsertDogName, row["Name"]);
        }

        [Test]
        public void PetaPocoRepository_Delete_Deletes_Item_From_DataBase()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PETAPOCO_RecordCount);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);
            var dog = new Dog
                            {
                                ID = TestConstants.PETAPOCO_DeleteDogId,
                                Age = TestConstants.PETAPOCO_DeleteDogAge,
                                Name = TestConstants.PETAPOCO_DeleteDogName
                            };

            //Act
            repository.Delete(dog);

            //Assert
            int actualCount = DataUtil.GetRecordCount(TestConstants.PETAPOCO_DatabaseName,
                TestConstants.PETAPOCO_TableName);
            Assert.AreEqual(TestConstants.PETAPOCO_RecordCount - 1, actualCount);
        }

        [Test]
        public void PetaPocoRepository_Delete_Deletes_Item_From_DataBase_With_Correct_ID()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PETAPOCO_RecordCount);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);
            var dog = new Dog
                            {
                                ID = TestConstants.PETAPOCO_DeleteDogId,
                                Age = TestConstants.PETAPOCO_DeleteDogAge,
                                Name = TestConstants.PETAPOCO_DeleteDogName
                            };

            //Act
            repository.Delete(dog);

            //Assert
            DataTable table = DataUtil.GetTable(TestConstants.PETAPOCO_DatabaseName, TestConstants.PETAPOCO_TableName);
            foreach (DataRow row in table.Rows)
            {
                Assert.IsFalse((int)row["ID"] == TestConstants.PETAPOCO_DeleteDogId);
            }
        }

        [Test]
        public void PetaPocoRepository_Delete_Does_Nothing_With_Invalid_ID()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PETAPOCO_RecordCount);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);
            var dog = new Dog
                            {
                                ID = TestConstants.PETAPOCO_InvalidDogId,
                                Age = TestConstants.PETAPOCO_DeleteDogAge,
                                Name = TestConstants.PETAPOCO_DeleteDogName
                            };

            //Act
            repository.Delete(dog);

            //Assert
            //Assert
            int actualCount = DataUtil.GetRecordCount(TestConstants.PETAPOCO_DatabaseName,
                TestConstants.PETAPOCO_TableName);
            Assert.AreEqual(TestConstants.PETAPOCO_RecordCount, actualCount);
        }

        [Test]
        [TestCase(1, "WHERE ID < @0", 2)]
        [TestCase(4, "WHERE Age <= @0", 5)]
        [TestCase(2, "WHERE Name LIKE @0", "S%")]
        public void PetaPocoRepository_Find_Returns_Correct_Rows(int count, string sqlCondition, object arg)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PETAPOCO_RecordCount);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);

            //Act
            IEnumerable<Dog> dogs = repository.Find(sqlCondition, arg);

            //Assert
            Assert.AreEqual(count, dogs.Count());
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        public void PetaPocoRepository_GetAll_Returns_All_Rows(int count)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(count);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);

            //Act
            IEnumerable<Dog> dogs = repository.GetAll();

            //Assert
            Assert.AreEqual(count, dogs.Count());
        }

        [Test]
        public void PetaPocoRepository_GetAll_Returns_List_Of_Models()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);

            //Act
            var dogs = repository.GetAll().ToList();

            //Assert
            for (int i = 0; i < dogs.Count(); i++)
            {
                Assert.IsInstanceOf<Dog>(dogs[i]);
            }
        }

        [Test]
        public void PetaPocoRepository_GetAll_Returns_Models_With_Correct_Properties()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);

            //Act
            var dogs = repository.GetAll();

            //Assert
            var dog = dogs.First();
            Assert.AreEqual(_dogAges[0], dog.Age.ToString());
            Assert.AreEqual(_dogNames[0], dog.Name);
        }

        [Test]
        public void PetaPocoRepository_GetById_Returns_Instance_Of_Model_If_Valid_Id()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);

            //Act
            var dog = repository.GetById(TestConstants.PETAPOCO_ValidDogId);

            //Assert
            Assert.IsInstanceOf<Dog>(dog);
        }

        [Test]
        public void PetaPocoRepository_GetById_Returns_Null_If_InValid_Id()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);

            //Act
            var dog = repository.GetById(TestConstants.PETAPOCO_InvalidDogId);

            //Assert
            Assert.IsNull(dog);
        }

        [Test]
        public void PetaPocoRepository_GetById_Returns_Model_With_Correct_Properties()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);

            //Act
            var dog = repository.GetById(TestConstants.PETAPOCO_ValidDogId);

            //Assert
            Assert.AreEqual(TestConstants.PETAPOCO_ValidDogAge, dog.Age);
            Assert.AreEqual(TestConstants.PETAPOCO_ValidDogName, dog.Name);
        }

        [Test]
        [TestCase("Spot", 2)]
        [TestCase("Buddy", 1)]
        public void PetaPocoRepository_GetByProperty_Returns_List_Of_Models_If_Valid_Property(string dogName, int count)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);

            //Act
            var dogs = repository.GetByProperty("Name", dogName);

            //Assert
            Assert.IsInstanceOf<IEnumerable<Dog>>(dogs);
            Assert.AreEqual(count, dogs.Count());
        }

        [Test]
        public void PetaPocoRepository_GetByProperty_Returns_Instance_Of_Model_If_Valid_Property()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);
            var dogName = _dogNames[2];

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);

            //Act
            var dog = repository.GetByProperty("Name", dogName).FirstOrDefault();

            //Assert
            Assert.IsInstanceOf<Dog>(dog);
        }

        [Test]
        public void PetaPocoRepository_GetByProperty_Returns_Empty_List_If_InValid_Proeprty()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);
            const string dogName = "Invalid";

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);

            //Act
            var dogs = repository.GetByProperty("Name", dogName);

            //Assert
            Assert.IsInstanceOf<IEnumerable<Dog>>(dogs);
            Assert.AreEqual(0, dogs.Count());
        }

        [Test]
        [TestCase("Spot")]
        [TestCase("Buddy")]
        public void PetaPocoRepository_GetByProperty_Returns_Models_With_Correct_Properties(string dogName)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(5);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);

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
        public void PetaPocoRepository_GetPage_Returns_Page_Of_Rows(int pageIndex, int pageSize)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PAGE_TotalCount);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);

            //Act
            var dogs = repository.GetPage(pageIndex, pageSize);

            //Assert
            Assert.AreEqual(pageSize, dogs.PageSize);
        }

        [Test]
        public void PetaPocoRepository_GetPage_Returns_List_Of_Models()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PAGE_TotalCount);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);

            //Act
            var dogs = repository.GetPage(TestConstants.PAGE_First, TestConstants.PAGE_RecordCount);

            //Assert
            for (int i = 0; i < dogs.Count(); i++)
            {
                Assert.IsInstanceOf<Dog>(dogs[i]);
            }
        }

        [Test]
        public void PetaPocoRepository_GetPage_Returns_Models_With_Correct_Properties()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PAGE_TotalCount);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);

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
        public void PetaPocoRepository_GetPage_Returns_Correct_Page(int pageIndex, int pageSize, int firstId)
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PAGE_TotalCount);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);

            //Act
            var dogs = repository.GetPage(pageIndex, pageSize);

            //Assert
            var dog = dogs.First();
            Assert.AreEqual(firstId, dog.ID);
        }

        [Test]
        public void PetaPocoRepository_Update_Updates_Item_In_DataBase()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PETAPOCO_RecordCount);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);
            var dog = new Dog
            {
                ID = TestConstants.PETAPOCO_UpdateDogId,
                Age = TestConstants.PETAPOCO_UpdateDogAge,
                Name = TestConstants.PETAPOCO_UpdateDogName
            };

            //Act
            repository.Update(dog);

            //Assert
            int actualCount = DataUtil.GetRecordCount(TestConstants.PETAPOCO_DatabaseName,
                TestConstants.PETAPOCO_TableName);
            Assert.AreEqual(TestConstants.PETAPOCO_RecordCount, actualCount);
        }

        [Test]
        public void PetaPocoRepository_Update_Updates_Item_With_Correct_ID()
        {
            //Arrange
            var mockCache = new Mock<ICacheProvider>();
            SetUpDatabase(TestConstants.PETAPOCO_RecordCount);

            var repository = new PetaPocoRepository<Dog>(_petaPocoUnitOfWork, mockCache.Object);
            var dog = new Dog
            {
                ID = TestConstants.PETAPOCO_UpdateDogId,
                Age = TestConstants.PETAPOCO_UpdateDogAge,
                Name = TestConstants.PETAPOCO_UpdateDogName
            };

            //Act
            repository.Update(dog);

            //Assert
            DataTable table = DataUtil.GetTable(TestConstants.PETAPOCO_DatabaseName, TestConstants.PETAPOCO_TableName);
            foreach (DataRow row in table.Rows)
            {
                if ((int)row["ID"] == TestConstants.PETAPOCO_UpdateDogId)
                {
                    Assert.AreEqual(TestConstants.PETAPOCO_UpdateDogAge, row["Age"]);
                    Assert.AreEqual(TestConstants.PETAPOCO_UpdateDogName, row["Name"]);
                }
            }
        }

        private PetaPocoUnitOfWork CreatePetaPocoUnitOfWork()
        {
            return new PetaPocoUnitOfWork(ConnectionStringName, _cache.Object);
        }

        private void SetUpDatabase(int count)
        {
            DataUtil.CreateDatabase(TestConstants.PETAPOCO_DatabaseName);
            DataUtil.ExecuteNonQuery(TestConstants.PETAPOCO_DatabaseName, TestConstants.PETAPOCO_CreateTableSql);
            for (int i = 0; i < count; i++)
            {
                var name = (i < _dogNames.Length) ? _dogNames[i] : String.Format("Name_{0}", i);
                var age = (i < _dogNames.Length) ? _dogAges[i] : i.ToString();

                DataUtil.ExecuteNonQuery(TestConstants.PETAPOCO_DatabaseName, String.Format(TestConstants.PETAPOCO_InsertRow, name, age));
            }
        }
    }
}
