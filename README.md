# AdCreative.ai Integration Case

## Description of the Problem

The project comprises two layers: Service and Backend.

For brevity, the API or presentation layer is excluded from the description.

This represents a typical integration scenario where items are externally sent by a third party to our integration service. The service is called with only the item content, to which we assign internal incremental identifiers, returning them (in text form here) to the third party.

The rule dictates that any item content should be saved only once and not occur twice in our systems. As per the agreement, the third party should wait for the result of their call, which can take a while (simulated as two seconds here, but realistically closer to forty seconds). However, in reality, the third party calls the service multiple times without waiting for a response.

Although protection is in place to check for duplicate items, if called rapidly in parallel (as demonstrated in the main Program), multiple entries with the same content are recorded on our end.

## Required Solution

### 1- Single Server Scenario

**a: Solution Description:**
- Modify the code exclusively within the Service layer (folder) to ensure that the same content is never saved more than once under any circumstances.
- Ensure that items with different content are processed concurrently without waiting for each other.

**b: Implementation:**
- Implement the solution within the Service layer.

**c: Demonstration in Program.cs:**
- Add code to Program.cs to showcase that the implemented solution allows parallel storage of items with different content.

### 2 - Distributed System Scenario

**a: Solution Description:**
- In case of multiple servers containing ItemIntegrationService, implement a solution for the distributed system scenario.

**b: Weaknesses:**
- Identify and describe any weaknesses that the solution might have in a text file.


### ANSWER FOR MULTIPLE SERVERS CONTAINING ItemIntegrationService.
### 2a(ans) - in Program.cs using memorycache.

**b: Weaknesses :**

1(Inconsistency): In distributed systems, maintaining cache consistency can be challenging. If multiple instances of the service are running, there might be delays or inconsistencies in updating the cache across all nodes.

2(Loss of Data): The simple sliding expiration policy used in the example may not be suitable for all scenarios. Items might be evicted from the cache even if they are still relevant or the processed content is lost if a server restarts or if the cache is cleared.

3(Latency): If there is latency or network issues between different servers running the service, it might affect the performance and reliability of the cache updates.

4(Scalability): The solution might face challenges in terms of scalability, especially if the number of processed items grows significantly. The cache might become a bottleneck.
