using Microsoft.Extensions.Logging;
using RestlessDb.Repositories;
using RestlessDb.Common.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestlessDb.Managers
{
    public class QueryItemsManager
    {
        private readonly ILogger<QueryItemsManager> logger;
        private readonly QueryItemsRepository queryItemsRepository;

        public QueryItemsManager(QueryItemsRepository queryConfigRepository, ILogger<QueryItemsManager> logger)
        {
            this.logger = logger;
            this.queryItemsRepository = queryConfigRepository;
        }

        public List<QueryItem> GetAll()
        {
            return queryItemsRepository.GetAll();
        }

        public QueryItem Get(string name)
        {
            return queryItemsRepository.Get(name);
        }

        public QueryItem Insert(QueryItem queryItem)
        {
            if (queryItemsRepository.Get(queryItem.Name) != null)
            {
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.DUPLICATE_KEY, $"Cannot insert item {queryItem.Name}: Key already exists");
            }

            if (queryItemsRepository.Insert(queryItem) != 1)
            {
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.DML_ERROR, $"Error insert item {queryItem.Name}: Inserted rows != 1");
            }

            return queryItemsRepository.Get(queryItem.Name);
        }

        public QueryItem Update(QueryItem queryItem)
        {
            if (queryItemsRepository.Get(queryItem.Name) == null)
            {
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.ITEM_NOTFOUND, $"Cannot update item {queryItem.Name}: Item not found");
            }

            if (queryItemsRepository.Update(queryItem) != 1)
            {
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.ITEM_NOTFOUND, $"Error update item {queryItem.Name}: Updated rows != 1");
            }

            return queryItemsRepository.Get(queryItem.Name);
        }

        public void Delete(string name)
        {
            if (queryItemsRepository.Get(name) == null)
            {
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.ITEM_NOTFOUND, $"Cannot delete item {name}: Item not found");
            }

            if (queryItemsRepository.Delete(name) != 1)
            {
                throw new GenericDbQueryException(GenericDbQueryExceptionCode.DML_ERROR, $"Error update item {name}: Deleted rows != 1");
            }
        }
    }
}
