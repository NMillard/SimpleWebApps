# CQRS Example

This example application illustrates a way to implement a simple yet powerful CQRS pattern.

The application has a few endpoints, and when data changes, the change is saved to a regular transactional store, 
and an internal event is published so that a specialized read-only table is updated as well.


## Materialized views

The read data is stored in a highly denormalized structure. The purpose is to tailor the read models to the interfaces 
and display requirements.

To create effective materialized views, you need to profile your data. This involves:
- Understand the data structure.
- Analyze data volume.
- Identify data relationships.
- Assess data quality.
- Capture data distribution.

## Data consistency

Since the write and read models are separate, there will be a time when the two models are out of sync for a brief 
period until the writes are transferred over to the read model. Eventual Consistency, in essence.


## Links
- https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs
- https://learn.microsoft.com/en-us/azure/well-architected/performance-efficiency/optimize-data-performance