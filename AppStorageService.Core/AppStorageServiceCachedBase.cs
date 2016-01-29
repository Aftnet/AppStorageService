﻿using System.Threading.Tasks;

namespace AppStorageService.Core
{
    public abstract class AppStorageServiceCachedBase<TService, TData> : IAppStorageService<TData> where TService : IAppStorageService<TData> where TData : class
    {
        protected abstract TService CreateBackingServiceInstance(string fileName);
        protected abstract TData CloneData(TData input);

        protected TService BackingService { get; set; }
        protected TData CachedData { get; set; }

        public bool OperationInProgress { get { return BackingService.OperationInProgress; } }

        public AppStorageServiceCachedBase(string fileName)
        {
            CachedData = null;
            BackingService = CreateBackingServiceInstance(fileName);
        }

        public Task SaveDataAsync(TData data)
        {
            CachedData = data;
            return BackingService.SaveDataAsync(CachedData);
        }

        public async Task<TData> LoadDataAsync()
        {
            if (CachedData == null)
            {
                CachedData = await BackingService.LoadDataAsync();
            }

            if(CachedData == null)
            {
                return null;
            }

            var output = CloneData(CachedData);
            return output;
        }

        public Task DeleteDataAsync()
        {
            CachedData = null;
            return BackingService.DeleteDataAsync();
        }
    }
}
