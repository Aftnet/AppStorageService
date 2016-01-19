﻿using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AppStorageService.Core.Test
{
    public abstract class AppStorageServiceTestBase<TService> where TService: IAppStorageService<TestModel>
    {
        private const string StorageFileName = "FileName.dat";
        private static readonly TestModel SampleData = TestModel.Generate();

        protected abstract IAppStorageService<T> GetServiceInstance<T>(string storageFileName) where T : class;

        [Fact]
        public async Task LoadFromStorageAsync_Returns_Null_If_No_Data_Is_Present()
        {
            var service = GetServiceInstance<TestModel>(StorageFileName);
            await service.DeleteDataAsync();

            var data = await service.LoadDataAsync();
            Assert.Null(data);
        }

        [Fact]
        public async Task LoadFromStorageAsync_Returns_Data_As_Saved()
        {
            var service = GetServiceInstance<TestModel>(StorageFileName);

            await service.SaveDataAsync(SampleData);

            var data = await service.LoadDataAsync();
            Assert.Equal(SampleData, data);
        }

        [Fact]
        public async Task DeleteDataAsync_Deletes_Data()
        {
            var service = GetServiceInstance<TestModel>(StorageFileName);

            await service.SaveDataAsync(SampleData);
            var data = await service.LoadDataAsync();
            Assert.NotNull(data);

            await service.DeleteDataAsync();
            data = await service.LoadDataAsync();
            Assert.Null(data);
        }

        [Fact]
        public async Task DeleteDataAsync_Works_When_No_Data_Is_Present()
        {
            var service = GetServiceInstance<TestModel>(StorageFileName);
            await service.DeleteDataAsync();
            await service.DeleteDataAsync();
        }

        [Fact]
        public async Task Shorter_Data_Truncates_Existing_File()
        {
            var testData = "TestString";
            var testDataLong = testData + testData;

            var testService = GetServiceInstance<string>(StorageFileName);
            await testService.SaveDataAsync(testDataLong);

            await testService.SaveDataAsync(testData);
            var data = await testService.LoadDataAsync();
            Assert.Equal(testData, data);
        }

        [Fact]
        public async Task OperationInProgress_Works()
        {
            var operationsToTest = new Func<IAppStorageService<TestModel>, Task>[]
            {
                d => d.SaveDataAsync(SampleData),
                d => d.LoadDataAsync(),
                d => d.DeleteDataAsync()
            };

            foreach(var i in operationsToTest)
            {
                var testService = GetServiceInstance<TestModel>(StorageFileName);
                Assert.False(testService.OperationInProgress);
                var task = i(testService);
                Assert.True(testService.OperationInProgress);
                await task;
                Assert.False(testService.OperationInProgress);
            }
        }

        private static string ToJSON<T>(T obj) where T : class
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, obj);
                var byteArr = stream.ToArray();
                return Encoding.UTF8.GetString(byteArr, 0, byteArr.Length);
            }
        }
    }
}
