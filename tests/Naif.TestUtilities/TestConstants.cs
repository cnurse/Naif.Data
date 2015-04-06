//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

namespace Naif.TestUtilities
{
    public class TestConstants
    {
// ReSharper disable InconsistentNaming
        public const string NPOCO_DatabaseName = "Test.sdf";
        public const string NPOCO_TableName = "Dogs";
        public const string NPOCO_CreateTableSql = "CREATE TABLE Dogs (ID int IDENTITY(1,1) NOT NULL, Name nvarchar(100) NOT NULL, Age int NULL)";
        public const string NPOCO_InsertRow = "INSERT INTO Dogs (Name, Age) VALUES ('{0}', {1})";
        public const string NPOCO_DogNames = "Spot,Buster,Buddy,Spot,Gizmo";
        public const string NPOCO_DogAges = "1,5,3,4,6";
        public const int NPOCO_RecordCount = 5;

        public const string NPOCO_InsertDogName = "Milo";
        public const int NPOCO_InsertDogAge = 3;

        public const int NPOCO_DeleteDogId = 2;
        public const string NPOCO_DeleteDogName = "Buster";
        public const int NPOCO_DeleteDogAge = 5;

        public const int NPOCO_ValidDogId = 3;
        public const string NPOCO_ValidDogName = "Buddy";
        public const int NPOCO_ValidDogAge = 3;

        public const int NPOCO_InvalidDogId = 999;

        public const string NPOCO_UpdateDogName = "Milo";
        public const int NPOCO_UpdateDogAge = 6;
        public const int NPOCO_UpdateDogId = 3;

        public const int PAGE_First = 0;
        public const int PAGE_Second = 1;
        public const int PAGE_Last = 4;
        public const int PAGE_RecordCount = 5;
        public const int PAGE_TotalCount = 22;

        public const int PAGE_NegativeIndex = -1;
        public const int PAGE_OutOfRange = 5;
        // ReSharper restore InconsistentNaming
    }
}