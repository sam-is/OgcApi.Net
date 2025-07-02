# Testing

Currently, this project contains tests for data providers. Testing the entire API can be done using the [OGC API - Features Conformance Test Suite](https://cite.opengeospatial.org/te2/about/ogcapi-features-1.0/1.0/site/).

## Test Application
A test application is included in the repository (`SampleWebApplication`). It is recommended to use Docker Compose for debugging. The included containers are:
- **SQL Server**: Used as the database.
- **Tomcat with TEAM Engine**: Includes the OGC API - Features test suite.

## Launching the Test Application
After launching the application, you can use the following links:
- `https://localhost/api/index.html`: To test the API.
- `http://localhost:8082/teamengine`: TEAM Engine application.

To run tests in the TEAM Engine, use the internal address: `http://samplewebapplication:8080/api/ogc`.

For instructions on launching the TEAM Engine, refer to the Docker section here: [TEAM Engine Documentation](https://cite.opengeospatial.org/teamengine/about/ogcapi-features-1.0/1.0/site/).