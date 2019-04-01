using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lexiconner.Domain.Entitites;
using Lexiconner.Persistence.JsonStore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Lexiconner.Persistence.Repositories
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

        public async Task Add(StudyItem entity)
        {
            var store = await ReadStore();
            store.SudyItems.Add(entity);
            await SaveStore(store);
        }

        public async Task AddAll(IEnumerable<StudyItem> entities)
        {
            var store = await ReadStore();
            store.SudyItems.AddRange(entities);
            await SaveStore(store);
        }

        public async Task Delete(StudyItem entity)
        {
            var store = await ReadStore();
            var existingIndex = store.SudyItems.FindIndex(0, x => x.Id == entity.Id);
            if (existingIndex == -1)
            {
                throw new ArgumentException("Item not found.");
            }
            store.SudyItems.RemoveAt(existingIndex);
            await SaveStore(store);
        }

        public async Task Delete(IEnumerable<StudyItem> entities)
        {
            var store = await ReadStore();
            foreach (var entity in entities)
            {
                var existingIndex = store.SudyItems.FindIndex(0, x => x.Id == entity.Id);
                if (existingIndex == -1)
                {
                    throw new ArgumentException("Item not found.");
                }
                store.SudyItems.RemoveAt(existingIndex);
            }
            await SaveStore(store);
        }

        public async Task Delete(Func<StudyItem, bool> predicate)
        {
            var store = await ReadStore();
            var existingIndex = store.SudyItems.FindIndex(0, x => predicate(x));
            if (existingIndex == -1)
            {
                throw new ArgumentException("Item not found.");
            }
            store.SudyItems.RemoveAt(existingIndex);
            await SaveStore(store);
        }

        public async Task<bool> Exists(Func<StudyItem, bool> predicate)
        {
            var store = await ReadStore();
            return store.SudyItems.Any(predicate);
        }

        public async Task<StudyItem> Get(Func<StudyItem, bool> predicate)
        {
            var store = await ReadStore();
            return store.SudyItems.FirstOrDefault(predicate);
        }

        public async Task<IEnumerable<StudyItem>> GetAll()
        {
            var store = await ReadStore();
            return store.SudyItems;
        }

        public async Task<StudyItem> GetById(string id)
        {
            var store = await ReadStore();
            return store.SudyItems.FirstOrDefault(x => x.Id == id);
        }

        public async Task<IEnumerable<StudyItem>> GetMany(Func<StudyItem, bool> predicate)
        {
            var store = await ReadStore();
            return store.SudyItems.Where(predicate);
        }

        public async Task Update(StudyItem entity)
        {
            var store = await ReadStore();
            var existingIndex = store.SudyItems.FindIndex(0, x => x.Id == entity.Id);
            if(existingIndex == -1)
            {
                throw new ArgumentException("Item not found.");
            }
            store.SudyItems[existingIndex] = entity;
            await SaveStore(store);
        }

        public async Task UpdateAll(IEnumerable<StudyItem> entities)
        {
            var store = await ReadStore();
            foreach (var entity in entities)
            {
                var existingIndex = store.SudyItems.FindIndex(0, x => x.Id == entity.Id);
                if (existingIndex == -1)
                {
                    throw new ArgumentException("Item not found.");
                }
                store.SudyItems[existingIndex] = entity;
            }
            await SaveStore(store);
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

        public Task<long> CountAll()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
