using Integration.Common;
using Integration.Backend;
using System.Collections.Generic;
using System.Runtime.Caching;// Add this for MemoryCache

namespace Integration.Service
{


    public sealed class ItemIntegrationService
    {
        //This is a dependency that is normally fulfilled externally.
        private ItemOperationBackend ItemIntegrationBackend { get; set; } = new();
        private readonly HashSet<string> processedContent = new HashSet<string>();//HashSet(processedContent) to ensure no duplicate content reaches backend. Checks if data is already processed.
        private readonly ObjectCache processedContentCache = MemoryCache.Default;//Storing in cache to compare and prevent multiple instances of "ItemIntegrationService"
        // This is called externally and can be called multithreaded, in parallel.
        // More than one item with the same content should not be saved. However,
        // calling this with different contents at the same time is OK, and should
        // be allowed for performance reasons.
        public Result ProcessItem(string itemContent)
        {
            if (!processedContent.Contains(itemContent))
            {
                // Add the content to the processed set
                processedContent.Add(itemContent);
                return new Result(true, $"Item with content {itemContent} processed.");
            }

            return new Result(false, $"Duplicate item received with content {itemContent}.");
        }
        //ProcessItem modification include using a HashSet (processedContent) to keep track of processed content in the ItemIntegrationService. This way, duplicate content is identified before checking the backend and ensures that the same content is not saved more than once.
        public Result SaveItem(string itemContent)
        {
            // Check the backend to see if the content is already saved.
            if (ItemIntegrationBackend.FindItemsWithContent(itemContent).Count != 0)
            {
                return new Result(false, $"Duplicate item received with content {itemContent}.");
            }

            // Save the item only if it hasn't been processed before
            var result = ProcessItem(itemContent);
            if (result.Success)
            {
                var item = ItemIntegrationBackend.SaveItem(itemContent);
                return new Result(true, $"Item with content {itemContent} saved with id {item.Id}");
            }

            return result;
        }
        public List<Item> GetAllItems()
        {
            return ItemIntegrationBackend.GetAllItems();
        }
        private bool IsContentProcessed(string itemContent)
        {
            return processedContentCache.Contains(itemContent);
        }

        private void MarkContentAsProcessed(string itemContent)
        {
            // Store the content in the cache with some expiration policy (Solution for distributed system scenario for multiple instances of "ItemIntegrationService")
            var cacheItemPolicy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(10) };// 10 minutes can be changed depending on cache memory.
            processedContentCache.Add(itemContent, true, cacheItemPolicy);
        }
    } }

