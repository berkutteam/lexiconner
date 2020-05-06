using Lexiconner.Domain.DTOs.CustomCollections;
using Lexiconner.Domain.Entitites.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexiconner.Domain.Entitites
{
    public class CustomCollectionEntity : BaseEntity
    {
        public CustomCollectionEntity()
        {
            ChildrenCollections = new List<CustomCollectionEntity>();
        }

        public string UserId { get; set; }
        public string Name { get; set; }
        public bool IsRoot { get; set; }
        public List<CustomCollectionEntity> ChildrenCollections { get; set; }


        #region Helper methods

        public void AddChildCollection(string parentCollectionId, CustomCollectionEntity entity)
        {
            if(parentCollectionId == null || this.Id == parentCollectionId)
            {
                this.ChildrenCollections.Add(entity);
            }
            else
            {
                this.ChildrenCollections.ForEach(x =>
                {
                    x.AddChildCollection(parentCollectionId, entity);
                });
            }
        }

        public void UpdateSelf(CustomCollectionUpdateDto updateDto)
        {
            this.Name = updateDto.Name;
        }

        public void UpdateChildCollection(string collectionId, CustomCollectionUpdateDto updateDto)
        {
            if(this.Id == collectionId)
            {
                this.UpdateSelf(updateDto);
            }
            else
            {
                this.ChildrenCollections.ForEach(x =>
                {
                    x.UpdateChildCollection(collectionId, updateDto);
                });
            }
        }

        public void RemoveChildCollection(string collectionId)
        {
            var existing = this.ChildrenCollections.FirstOrDefault(x => x.Id == collectionId);
            if(existing != null)
            {
                this.ChildrenCollections.Remove(existing);
            }
            else
            {
                this.ChildrenCollections.ForEach(x =>
                {
                    x.RemoveChildCollection(collectionId);
                });
            }
        }

        public void DuplicateCollection(string collectionId)
        {
            if (this.Id == collectionId && this.IsRoot)
            {
                // do nothing
            }
            else if (this.ChildrenCollections.Any(x => x.Id == collectionId))
            {
                int sourceIndex = this.ChildrenCollections.FindIndex(0, x => x.Id == collectionId);
                var source = this.ChildrenCollections.First(x => x.Id == collectionId);
                var duplicate = JsonConvert.DeserializeObject<CustomCollectionEntity>(JsonConvert.SerializeObject(source));
                
                // assign new ids
                duplicate.RegenerateId();
                duplicate.ChildrenCollections.ForEach(x =>
                {
                    x.RegenerateId();
                });
                duplicate.Name += " (Duplicate)";

                this.ChildrenCollections.Insert(sourceIndex + 1, duplicate);
            }
            else
            {
                this.ChildrenCollections.ForEach(x =>
                {
                    x.DuplicateCollection(collectionId);
                });
            }
        }

        #endregion
    }
}
