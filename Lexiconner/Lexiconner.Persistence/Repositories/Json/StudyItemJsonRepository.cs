using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Lexiconner.Domain.Entitites;
using Lexiconner.Domain.Entitites.Base;
using Lexiconner.Persistence.JsonStore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Lexiconner.Persistence.Repositories.Json
{
    public class StudyItemJsonRepository : IStudyItemJsonRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _storePath;
        private object _lock = new object();

        public StudyItemJsonRepository(string storePath)
        {
            _storePath = storePath;
        }

        #region Implementation

        public async Task<long> CountAll()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(int offset, int limit, string search = "") where T : class, new()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetManyAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetOneAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync<T>(T entity) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync<T>(IEnumerable<T> entities) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync<T>(T entity) where T : BaseEntity, new()
        {
            var store = await ReadStore();
            var existingIndex = store.StudyItems.FindIndex(0, x => x.Id == entity.Id);
            if (existingIndex == -1)
            {
                throw new ArgumentException("Item not found.");
            }
            store.StudyItems[existingIndex] = entity as StudyItemEntity;
            await SaveStore(store);
        }

        public async Task UpdateAsync<T>(IEnumerable<T> entities) where T : BaseEntity, new()
        {
            var store = await ReadStore();
            foreach (var entity in entities)
            {
                var existingIndex = store.StudyItems.FindIndex(0, x => x.Id == entity.Id);
                if (existingIndex == -1)
                {
                    throw new ArgumentException("Item not found.");
                }
                store.StudyItems[existingIndex] = entity as StudyItemEntity;
            }
            await SaveStore(store);
        }

        public async Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAllAsync<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public async Task<long> CountAllAsync<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private

        private async Task<JsonStoreModel> ReadStore()
        {
            lock(_lock)
            {
                CreateStoreIfNotExists();
                var textContent = File.ReadAllText(Path.Combine(_storePath));
                var model = JsonConvert.DeserializeObject<JsonStoreModel>(textContent);
                return model ?? new JsonStoreModel();
            }
        }

        private async Task SaveStore(JsonStoreModel model)
        {
            lock (_lock)
            {
                CreateStoreIfNotExists();
                var textContent = JsonConvert.SerializeObject(model, Formatting.Indented);
                File.WriteAllText(Path.Combine(_storePath), textContent);
            }
        }

        private void CreateStoreIfNotExists()
        {
            var dirPath = Path.GetDirectoryName(_storePath);
            if (!Directory.Exists(Path.Combine(dirPath)))
            {
                Directory.CreateDirectory(Path.Combine(dirPath));
            }
            if (!File.Exists(Path.Combine(_storePath)))
            {
                var fs = File.Create(Path.Combine(_storePath));
                fs.Close();
                fs.Dispose();
            }
        }

        public Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
